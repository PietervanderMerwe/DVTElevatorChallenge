﻿using DVTElevatorChallange.Application.FloorManager;
using DVTElevatorChallange.Core.Entities;
using DVTElevatorChallange.Domain.Enum;
using DVTElevatorChallange.Domain.Interface;
using Moq;
using Xunit;

namespace DVTElevatorChallange.Tests
{
    public class FloorManagerTests
    {
        private readonly FloorManager _floorManager;
        private readonly Mock<ILoggerService> _loggerServiceMock;

        public FloorManagerTests()
        {
            _loggerServiceMock = new Mock<ILoggerService>();
            _floorManager = new FloorManager(_loggerServiceMock.Object);
        }

        [Fact]
        public void AddFloors_ShouldAddCorrectNumberOfFloors()
        {
            int floorCount = 5;

            var result = _floorManager.AddFloors(floorCount);

            Assert.True(result);
        }

        [Fact]
        public void GetTotalFloors_ShouldReturnCorrectFloorCount()
        {
            _floorManager.AddFloors(5);
            var totalFloors = _floorManager.GetTotalFloors();

            Assert.Equal(5, totalFloors);
        }

        [Fact]
        public void AddPassenger_ShouldAddPassengerToCorrectQueue()
        {
            _floorManager.AddFloors(3);
            int currentFloor = 1;
            int destinationFloor = 2;

            _floorManager.AddPassenger(3, currentFloor, destinationFloor);

            Assert.Equal(3, _floorManager.GetRemainingQueueCount(currentFloor, Direction.Up));
        }

        [Fact]
        public void LoadQueuePassengers_ShouldDequeuePassengersCorrectly()
        {
            _floorManager.AddFloors(3);
            int floorNumber = 1;
            _floorManager.AddPassenger(5, floorNumber, 2);

            var loadedPassengers = _floorManager.LoadQueuePassengers(floorNumber, 3, Direction.Up);

            Assert.Equal(3, loadedPassengers.Count);
            Assert.Equal(2, _floorManager.GetRemainingQueueCount(floorNumber, Direction.Up));
        }

        [Fact]
        public void DetermineDirection_ShouldChooseCorrectDirection()
        {
            _floorManager.AddFloors(3);
            var elevator = new Elevator { Direction = Direction.Up };
            int floorNumber = 1;
            _floorManager.AddPassenger(5, floorNumber, 2);

            var direction = _floorManager.DetermineDirection(elevator, floorNumber);

            Assert.Equal(Direction.Up, direction);
        }

        [Fact]
        public void GetFloorByNumber_ShouldThrowExceptionForInvalidFloor()
        {
            _floorManager.AddFloors(3);
            int invalidFloor = 10;

            Assert.Equal(0, _floorManager.GetRemainingQueueCount(invalidFloor, Direction.Up));
        }
    }
}
