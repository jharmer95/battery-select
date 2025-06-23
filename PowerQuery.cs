using System.Runtime.InteropServices;

namespace BatterySelect
{
    internal static partial class PowerQuery
    {
        internal enum PowerMode
        {
            Efficiency,
            Balanced,
            Performance,
        }

        private const string _efficiencyGuid = "961cc777-2547-4f9d-8174-7d86181b8a7a";
        private const string _balancedGuid = "00000000-0000-0000-0000-000000000000";
        private const string _performanceGuid = "ded574b5-45a0-4f42-8737-46345c09c238";

        private static PowerMode _currentMode = ReadPowerMode();

        internal static PowerMode CurrentMode
        {
            get => _currentMode;

            set
            {
                try
                {
                    WritePowerMode(value);
                    _currentMode = value;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        internal static bool IsAcPower()
        {
            return System.Windows.SystemParameters.PowerLineStatus == System.Windows.PowerLineStatus.Online;
        }

        [LibraryImport("powrprof.dll", EntryPoint = "PowerSetActiveOverlayScheme")]
        private static partial uint PowerSetActiveOverlayScheme(Guid overlaySchemeGuid);

        [LibraryImport("powrprof.dll", EntryPoint = "PowerGetEffectiveOverlayScheme")]
        private static partial uint PowerGetEffectiveOverlayScheme(out Guid effectiveOverlayGuid);

        private static PowerMode StringToPowerMode(string s)
        {
            return s switch
            {
                _efficiencyGuid => PowerMode.Efficiency,
                _balancedGuid => PowerMode.Balanced,
                _performanceGuid => PowerMode.Performance,
                _ => throw new ArgumentException("Invalid PowerMode GUID supplied"),
            };
        }

        private static string PowerModeToString(PowerMode mode)
        {
            return mode switch
            {
                PowerMode.Efficiency => _efficiencyGuid,
                PowerMode.Balanced => _balancedGuid,
                PowerMode.Performance => _performanceGuid,
                _ => throw new ArgumentException("Invalid PowerMode supplied"),
            };
        }

        private static PowerMode ReadPowerMode()
        {
            var result = PowerGetEffectiveOverlayScheme(out var guid);

            if (result != 0)
            {
                throw new InvalidOperationException(
                    $"Failed to read power mode. Error code: {result}");
            }

            return StringToPowerMode(guid.ToString());
        }

        private static void WritePowerMode(PowerMode mode)
        {
            var guid = new Guid(PowerModeToString(mode));
            var result = PowerSetActiveOverlayScheme(guid);

            if (result != 0)
            {
                throw new InvalidOperationException(
                    $"Failed to set power mode. Error code: {result}");
            }

            if (ReadPowerMode() != mode)
            {
                throw new InvalidOperationException(
                    "Failed to verify that the power mode was set correctly.");
            }
        }
    }
}
