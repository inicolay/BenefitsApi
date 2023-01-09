using Benefits.Api.Models.Api;

namespace Benefits.Api.Services
{
    public interface IEmployeeService
    {
        Task CreateEmployeeAsync(CreateEmployeeRequest request);
    }
}