using System.ComponentModel.DataAnnotations;

namespace Benefits.Api.Models.Api
{
    public class CreateDependentRequest
    {
        [Required]
        public long EmployeeId { get; set; }

        [Required]
        public string FirstName { get; set; } = String.Empty;

        [Required]
        public string LastName { get; set; } = String.Empty;
    }
}