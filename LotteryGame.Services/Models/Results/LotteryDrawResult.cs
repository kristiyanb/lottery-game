using LotteryGame.Data.Models;

namespace LotteryGame.Services.Models.Results
{
    public class LotteryDrawResult
    {
        public Player GrandPrizeWinner { get; set; }

        public decimal GrandPrizeAmount { get; set; }

        public List<Player> SecondTierWinners { get; set; }

        public decimal SecondTierPrizeAmount { get; set; }

        public List<Player> ThirdTierWinners { get; set; }

        public decimal ThirdTierPrizeAmount { get; set; }

        public decimal HouseRevenue { get; set; }
    }
}
