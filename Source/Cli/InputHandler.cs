using Core;
using Microsoft.Extensions.DependencyInjection;

using Analyzers;

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

    private IServiceProvider ServiceProvider { get; set; }

    /// <summary>
    /// Handle input.
    /// </summary>
    public void Handle()
    {
        var dbUpdaterFromScvFiles = ServiceProvider.GetRequiredService<DbUpdaterFromScvFiles>();
        string result = dbUpdaterFromScvFiles.UpdateAll();
        Console.WriteLine(result);
    }
}