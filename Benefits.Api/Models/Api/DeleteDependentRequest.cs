using System.ComponentModel.DataAnnotations;

namespace Benefits.Api.Models.Api
{
    public class DeleteDependentRequest
    {
        [Required]
        public string DependentId { get; set; }
        [Required]
        public long EmployeeId { get; set; }
    }
}