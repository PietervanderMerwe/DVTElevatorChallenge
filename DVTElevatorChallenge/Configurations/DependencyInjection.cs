using DVTElevatorChallange.Application.Building;
using DVTElevatorChallange.Application.ElevatorManager;
using DVTElevatorChallange.Application.FloorManager;
using Microsoft.Extensions.DependencyInjection;

namespace DVTElevatorChallenge.Presentation.Configurations
{
    internal class DependencyInjection
    {
        public ServiceProvider SetupDI()
        {
            return new ServiceCollection()
                .AddSingleton<IBuildingManager, BuildingManager>()
                .AddSingleton<IElevatorManager, ElevatorManager>()
                .AddSingleton<IFloorManager, FloorManager>()
                .AddTransient<ElevatorConsole>()
                .BuildServiceProvider();
        }
    }
}
