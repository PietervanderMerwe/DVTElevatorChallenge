﻿using DVTElevatorChallange.Application.Building;
using DVTElevatorChallange.Application.ElevatorManager;
using DVTElevatorChallange.Application.FloorManager;
using DVTElevatorChallange.Domain.Enum;
using System.Threading;

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
        private Task _movingTask = null;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

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

            

            while (_key != 's')
            {
                if (Console.KeyAvailable)
                {
                    var keyPress = Console.ReadKey(true).Key;
                    HandleKeyPress(keyPress);
                }

                await Task.Delay(200);

                if (_movingTask == null || _movingTask.IsCompleted)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = new CancellationTokenSource();

                    _movingTask = _elevatorManager.MoveAllElevatorsToNextStopsAsync(_cancellationTokenSource.Token);
                }

                RefreshElevatorStatus();

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

                string statusLine = $"[{elevator.Id}]: Floor {currentFloor} {elevatorDirection} | Passengers: {passengerCount} | Destinations: {destinations}";
                int consoleWidth = Console.WindowWidth;
                Console.Write(statusLine.PadRight(consoleWidth));
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
            var floorLimit = _floorManager.GetTotalFloors();

            var totalPassengers = AskInt("How many passengers do you want to add?", floorLimit);
            var currentFloor = AskInt("What floor do you want to add them to?", floorLimit);
            var destinationFloor = AskInt("What floor do you want them to go to?", floorLimit);

            _floorManager.AddPassenger(totalPassengers, currentFloor, destinationFloor);
            var direction = DetermineDirection(currentFloor, destinationFloor);
            _cancellationTokenSource.Cancel();
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

        private Direction DetermineDirection(int currentFloor, int destinationFloor)
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

        private int AskInt(string prompt, int? limit = null)
        {
            int value;
            while (true)
            {
                Console.WriteLine(limit.HasValue
                    ? $"{prompt} (0 to {limit.Value - 1}):"
                    : prompt);
                var input = Console.ReadLine();

                if (int.TryParse(input, out value) && (!limit.HasValue || (value >= 0 && value < limit.Value)))
                {
                    return value;
                }

                Console.WriteLine(limit.HasValue
                    ? $"Invalid input. Please enter an integer between 0 and {limit.Value - 1}."
                    : "Invalid input. Please enter a valid integer.");
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
