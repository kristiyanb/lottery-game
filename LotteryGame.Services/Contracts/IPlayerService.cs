using LotteryGame.Data.Models;
using LotteryGame.Services.Models.Results;

namespace LotteryGame.Services.Contracts
{
    public interface IPlayerService
    {
        public List<Player> GetPlayers();

        public Result<Player> GetPlayerById(string id);

        public Result<Player> AddPlayer(string name);

        Result<decimal> UpdatePlayerBalance(string playerId, decimal amount);
    }
}
