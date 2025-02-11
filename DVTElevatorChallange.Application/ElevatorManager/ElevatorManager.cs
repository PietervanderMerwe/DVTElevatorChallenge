﻿using DVTElevatorChallange.Application.FloorManager;
using DVTElevatorChallange.Core.Entities;
using DVTElevatorChallange.Domain.Enum;
using DVTElevatorChallange.Domain.Interface;

namespace DVTElevatorChallange.Application.ElevatorManager
{
    public class ElevatorManager : IElevatorManager
    {
        private readonly List<Elevator> _elevatorList = new();

        private readonly IFloorManager _floorManager;
        private readonly ILoggerService _logger;

        public ElevatorManager(IFloorManager floorManager, ILoggerService logger)
        {
            _floorManager = floorManager;
            _logger = logger;
        }

        public bool AddElevators(int elevatorCount)
        {
            _elevatorList.AddRange(Enumerable.Range(0, elevatorCount).Select(_ => new Elevator()));
            return true;
        }

        public List<Elevator> GetAllElevators() => _elevatorList;

        public async Task MoveAllElevatorsToNextStopsAsync(CancellationToken cancellationToken)
        {
            var tasks = _elevatorList
                .Where(e => e.NextStop != null)
                .Select(e => MoveToNextStop(e.Id, cancellationToken));

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("\"Elevator movement operations were canceled.");
            }
        }

        public void DispatchElevatorToFloor(int floorNum, Direction direction)
        {
            var bestElevator = GetBestElevatorToDispatch(floorNum, direction);

            if (bestElevator == null)
            {
                _logger.LogError($"No elevator found for floor {floorNum} in direction {direction}");
                return;
            }

            AddFloorStop(bestElevator, floorNum);
        }

        public Elevator GetBestElevatorToDispatch(int floorNum, Direction direction)
        {
            var availableElevators = _elevatorList
                .Where(e => e.PassengerList.Count < e.CapacityLimit)
                .ToList();

            var bestElevator = availableElevators
                .OrderBy(e => e.Status == ElevatorStatus.Idle ? 0 : 1)
                .ThenBy(e => Math.Abs(e.CurrentFloor - floorNum))
                .ThenBy(e => e.Direction == direction ? 0 : 1)
                .FirstOrDefault(e => e.Status == ElevatorStatus.Idle || IsMovingTowardFloor(e, floorNum));

            return bestElevator ?? availableElevators.FirstOrDefault();
        }

        private async Task MoveToNextStop(int elevatorId, CancellationToken cancellationToken)
        {
            var elevator = GetElevatorById(elevatorId);

            if (elevator == null)
            {
                _logger.LogError($"Unable to move elevator. Elevator with ID {elevatorId} not found.");
                return;
            }

            if (elevator.NextStop == null)
            {
                elevator.Status = ElevatorStatus.Idle;
                elevator.Direction = Direction.Idle;
                return;
            }

            elevator.Status = ElevatorStatus.Moving;
            elevator.Direction = elevator.CurrentFloor < elevator.NextStop ? Direction.Up : Direction.Down;

            try
            {
                await MoveElevatorToFloor(elevator, elevator.NextStop.Value, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                elevator.FloorStopList.Remove(elevator.NextStop.Value);

                ProcessFloorStop(elevator, elevator.NextStop.Value);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Move to next stop for elevator {elevator.Id} was canceled.");
            }
        }

        public void ProcessFloorStop(Elevator elevator, int floorNum)
        {
            var passengersToUnload = elevator.PassengerList
                .Where(p => p.DestinationFloor == floorNum)
                .ToList();

            foreach (var passenger in passengersToUnload)
            {
                RemovePassenger(passenger, elevator.Id);
            }

            var direction = _floorManager.DetermineDirection(elevator, floorNum);
            var passengersToLoad = _floorManager.LoadQueuePassengers(floorNum, elevator.CapacityLimit - elevator.PassengerList.Count, direction);

            if (passengersToLoad != null && passengersToLoad.Any())
            {
                foreach (var passenger in passengersToLoad)
                {
                    AddPassengerToElevator(passenger, elevator);
                }

                AddFloorStop(elevator, passengersToLoad.First().DestinationFloor);
            }
            else
            {
                elevator.NextStop = null;
                elevator.Status = ElevatorStatus.Idle;
                elevator.Direction = Direction.Idle;
            }

            if (_floorManager.GetRemainingQueueCount(floorNum, direction) > 0)
            {
                DispatchElevatorToFloor(floorNum, direction);
            }
        }

        public void AddPassengerToElevator(Passenger passenger, Elevator elevator)
        {
            if (elevator.PassengerList.Count >= elevator.CapacityLimit)
            {
                _logger.LogInformation($"Elevator {elevator.Id} is at full capacity.");
                return;
            }

            elevator.PassengerList.Add(passenger);
        }

        public void RemovePassenger(Passenger passenger, int elevatorId)
        {
            var elevator = GetElevatorById(elevatorId);
            elevator.PassengerList.Remove(passenger);
        }

        public void AddFloorStop(Elevator elevator, int newFloor)
        {
            if (elevator.FloorStopList.Contains(newFloor) || newFloor < 0)
            {
                return;
            }

            elevator.FloorStopList.Add(newFloor);

            elevator.NextStop = newFloor;
            elevator.Direction = newFloor > elevator.CurrentFloor ? Direction.Up : Direction.Down;

        }

        private async Task MoveElevatorToFloor(Elevator elevator, int targetFloor, CancellationToken cancellationToken)
        {
            while (elevator.CurrentFloor != targetFloor)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(elevator.TimeBetweenFloors, cancellationToken);

                elevator.CurrentFloor += elevator.Direction == Direction.Up ? 1 : -1;
            }
        }

        private bool IsMovingTowardFloor(Elevator elevator, int floor) =>
            (elevator.Direction == Direction.Up && floor > elevator.CurrentFloor) ||
            (elevator.Direction == Direction.Down && floor < elevator.CurrentFloor);

        private Elevator? GetElevatorById(int elevatorId)
        {
            var elevator = _elevatorList.FirstOrDefault(e => e.Id == elevatorId);

            if (elevator == null)
            {
                _logger.LogError($"Elevator with ID {elevatorId} not found.");
            }

            return elevator;
        }
    }
}
