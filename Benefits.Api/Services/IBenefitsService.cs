using Benefits.Api.Models.Api;
using Benefits.Api.Models.Domain;

namespace Benefits.Api.Services
{
    public interface IBenefitsService
    {
        Task<Models.Data.Dependent> CreateDependentAsync(CreateDependentRequest request);
        Task<Models.Data.Dependent> UpdateDependentAsync(UpdateDependentRequest request);
        Task DeleteDependentAsync(DeleteDependentRequest request);
        Task<IEnumerable<Models.Data.Dependent>> GetDependentsAsync(long employeeId);
        Task<BenefitsCost> GetBenefitsCostAsync(long employeeId, long companyId);
    }
}