namespace StephanHooft.Dialogue
{
    /// <summary>
    /// An enum to describe differen degrees of debug feedback.
    /// </summary>
    public enum DebugMode
    {
        /// <summary>
        /// Show no debug messages.
        /// </summary>
        None,

        /// <summary>
        /// Show a minimal amount of debug messages.
        /// </summary>
        Minimal,

        /// <summary>
        /// Show all debug messages.
        /// </summary>
        Full,
    }

    public static class DebugModeExtensions
    {
        /// <summary>
        /// Returns <see cref="true"/> if the <see cref="DebugMode"/> has value "Full".
        /// </summary>
        public static bool Full(this DebugMode debugMode)
            => debugMode == DebugMode.Full;

        /// <summary>
        /// Returns <see cref="true"/> if the <see cref="DebugMode"/> has value "Minimal".
        /// </summary>
        public static bool Minimal(this DebugMode debugMode)
            => debugMode == DebugMode.Minimal;

        /// <summary>
        /// Returns <see cref="true"/> if the <see cref="DebugMode"/> has value "None".
        /// </summary>
        public static bool None(this DebugMode debugMode)
            => debugMode == DebugMode.None;
    }
}
