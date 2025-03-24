using LotteryGame.Data;
using LotteryGame.Data.Models;
using LotteryGame.Services;
using LotteryGame.Services.Contracts;
using LotteryGame.Services.Models.Configurations;
using LotteryGame.Services.Models.Results;
using Microsoft.Extensions.Options;
using Moq;

namespace LotteryGame.Tests
{
    public class TicketServiceTests
    {
        private IOptions<TicketOptions> ticketOptions;
        private Mock<IPlayerService> playerServiceMock;
        private Database db;

        [SetUp]
        public void Setup()
        {
            db = new Database();
            playerServiceMock = new Mock<IPlayerService>();
            ticketOptions = Options.Create(new TicketOptions
            {
                TicketPrice = 1,
                MinTickets = 1,
                MaxTickets = 5,
            });
        }

        [Test]
        public void PurchaseTickets_InvalidPlayer()
        {
            playerServiceMock
                .Setup(x => x.GetPlayerById(It.IsAny<string>()))
                .Returns(Result<Player>.Error());
            var ticketService = new TicketService(db, playerServiceMock.Object, ticketOptions);

            var result = ticketService.PurchaseTickets("player1", 1);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<List<Ticket>>>());
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Message, Is.EqualTo("Player not found."));
            });
        }

        [Test]
        public void PurchaseTickets_MaxTicketsExceeded()
        {
            var mockPlayer = new Player()
            {
                Id = "player1",
                Tickets = [new(), new(), new(), new(), new()]
            };
            playerServiceMock
                .Setup(x => x.GetPlayerById(It.IsAny<string>()))
                .Returns(Result<Player>.Success(mockPlayer));
            var ticketService = new TicketService(db, playerServiceMock.Object, ticketOptions);

            var result = ticketService.PurchaseTickets("player1", 1);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<List<Ticket>>>());
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Message, Is.EqualTo("Player has bought the max number of tickets."));
            });
        }

        [Test]
        public void PurchaseTickets_InsufficientBalance()
        {
            var mockPlayer = new Player()
            {
                Id = "player1",
                Balance = 0,
            };
            playerServiceMock
                .Setup(x => x.GetPlayerById(It.IsAny<string>()))
                .Returns(Result<Player>.Success(mockPlayer));
            var ticketService = new TicketService(db, playerServiceMock.Object, ticketOptions);

            var result = ticketService.PurchaseTickets("player1", 1);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<List<Ticket>>>());
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Message, Is.EqualTo("Insufficient balance."));
            });
        }

        [Test]
        public void PurchaseTickets_MaxAvailableTicketsByConfiguration()
        {
            var mockPlayer = new Player()
            {
                Id = "player1",
                Balance = 100,
                Tickets = [new(), new()]
            };
            playerServiceMock
                .Setup(x => x.GetPlayerById(It.IsAny<string>()))
                .Returns(Result<Player>.Success(mockPlayer));
            var ticketService = new TicketService(db, playerServiceMock.Object, ticketOptions);

            var result = ticketService.PurchaseTickets("player1", 5);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<List<Ticket>>>());
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data.Count, Is.EqualTo(3));
                Assert.That(result.Message, Is.EqualTo("3 tickets bought successfully."));
            });
        }

        [Test]
        public void PurchaseTickets_MaxAvailableTicketsByAvailableBalance()
        {
            var mockPlayer = new Player()
            {
                Id = "player1",
                Balance = 3,
                Tickets = []
            };
            playerServiceMock
                .Setup(x => x.GetPlayerById(It.IsAny<string>()))
                .Returns(Result<Player>.Success(mockPlayer));
            var ticketService = new TicketService(db, playerServiceMock.Object, ticketOptions);

            var result = ticketService.PurchaseTickets("player1", 5);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<List<Ticket>>>());
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data.Count, Is.EqualTo(3));
                Assert.That(result.Message, Is.EqualTo("3 tickets bought successfully."));
            });
        }

        [Test]
        public void PurchaseTickets()
        {
            var mockPlayer = new Player()
            {
                Id = "player1",
                Balance = 100,
                Tickets = []
            };
            playerServiceMock
                .Setup(x => x.GetPlayerById(It.IsAny<string>()))
                .Returns(Result<Player>.Success(mockPlayer));
            var ticketService = new TicketService(db, playerServiceMock.Object, ticketOptions);

            var result = ticketService.PurchaseTickets("player1", 5);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<List<Ticket>>>());
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Data.Count, Is.EqualTo(5));
                Assert.That(result.Message, Is.EqualTo("5 tickets bought successfully."));
            });
        }
    }
}
