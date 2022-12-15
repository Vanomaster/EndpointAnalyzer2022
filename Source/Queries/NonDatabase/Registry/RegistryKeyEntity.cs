using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace Queries.NonDatabase;

/// <summary>
/// Wraps a RegistryKey object and corresponding last write time.
/// </summary>
/// <remarks>
/// .NET doesn't expose the last write time for a registry key 
/// in the RegistryKey class, so P/Invoke is required.
/// </remarks>
public class RegistryKeyEntity
{
    #region P/Invoke Declarations
    // This declaration is intended to be used for the last write time only. int is used
    // instead of more convenient types so that dummy values of 0 reduce verbosity.
    [DllImport("advapi32.dll", EntryPoint = "RegQueryInfoKey", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
    private static extern int RegQueryInfoKey(
        SafeRegistryHandle hkey,
        int lpClass,
        int lpcbClass,
        int lpReserved,
        int lpcSubKeys,
        int lpcbMaxSubKeyLen,
        int lpcbMaxClassLen,
        int lpcValues,
        int lpcbMaxValueNameLen,
        int lpcbMaxValueLen,
        int lpcbSecurityDescriptor,
        IntPtr lpftLastWriteTime);
    #endregion

    #region Public Poperties

    /// <summary>
    /// Creates and initializes a new RegistryKeyInfo object from the provided RegistryKey object.
    /// </summary>
    /// <param name="key">RegistryKey component providing a handle to the key.</param>
    public RegistryKeyEntity(RegistryKey key)
    {
        Key = key;
        SetLastWriteTime();
    }

    /// <summary>
    /// Creates and initializes a new RegistryKeyInfo object from a registry key path string.
    /// </summary>
    /// <param name="parent">Parent key for the key being loaded.</param>
    /// <param name="keyName">Path to the registry key.</param>
    public RegistryKeyEntity(RegistryKey parent, string keyName)
        : this(parent.OpenSubKey(keyName))
    {
    }

    /// <summary>
    /// Gets the registry key owned by the info object.
    /// </summary>
    public RegistryKey Key { get; private set; }

    /// <summary>
    /// Gets the last write time for the corresponding registry key.
    /// </summary>
    public DateTime LastWriteTime { get; private set; }

    #endregion

    /// <summary>
    /// Queries the currently set registry key through P/Invoke for the last write time.
    /// </summary>
    private void SetLastWriteTime()
    {
        Debug.Assert(Key != null, "RegistryKey component must be initialized");
        var pin = new GCHandle();
        const long lastWriteTime = 0;

        try
        {
            pin = GCHandle.Alloc(lastWriteTime, GCHandleType.Pinned);

            if (RegQueryInfoKey(Key.Handle, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, pin.AddrOfPinnedObject()) == 0)
            {
                LastWriteTime = DateTime.FromFileTime((long)pin.Target);
            }
            else
            {
                LastWriteTime = DateTime.MinValue;
            }
        }
        finally
        {
            if (pin.IsAllocated)
            {
                pin.Free();
            }
        }
    }
}