namespace LotteryGame.Services.Models.Configurations
{
    public class PrizeOptions
    {
        public GrandPrize GrandPrize { get; set; } = new();

        public PrizeTier SecondTier { get; set; } = new();

        public PrizeTier ThirdTier { get; set; } = new();
    }

    public class GrandPrize
    {
        public int TicketsCount { get; set; }

        public int RevenuePercentage { get; set; }
    }

    public class PrizeTier
    {
        public int TicketsPercentage { get; set; }

        public int RevenuePercentage { get; set; }
    }
}
