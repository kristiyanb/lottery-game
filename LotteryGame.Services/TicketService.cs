using LotteryGame.Data;
using LotteryGame.Data.Models;
using LotteryGame.Services.Contracts;
using LotteryGame.Services.Models.Configurations;
using LotteryGame.Services.Models.Results;
using Microsoft.Extensions.Options;

namespace LotteryGame.Services
{
    public class TicketService : ITicketService
    {
        private readonly Database db;
        private readonly IPlayerService playerService;
        private readonly TicketOptions ticketOptions;

        public TicketService(
            Database db,
            IPlayerService playerService,
            IOptions<TicketOptions> ticketOptions)
        {
            this.db = db;
            this.playerService = playerService;
            this.ticketOptions = ticketOptions.Value;
        }

        public decimal GetTicketPrice()
            => ticketOptions.TicketPrice;

        public List<Ticket> GetTickets() 
            => db.Tickets;

        public Result<List<Ticket>> PurchaseTickets(string playerId, int numberOfTickets)
        {
            var playerResult = this.playerService.GetPlayerById(playerId);
            if (!playerResult.IsSuccess)
            {
                return Result<List<Ticket>>.Error("Player not found.");
            }

            var player = playerResult.Data;

            if (player.Tickets.Count >= ticketOptions.MaxTickets)
            {
                return Result<List<Ticket>>.Error("Player has bought the max number of tickets.");
            }

            if (player.Balance < ticketOptions.TicketPrice)
            {
                return Result<List<Ticket>>.Error("Insufficient balance.");
            }

            if (player.Tickets.Count + numberOfTickets > ticketOptions.MaxTickets)
            {
                numberOfTickets = ticketOptions.MaxTickets - player.Tickets.Count;
            }

            if (player.Balance < numberOfTickets * ticketOptions.TicketPrice)
            {
                numberOfTickets = (int)Math.Floor(player.Balance / ticketOptions.TicketPrice);
            }

            var newTickets = new List<Ticket>();
            for (int i = 0; i < numberOfTickets; i++)
            {
                var newTicket = new Ticket
                {
                    Id = Guid.NewGuid().ToString(),
                    PlayerId = playerId,
                };

                newTickets.Add(newTicket);
            }

            player.Tickets.AddRange(newTickets);
            playerService.UpdatePlayerBalance(player.Id, ticketOptions.TicketPrice * numberOfTickets);

            return Result<List<Ticket>>.Success(newTickets, $"{newTickets.Count} tickets bought successfully.");
        }
    }
}
