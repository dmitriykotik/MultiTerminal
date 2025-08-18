using MultiTerminal.Logger;
using MultiTerminal.Output;
using static MultiTerminal.InternalVars;

namespace MultiTerminal
{
    internal class GraphicException : IException
    {
        public void Start(Exception ex)
        {
            log.Write(LogType.Exception, $"Failed to load the pseudo-graphical window. Exception: {ex.Message}. Trace: {ex.StackTrace}");
            output.WriteLine("Failed to load the pseudo-graphical window! Check the log file.", ConsoleColor.Red, null);
        }
    }
}
