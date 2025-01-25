using DVTElevatorChallange.Application.ElevatorManager;
using DVTElevatorChallange.Application.FloorManager;

namespace DVTElevatorChallange.Application.Building
{
    public class BuildingManager : IBuildingManager
    {
        private IElevatorManager _elevatorManager;
        private IFloorManager _floorManager;

        public BuildingManager(IElevatorManager elevatorManager, IFloorManager floorManager)
        {
            _elevatorManager = elevatorManager;
            _floorManager = floorManager;
        }

        public bool CreeateBuilding(int floorCount, int elevatorCount)
        {
            try
            {
                _elevatorManager.AddElevators(elevatorCount);
                _floorManager.AddFloors(floorCount);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
