using DVTElevatorChallange.Domain.Enum;

namespace DVTElevatorChallange.Core.Entities
{
    public class Elevator
    {
        public int CapacityLimit { get; private set; }
        public int TimeBetweenFloors { get; private set; }
        public int CurrentFloor { get; private set; }
        public int? NextStop { get; private set; }
        public ElevatorStatus Status { get; private set; }
        public Direction Direction { get; private set; }
        public List<Passenger> PassengerList { get; private set; }
    }
}
