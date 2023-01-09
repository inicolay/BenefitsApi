using Benefits.Api.Models.Api;
using Dynamo.Common;

namespace Benefits.Api.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IDynamoRepository _dynamoRepo;
        
        public EmployeeService(IDynamoRepository dynamoRepository)
        {
            _dynamoRepo = dynamoRepository;
        }

        public async Task CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            var employee = new Models.Data.Employee()
            {
                EmployeeId = request.EmployeeId,
                CompanyId = request.CompanyId,
                AnnualSalary = request.AnnualSalary,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            await _dynamoRepo.SaveItemAsync<Models.Data.Employee>(employee);
        }
    }
}