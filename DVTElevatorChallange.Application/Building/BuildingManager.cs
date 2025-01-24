using DVTElevatorChallange.Application.ElevatorManager;

namespace DVTElevatorChallange.Application.Building
{
    public class BuildingManager : IBuildingManager
    {
        private IElevatorManager _elevatorManager;

        public BuildingManager(IElevatorManager elevatorManager)
        {
            _elevatorManager = elevatorManager;
        }

        public void SetElevators()
        {
            _elevatorManager.AddElevators(3);
        }

        public void Setloors()
        {

        }
    }
}
