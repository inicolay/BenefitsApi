using Amazon.DynamoDBv2.DataModel;

namespace Benefits.Api.Models.Data
{
    [DynamoDBTable("Employee")]
    public class Employee
    {
        [DynamoDBHashKey]
        public long EmployeeId { get; set; }
        public long CompanyId { get; set; }
        public decimal AnnualSalary { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public Employee() 
        {
        }
    }
}