using LotteryGame.Data;
using LotteryGame.Data.Models;
using LotteryGame.Services;
using LotteryGame.Services.Models.Configurations;
using LotteryGame.Services.Models.Results;
using Microsoft.Extensions.Options;

namespace LotteryGame.Tests
{
    public class PlayerServiceTests
    {
        private IOptions<PlayerOptions> playerOptions;
        private Database db;

        [SetUp]
        public void Setup()
        {
            db = new Database();
            playerOptions = Options.Create(new PlayerOptions
            {
                MinPlayers = 1,
                MaxPlayers = 2,
                StartingBalance = 10,
            });
        }

        [Test]
        public void AddPlayer()
        {
            var playerService = new PlayerService(db, playerOptions);

            var result = playerService.AddPlayer("Player 1");

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<Player>>());
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result?.Data?.Balance, Is.EqualTo(10));
                Assert.That(result?.Data?.Name, Is.EqualTo("Player 1"));
                Assert.That(result?.Message, Is.EqualTo("Player added successfully."));
            });
        }

        [Test]
        public void AddPlayer_MaxNumberReached()
        {
            var playerService = new PlayerService(db, playerOptions);

            playerService.AddPlayer("Player 1");
            playerService.AddPlayer("Player 2");
            var result = playerService.AddPlayer("Player 3");

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<Player>>());
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result?.Message, Is.EqualTo("Max number of players reached."));
            });
        }

        [Test]
        public void UpdatePlayerBalance()
        {
            var playerService = new PlayerService(db, playerOptions);

            var playerResult = playerService.AddPlayer("Player 1");
            var balanceUpdateResult = playerService.UpdatePlayerBalance(playerResult.Data.Id, 5);

            Assert.Multiple(() =>
            {
                Assert.That(balanceUpdateResult, Is.InstanceOf<Result<decimal>>());
                Assert.That(balanceUpdateResult.IsSuccess, Is.True);
                Assert.That(balanceUpdateResult?.Data, Is.EqualTo(5));
            });
        }
    }
}