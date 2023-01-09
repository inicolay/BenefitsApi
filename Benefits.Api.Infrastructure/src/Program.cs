using Amazon.CDK;
using BenefitsApiInfrastructure.Settings;
using Microsoft.Extensions.Configuration;

namespace BenefitsApiInfrastructure
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();

            var config = new ConfigurationBuilder()
                .AddJsonFile("infrastructure.appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var settings = new AppSettings(); 
            config.Bind("AppSettings", settings);

            new BenefitsApiComputeStack(settings, app, settings.ComputeStackName, new StackProps { });
            new BenefitsApiDataStack(settings, app, settings.DataStackName, new StackProps { });
            app.Synth();
        }
    }
}