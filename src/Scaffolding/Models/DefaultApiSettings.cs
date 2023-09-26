namespace Scaffolding.Models
{
    public class DefaultApiSettings
    {
        public string EnvironmentVariablesPrefix { get; private set; }

        public DefaultApiSettings(string environmentVariablesPrefix)
        {
            EnvironmentVariablesPrefix = environmentVariablesPrefix;
        }
    }
}
