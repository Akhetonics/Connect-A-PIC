namespace CAP_Contracts.Logger
{
    public enum LogLevel
    {
        Trace,  // Very detailed information, mainly for developers
        Debug,  // Information that is useful for diagnosing issues
        Info,   // General runtime information
        Warn,   // Something unusual happened, but the application continues to run
        Error,  // An error occurred that affects the execution of a function
        Fatal   // A critical error that can cause the application to crash
    }
}
