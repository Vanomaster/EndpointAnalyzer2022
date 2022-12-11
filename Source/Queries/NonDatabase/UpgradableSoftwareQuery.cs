using Queries.Base;
using WGetNET;

namespace Queries.NonDatabase;

/// <inheritdoc />
public class UpgradableSoftwareQuery : NonDbQueryBase<List<string>>
{
    /// <inheritdoc/>
    protected override QueryResult<List<string>> ExecuteCore()
    {
        WinGetPackageManager manager = new();
        var upgradeablePackagesResult = manager.GetUpgradeablePackages();
        var upgradableSoftware = upgradeablePackagesResult
            .Select(package => package.PackageName + package.PackageId + " " + package.PackageVersion)
            .ToList();

        return GetSuccessfulResult(upgradableSoftware);
    }
}