namespace DVTElevatorChallange.Core.Entities
{
    public class Passenger
    {
        private static int _nextId = 0;

        public int Id { get; }
        public int DestinationFloor { get; set; }

        public Passenger()
        {
            Id = _nextId++;
        }
    }
}
