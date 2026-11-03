using ConfigGarden.Interfaces;

namespace ConfigGarden
{
    internal class LogWrapper : ILogger
    {
        private readonly ILogger logger = null;

        internal LogWrapper(ILogger logger) 
        {
            this.logger = logger;
        }

        public void Info(string message) => logger?.Info(message);
        public void Warn(string message) => logger?.Warn(message);
        public void Error(string message) => logger?.Error(message);
    }
}
