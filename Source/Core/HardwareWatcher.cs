using System.Diagnostics.CodeAnalysis;
using System.Management;
using Analyzers;
using Queries.NonDatabase;

namespace Core;

/// <summary>
/// Hardware watcher.
/// </summary>
[SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Pending>")]
public class HardwareWatcher
{
    private const string Undefined = "";
    private const string LogFileName = @"Журнал подключенных устройств.txt";
    private readonly ManagementScope scope = new (@"root\CIMV2");
    private readonly WqlEventQuery insertQuery = new ()
    {
        EventClassName = "__InstanceCreationEvent",
        WithinInterval = new TimeSpan(0, 0, 3),
        Condition = @"TargetInstance ISA 'Win32_DiskDrive'",
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="HardwareWatcher"/> class.
    /// </summary>
    /// <param name="connectedHardwareAnalyzer">Connected hardware analyzer.</param>
    /// <param name="logger">Logger.</param>
    public HardwareWatcher(ConnectedHardwareAnalyzer connectedHardwareAnalyzer, Logger logger)
    {
        ConnectedHardwareAnalyzer = connectedHardwareAnalyzer;
        Logger = logger;
        StartWatcher(insertQuery, HardwareConnected);
    }

    private ConnectedHardwareAnalyzer ConnectedHardwareAnalyzer { get; }

    private Logger Logger { get; }

    private void StartWatcher(EventQuery query, EventArrivedEventHandler handler)
    {
        using var watcher = new ManagementEventWatcher(scope, query);
        watcher.Options.Timeout = ManagementOptions.InfiniteTimeout;
        watcher.EventArrived += handler;
        watcher.Start();
    }

    private void HardwareConnected(object sender, EventArrivedEventArgs e)
    {
        var hardware = (ManagementBaseObject)e.NewEvent["TargetInstance"];
        string pnpDeviceId = hardware.Properties["PNPDeviceID"].Value.ToString() ?? Undefined;
        dynamic? pnpEntity = WmiQuery
            .GetAllObjects(Win32.PnPEntity)
            .Cast<dynamic>()
            .FirstOrDefault(item => item.PNPDeviceID == pnpDeviceId);

        string hardwareId = (pnpEntity?.HardwareID as string[])?.MaxBy(id => id.Length) ?? Undefined;
        var analyzeResult = ConnectedHardwareAnalyzer.Analyze(hardwareId);
        bool isTrustedHardware = analyzeResult.Data;
        if (isTrustedHardware)
        {
            return;
        }

        Console.Clear();
        string hardwareName = hardware.Properties["Caption"].Value.ToString() ?? Undefined;
        string hardwareDescription = hardware.Properties["Description"].Value.ToString() ?? Undefined;
        string hardwareManufacturer = hardware.Properties["Manufacturer"].Value.ToString() ?? Undefined;
        string hardwareModel = hardware.Properties["Model"].Value.ToString() ?? Undefined;
        string hardwareInterfaceType = hardware.Properties["InterfaceType"].Value.ToString() ?? Undefined;
        string recommendation =
            @"Подключено недоверенное устройство:"
            + "\n\n"
            + @"Идентификатор: " + hardwareId + "\n"
            + @"Название: " + hardwareName + "\n"
            + @"Описание: " + hardwareDescription + "\n"
            + @"Производитель: " + hardwareManufacturer + "\n"
            + @"Модель: " + hardwareModel + "\n"
            + @"Интерфейс подключения: " + hardwareInterfaceType;

        Console.WriteLine(recommendation);
        string logResult = Logger.Log(new List<string> { recommendation }, LogFileName);
        Console.WriteLine("\n\n" + logResult);
        Console.WriteLine("\n\n_____________________________________");
        Console.WriteLine(@"Нажмите любую клавишу для продолжения");
    }
}