using System.ComponentModel.DataAnnotations;

namespace Benefits.Api.Models.Api
{
    public class UpdateDependentRequest
    {
        [Required]
        public string DependentId { get; set; }
        [Required]
        public long EmployeeId { get; set; }
        
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}