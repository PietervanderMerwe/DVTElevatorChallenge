using DVTElevatorChallange.Application.FloorManager;
using DVTElevatorChallange.Core.Entities;
using DVTElevatorChallange.Domain.Enum;

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

        public List<Elevator> GetAllElevators() => _elevatorList;

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
                .OrderBy(e => e.Status == ElevatorStatus.Idle ? 0 : 1)
                .ThenBy(e => Math.Abs(e.CurrentFloor - floorNum))
                .ThenBy(e => e.Direction == direction ? 0 : 1)
                .FirstOrDefault(e => e.Status == ElevatorStatus.Idle || IsMovingTowardFloor(e, floorNum));

            return bestElevator ?? availableElevators.FirstOrDefault();
        }

        private async Task MoveToNextStop(int elevatorId)
        {
            var elevator = GetElevatorById(elevatorId);

            if (elevator.NextStop == null)
            {
                elevator.Status = ElevatorStatus.Idle;
                elevator.Direction = Direction.Idle;
                return;
            }

            elevator.Status = ElevatorStatus.Moving;
            elevator.Direction = elevator.CurrentFloor < elevator.NextStop ? Direction.Up : Direction.Down;

            await MoveElevatorToFloor(elevator, elevator.NextStop.Value);
            elevator.FloorStopList.Remove(elevator.CurrentFloor);

            ProcessFloorStop(elevator, elevator.CurrentFloor);
        }

        public void ProcessFloorStop(Elevator elevator, int floorNum)
        {
            _floorManager.AddElevatorToStoppedElevators(elevator, floorNum);

            var passengersToUnload = elevator.PassengerList
                .Where(p => p.DestinationFloor == floorNum)
                .ToList();

            foreach (var passenger in passengersToUnload)
            {
                RemovePassenger(passenger, elevator.Id);
            }

            var direction = _floorManager.DetermineDirection(elevator, floorNum);
            var passengersToLoad = _floorManager.LoadQueuePassengers(floorNum, elevator.CapacityLimit - elevator.PassengerList.Count, direction);

            foreach (var passenger in passengersToLoad)
            {
                AddPassengerToElevator(passenger, elevator);
            }

            if (passengersToLoad.Any())
            {
                AddFloorStop(elevator, passengersToLoad.First().DestinationFloor);
            }
            else
            {
                elevator.NextStop = null;
                elevator.Status = ElevatorStatus.Idle;
                elevator.Direction = Direction.Idle;
            }

            _floorManager.RemoveElevatorFromStoppedElevators(elevator, floorNum);

            if (_floorManager.GetRemainingQueueCount(floorNum, direction) > 0)
            {
                DispatchElevatorToFloor(floorNum, direction);
            }
        }

        public void AddPassengerToElevator(Passenger passenger, Elevator elevator)
        {
            if (elevator.PassengerList.Count >= elevator.CapacityLimit)
            {
                throw new InvalidOperationException("Elevator is at full capacity.");
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

        private async Task MoveElevatorToFloor(Elevator elevator, int targetFloor)
        {
            while (elevator.CurrentFloor != targetFloor)
            {
                await Task.Delay(elevator.TimeBetweenFloors);
                elevator.CurrentFloor += elevator.Direction == Direction.Up ? 1 : -1;
            }
        }

        private bool IsMovingTowardFloor(Elevator elevator, int floor) =>
            (elevator.Direction == Direction.Up && floor > elevator.CurrentFloor) ||
            (elevator.Direction == Direction.Down && floor < elevator.CurrentFloor);

        private Elevator GetElevatorById(int elevatorId) =>
            _elevatorList.FirstOrDefault(e => e.Id == elevatorId)
            ?? throw new InvalidOperationException($"Elevator with ID {elevatorId} not found.");
    }
}
