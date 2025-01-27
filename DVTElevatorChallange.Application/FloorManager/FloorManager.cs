using DVTElevatorChallange.Core.Entities;
using DVTElevatorChallange.Domain.Enum;

namespace DVTElevatorChallange.Application.FloorManager
{
    public class FloorManager : IFloorManager
    {
        private readonly List<Floor> _floorList = new();

        public bool AddFloors(int floorCount)
        {
            _floorList.AddRange(Enumerable.Range(0, floorCount).Select(floorNum => new Floor { FloorNumber = floorNum }));
            return true;
        }

        public int GetTotalFloors() => _floorList.Count;

        public int GetRemainingQueueCount(int floorNumber, Direction direction)
        {
            var targetFloor = GetFloorByNumber(floorNumber);

            return direction switch
            {
                Direction.Up => targetFloor.UpQueue.Count,
                Direction.Down => targetFloor.DownQueue.Count,
                _ => 0
            };
        }

        public List<Passenger> LoadQueuePassengers(int floorNumber, int passengerAmount, Direction direction)
        {
            var targetFloor = GetFloorByNumber(floorNumber);
            var queue = direction == Direction.Up ? targetFloor.UpQueue : targetFloor.DownQueue;

            return DequeuePassengers(queue, passengerAmount);
        }

        public void AddPassenger(int totalPassengers, int currentFloor, int destinationFloor)
        {
            var targetFloor = GetFloorByNumber(currentFloor);
            var direction = GetDirection(currentFloor, destinationFloor);

            var queue = direction == Direction.Up ? targetFloor.UpQueue : targetFloor.DownQueue;

            for (int i = 0; i < totalPassengers; i++)
            {
                queue.Enqueue(new Passenger { DestinationFloor = destinationFloor });
            }
        }

        public void AddElevatorToStoppedElevators(Elevator elevator, int floorNumber)
        {
            var targetFloor = GetFloorByNumber(floorNumber);
            targetFloor.StoppedElevators.Add(elevator);
        }

        public void RemoveElevatorFromStoppedElevators(Elevator elevator, int floorNumber)
        {
            var targetFloor = GetFloorByNumber(floorNumber);
            targetFloor.StoppedElevators.Remove(elevator);
        }

        public List<Elevator> GetElevatorFromStoppedElevators(int floorNumber)
        {
            var targetFloor = GetFloorByNumber(floorNumber);
            return targetFloor.StoppedElevators;
        }

        public Direction DetermineDirection(Elevator elevator, int floorNumber)
        {
            var targetFloor = GetFloorByNumber(floorNumber);

            if (elevator.Direction != Direction.Idle || elevator.FloorStopList.Any())
            {
                return targetFloor.UpQueue.Count > targetFloor.DownQueue.Count ? Direction.Up : Direction.Down;
            }

            return elevator.Direction;
        }

        private Direction GetDirection(int currentFloor, int destinationFloor)
        {
            return destinationFloor > currentFloor
                ? Direction.Up
                : destinationFloor < currentFloor
                    ? Direction.Down
                    : Direction.Idle;
        }

        private Floor GetFloorByNumber(int floorNumber)
        {
            return _floorList.FirstOrDefault(f => f.FloorNumber == floorNumber)
                ?? throw new InvalidOperationException($"Floor with floor number {floorNumber} not found.");
        }

        private List<Passenger> DequeuePassengers(Queue<Passenger> queue, int count)
        {
            var dequeuedPassengers = new List<Passenger>();

            for (int i = 0; i < count && queue.Count > 0; i++)
            {
                dequeuedPassengers.Add(queue.Dequeue());
            }

            return dequeuedPassengers;
        }
    }
}
