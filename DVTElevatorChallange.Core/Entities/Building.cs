namespace DVTElevatorChallange.Core.Entities
{
    public class Building
    {
        public int ElevatorCount { get; private set; }
        public int FloorCount { get; private set; }
        public List<Elevator> ElevatorList { get; private set; }
        public List<Floor> FloorList { get; private set;}
    }
}
