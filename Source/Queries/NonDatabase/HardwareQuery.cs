using Queries.Base;

namespace Queries.NonDatabase;

/// <summary>
/// Hardware PnPEntity query.
/// </summary>
public class HardwareQuery : NonDbQueryBase<List<string>?, List<WmiHardware>>
{
    private const string Undefined = "";

    /// <summary>
    /// Initializes a new instance of the <see cref="HardwareQuery"/> class.
    /// </summary>
    public HardwareQuery()
    {
    }

    /// <inheritdoc/>
    protected override QueryResult<List<WmiHardware>> ExecuteCore(List<string>? hardwareIds)
    {
        var pnpDevices = new List<WmiHardware>();
        foreach (dynamic item in WmiQuery.GetAllObjects(Win32.PnPEntity))
        {
            var pnpDevice = new WmiHardware
            {
                Id = (item?.HardwareID as string[])?.MaxBy(id => id.Length) ?? Undefined,
                Name = item?.Caption ?? Undefined,
                Description = item?.Description ?? Undefined,
                Manufacturer = item?.Manufacturer ?? Undefined,
            };

            pnpDevices.Add(pnpDevice);
        }

        if (hardwareIds is not null)
        {
            pnpDevices = pnpDevices.Where(entity => hardwareIds.Contains(entity.Id)).ToList();
        }

        return GetSuccessfulResult(pnpDevices);
    }
}