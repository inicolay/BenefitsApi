namespace Benefits.Api.Models.Domain
{
    public class Employee : Person
    {
        public long EmployeeId { get; set; }
        public long CompanyId { get; set; }
        public IEnumerable<Dependent>? Dependents { get; set; }
        public decimal AnnualSalary { get; set; }

        public Employee(long employeeId, long companyId, string firstName, string lastName, 
            IEnumerable<Dependent> dependents, decimal annualSalary)
        {
            FirstName = firstName;
            LastName = lastName;
            Dependents = dependents;
            EmployeeId = employeeId;
            CompanyId = companyId;
            AnnualSalary = annualSalary;
        }
    }
}