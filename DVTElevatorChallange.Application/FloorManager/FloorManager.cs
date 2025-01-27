using DVTElevatorChallange.Core.Entities;
using DVTElevatorChallange.Domain.Enum;
using DVTElevatorChallange.Domain.Interface;

namespace DVTElevatorChallange.Application.FloorManager
{
    public class FloorManager : IFloorManager
    {
        private readonly List<Floor> _floorList = new();
        private readonly ILoggerService _logger;

        public FloorManager(ILoggerService logger)
        {
            _logger = logger;
        }

        public bool AddFloors(int floorCount)
        {
            _floorList.AddRange(Enumerable.Range(0, floorCount).Select(floorNum => new Floor { FloorNumber = floorNum }));
            return true;
        }

        public int GetTotalFloors() => _floorList.Count;

        public int GetRemainingQueueCount(int floorNumber, Direction direction)
        {
            var targetFloor = GetFloorByNumber(floorNumber);

            if (targetFloor == null)
            {
                _logger.LogError($"Unable to get floor number {floorNumber}");
                return 0;
            }

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

            if (targetFloor == null)
            {
                _logger.LogError($"Unable to get floor number {floorNumber}");
                return new List<Passenger>();
            }

            var queue = direction == Direction.Up ? targetFloor.UpQueue : targetFloor.DownQueue;

            return DequeuePassengers(queue, passengerAmount);
        }

        public void AddPassenger(int totalPassengers, int currentFloor, int destinationFloor)
        {
            var targetFloor = GetFloorByNumber(currentFloor);

            if (targetFloor == null)
            {
                _logger.LogError($"Unable to get floor number {currentFloor}");
                return;
            }

            var direction = GetDirection(currentFloor, destinationFloor);

            var queue = direction == Direction.Up ? targetFloor.UpQueue : targetFloor.DownQueue;

            for (int i = 0; i < totalPassengers; i++)
            {
                queue.Enqueue(new Passenger { DestinationFloor = destinationFloor });
            }
        }

        public Direction DetermineDirection(Elevator elevator, int floorNumber)
        {
            var targetFloor = GetFloorByNumber(floorNumber);

            if (targetFloor == null)
            {
                _logger.LogError($"Unable to get floor number {floorNumber}");
                return elevator.Direction;
            }

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

        private Floor? GetFloorByNumber(int floorNumber)
        {
            var floor = _floorList.FirstOrDefault(f => f.FloorNumber == floorNumber);

            if (floor == null)
            {
                _logger.LogError($"Floor with floor number {floorNumber} not found.");
                return null;
            }
                
            return floor;
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
