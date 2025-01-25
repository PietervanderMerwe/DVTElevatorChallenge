using DVTElevatorChallange.Core.Entities;
using DVTElevatorChallange.Domain.Enum;

namespace DVTElevatorChallange.Application.ElevatorManager
{
    public interface IElevatorManager
    {
        bool AddElevators(int elevatorCount);
        List<Elevator> GetAllElevators();
        Task MoveAllElevatorsToNextStopsAsync();
        void AddPassengerToElevator(Passenger passenger, int elevatorId);
        void RemovePassenger(Passenger passenger, int elevatorId);
        void DispatchElevatorToFloor(int floorNum, Direction direction);
        void ProcessFloorStop(Elevator elevator, int floorNum);

        bool IsAnyElevatorMoving();

    }
}
