using System.Runtime.InteropServices;

namespace PowerModeSelect;

public static partial class PowerSystem
{
    private enum ACLineStatus : byte
    {
        Offline = 0,
        Online = 1,
        Unknown = 255,
    }

    private enum BatteryFlag : byte
    {
        High = 1,
        Low = 2,
        Critical = 4,
        Charging = 8,
        NoBattery = 128,
        Unknown = 255,
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PowerState
    {
        public ACLineStatus ACLineStatus;
        public BatteryFlag BatteryFlag;
        public Byte BatteryLifePercent;
        public Byte Reserved1;
        public Int32 BatteryLifeTime;
        public Int32 BatteryFullLifeTime;
    }

    [DllImport("Kernel32", EntryPoint = "GetSystemPowerStatus")]
    private static extern bool GetSystemPowerStatus(out PowerState powerState);

    [LibraryImport("powrprof.dll", EntryPoint = "PowerSetActiveOverlayScheme")]
    private static partial uint PowerSetActiveOverlayScheme(Guid overlaySchemeGuid);

    [LibraryImport("powrprof.dll", EntryPoint = "PowerGetEffectiveOverlayScheme")]
    private static partial uint PowerGetEffectiveOverlayScheme(out Guid effectiveOverlayGuid);

    public static bool IsAcPower()
    {
        if (GetSystemPowerStatus(out PowerState powerState))
        {
            return powerState.ACLineStatus == ACLineStatus.Online;
        }

        throw new InvalidOperationException("Unable to retrieve system power status.");
    }

    public static PowerMode GetCurrentPowerMode()
    {
        var result = PowerGetEffectiveOverlayScheme(out Guid guid);

        if (result != 0)
        {
            throw new InvalidOperationException($"Failed to retrieve the effective power mode. Error code: {result}");
        }

        return PowerModeExtensions.FromGuid(guid);
    }

    public static void SetPowerMode(PowerMode powerMode)
    {
        var result = PowerSetActiveOverlayScheme(powerMode.ToGuid());

        if (result != 0)
        {
            throw new InvalidOperationException($"Failed to set the active power mode. Error code: {result}");
        }
    }
}
