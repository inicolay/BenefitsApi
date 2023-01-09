using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Apigatewayv2;
using Constructs;
using BenefitsApiInfrastructure.Settings;
using System.IO;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.APIGateway;

namespace BenefitsApiInfrastructure
{
    public class BenefitsApiComputeStack : Stack
    {
        public BenefitsApiComputeStack(AppSettings settings, Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            new Function(this, settings.ApiLambdaName, new FunctionProps
            {
                FunctionName = settings.ApiLambdaName,
                Code = Code.FromAsset(Path.Join(Directory.GetCurrentDirectory(), "/../Benefits.Api/bin/Release/net6.0/win-x64/publish/")),
                Handler = "Benefits.Api::Benefits.Api.LambdaEntryPoint::FunctionHandlerAsync",
                Runtime = Runtime.DOTNET_6,
                Description = "Benefits api lambda",
                Timeout = Duration.Seconds(30),
                MemorySize = 256
            });

            // todo add api gateway here
        }
    }
}