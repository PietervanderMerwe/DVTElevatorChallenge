using DVTElevatorChallange.Core.Entities;
using System.Security.Cryptography;

namespace DVTElevatorChallange.Application.FloorManager
{
    public interface IFloorManager
    {
        bool AddFloors(int floorCount);
        void ClearUpQueue(int floorNumber);
        void ClearDownQueue(int floorNumber);
        bool AddPassenger(Passenger passenger, int currentFloor);
        void AddElevatorToStoppedElevators(Elevator elevator, int floorNumber);
        void RemoveElevatorFromStoppedElevators(Elevator elevator, int floorNumber);
    }
}
