using DVTElevatorChallange.Domain.Enum;

namespace DVTElevatorChallange.Core.Entities
{
    public class Elevator
    {
        private static int _nextId = 0;

        public int Id { get; set; }
        public int CapacityLimit { get; set; } = 10;
        public int TimeBetweenFloors { get; set; } = 2000; //In ms
        public int CurrentFloor { get; set; } = 0;
        public int? NextStop { get; set; }
        public ElevatorStatus Status { get; set; } = ElevatorStatus.Idle;
        public Direction Direction { get; set; } = Direction.Idle;
        public List<Passenger> PassengerList { get; set; } = new List<Passenger>();
        public SortedSet<int> FloorStopList { get; private set; } = new SortedSet<int>();

        public Elevator()
        {
            Id = _nextId++;
        }
    }
}
