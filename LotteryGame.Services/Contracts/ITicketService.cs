using LotteryGame.Data.Models;
using LotteryGame.Services.Models.Results;

namespace LotteryGame.Services.Contracts
{
    public interface ITicketService
    {
        public decimal GetTicketPrice();

        public List<Ticket> GetTickets();

        public Result<List<Ticket>> PurchaseTickets(string playerId, int numberOfTickets);
    }
}
