namespace DVTElevatorChallange.Core.Entities
{
    public class Building
    {
        public int ElevatorCount { get; set; }
        public int FloorCount { get; set; }
        public List<Elevator> ElevatorList { get; set; }
        public List<Floor> FloorList { get; set;}
    }
}
