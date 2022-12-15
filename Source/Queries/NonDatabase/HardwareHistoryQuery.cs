using Queries.Base;

namespace Queries.NonDatabase;

/// <summary>
/// Usb hardware query.
/// </summary>
public class HardwareHistoryQuery : NonDbQueryBase<List<RegistryHardware>>
{
    private const string DescriptionSeparator = "%;";

    private readonly List<string> usbKeyNames = new () // \HKEY_LOCAL_MACHINE\
    {
        @"SYSTEM\CurrentControlSet\Enum\BTHENUM",
        @"SYSTEM\CurrentControlSet\Enum\BTHHFENUM",
        @"SYSTEM\CurrentControlSet\Enum\BTHLE",
        @"SYSTEM\CurrentControlSet\Enum\USB",
        @"SYSTEM\CurrentControlSet\Enum\USBSTOR",
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="HardwareHistoryQuery"/> class.
    /// </summary>
    /// <param name="registryParametersQueryByKeyName">Registry parameter query.</param>
    public HardwareHistoryQuery(RegistryParametersQueryByKeyName registryParametersQueryByKeyName)
    {
        RegistryParametersQueryByKeyName = registryParametersQueryByKeyName;
    }

    private RegistryParametersQueryByKeyName RegistryParametersQueryByKeyName { get; }

    /// <inheritdoc/>
    protected override QueryResult<List<RegistryHardware>> ExecuteCore()
    {
        var registryParameterQueryResult = RegistryParametersQueryByKeyName.Execute(usbKeyNames);
        if (!registryParameterQueryResult.IsSuccessful)
        {
            return GetFailedResult(registryParameterQueryResult.ErrorMessage);
        }

        var registryParametersGroupByKeyName = registryParameterQueryResult.Data.GroupBy(parameter => parameter.KeyName);

        var usbDevices = new List<RegistryHardware>();
        foreach (var parameterGroup in registryParametersGroupByKeyName)
        {
            var description = parameterGroup
                .FirstOrDefault(parameter => parameter.ParameterName == "DeviceDesc")
                .Value
                .ToString();
            if (description.Contains(DescriptionSeparator))
            {
                description = description[(description
                    .LastIndexOf(DescriptionSeparator, StringComparison.Ordinal) + DescriptionSeparator.Length) ..];
            }

            var usbDevice = new RegistryHardware
            {
                Id = parameterGroup.Where(parameter => parameter.ParameterName == "HardwareID")
                    .Select(parameter => (parameter.Value as string[])?.OrderByDescending(id => id.Length).First())
                    .FirstOrDefault()!,
                Name = parameterGroup.Where(parameter => parameter.ParameterName == "FriendlyName")
                    .Select(parameter => parameter.Value?.ToString())
                    .FirstOrDefault()!,
                Description = description,
                LastHardwareDataModifiedDateTime = parameterGroup.Select(parameter => parameter.LastWriteDateTime)
                    .FirstOrDefault(),
                Location = parameterGroup.Where(parameter => parameter.ParameterName == "LocationInformation")
                    .Select(parameter => parameter.Value?.ToString())
                    .FirstOrDefault()!,
            };

            usbDevices.Add(usbDevice);
        }

        usbDevices = usbDevices
            .OrderByDescending(usbDevice => usbDevice.LastHardwareDataModifiedDateTime)
            .ToList();

        return GetSuccessfulResult(usbDevices);
    }
}