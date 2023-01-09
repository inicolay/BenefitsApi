using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Benefits.Api.Models.Api;
using Benefits.Api.Models.Domain;
using Benefits.Api.Settings;
using Dynamo.Common;

namespace Benefits.Api.Services
{
    public class BenefitsService : IBenefitsService
    {
        private readonly IDynamoRepository _dynamoRepo;
        private readonly BenefitsSettings _settings;

        public BenefitsService(IDynamoRepository dynamoRepo, BenefitsSettings settings)
        {
            _dynamoRepo = dynamoRepo;
            _settings = settings;
        }

        public async Task<IEnumerable<Models.Data.Dependent>> GetDependentsAsync(long employeeId)
        {
            var scan = new ScanCondition("EmployeeId", Amazon.DynamoDBv2.DocumentModel.ScanOperator.Equal, employeeId);
            var dependents = await _dynamoRepo.GetAllItemsAsync<Models.Data.Dependent>(new List<ScanCondition>() { scan }, null);
            return dependents;
        }

        public async Task<Models.Data.Dependent> CreateDependentAsync(CreateDependentRequest request)
        {
            var dependent = new Models.Data.Dependent()
            {
                DependentId = Guid.NewGuid().ToString(),
                EmployeeId = request.EmployeeId,
                FirstName = request.FirstName,
                LastName = request.LastName
            };
            await _dynamoRepo.SaveItemAsync<Models.Data.Dependent>(dependent);
            return dependent;
        }

        public async Task<Models.Data.Dependent> UpdateDependentAsync(UpdateDependentRequest request)
        {
            var dependent = await _dynamoRepo.GetItemAsync<Models.Data.Dependent>(request.DependentId, request.EmployeeId);
            dependent.FirstName = request.FirstName;
            dependent.LastName = request.LastName;
            dependent.EmployeeId = request.EmployeeId;
            await _dynamoRepo.SaveItemAsync<Models.Data.Dependent>(dependent);
            return dependent;
        }

        public async Task DeleteDependentAsync(DeleteDependentRequest request)
        {
            await _dynamoRepo.DeleteItemAsync<Models.Data.Dependent>(request.DependentId, request.EmployeeId);
        }

        public async Task<BenefitsCost> GetBenefitsCostAsync(long employeeId, long companyId)
        {
            var employee = await GetEmployeeFromDataSource(employeeId, companyId);
            if (employee == null)
                return null;
            var annualCost = _settings.EmployeeCostAnnual;
            var discountAnnual = 0m;
            if(employee.Dependents.Any())
                annualCost += employee.Dependents.Count() * _settings.PerDependentCostAnnual;

            // verify with stakeholders if 'a' should also recieve a discount
            if (employee.FirstName.StartsWith('A')
                || employee.Dependents.Any(x => x.FirstName.StartsWith('A')))
                discountAnnual = (decimal)((float)annualCost * _settings.DiscountPercentage);

            // we are assuming the 10% discount is applied to the total annual cost after calculation
            // and that only 1 10% discount is applied even if multiple dependents and or the employee all start with 'A'
            // calarify with stakeholders that this is the intended logic. Another interpretation could be
            // 10% off employee cost if their name starts with 'A' and 10% off each dependent cost if the dependent's name
            // starts with 'A'

            return new BenefitsCost()
            {
                CostAnnualBeforeDiscount = annualCost,
                CostPayPeriodBeforeDiscount = annualCost / _settings.PayPeriodsAnnual,
                DiscountAnnual = discountAnnual,
                DiscountPerPayPeriod = discountAnnual / _settings.PayPeriodsAnnual,
                CostAnnualAfterDiscount = annualCost - discountAnnual,
                CostPerPayPeriodAfterDiscount = (annualCost - discountAnnual) / _settings.PayPeriodsAnnual
            };
        }

        /// <summary>
        /// Retrieves and maps employee info from the data store model to the domain model 
        /// </summary>
        /// <param name="employeeId"></param>
        private async Task<Models.Domain.Employee> GetEmployeeFromDataSource(long employeeId, long companyId)
        {
            var empData = await _dynamoRepo.GetItemAsync<Models.Data.Employee>(employeeId, companyId);
            if (empData == null)
                return null;
            var depData = await GetDependentsAsync(employeeId);

            var dependents = new List<Models.Domain.Dependent>();
            foreach (var dependent in depData)
                dependents.Add(new Models.Domain.Dependent() { FirstName = dependent.FirstName, LastName = dependent.LastName });

            return new Models.Domain.Employee(employeeId, empData.CompanyId, empData.FirstName, empData.LastName,
                dependents, empData.AnnualSalary);
        }
    }
}