using Confluent.Kafka;
using OtusSpaceBattle.Commands;
using OtusSpaceBattle.Infrastructure;
using OtusSpaceBattle.Interfaces;
using OtusSpaceBattle.Models;
using OtusSpaceBattle.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static System.Formats.Asn1.AsnWriter;

namespace OtusSpaceBattle.Tests
{
    public class KafkaTests
    {
        private readonly ICommand testcommand;
        private IUObject mockUObject1;

        public KafkaTests() 
        {
            var router = IoC.Resolve<object>("IoC.Register", "router", new Func<object[], object>(args =>
            {
                return new InMemoryRouter();
            }));

            var consumer = IoC.Resolve<object>("IoC.Register", "consume", new Func<object[], object>(args =>
            {
                return new GameCommandConsumer((IGameRouter)IoC.Resolve<object>("router"));
            }));

            var commandCollection = IoC.Resolve<object>("IoC.Register", "commandCollection", new Func<object[], object>(args =>
            {
                return new CommandCollection();
            }));

            //var startProcessingCommand = IoC.Resolve<ICommand>("IoC.Register", "startProcessing", new Func<object[], object>(args =>
            //{
            //    return new StartProcessingCommandCollectionCommand((CommandCollection)args[0]);
            //}));
            testcommand = new StartProcessingCommandCollectionCommand(IoC.Resolve<object>("commandCollection") as CommandCollection);
            //var m = startProcessingCommand as ICommand;
            //m.Execute();
            //var mockUObject = new Moq.Mock<IUObject>();
            //var mockRotatable = mockUObject.As<IRotatableObject>();
            //mockRotatable.Setup(x => x.GetDirection()).Returns(3);
            //mockRotatable.Setup(x => x.GetDirectionsNumber()).Returns(8);
            //mockRotatable.Setup(x => x.GetAngularVelocity()).Returns(2);
            //int actualDirection = 0;
            //mockRotatable.Setup(x => x.SetDirection(Moq.It.IsAny<int>())).Callback<int>(v => actualDirection = v);
            //mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.GetDirection))).Returns(3);
            //mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.GetDirectionsNumber))).Returns(8);
            //mockUObject.Setup(x => x.GetProperty(nameof(IRotatableObject.GetAngularVelocity))).Returns(2);
            //mockUObject1 = mockUObject.Object;
        }

        [Fact]
        public void CanSendAndMove()
        {


            IoC.Resolve("IoC.Register", "move", new Func<object[], object>(args =>
                new MoveCommand((IMovableObject)args[0])));

            //var moveCommand = (ICommand)IoC.Resolve("move", mockUObject.Object);

            var gcc = (ICommandAsync)IoC.Resolve("consume");
            //gcc.ExecuteAsync();
            Assert.NotNull(gcc);
        }


        [Fact]
        public async Task CanConsumeAndExecuteMoveCommand()
        {
            // Arrange
            Vector2 initialPosition = new Vector2(12, 5);
            Vector2 velocity = new Vector2(3, 4);
            Vector2 expectedNewPosition = new Vector2(15, 9); // 12+3, 5+4

            var mockUObject = new Moq.Mock<IUObject>();
            var mockMovable = mockUObject.As<IMovableObject>();

            // Setup the movable object
            mockMovable.Setup(x => x.GetPosition()).Returns(initialPosition);
            mockMovable.Setup(x => x.GetVelocity()).Returns(velocity);

            Vector2 actualSetPosition = default;
            mockMovable.Setup(x => x.SetPosition(Moq.It.IsAny<Vector2>()))
                .Callback<Vector2>(v => actualSetPosition = v);

            var router = new InMemoryRouter();
            var consumer = new GameCommandConsumer(router);

            // Set up router and consumer
            IoC.Resolve<object>("IoC.Register", "router", new Func<object[], object>(args =>
                router));

            IoC.Resolve<object>("IoC.Register", "consume", new Func<object[], object>(args =>
                consumer));

            // Register GameObject with the specific targetId
            IoC.Resolve("IoC.Register", "GameObjects", new Func<object[], object>(args =>
            {
                // When called with targetId "target1", return our mock object
                return mockUObject.Object;
            }));

            // Register move command factory
            IoC.Resolve("IoC.Register", "move", new Func<object[], object>(args =>
                new MoveCommand((IMovableObject)args[0])));

            // Create a mock envelope and serialize it
            var mockEnvelope = new AgentEnvelope(
                GameId: "game1",
                TargetId: "target1",
                OpId: "move",
                Args: JsonSerializer.SerializeToElement(new object[] { }),
                Seq: 1,
                Ts: DateTime.UtcNow
            );

            var mockMessage = JsonSerializer.Serialize(mockEnvelope);

            // Produce message to Kafka
            using var producer = new ProducerBuilder<string, string>(new ProducerConfig { BootstrapServers = "localhost:9092" }).Build();
            producer.Produce("game.commands", new Message<string, string> { Key = "key1", Value = mockMessage });
            producer.Flush();

            // Act
            // Directly create and execute the InterpretCommand to avoid Kafka dependency
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var gcc = (ICommandAsync)IoC.Resolve("consume");
            gcc.ExecuteAsync(cts.Token);

            // Get the game context and process all commands
            var routerInstance = (IGameRouter)IoC.Resolve("router", "Scopes.Current");
            Assert.True(routerInstance.TryGet("game1", out var gameContext));

            // And yet i don't know how to correctly assert type of command without changing
            // the CommandCollection :c
            // Assert.IsType<MoveCommand>(gameContext.CommandCollection);

            // Process all commands in the collection
            gameContext.CommandCollection.LoopUntilNotEmpty();

            // Assert
            // Check that the position was updated correctly by adding velocity to position
            Assert.Equal(expectedNewPosition, actualSetPosition);
        }

    }
}
