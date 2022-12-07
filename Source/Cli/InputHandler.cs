using Analyzers;
using Queries.NonDatabase;

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

    /// <summary>
    /// Handle input.
    /// </summary>
    public void Handle()
    {
        var analyzeModel = new List<RegistryParameterValueQueryModel>
        {
            new ()
            {
                KeyName = "null",
                ValueName = "null",
            },
        };

        var analyzeResult = HardwareAnalyzer.Analyze(analyzeModel);
        foreach (string recommendation in analyzeResult.Data)
        {
            Console.WriteLine(recommendation);
        }
    }
}