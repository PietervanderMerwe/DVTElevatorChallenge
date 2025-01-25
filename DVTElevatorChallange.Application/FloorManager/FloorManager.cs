using DVTElevatorChallange.Core.Entities;
using DVTElevatorChallange.Domain.Enum;

namespace DVTElevatorChallange.Application.FloorManager
{
    public class FloorManager : IFloorManager
    {
        private readonly List<Floor> _floorList = new();

        public bool AddFloors(int floorCount)
        {
            _floorList.AddRange(Enumerable.Range(0, floorCount).Select(_ => new Floor()));
            return true;
        }

        public void ClearUpQueue(int floorNumber)
        {
            var targetFloor = _floorList.FirstOrDefault(f => f.FloorNumber == floorNumber)
                ?? throw new InvalidOperationException($"Floor with floor number {floorNumber} not found.");

            targetFloor.UpQueue.Clear();
        }

        public void ClearDownQueue(int floorNumber)
        {
            var targetFloor = _floorList.FirstOrDefault(f => f.FloorNumber == floorNumber)
                ?? throw new InvalidOperationException($"Floor with floor number {floorNumber} not found.");
          
            targetFloor.DownQueue.Clear();
        }

        public bool AddPassenger(Passenger passenger, int currentFloor)
        {
            var floor = _floorList.FirstOrDefault(f => f.FloorNumber == currentFloor)
                ?? throw new InvalidOperationException($"Floor with floor number {currentFloor} not found.");

            var direction = GetDirection(currentFloor, passenger.DestinationFloor);

            switch (direction)
            {
                case Direction.Up:
                    floor.UpQueue.Enqueue(passenger);
                    break;
                case Direction.Down:
                    floor.DownQueue.Enqueue(passenger);
                    break;
                case Direction.Idle:
                    return false;
            }

            return true;
        }

        public void AddElevatorToStoppedElevators(Elevator elevator, int floorNumber)
        {
            var floor = _floorList.FirstOrDefault(f => f.FloorNumber == floorNumber)
                ?? throw new InvalidOperationException($"Floor with floor number {floorNumber} not found.");

            floor.StoppedElevators.Add(elevator);
        }

        public void RemoveElevatorFromStoppedElevators(Elevator elevator, int floorNumber)
        {
            var floor = _floorList.FirstOrDefault(f => f.FloorNumber == floorNumber)
                ?? throw new InvalidOperationException($"Floor with floor number {floorNumber} not found.");

            floor.StoppedElevators.Remove(elevator);
        }

        private Direction GetDirection(int currentFloor, int destinationFloor)
        {
            if (destinationFloor > currentFloor) return Direction.Up;
            if (destinationFloor < currentFloor) return Direction.Down;
            return Direction.Idle;
        }
    }
}
