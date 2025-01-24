using Microsoft.Extensions.DependencyInjection;

namespace DVTElevatorChallenge.Presentation.Configurations
{
    internal class DependencyInjection
    {
        public ServiceProvider SetupDI()
        {
            return new ServiceCollection()
            .AddTransient<ElevatorConsole>()
            .BuildServiceProvider();
        }
    }
}
