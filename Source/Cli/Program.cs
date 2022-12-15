using System.Security.Principal;
using Analyzers;
using Commands;
using Core;
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
        var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
        bool hasAdministrativeRight = principal.IsInRole(WindowsBuiltInRole.Administrator);
        if (!hasAdministrativeRight)
        {
            Console.WriteLine(@"Программа запущена без прав администратора."
                              + "\n" + @"Перезапустите программу с правами администратора");
            Console.ReadLine();
            return;
        }

        ConfigureConsole();
        ConfigureExceptionsHandling();
        ConfigureServices();
        RunDbUpdater();
        RunWatchers();
        RunInputHandler();
    }

    private static void ConfigureConsole()
    {
        Console.Title = Constants.ProgramName;
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

    private static void RunDbUpdater()
    {
        Console.WriteLine(@"Обновление базы данных...");
        var dbUpdaterFromScvFiles = ServiceProvider.GetRequiredService<DbUpdaterFromScvFiles>();
        string updateResult = dbUpdaterFromScvFiles.UpdateAll();
        Console.WriteLine(@"База данных обновлена!");

        // Console.WriteLine(updateResult);
        // CommonDisplay.DisplayContinueMessage();
    }

    private static void RunWatchers()
    {
        ServiceProvider.GetRequiredService<HardwareWatcher>();
        Console.WriteLine(@"Модуль слежения за подключаемыми устройствами запущен.");
        Thread.Sleep(2000);
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