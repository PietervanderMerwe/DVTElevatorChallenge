using DVTElevatorChallange.Application.ElevatorManager;
using DVTElevatorChallange.Application.FloorManager;
using DVTElevatorChallange.Domain.Interface;
using DVTElevatorChallange.Infrastructure.Service;
using Microsoft.Extensions.DependencyInjection;

namespace DVTElevatorChallenge.Presentation.Configurations
{
    internal class DependencyInjection
    {
        public ServiceProvider SetupDI()
        {
            return new ServiceCollection()
                .AddSingleton<IElevatorManager, ElevatorManager>()
                .AddSingleton<IFloorManager, FloorManager>()
                .AddSingleton<ILoggerService, LoggerService>()
                .AddTransient<ElevatorConsole>()
                .BuildServiceProvider();
        }
    }
}
