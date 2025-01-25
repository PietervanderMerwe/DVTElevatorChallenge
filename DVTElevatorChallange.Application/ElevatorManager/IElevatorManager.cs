using DVTElevatorChallange.Core.Entities;
using DVTElevatorChallange.Domain.Enum;

namespace DVTElevatorChallange.Application.ElevatorManager
{
    public interface IElevatorManager
    {
        bool AddElevators(int elevatorCount);
        void AddPassengerToElevator(Passenger passenger, int elevatorId);
        void RemovePassenger(Passenger passenger, int elevatorId);
        Elevator GetBestElevatorToDispatch(int floorNum, Direction direction);
        bool IsMovingTowardFloor(Elevator elevator, int floor);
    }
}
