using Analyzers;
using Analyzers.Base;
using Core;
using Microsoft.Extensions.DependencyInjection;

namespace Cli;

/// <summary>
/// Input handler.
/// </summary>
public class InputHandler
{
    private static readonly List<string> MenuItems = new()
    {
        "Проверка параметров учётных записей",
        "анализ 2",
        "анализ 3",
        "Выйти",
    };

    private static readonly List<string> SoftwareMenuItems = new()
    {
        "Анализ 1",
        "анализ 2",
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
        while (true)
        {
            DrawMainMenu();
        }
    }

    private void DrawMainMenu()
    {
        Console.Clear();
        int selectedMenuItem = Drawer.DrawMenu(MenuItems);
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

            case 0: //GPPARAM
            {
                var service = ServiceProvider.GetRequiredService<GpParametersAnalyzer>();
                DisplayRecommendations(service, "Анализатор параметров групповых политик");
                break;
            }

            case 1: // HARDWARE
            {
                //DisplayRecommendations(Analyzer, "");
                break;
            }

            case 2: // SOFTWARE
            {
                Console.Clear();
                DrawSoftwareMenu();
                break;
            }

            default: // выход
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

                case 0: // SOFTWAREUPD
                {
                    // DisplayRecommendations(Analyzer, "");
                    break;
                }

                case 1: // SOFTWARE
                {
                    // DisplayRecommendations(Analyzer, "");
                    break;
                }

                default: // выход
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

        Console.WriteLine($"{analayzerName} вывел следующие рекоммендации:");

        foreach (string data in result.Data)
        {
            Console.WriteLine(data);
        }

        Console.WriteLine("\n_____________________________________");
        Console.WriteLine("Нажмите любую клавишу для продолжения");
        Thread.Sleep(3000);
        Console.ReadKey();
        Console.Clear();
    }

    private void DisplayInputError()
    {
        Console.Clear();
        Console.WriteLine("Выберите один вариант с помощью клавиш вверх/вниз и введите для подтверждения");
        Thread.Sleep(3000);
        Console.Clear();
    }
}