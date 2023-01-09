namespace BenefitsApiInfrastructure.Settings
{
    public class AppSettings
    {
        public string ApiLambdaName { get; set; }
        public string ApiGatewayName { get; set; }
        public string EmployeeTableName { get; set; }
        public string DependentTableName { get; set; }
        public string ComputeStackName { get; set; }
        public string DataStackName { get; set; }
    }
}