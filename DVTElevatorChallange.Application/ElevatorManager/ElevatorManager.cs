using DVTElevatorChallange.Application.FloorManager;
using DVTElevatorChallange.Core.Entities;
using DVTElevatorChallange.Domain.Enum;
using System.Xml.Linq;

namespace DVTElevatorChallange.Application.ElevatorManager
{
    public class ElevatorManager : IElevatorManager
    {
        private readonly List<Elevator> _elevatorList = new();

        private readonly IFloorManager _floorManager;

        public ElevatorManager(IFloorManager floorManager)
        {
            _floorManager = floorManager;
        }

        public bool AddElevators(int elevatorCount)
        {
            _elevatorList.AddRange(Enumerable.Range(0, elevatorCount).Select(_ => new Elevator()));
            return true;
        }

        public List<Elevator> GetAllElevators()
        {
            return _elevatorList;
        }

        public async Task MoveAllElevatorsToNextStopsAsync()
        {
            var tasks = _elevatorList
                .Where(e => e.NextStop != null)
                .Select(e => MoveToNextStop(e.Id));

            await Task.WhenAll(tasks);
        }

        public void DispatchElevatorToFloor(int floorNum, Direction direction)
        {
            var bestElevator = GetBestElevatorToDispatch(floorNum, direction)
                ?? throw new InvalidOperationException($"No elevator found");
            AddFloorStop(bestElevator , floorNum);
        }

        public Elevator GetBestElevatorToDispatch(int floorNum, Direction direction)
        {
            var availableElevators = _elevatorList
                .Where(e => e.PassengerList.Count < e.CapacityLimit)
                .ToList();

            var bestElevator = availableElevators
                .FirstOrDefault(e => e.CurrentFloor == floorNum &&
                                     (e.Status == ElevatorStatus.Idle || e.Status == ElevatorStatus.Stopped));

            if (bestElevator != null)
            {
                return bestElevator;
            }

            bestElevator = availableElevators
                .Where(e => IsMovingTowardFloor(e, floorNum) && e.Direction == direction ||
                            (e.Status == ElevatorStatus.Idle && !e.FloorStopList.Any()))
                .OrderBy(e => Math.Abs(e.CurrentFloor - floorNum))
                .ThenBy(e => e.Direction == direction ? 0 : 1)
                .ThenBy(e => e.Status == ElevatorStatus.Idle ? 0 : 1)
                .FirstOrDefault();

            if (bestElevator != null)
            {
                return bestElevator;
            }

            bestElevator = availableElevators
                .OrderBy(e =>
                {
                    var turningDistance = e.Direction == Direction.Up
                        ? (e.FloorStopList.Max - e.CurrentFloor) + Math.Abs(e.FloorStopList.Max - floorNum)
                        : (e.CurrentFloor - e.FloorStopList.Min) + Math.Abs(floorNum - e.FloorStopList.Min);
                    return turningDistance;
                })
                .FirstOrDefault();

            if (bestElevator != null)
            {
                return bestElevator;
            }

            return availableElevators.FirstOrDefault() ?? _elevatorList.FirstOrDefault();
        }

        private async Task MoveToNextStop(int elevatorId)
        {
            var targetElevator = _elevatorList.FirstOrDefault(e => e.Id == elevatorId)
               ?? throw new InvalidOperationException($"Elevator with ID {elevatorId} not found.");

            if (targetElevator.NextStop is null)
            {
                targetElevator.Status = ElevatorStatus.Idle;
                targetElevator.Direction = Direction.Idle;
                return;
            }
            targetElevator.Status = ElevatorStatus.Moving;
            targetElevator.Direction = targetElevator.CurrentFloor < targetElevator.NextStop ? Direction.Up : Direction.Down;
            while (targetElevator.CurrentFloor != targetElevator.NextStop)
            {
                await Task.Delay(targetElevator.TimeBetweenFloors);
                targetElevator.CurrentFloor = targetElevator.Direction == Direction.Up ? targetElevator.CurrentFloor + 1 : targetElevator.CurrentFloor - 1;
            }

            targetElevator.FloorStopList.Remove(targetElevator.CurrentFloor);
            SetBestNextStop(targetElevator, targetElevator.CurrentFloor);
            ProcessFloorStop(targetElevator, targetElevator.CurrentFloor);
        }

