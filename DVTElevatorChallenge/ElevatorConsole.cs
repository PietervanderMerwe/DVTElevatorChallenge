using DVTElevatorChallange.Application.Building;
using DVTElevatorChallange.Application.ElevatorManager;
using DVTElevatorChallange.Application.FloorManager;

namespace DVTElevatorChallenge.Presentation
{
    public class ElevatorConsole
    {
        private IBuildingManager _buildingManager;
        private IElevatorManager _elevatorManager;
        private IFloorManager _floorManager;
        public ElevatorConsole(IBuildingManager buildingManager, IElevatorManager elevatorManager, IFloorManager floorManager)
        {
            _buildingManager = buildingManager;
            _elevatorManager = elevatorManager;
            _floorManager = floorManager;
        }
        public async Task RunAsync(string[] args)
        {
            CreateBuilding();
        }

        private void CreateBuilding()
        {
            var floors = AskInt("Please enter the amount of floors");
            var elevators = AskInt("Please enter the amount of elevators");
            _buildingManager.CreeateBuilding(floors, elevators);
        }

        private int AskInt(string prompt)
        {
            int value;
            while (true)
            {
                Console.WriteLine(prompt);
                var input = Console.ReadLine();

                if (int.TryParse(input, out value))
                {
                    return value;
                }

                Console.WriteLine("Invalid input. Please enter a valid integer.");
            }
        }
    }
}
