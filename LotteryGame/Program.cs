using LotteryGame.Data;
using LotteryGame.Services;
using LotteryGame.Services.Contracts;
using LotteryGame.Services.Models.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LotteryGame
{
    public class Program
    {
        public static IServiceProvider serviceProvider;

        public static void Main()
        {
            ConfigureServices();
            Run();
        }

        public static void Run()
        {
            var playerService = serviceProvider.GetRequiredService<IPlayerService>();
            var ticketService = serviceProvider.GetRequiredService<ITicketService>();
            var ticketOptions = serviceProvider.GetRequiredService<IOptions<TicketOptions>>();

            var addPlayerResult = playerService.AddPlayer("Player 1");
            var currentUser = addPlayerResult.Data;

            Console.WriteLine($"Welcome to the Bede Lottery, {currentUser.Name}!");
            Console.WriteLine();
            Console.WriteLine($"* Your digital balance: ${currentUser.Balance:F2}");
            Console.WriteLine($"* Ticket Price: ${ticketOptions.Value.TicketPrice:F2} each");
            Console.WriteLine();
            Console.WriteLine($"How many tickets would you like to buy, {currentUser.Name}?");

            var numberOfTickets = int.Parse(Console.ReadLine());

            Console.WriteLine();

            ticketService.PurchaseTickets(currentUser.Id, numberOfTickets);

            SeedPlayers();
            Draw();
        }

        public static void Draw()
        {
            Console.WriteLine("Ticket Draw Results:");
            Console.WriteLine();

            var lotteryService = serviceProvider.GetRequiredService<ILotteryService>();
            var lotteryResults = lotteryService.DrawWinners();

            Console.WriteLine($"* Grand Prize: {lotteryResults.GrandPrizeWinner.Name} wins ${lotteryResults.GrandPrizeAmount:F2}!");
            Console.WriteLine($"* Second Tier: Players {string.Join(", ", lotteryResults.SecondTierWinners.Select(x => x.Name))} win ${lotteryResults.SecondTierPrizeAmount:F2} each!");
            Console.WriteLine($"* Third Tier: Players {string.Join(", ", lotteryResults.ThirdTierWinners.Select(x => x.Name))} win ${lotteryResults.ThirdTierPrizeAmount:F2} each!");
            Console.WriteLine();
            Console.WriteLine("Congratulations to the winners!");
            Console.WriteLine();
            Console.WriteLine($"House Revenue: ${lotteryResults.HouseRevenue:F2}");
        }

        public static void SeedPlayers()
        {
            var playerService = serviceProvider.GetRequiredService<IPlayerService>();
            var ticketService = serviceProvider.GetRequiredService<ITicketService>();
            var playerOptions = serviceProvider.GetRequiredService<IOptions<PlayerOptions>>();
            var ticketOptions = serviceProvider.GetRequiredService<IOptions<TicketOptions>>();

            var random = new Random();
            var numberOfPlayers = random.Next(playerOptions.Value.MinPlayers, playerOptions.Value.MaxPlayers);

            for (int i = 2; i <= numberOfPlayers; i++)
            {
                var addPlayerResult = playerService.AddPlayer($"Player {i}");
                var numberOfTickets = random.Next(ticketOptions.Value.MinTickets, ticketOptions.Value.MaxTickets);

                ticketService.PurchaseTickets(addPlayerResult.Data.Id, numberOfTickets);
            }

            Console.WriteLine($"{numberOfPlayers} other CPU players also have purchased tickets.");
            Console.WriteLine();
        }

        public static void ConfigureServices()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();

            services.AddOptions<PrizeOptions>().Bind(config.GetSection(nameof(PrizeOptions)));
            services.AddOptions<PlayerOptions>().Bind(config.GetSection(nameof(PlayerOptions)));
            services.AddOptions<TicketOptions>().Bind(config.GetSection(nameof(TicketOptions)));

            services.AddSingleton<Database>();

            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<ILotteryService, LotteryService>();

            serviceProvider = services.BuildServiceProvider();
        }
    }
}
