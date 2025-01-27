using DVTElevatorChallange.Application.ElevatorManager;
using DVTElevatorChallange.Application.FloorManager;
using DVTElevatorChallange.Core.Entities;
using DVTElevatorChallange.Domain.Enum;
using DVTElevatorChallange.Domain.Interface;
using Moq;
using Xunit;

namespace DVTElevatorChallange.Tests
{
    public class ElevatorManagerTests
    {
        private readonly Mock<IFloorManager> _floorManagerMock;
        private readonly Mock<ILoggerService> _loggerServiceMock;
        private readonly ElevatorManager _elevatorManager;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public ElevatorManagerTests()
        {
            _floorManagerMock = new Mock<IFloorManager>();
            _loggerServiceMock = new Mock<ILoggerService>();
            _elevatorManager = new ElevatorManager(_floorManagerMock.Object, _loggerServiceMock.Object);
        }

        [Fact]
        public void AddElevators_ShouldAddSpecifiedNumberOfElevators()
        {
            int elevatorCount = 3;

            var result = _elevatorManager.AddElevators(elevatorCount);

            Assert.True(result);
            Assert.Equal(elevatorCount, _elevatorManager.GetAllElevators().Count);
        }

        [Fact]
        public void GetAllElevators_ShouldReturnAllElevators()
        {
            _elevatorManager.AddElevators(2);

            var elevators = _elevatorManager.GetAllElevators();

            Assert.Equal(2, elevators.Count);
        }

        [Fact]
        public void DispatchElevatorToFloor_ShouldAddFloorStopForBestElevator()
        {
            _elevatorManager.AddElevators(1);
            var elevator = _elevatorManager.GetAllElevators().First();
            int floorNumber = 5;
            var direction = Direction.Up;

            _floorManagerMock
                .Setup(f => f.AddElevatorToStoppedElevators(It.IsAny<Elevator>(), floorNumber));

            _elevatorManager.DispatchElevatorToFloor(floorNumber, direction);

            Assert.Contains(floorNumber, elevator.FloorStopList);
            Assert.Equal(Direction.Up, elevator.Direction);
        }

        [Fact]
        public async Task MoveAllElevatorsToNextStopsAsync_ShouldMoveElevatorsToNextStop()
        {
            _elevatorManager.AddElevators(1);
            var elevator = _elevatorManager.GetAllElevators().First();
            elevator.CurrentFloor = 1;
            elevator.NextStop = 5;

            _floorManagerMock
                .Setup(f => f.AddFloors(10));

            await _elevatorManager.MoveAllElevatorsToNextStopsAsync(_cancellationTokenSource.Token);

            Assert.Equal(5, elevator.CurrentFloor);
            Assert.Null(elevator.NextStop);
        }

        [Fact]
        public void GetBestElevatorToDispatch_ShouldReturnBestElevator()
        {
            _elevatorManager.AddElevators(2);
            var elevator1 = _elevatorManager.GetAllElevators()[0];
            var elevator2 = _elevatorManager.GetAllElevators()[1];

            elevator1.CurrentFloor = 2;
            elevator1.Status = ElevatorStatus.Idle;
            elevator2.CurrentFloor = 10;
            elevator2.Status = ElevatorStatus.Moving;

            int floorNumber = 5;
            var direction = Direction.Up;

            var bestElevator = _elevatorManager.GetBestElevatorToDispatch(floorNumber, direction);

            Assert.Equal(elevator1, bestElevator);
        }

        [Fact]
        public void ProcessFloorStop_ShouldAddPassengersFromQueue()
        {
            _elevatorManager.AddElevators(1);
            var elevator = _elevatorManager.GetAllElevators().First();
            int floorNumber = 3;

            _floorManagerMock
                .Setup(f => f.LoadQueuePassengers(floorNumber, elevator.CapacityLimit, It.IsAny<Direction>()))
                .Returns(new List<Passenger>
                {
            new Passenger { DestinationFloor = 10 }
                });

            _elevatorManager.ProcessFloorStop(elevator, floorNumber);

            Assert.Single(elevator.PassengerList);
            Assert.Contains(10, elevator.FloorStopList);
        }

        [Fact]
        public void AddPassengerToElevator_ShouldLogMessageIfElevatorIsFull()
        {
            _elevatorManager.AddElevators(1);

            var elevator = _elevatorManager.GetAllElevators().First();
            elevator.CapacityLimit = 1;
            elevator.PassengerList.Add(new Passenger { DestinationFloor = 5 });

            _elevatorManager.AddPassengerToElevator(new Passenger { DestinationFloor = 10 }, elevator);

            _loggerServiceMock.Verify(
                x => x.LogInformation($"Elevator {elevator.Id} is at full capacity."),
                Times.Once
            );
            Assert.Single(elevator.PassengerList);
        }

        [Fact]
        public void RemovePassenger_ShouldRemovePassengerFromElevator()
        {
            _elevatorManager.AddElevators(1);
            var elevator = _elevatorManager.GetAllElevators().First();
            var passenger = new Passenger { DestinationFloor = 5 };
            elevator.PassengerList.Add(passenger);

            _elevatorManager.RemovePassenger(passenger, elevator.Id);

            Assert.Empty(elevator.PassengerList);
        }
    }
}
