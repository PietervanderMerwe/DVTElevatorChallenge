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

        private int _cursorElevatorArea;
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
            _cursorElevatorArea = Console.CursorTop;
            ElevatorStatus();
            ElevatorCommand();

            Task movingTask = null;

            while (_key != 's')
            {
                if (Console.KeyAvailable)
                {
                    var keyPress = Console.ReadKey(true).Key;
                    HandleKeyPress(keyPress);
                }

                await Task.Delay(200);

                if (movingTask == null || movingTask.IsCompleted)
                {
                    movingTask = _elevatorManager.MoveAllElevatorsToNextStopsAsync();
                }

                if (_elevatorManager.IsAnyElevatorMoving())
                {
                    RefreshElevatorStatus();
                }
            }
        }

        public void ElevatorStatus()
        {
            Console.WriteLine(new string('-', 80));
            Console.WriteLine("Elevators Status:");
            foreach (var elevator in _elevatorManager.GetAllElevators())
            {
                string elevatorDirection = GetDirectionSymbol(elevator.Direction);
                string currentFloor = elevator.CurrentFloor.ToString().PadRight(2, ' ');
                string passengerCount = elevator.PassengerList.Count.ToString().PadLeft(2, ' ');
                string destinations = string.Join(", ", elevator.FloorStopList);

                Console.Write($"[{elevator.Id}]:");
                Console.Write($" Floor {currentFloor} {elevatorDirection}");
                Console.Write($" |  Passengers:");
                Console.Write($" {passengerCount}");
                Console.Write($" |  Destinations:");
                Console.Write($" {destinations}");
                Console.WriteLine("");
            }
        }

        private void RefreshElevatorStatus()
        {
            Console.SetCursorPosition(0, _cursorElevatorArea);
            ElevatorStatus();
            Console.SetCursorPosition(0, _cursorInputArea);
        }

        private void ElevatorCommand()
        {
            Console.WriteLine(new string('-', 80));
            Console.WriteLine("");
            Console.WriteLine("Press S to Stop application");
            Console.WriteLine("");
            Console.WriteLine("Press A to Add passangers to a floor");
            Console.WriteLine("");
            Console.WriteLine(new string('-', 80));
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
            Console.SetCursorPosition(0, _cursorInputArea);
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
            var direction = DetermineDIrection(currentFloor, destinationFloor);
            _elevatorManager.DispatchElevatorToFloor(currentFloor, direction);

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

        private Direction DetermineDIrection(int currentFloor, int destinationFloor)
        {
            if (currentFloor > destinationFloor)
            {
                return Direction.Down;
            }
            else
            {
                return Direction.Up;
            }
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

        private string GetDirectionSymbol(Direction direction)
        {
            return direction switch
            {
                Direction.Up => "UP",
                Direction.Down => "DOWN",
                _ => "-"
            };
        }
    }
}
