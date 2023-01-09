using Amazon.DynamoDBv2.DataModel;

namespace Benefits.Api.Models.Data
{
    [DynamoDBTable("Dependent")]
    public class Dependent
    {
        [DynamoDBHashKey]
        public string? DependentId { get; set; }
        [DynamoDBRangeKey]
        public long EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}