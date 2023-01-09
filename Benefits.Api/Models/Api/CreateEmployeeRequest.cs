using System.ComponentModel.DataAnnotations;

namespace Benefits.Api.Models.Api
{
    public class CreateEmployeeRequest
    {
        [Required]
        public long EmployeeId { get; set; }

        [Required]
        public long CompanyId { get; set; }
        
        [Required]
        public decimal AnnualSalary { get; set; }

        [Required]
        public string FirstName { get; set; } = String.Empty;

        [Required]
        public string LastName { get; set; } = String.Empty;
    }
}