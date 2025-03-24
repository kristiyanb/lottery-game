using LotteryGame.Data.Models;
using LotteryGame.Services;
using LotteryGame.Services.Contracts;
using LotteryGame.Services.Models.Configurations;
using LotteryGame.Services.Models.Results;
using Microsoft.Extensions.Options;
using Moq;

namespace LotteryGame.Tests
{
    public class LotteryServiceTests
    {
        private IOptions<PrizeOptions> prizeOptions;
        private Mock<IPlayerService> playerServiceMock = new();
        private Mock<ITicketService> ticketServiceMock = new();

        [SetUp]
        public void Setup()
        {
            prizeOptions = Options.Create(new PrizeOptions
            {
                GrandPrize = new()
                {
                    TicketsCount = 1,
                    RevenuePercentage = 50
                },
                SecondTier = new()
                {
                    TicketsPercentage = 10,
                    RevenuePercentage = 30,
                },
                ThirdTier = new()
                {
                    TicketsPercentage = 20,
                    RevenuePercentage = 10
                }
            });
        }

        [Test]
        public void DrawWinners()
        {
            var mockTickets = GetMockTickets();
            playerServiceMock
                .Setup(x => x.GetPlayerById(It.IsAny<string>()))
                .Returns(Result<Player>.Success(new()));
            ticketServiceMock
                .Setup(x => x.GetTickets())
                .Returns(mockTickets);
            ticketServiceMock
                .Setup(x => x.GetTicketPrice())
                .Returns(1);
            var lotteryService = new LotteryService(prizeOptions, playerServiceMock.Object, ticketServiceMock.Object);

            var result = lotteryService.DrawWinners();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<LotteryDrawResult>());

                Assert.That(result.GrandPrizeWinner, Is.InstanceOf<Player>());
                Assert.That(result.GrandPrizeAmount, Is.EqualTo(50));

                Assert.That(result.SecondTierWinners, Is.InstanceOf<List<Player>>());
                Assert.That(result.SecondTierWinners.Count, Is.EqualTo(10));
                Assert.That(result.SecondTierPrizeAmount, Is.EqualTo(3));

                Assert.That(result.ThirdTierWinners, Is.InstanceOf<List<Player>>());
                Assert.That(result.ThirdTierWinners.Count, Is.EqualTo(20));
                Assert.That(result.ThirdTierPrizeAmount, Is.EqualTo(0.5));
            });
        }

        private List<Ticket> GetMockTickets()
        {
            var result = new List<Ticket>();

            for (int i = 0; i < 100; i++)
            {
                result.Add(new Ticket());
            }

            return result;
        }
    }
}
