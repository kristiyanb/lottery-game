using LotteryGame.Data.Models;

namespace LotteryGame.Data
{
    public class Database
    {
        public List<Player> Players { get; set; } = new();

        public List<Ticket> Tickets => Players.SelectMany(x => x.Tickets).ToList();
    }
}
