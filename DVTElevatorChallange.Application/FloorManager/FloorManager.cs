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

        public int GetRemainingQueueCount(int floorNumber, Direction direction)
        {
            var targetFloor = _floorList.FirstOrDefault(f => f.FloorNumber == floorNumber)
                ?? throw new InvalidOperationException($"Floor with floor number {floorNumber} not found.");

            if (direction == Direction.Up)
            {
                return targetFloor.UpQueue.Count;
            }
            else if(direction == Direction.Down)
            {
                return targetFloor.DownQueue.Count;
            }
            return 0;
        }

        public List<Passenger> LoadDownQueuePassengers(int floorNumber, int passengerAmount)
        {
            var targetFloor = _floorList.FirstOrDefault(f => f.FloorNumber == floorNumber)
        ?? throw new InvalidOperationException($"Floor with floor number {floorNumber} not found.");

            var dequeuedPassengers = new List<Passenger>();

            for (int i = 0; i < passengerAmount && targetFloor.DownQueue.Count > 0; i++)
            {
                dequeuedPassengers.Add(targetFloor.DownQueue.Dequeue());
            }

            return dequeuedPassengers;
        }

        public List<Passenger> LoadUpQueuePassengers(int floorNumber, int passengerAmount)
        {
            var targetFloor = _floorList.FirstOrDefault(f => f.FloorNumber == floorNumber)
        ?? throw new InvalidOperationException($"Floor with floor number {floorNumber} not found.");

            var dequeuedPassengers = new List<Passenger>();

            for (int i = 0; i < passengerAmount && targetFloor.DownQueue.Count > 0; i++)
            {
                dequeuedPassengers.Add(targetFloor.UpQueue.Dequeue());
            }

            return dequeuedPassengers;
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

        public Direction DetermineDirection(Elevator elevator, int floorNumber)
        {
            var floor = _floorList.FirstOrDefault(f => f.FloorNumber == floorNumber)
                ?? throw new InvalidOperationException($"Floor with floor number {floorNumber} not found.");

            if (elevator.Direction == Direction.Idle || elevator.FloorStopList.Count == 0)
            {
                return floor.UpQueue.Count > floor.DownQueue.Count ? Direction.Up : Direction.Down;
            }

            return elevator.Direction;
        }

        private Direction GetDirection(int currentFloor, int destinationFloor)
        {
            if (destinationFloor > currentFloor) return Direction.Up;
            if (destinationFloor < currentFloor) return Direction.Down;
            return Direction.Idle;
        }
    }
}
