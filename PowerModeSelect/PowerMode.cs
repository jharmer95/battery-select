namespace PowerModeSelect
{
    public enum PowerMode
    {
        Efficiency,
        Balanced,
        Performance,
    }

    public static class PowerModeExtensions
    {
        private const string _efficiencyGuid = "961cc777-2547-4f9d-8174-7d86181b8a7a";
        private const string _balancedGuid = "00000000-0000-0000-0000-000000000000";
        private const string _performanceGuid = "ded574b5-45a0-4f42-8737-46345c09c238";

        public static string ToGuidString(this PowerMode mode)
        {
            return mode switch
            {
                PowerMode.Efficiency => _efficiencyGuid,
                PowerMode.Balanced => _balancedGuid,
                PowerMode.Performance => _performanceGuid,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
            };
        }

        public static Guid ToGuid(this PowerMode mode)
        {
            return new Guid(mode.ToGuidString());
        }

        public static PowerMode FromGuidString(string guid)
        {
            return guid switch
            {
                _efficiencyGuid => PowerMode.Efficiency,
                _balancedGuid => PowerMode.Balanced,
                _performanceGuid => PowerMode.Performance,
                _ => throw new ArgumentException("Invalid PowerMode GUID supplied", nameof(guid)),
            };
        }

        public static PowerMode FromGuid(Guid guid)
        {
            return FromGuidString(guid.ToString());
        }

        public static string ToFriendlyName(this PowerMode mode)
        {
            return mode switch
            {
                PowerMode.Efficiency => "Efficiency",
                PowerMode.Balanced => "Balanced",
                PowerMode.Performance => "Performance",
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
            };
        }
    }
}
