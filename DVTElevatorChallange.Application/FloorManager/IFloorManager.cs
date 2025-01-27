using DVTElevatorChallange.Core.Entities;
using DVTElevatorChallange.Domain.Enum;

namespace DVTElevatorChallange.Application.FloorManager
{
    public interface IFloorManager
    {
        bool AddFloors(int floorCount);
        int GetTotalFloors();
        int GetRemainingQueueCount(int floorNumber, Direction direction);
        void AddPassenger(int totalPassengers, int currentFloor, int destinationFloor);
        void AddElevatorToStoppedElevators(Elevator elevator, int floorNumber);
        void RemoveElevatorFromStoppedElevators(Elevator elevator, int floorNumber);
        List<Elevator> GetElevatorFromStoppedElevators(int floorNumber);
        Direction DetermineDirection(Elevator elevator, int floorNumber);
        List<Passenger> LoadQueuePassengers(int floorNumber, int passengerAmount, Direction direction);
    }
}
