using Analyzers;
using Commands;
using Dal;
using Microsoft.Extensions.DependencyInjection;
using Queries;

namespace Cli;

/// <summary>
/// The main entry point for the application.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Service provider.
    /// </summary>
    private static IServiceProvider ServiceProvider { get; set; } = null!;

    private static void Main(string[] args)
    {
        ConfigureConsole();
        ConfigureExceptionsHandling();
        ConfigureServices();
        RunInputHandler();
    }

    private static void ConfigureConsole()
    {
        Console.Title = "Endpoint analyzer 2022";
        Console.WriteLine(@"Запуск...");
    }

    private static void ConfigureExceptionsHandling()
    {
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionThrowing;
        TaskScheduler.UnobservedTaskException += UnobservedTaskExceptionThrowing;
    }

    private static void ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddCli();
        services.AddDal();
        services.AddCore();
        services.AddQueries();
        services.AddCommands();
        services.AddAnalyzers();
        ServiceProvider = services.BuildServiceProvider();
    }

    private static void RunInputHandler()
    {
        var inputHandler = ServiceProvider.GetRequiredService<InputHandler>();
        inputHandler.Handle();
    }

    private static void UnhandledExceptionThrowing(object sender, UnhandledExceptionEventArgs e)
    {
        string errorMessage = @$"Произошло необработанное исключение: {e.ExceptionObject}";
        Console.WriteLine(errorMessage);
    }

    private static void UnobservedTaskExceptionThrowing(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        string errorMessage = @$"Произошло необработанное исключение: {e.Exception}";
        Console.WriteLine(errorMessage);
        e.SetObserved();
    }
}