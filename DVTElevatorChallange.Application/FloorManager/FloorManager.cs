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

        public int GetAmountOfFloors()
        {
            return _floorList.Count;
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

        public bool AddPassenger(int totalPassengers, int currentFloor, int destinationFloor)
        {
            var floor = _floorList.FirstOrDefault(f => f.FloorNumber == currentFloor)
                ?? throw new InvalidOperationException($"Floor with floor number {currentFloor} not found.");

            var direction = GetDirection(currentFloor, destinationFloor);

            if (direction == Direction.Idle)
            {
                return false;
            }

            for (int i = 0; i < totalPassengers; i++)
            {
                var passenger = new Passenger
                {
                    DestinationFloor = destinationFloor
                };

                if (direction == Direction.Up)
                {
                    floor.UpQueue.Enqueue(passenger);
                }
                else if (direction == Direction.Down)
                {
                    floor.DownQueue.Enqueue(passenger);
                }
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
