using DVTElevatorChallange.Core.Entities;
using DVTElevatorChallange.Domain.Enum;
using System.Security.Cryptography;

namespace DVTElevatorChallange.Application.FloorManager
{
    public interface IFloorManager
    {
        bool AddFloors(int floorCount);
        int GetAmountOfFloors();
        int GetRemainingQueueCount(int floorNumber, Direction direction);
        void ClearUpQueue(int floorNumber);
        void ClearDownQueue(int floorNumber);
        bool AddPassenger(int totalPassengers, int currentFloor, int destinationFloor);
        void AddElevatorToStoppedElevators(Elevator elevator, int floorNumber);
        void RemoveElevatorFromStoppedElevators(Elevator elevator, int floorNumber);
        Direction DetermineDirection(Elevator elevator, int floorNumber);
        List<Passenger> LoadUpQueuePassengers(int floorNumber, int passengerAmount);
        List<Passenger> LoadDownQueuePassengers(int floorNumber, int passengerAmount);
    }
}
