using DVTElevatorChallenge.Presentation;
using DVTElevatorChallenge.Presentation.Configurations;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        var DI = new DependencyInjection();

        using var scope = DI.SetupDI().CreateScope();
        var elevatorConsole = scope.ServiceProvider.GetService<ElevatorConsole>();
        try
        {
            await elevatorConsole.RunAsync(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

    }
}
