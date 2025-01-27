namespace DVTElevatorChallange.Core.Entities
{
    public class Floor
    {
        private static int _nextFloor = 0;

        public int FloorNumber { get; set; }
        public Queue<Passenger> UpQueue { get; set; } = new Queue<Passenger>();
        public Queue<Passenger> DownQueue { get; set; } = new Queue<Passenger>();

        public Floor()
        {
            FloorNumber = _nextFloor++;
        }
    }
}
