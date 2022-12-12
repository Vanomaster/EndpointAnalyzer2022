using Analyzers;
using Analyzers.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Cli;

/// <summary>
/// Input handler.
/// </summary>
public class InputHandler
{
    private static readonly List<string> MenuItems = new()
    {
        "Запустить анализ параметров групповых политик",
        "Запустить анализ подключённых устройств",
        "Запустить анализ программного обеспечения",
        "Выйти",
    };

    private static readonly List<string> SoftwareMenuItems = new()
    {
        "Запустить анализ обновлений программного обеспечения",
        "Запустить анализ довереного программного обеспечения",
        "Назад",
    };


    /// <summary>
    /// Initializes a new instance of the <see cref="InputHandler"/> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider.</param>
    public InputHandler(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    private IServiceProvider ServiceProvider { get; }

    private Drawer Drawer { get; set; }

    /// <summary>
    /// Handle input.
    /// </summary>
    public void Handle()
    {
        Console.Title = "Endpoint analyzer 2022";
        while (true)
        {
            DrawMainMenu();
        }
    }

    private void DrawMainMenu()
    {
        Console.Clear();
        var selectedMenuItem = Drawer.DrawMenu(MenuItems);
        switch (selectedMenuItem)
        {
            case -2:
            {
                Console.Clear();
                break;
            }

            case -1:
            {
                DisplayInputError();
                break;
            }

            case 0:
            {
                var service = ServiceProvider.GetRequiredService<GpParametersAnalyzer>();
                DisplayRecommendations(service, "анализатора параметров групповых политик");
                break;
            }

            case 1:
            {
                //DisplayRecommendations(Analyzer, "");
                break;
            }

            case 2:
            {
                Console.Clear();
                DrawSoftwareMenu();
                break;
            }

            default:
            {
                Environment.Exit(0);
                break;
            }
        }
    }

    private void DrawSoftwareMenu()
    {
        while (true)
        {
            Console.Clear();
            int selectedMenuItem = Drawer.DrawMenu(SoftwareMenuItems);
            switch (selectedMenuItem)
            {
                case -2:
                {
                    Console.Clear();
                    break;
                }

                case -1:
                {
                    DisplayInputError();
                    break;
                }

                case 0:
                {
                    var service = ServiceProvider.GetRequiredService<SoftwareUpdateAnalyzer>();
                    DisplayRecommendations(service, "анализатора обновлений ПО");
                    break;
                }

                case 1:
                {
                    var service = ServiceProvider.GetRequiredService<SoftwareTrustAnalyzer>();
                    DisplayRecommendations(service, "анализатора доверенных ПО");
                    break;
                }

                default:
                {
                    return;
                }
            }
        }
    }

    private void DisplayRecommendations(IAnalyzer<List<string>> analyzer, string analayzerName)
    {
        Console.Clear();
        var result = analyzer.Analyze();
        if (!result.IsSuccessful)
        {
            Console.WriteLine(result.ErrorMessage);
            return;
        }

        Console.WriteLine($"В результате работы {analayzerName} был получен следующий результат:\n");

        foreach (var data in result.Data)
        {
            Console.WriteLine(data);
        }

        Console.WriteLine("\n_____________________________________");
        Console.WriteLine("Нажмите любую клавишу для продолжения");
        Console.SetCursorPosition(0, 0);
        Thread.Sleep(3000);
        Console.ReadKey();
        Console.Clear();
    }

    private void DisplayInputError()
    {
        Console.Clear();
        Console.WriteLine("Выберите один вариант с помощью клавиш вверх/вниз и нажмите клавишу ввод для подтверждения");
        Thread.Sleep(3000);
        Console.Clear();
    }
}