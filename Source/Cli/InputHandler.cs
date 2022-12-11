namespace Cli;

/// <summary>
/// Input handler.
/// </summary>
public class InputHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputHandler"/> class.
    /// </summary>
    /// <param name="hardwareAnalyzer">Hardware analyzer.</param>
    public InputHandler(HardwareAnalyzer hardwareAnalyzer)
    {
        HardwareAnalyzer = hardwareAnalyzer;
    }

    private HardwareAnalyzer HardwareAnalyzer { get; }

    private static readonly List<string> MenuItems = new()
    {
        "Проверка параметров учётных записей",
        "анализ 2",
        "анализ 3",
        "Выйти",
    };

    private static readonly List<string> MenuItemsSoftware = new()
    {
        "Анализ 1",
        "анализ 2",
        "Назад",
    };

    private Drawer Drawer { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputHandler"/> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider.</param>
    public InputHandler(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    private IServiceProvider ServiceProvider { get; set; }

    /// <summary>
    /// Handle input.
    /// </summary>
    public void Handle()
    {
        var dbUpdaterFromScvFiles = ServiceProvider.GetRequiredService<DbUpdaterFromScvFiles>();
        string result = dbUpdaterFromScvFiles.UpdateAll();
        Console.WriteLine(result);
        while (true)
        {
            DrawMainMenu();
        }
    }


    public void DrawMainMenu()
    {
        int selectedMenuItem = Drawer.DrawMenu(MenuItems);
        switch (selectedMenuItem)
        {
            case -2:
            {
                Console.Clear();
            }
                break;
            case -1:
            {
                Console.Clear();
                Console.WriteLine("Выберите один вариант с помощью клавиш вверх/вниз и введите для подтверждения");
                Thread.Sleep(3000);
                Console.Clear();
                Drawer.DrawMenu(MenuItems);
            }
                break;
            case 0: //GPPARAM
            {
                DispayRecommendations(Analyzer, "");
            }
                break;
            case 1: // HARDWARE
            {
                DispayRecommendations(Analyzer, "");
            }
                break;
            case 2: // SOFTWARE
            {
                DrawSoftwareMenu();
            }
                break;
            default: // выход
            {
                Environment.Exit(0);
            }
                break;
        }
    }

    private void DrawSoftwareMenu()
    {
        while (true)
        {
            int selectedMenuItem = Drawer.DrawMenu(MenuItemsSoftware);
            switch (selectedMenuItem)
            {
                case -2:
                {
                    Console.Clear();
                }
                    break;
                case -1:
                {
                    Console.Clear();
                    Console.WriteLine("Выберите один вариант с помощью клавиш вверх/вниз и введите для подтверждения");
                    Thread.Sleep(3000);
                    Console.Clear();
                    Drawer.DrawMenu(MenuItemsSoftware);
                }
                    break;
                case 0: // SOFTWAREUPD
                {
                    DispayRecommendations(Analyzer, "");
                }
                    break;
                case 1: // SOFTWARE
                {
                    DispayRecommendations(Analyzer, "");
                }
                    break;
                default: // выход
                {
                    Console.Clear();
                    Drawer.DrawMenu(MenuItems);
                }
                    break;
            }
        }
    }

    private void DispayRecommendations(IAnalyzer<List<string>> analyzer, string analayzerName)
    {
        Console.Clear();
        Console.WriteLine($"{analayzerName} вывел следующие рекоммендации:");
        Console.Write(analyzer.Analyze());
        Console.WriteLine("\n_____________________________________");
        Console.WriteLine("Нажмите любую клавишу для продолжения");
        Thread.Sleep(3000);
        Console.ReadKey();
        Console.Clear();
    }
}