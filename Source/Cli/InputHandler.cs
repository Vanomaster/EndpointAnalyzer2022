using Analyzers;
using Core;
using Microsoft.Extensions.DependencyInjection;

namespace Cli;

/// <summary>
/// Input handler.
/// </summary>
public class InputHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputHandler"/> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider.</param>
    public InputHandler(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    private IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Handle input.
    /// </summary>
    public void Handle()
    {
        var service = ServiceProvider.GetRequiredService<GpParametersAnalyzer>();
        var result = service.Analyze();
        if (!result.IsSuccessful)
        {
            Console.WriteLine(result.ErrorMessage);
            return;
        }

        foreach (string data in result.Data)
        {
            Console.WriteLine(data);
        }
    }
}