using LotteryGame.Services.Models.Results;

namespace LotteryGame.Services.Contracts
{
    public interface ILotteryService
    {
        LotteryDrawResult DrawWinners();
    }
}
