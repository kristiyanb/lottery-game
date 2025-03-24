namespace LotteryGame.Data.Models
{
    public class Ticket
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string PlayerId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