        public void ProcessFloorStop(Elevator elevator, int floorNum)
        {
            _floorManager.AddElevatorToStoppedElevators(elevator, floorNum);

            elevator.FloorStopList.Remove(floorNum);

            var direction = _floorManager.DetermineDirection(elevator, floorNum);

            var passengersToUnload = elevator.PassengerList
                .Where(p => p.DestinationFloor == floorNum)
                .ToList();

            foreach (var passenger in passengersToUnload)
            {
                RemovePassenger(passenger, elevator.Id);
            }

            var passengersToLoad = direction == Direction.Up
                ? _floorManager.LoadUpQueuePassengers(floorNum, elevator.CapacityLimit - elevator.PassengerList.Count)
                : _floorManager.LoadDownQueuePassengers(floorNum, elevator.CapacityLimit - elevator.PassengerList.Count);

            foreach (var passenger in passengersToLoad)
            {
                AddPassengerToElevator(passenger, elevator.Id);
            }

            SetBestNextStop(elevator, floorNum);

            _floorManager.RemoveElevatorFromStoppedElevators(elevator, floorNum);

            var remainingPassengers = direction == Direction.Up
                ? _floorManager.GetRemainingUpQueueCount(floorNum)
                : _floorManager.GetRemainingDownQueueCount(floorNum);

            if (remainingPassengers > 0)
            {
                DispatchElevatorToFloor(floorNum, direction);
            }
        }

        private void SetBestNextStop(Elevator elevator, int floorNum)
        {
            var upStops = elevator.FloorStopList.Where(x => x > floorNum);
            var downStops = elevator.FloorStopList.Where(x => x < floorNum);

            if (elevator.Direction == Direction.Up)
            {
                if (upStops.Any())
                    elevator.Direction = Direction.Up;
                else if (downStops.Any())
                    elevator.Direction = Direction.Down;
            }
            else if (elevator.Direction == Direction.Down)
            {
                if (downStops.Any())
                    elevator.Direction = Direction.Down;
                else if (upStops.Any())
                    elevator.Direction = Direction.Up;
            }
            else if (elevator.Direction == Direction.Idle)
            {
                if (upStops.Any())
                    elevator.Direction = Direction.Up;
                else if (downStops.Any())
                    elevator.Direction = Direction.Down;
            }

            elevator.Direction = Direction.Idle;
        }

        public void AddPassengerToElevator(Passenger passenger, int elevatorId)
        {
            var targetElevator = _elevatorList.FirstOrDefault(e => e.Id == elevatorId)
                ?? throw new InvalidOperationException($"Elevator with ID {elevatorId} not found.");

            if (targetElevator.PassengerList.Count >= targetElevator.CapacityLimit)
            {
                throw new InvalidOperationException("Elevator is at full capacity.");
            }

            targetElevator.PassengerList.Add(passenger);
        }

        public void RemovePassenger(Passenger passenger, int elevatorId)
        {
            var targetElevator = _elevatorList.FirstOrDefault(e => e.Id == elevatorId)
                ?? throw new InvalidOperationException($"Elevator with ID {elevatorId} not found.");

            targetElevator.PassengerList.Remove(passenger);
        }

        public bool IsAnyElevatorMoving()
        {
            return _elevatorList.Any(e => e.Status == ElevatorStatus.Moving);
        }

        private bool IsMovingTowardFloor(Elevator elevator, int floor)
        {
            if (elevator.Direction == Direction.Up && floor > elevator.CurrentFloor)
                return true;

            if (elevator.Direction == Direction.Down && floor < elevator.CurrentFloor)
                return true;

            return false;
        }

        public void AddFloorStop(Elevator elevator, int newFloor)
        {
            if (elevator.FloorStopList.Contains(newFloor))
            {
                return;
            }

            elevator.FloorStopList.Add(newFloor);

            if (elevator.Direction == Direction.Idle)
            {
                UpdateNextStopForIdleElevator(elevator, newFloor);
                return;
            }

            UpdateNextStopForMovingElevator(elevator, newFloor);
        }

        private void UpdateNextStopForIdleElevator(Elevator elevator, int newFloor)
        {
            if (elevator.NextStop is null || Math.Abs(elevator.CurrentFloor - newFloor) < Math.Abs(elevator.CurrentFloor - elevator.NextStop.Value))
            {
                elevator.NextStop = newFloor;
                elevator.Direction = elevator.CurrentFloor < newFloor ? Direction.Up : Direction.Down;
            }
        }

        private void UpdateNextStopForMovingElevator(Elevator elevator, int newFloor)
        {
            var newFloorDir = elevator.CurrentFloor < newFloor ? Direction.Up : Direction.Down;

            if ((elevator.Direction == Direction.Up && newFloorDir == Direction.Up && newFloor < elevator.NextStop) ||
                (elevator.Direction == Direction.Down && newFloorDir == Direction.Down && newFloor > elevator.NextStop))
            {
                elevator.NextStop = newFloor;
            }
        }
    }
}
