namespace CuteNoisesBot
{
    public static class Globals
    {
        public static bool SFWMode { get; private set; } = false;

        public static void SetSFW(bool mode)
        {
            SFWMode = mode;
        }

        public static void ToggleSFW()
        {
            SFWMode = !SFWMode;
        }

        public static string ActivityStatus()
        {
            return SFWMode ? "!noise (SFW Mode)" : "!noise";
        }

        public static string SFWMessage()
        {
            return SFWMode ? "SFW Mode Enabled!" : "SFW Mode Disabled!";
        }
    }
}