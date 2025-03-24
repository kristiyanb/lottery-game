using LotteryGame.Data.Models;
using LotteryGame.Services.Contracts;
using LotteryGame.Services.Models.Configurations;
using LotteryGame.Services.Models.Results;
using Microsoft.Extensions.Options;

namespace LotteryGame.Services
{
    public class LotteryService : ILotteryService
    {
        private readonly PrizeOptions prizeOptions;
        private readonly IPlayerService playerService;
        private readonly ITicketService ticketService;

        public LotteryService(
            IOptions<PrizeOptions> prizeOptions,
            IPlayerService playerService,
            ITicketService ticketService)
        {
            this.prizeOptions = prizeOptions.Value;
            this.playerService = playerService;
            this.ticketService = ticketService;
        }

        public LotteryDrawResult DrawWinners()
        {
            var tickets = ticketService.GetTickets();
            var totalRevenue = tickets.Count * ticketService.GetTicketPrice();

            var grandPrizeAmount = totalRevenue * (prizeOptions.GrandPrize.RevenuePercentage / 100M);

            var secondTierNumberOfWinners = GetTierNumberOfWinners(tickets.Count, prizeOptions.SecondTier.TicketsPercentage);
            var secondTierPrizePool = GetTierPrizePool(totalRevenue, prizeOptions.SecondTier.RevenuePercentage);
            var secondTierPrizeAmount = GetTierPrizeAmount(secondTierPrizePool, secondTierNumberOfWinners);

            var thirdTierNumberOfWinners = GetTierNumberOfWinners(tickets.Count, prizeOptions.ThirdTier.TicketsPercentage);
            var thirdTierPrizePool = GetTierPrizePool(totalRevenue, prizeOptions.ThirdTier.RevenuePercentage);
            var thirdTierPrizeAmount = GetTierPrizeAmount(thirdTierPrizePool, thirdTierNumberOfWinners);

            var houseRevenue = totalRevenue - grandPrizeAmount - secondTierPrizePool - thirdTierPrizePool;

            var random = new Random();

            var grandPrizeWinnerTicket = tickets[random.Next(0, tickets.Count - 1)];
            grandPrizeWinnerTicket.IsActive = false;

            var grandPrizeWinner = playerService.GetPlayerById(grandPrizeWinnerTicket.PlayerId).Data;
            var secondTierWinners = DrawTierWinners(tickets, secondTierNumberOfWinners, random);
            var thirdTierWinners = DrawTierWinners(tickets, thirdTierNumberOfWinners, random);

            var drawResults = new LotteryDrawResult
            {
                GrandPrizeWinner = grandPrizeWinner,
                GrandPrizeAmount = grandPrizeAmount,
                SecondTierWinners = secondTierWinners,
                SecondTierPrizeAmount = secondTierPrizeAmount,
                ThirdTierWinners = thirdTierWinners,
                ThirdTierPrizeAmount = thirdTierPrizeAmount,
                HouseRevenue = houseRevenue
            };

            return drawResults;
        }

        private int GetTierNumberOfWinners(int ticketCount, int ticketsPercentage)
            => (int)Math.Round(ticketCount * (ticketsPercentage / 100.0), 0);

        private decimal GetTierPrizePool(decimal totalRevenue, int revenuePercentage)
            => totalRevenue * (revenuePercentage / 100M);

        private decimal GetTierPrizeAmount(decimal prizePool, int numberOfWinners)
            => prizePool / numberOfWinners;

        private List<Player> DrawTierWinners(List<Ticket> tickets, int numberOfWinners, Random random)
        {
            var tierWinners = new List<Player>();
            while (tierWinners.Count < numberOfWinners)
            {
                var winningTicket = tickets[random.Next(0, tickets.Count - 1)];

                if (!winningTicket.IsActive)
                {
                    continue;
                }

                winningTicket.IsActive = false;
                var winner = playerService.GetPlayerById(winningTicket.PlayerId).Data;
                tierWinners.Add(winner);
            }

            return tierWinners;
        }
    }
}
