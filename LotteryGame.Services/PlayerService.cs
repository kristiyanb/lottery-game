using LotteryGame.Services.Contracts;
using LotteryGame.Data.Models;
using LotteryGame.Services.Models.Configurations;
using LotteryGame.Services.Models.Results;
using Microsoft.Extensions.Options;
using LotteryGame.Data;

namespace LotteryGame.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly Database db;
        private readonly PlayerOptions playerOptions;

        public PlayerService(
            Database db, 
            IOptions<PlayerOptions> playerOptions)
        {
            this.playerOptions = playerOptions.Value;
            this.db = db;
        }

        public List<Player> GetPlayers()
            => db.Players;

        public Result<Player> GetPlayerById(string id)
        {
            var player = db.Players.FirstOrDefault(x => x.Id == id);

            if (player == null)
            {
                return Result<Player>.Error("Invalid player ID.");
            }

            return Result<Player>.Success(player);
        }

        public Result<Player> AddPlayer(string name)
        {
            if (db.Players.Count >= playerOptions.MaxPlayers)
            {
                return Result<Player>.Error("Max number of players reached.");
            }

            var newPlayer = new Player
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Balance = playerOptions.StartingBalance,
                Tickets = new()
            };

            db.Players.Add(newPlayer);

            return Result<Player>.Success(newPlayer, "Player added successfully.");
        }

        public Result<decimal> UpdatePlayerBalance(string playerId, decimal amount)
        {
            var player = db.Players.FirstOrDefault(x => x.Id == playerId);

            player.Balance -= amount;

            return Result<decimal>.Success(player.Balance);
        }
    }
}
