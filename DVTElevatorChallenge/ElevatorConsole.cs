using DVTElevatorChallange.Application.Building;
using DVTElevatorChallange.Application.ElevatorManager;
using DVTElevatorChallange.Application.FloorManager;
using DVTElevatorChallange.Domain.Enum;

namespace DVTElevatorChallenge.Presentation
{
    public class ElevatorConsole
    {
        private IBuildingManager _buildingManager;
        private IElevatorManager _elevatorManager;
        private IFloorManager _floorManager;

        private char _key { get; set; }
        private int _cursorInputArea;

        public ElevatorConsole(IBuildingManager buildingManager, IElevatorManager elevatorManager, IFloorManager floorManager)
        {
            _buildingManager = buildingManager;
            _elevatorManager = elevatorManager;
            _floorManager = floorManager;
        }
        public async Task RunAsync(string[] args)
        {
            CreateBuilding();
            await setupLoop();
        }

        private async Task setupLoop()
        {
            Console.Clear();
            ElevatorStatus();
            ElevatorCommand();

            while (_key != 's')
            {
                if (Console.KeyAvailable)
                {
                    var keyPress = Console.ReadKey(true).Key;
                    HandleKeyPress(keyPress);
                }

                await Task.Delay(200);
            }
        }

        private void ElevatorStatus()
        {
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("Elevator statistics");
            Console.WriteLine("");
            Console.Write("Elevator Count = " + _elevatorManager.GetAmountOfElevators().ToString());
            Console.Write(" | ");
            Console.Write("Floor Count = " + _floorManager.GetAmountOfFloors().ToString());
            Console.WriteLine("");
        }

        private void ElevatorCommand()
        {
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("Press S to Stop application");
            Console.WriteLine("");
            Console.WriteLine("Press A to Add passangers to a floor");
            Console.WriteLine("");
            Console.WriteLine("------------------------------------------------------------------");
            _cursorInputArea = Console.CursorTop;
        }

        private void CreateBuilding()
        {
            try
            {
                var floors = AskInt("Please enter the amount of floors");
                var elevators = AskInt("Please enter the amount of elevators");
                _buildingManager.CreeateBuilding(floors, elevators);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Setup failed with exception: "+ ex.ToString());
            }
        }

        private async void HandleKeyPress(ConsoleKey keyPress)
        {

            switch (keyPress)
            {
                case ConsoleKey.S:
                    ClearConsoleFromRow();
                    Console.WriteLine("Stopping application...");
                    _key = 's';
                    break;

                case ConsoleKey.A:
                    ClearConsoleFromRow();
                    await addPassangersToFloor();
                    break;

                default:
                    ClearConsoleFromRow();
                    Console.WriteLine("Invalid key. Press S, A");
                    break;
            }
        }

        private async Task addPassangersToFloor()
        {
            var totalPassengers = AskInt("How many passengers do you want to add?");
            var currentFloor = AskInt("What floor do you want to add them to?");
            var destinationFloor = AskInt("What floor do you want them to go to?");

            _floorManager.AddPassenger(totalPassengers, currentFloor, destinationFloor);

            ClearConsoleFromRow();
        }

        private void ClearConsoleFromRow()
        {
            for (int row = _cursorInputArea; row < Console.WindowHeight; row++)
            {
                Console.SetCursorPosition(0, row);
                Console.Write(new string(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(0, _cursorInputArea);
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

        private char AskKey(string prompt, params char[] validKeys)
        {
            while (true)
            {
                Console.WriteLine($"{prompt} (Valid keys: {string.Join(", ", validKeys)})");
                var input = Console.ReadKey(true).KeyChar;

                if (validKeys.Contains(input))
                {
                    return input;
                }

                Console.WriteLine("Invalid input. Please enter one of the valid keys.");
            }
        }
    }
}
