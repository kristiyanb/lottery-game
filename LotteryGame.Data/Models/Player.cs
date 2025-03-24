namespace LotteryGame.Data.Models
{
    public class Player
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public decimal Balance { get; set; }

        public string Name { get; set; }

        public List<Ticket> Tickets { get; set; } = new();
    }
}
