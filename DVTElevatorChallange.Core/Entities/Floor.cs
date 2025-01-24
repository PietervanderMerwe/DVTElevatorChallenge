namespace DVTElevatorChallange.Core.Entities
{
    public class Floor
    {
        public int FloorNumber { get; set; }
        public Queue<Passenger> UpQueue { get; set; }
        public Queue<Passenger> DownQueue { get; set; }
        public List<Elevator> StoppedElevators { get; set; }
    }
}
