# Elevator System Console Application

This project is a .NET console application designed to simulate an elevator system using **Clean Architecture** principles. The application allows users to configure the number of floors in the building and the number of elevators. It provides a real-time console-based interface to add passengers on specific floors and specify their destination floors.

## Features

- **Customizable Building Setup**: Configure the number of floors and elevators in the building at startup.
- **Passenger Management**: Add passengers on specific floors with destination floors during runtime.
- **Dynamic Elevator Movement**: Simulate elevator operation based on passenger requests and system logic.
- **Clean Architecture**: Ensures separation of concerns, maintainability, and scalability of the codebase.
- **Console**: Intuitive and interactive text-based user interface.

## Getting Started

Follow the steps below to set up and run the application on your local machine.

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (version 8.0 or later)
- A code editor such as [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/PietervanderMerwe/DVTElevatorChallenge.git

2. Restore dependencies:
   ```bash
   dotnet restore
   
3. Build the project:
   ```bash
   dotnet build

## Code Structure
The application follows the Clean Architecture pattern:

- Domain: Contains core entities, enums, and shared logic.
- Application: Includes use cases and interfaces (e.g., ElevatorManager FloorManager requests).
- Infrastructure: Handles lower-level details (e.g., logging).
- Presentation: Manages the console and user interaction.
  
## Key Classes and Modules
- ElevatorManager: Handles individual elevator operations and state.
- FloorManager: Handles individual floor operations and state.
- Elevator: Represents an elevator request with capacity, state and direction
- Floor: Represents a floor request with amount of passangers going up and going down.
- Passenger: Represents a passenger's request with origin and destination floors.
- Console: Provides the command-line interface for user interaction.
