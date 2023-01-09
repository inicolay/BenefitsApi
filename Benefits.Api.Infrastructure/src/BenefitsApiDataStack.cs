using Amazon.CDK;
using BenefitsApiInfrastructure.Settings;
using Constructs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BenefitsApiInfrastructure
{
    public class BenefitsApiDataStack : Stack
    {
        internal BenefitsApiDataStack(AppSettings settings, Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // todo add Employee and Dependent dynamo tables here
        }
    }
}
