using DVTElevatorChallange.Core.Entities;
using DVTElevatorChallange.Domain.Enum;

namespace DVTElevatorChallange.Application.ElevatorManager
{
    public class ElevatorManager : IElevatorManager
    {
        private readonly List<Elevator> _elevatorList = new();

        public bool AddElevators(int elevatorCount)
        {
            _elevatorList.AddRange(Enumerable.Range(0, elevatorCount).Select(_ => new Elevator()));
            return true;
        }

        public void AddPassengerToElevator(Passenger passenger, int elevatorId)
        {
            var targetElevator = _elevatorList.FirstOrDefault(e => e.Id == elevatorId);

            if (targetElevator == null)
            {
                throw new InvalidOperationException($"Elevator with ID {elevatorId} not found.");
            }

            if (targetElevator.PassengerList.Count >= targetElevator.CapacityLimit)
            {
                throw new InvalidOperationException("Elevator is at full capacity.");
            }

            targetElevator.PassengerList.Add(passenger);
        }

        public void RemovePassenger(Passenger passenger, int elevatorId)
        {
            var targetElevator = _elevatorList.FirstOrDefault(e => e.Id == elevatorId);

            if (targetElevator == null)
            {
                throw new InvalidOperationException($"Elevator with ID {elevatorId} not found.");
            }

            targetElevator.PassengerList.Remove(passenger);
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



        private bool IsMovingTowardFloor(Elevator elevator, int floor)
        {
            if (elevator.Direction == Direction.Up && floor > elevator.CurrentFloor)
                return true;

            if (elevator.Direction == Direction.Down && floor < elevator.CurrentFloor)
                return true;

            return false;
        }
    }
}
