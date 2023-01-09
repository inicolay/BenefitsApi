using Amazon.DynamoDBv2.DataModel;
using Benefits.Api.Models.Api;
using Benefits.Api.Services;
using Benefits.Api.Settings;
using Dynamo.Common;
using Moq;
using Xunit;

namespace Benefits.Api.Tests
{
    public class BenefitsServiceTests
    {
        private BenefitsService _sut;
        private AppSettings _settings;
        private Mock<IDynamoRepository> _dynamoRepo;

        public BenefitsServiceTests()
        {
            _settings = new AppSettings()
            {
                BenefitsSettings = new BenefitsSettings()
                {
                    EmployeeCostAnnual = 1000m,
                    PerDependentCostAnnual = 500m,
                    DiscountPercentage = 0.10f,
                    PayPeriodsAnnual = 26
                }
            };
            _dynamoRepo = new Mock<IDynamoRepository>();
            _sut = new BenefitsService(_dynamoRepo.Object, _settings.BenefitsSettings);
        }

        [Fact]
        public async Task GivenNoDependentsExist_GetDependentsReturnsEmptyDependentList() 
        {
            _dynamoRepo.Setup(x => x.GetAllItemsAsync<Models.Data.Dependent>(It.IsAny<List<ScanCondition>>(),
                It.IsAny<DynamoDBOperationConfig>())).ReturnsAsync(new List<Models.Data.Dependent>());
            var test = await _sut.GetDependentsAsync(1);
            Assert.True(test != null && test.Count() == 0);
        }

        [Fact]
        public async Task GivenDependentsExist_GetDependentsReturnsExpectedCount()
        {
            _dynamoRepo.Setup(x => x.GetAllItemsAsync<Models.Data.Dependent>(It.IsAny<List<ScanCondition>>(),
                It.IsAny<DynamoDBOperationConfig>())).ReturnsAsync(new List<Models.Data.Dependent>() { new Models.Data.Dependent() { } });
            var test = await _sut.GetDependentsAsync(1);
            Assert.True(test != null && test.Count() == 1);
        }

        [Fact]
        public async Task CreateDependentSavesItem()
        {
            var createRequest = new CreateDependentRequest()
            {
                EmployeeId = 1,
                FirstName = "Isaac",
                LastName = "Nicolay"
            };

            var test = await _sut.CreateDependentAsync(createRequest);
            _dynamoRepo.Verify(x => x.SaveItemAsync<Models.Data.Dependent>(It.IsAny<Models.Data.Dependent>()), Times.Exactly(1));
            Assert.True(test.FirstName == "Isaac" && test.LastName == "Nicolay" && test.EmployeeId == 1 
                && test.DependentId != string.Empty && test.DependentId != null);
        }

        [Fact]
        public async Task UpdateDependentSavesItemAndUpdatesFields()
        {
            var updateRequest = new UpdateDependentRequest()
            {
                DependentId = "d9e11fab-0306-4ade-82a4-4250b29c5178",
                EmployeeId = 1,
                FirstName = "Isaac",
                LastName = "Nicolay"
            };

            var existingItem = new Models.Data.Dependent()
            {
                DependentId = "d9e11fab-0306-4ade-82a4-4250b29c5178",
                EmployeeId = 1,
                FirstName = "Jim",
                LastName = "Jimmerson"
            };

            _dynamoRepo.Setup(x => x.GetItemAsync<Models.Data.Dependent>(It.IsAny<string>(), It.IsAny<long>()))
               .ReturnsAsync(existingItem);

            var test = await _sut.UpdateDependentAsync(updateRequest);
            _dynamoRepo.Verify(x => x.SaveItemAsync<Models.Data.Dependent>(
                It.Is<Models.Data.Dependent>(x => x.FirstName == "Isaac" && x.LastName == "Nicolay")), Times.Once);
            Assert.True(test.FirstName == "Isaac" && test.LastName == "Nicolay");
        }

        [Fact]
        public async Task DeleteItemDeletes()
        {
            var deleteRequest = new DeleteDependentRequest()
            {

                DependentId = "d9e11fab-0306-4ade-82a4-4250b29c5178",
                EmployeeId = 1
            };

            await _sut.DeleteDependentAsync(deleteRequest);
            _dynamoRepo.Verify(x => x.DeleteItemAsync<Models.Data.Dependent>(It.IsAny<string>(), It.IsAny<long>()), Times.Once);
        }

        [Fact]
        public async Task GivenEmployeeNotFound_GetBenefitsCost_ReturnsNull()
        {
            var result = await _sut.GetBenefitsCostAsync(4, 1);
            Assert.True(result == null);
        }

        [Fact]
        public async Task GivenEmployee_HasNoDependentsOrDiscounts_ReturnsExpectedCosts()
        {
            var foundEmployee = new Models.Data.Employee()
            {
                AnnualSalary = 5200,
                CompanyId = 1,
                EmployeeId = 1,
                FirstName = "Isaac",
                LastName = "Nicolay"
            };

            _dynamoRepo.Setup(x => x.GetItemAsync<Models.Data.Employee>(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(foundEmployee);
            _dynamoRepo.Setup(x => x.GetAllItemsAsync<Models.Data.Dependent>(It.IsAny<IEnumerable<ScanCondition>>(),
                It.IsAny<DynamoDBOperationConfig>())).ReturnsAsync(new List<Models.Data.Dependent>());    

            var result = await _sut.GetBenefitsCostAsync(1, 1);
            Assert.True(result.CostAnnualBeforeDiscount == 1000m);
            Assert.True(result.CostAnnualAfterDiscount == 1000m);
            Assert.True(result.CostPayPeriodBeforeDiscount == 38.461538461538461538461538462m);
            Assert.True(result.CostPerPayPeriodAfterDiscount == 38.461538461538461538461538462m);
            Assert.True(result.DiscountAnnual == 0m);
            Assert.True(result.DiscountPerPayPeriod == 0m);
        }

        [Fact]
        public async Task GivenEmployee_HasDependentsNoDiscounts_ReturnsExpectedCosts()
        {
            var foundEmployee = new Models.Data.Employee()
            {
                AnnualSalary = 5200,
                CompanyId = 1,
                EmployeeId = 1,
                FirstName = "Isaac",
                LastName = "Nicolay"
            };

            var foundDependents = new List<Models.Data.Dependent>()
            {
                new Models.Data.Dependent()
                {
                    EmployeeId = 1,
                    DependentId = "d9e11fab-0306-4ade-82a4-4250b29c5178",
                    FirstName = "Momathan",
                    LastName = "Jonbonavich"
                },
                new Models.Data.Dependent()
                {
                    EmployeeId = 1,
                    DependentId = "x4e11fab-0306-4ade-82a4-4250b29c5178",
                    FirstName = "Timberly",
                    LastName = "Hampsterson"
                }
            };

            _dynamoRepo.Setup(x => x.GetItemAsync<Models.Data.Employee>(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(foundEmployee);
            _dynamoRepo.Setup(x => x.GetAllItemsAsync<Models.Data.Dependent>(It.IsAny<IEnumerable<ScanCondition>>(),
                It.IsAny<DynamoDBOperationConfig>())).ReturnsAsync(foundDependents);

            var result = await _sut.GetBenefitsCostAsync(1, 1);
            Assert.True(result.CostAnnualBeforeDiscount == 2000m);
            Assert.True(result.CostAnnualAfterDiscount == 2000m);
            Assert.True(result.CostPayPeriodBeforeDiscount == 76.923076923076923076923076923m);
            Assert.True(result.CostPerPayPeriodAfterDiscount == 76.923076923076923076923076923m);
            Assert.True(result.DiscountAnnual == 0m);
            Assert.True(result.DiscountPerPayPeriod == 0m);
        }

        [Fact]
        public async Task GivenEmployee_HasDependents_EmployeeNameDiscount_ReturnsExpectedCosts()
        {
            var foundEmployee = new Models.Data.Employee()
            {
                AnnualSalary = 5200,
                CompanyId = 1,
                EmployeeId = 1,
                FirstName = "Anthony",
                LastName = "Nicolay"
            };

            var foundDependents = new List<Models.Data.Dependent>()
            {
                new Models.Data.Dependent()
                {
                    EmployeeId = 1,
                    DependentId = "d9e11fab-0306-4ade-82a4-4250b29c5178",
                    FirstName = "Momathan",
                    LastName = "Jonbonavich"
                },
                new Models.Data.Dependent()
                {
                    EmployeeId = 1,
                    DependentId = "x4e11fab-0306-4ade-82a4-4250b29c5178",
                    FirstName = "Timberly",
                    LastName = "Hampsterson"
                }
            };

            _dynamoRepo.Setup(x => x.GetItemAsync<Models.Data.Employee>(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(foundEmployee);
            _dynamoRepo.Setup(x => x.GetAllItemsAsync<Models.Data.Dependent>(It.IsAny<IEnumerable<ScanCondition>>(),
                It.IsAny<DynamoDBOperationConfig>())).ReturnsAsync(foundDependents);

            var result = await _sut.GetBenefitsCostAsync(1, 1);
            Assert.True(result.CostAnnualBeforeDiscount == 2000m);
            Assert.True(result.CostAnnualAfterDiscount == 1800m);
            Assert.True(result.CostPayPeriodBeforeDiscount == 76.923076923076923076923076923m);
            Assert.True(result.CostPerPayPeriodAfterDiscount == 69.230769230769230769230769231m);
            Assert.True(result.DiscountAnnual == 200m);
            Assert.True(result.DiscountPerPayPeriod == 7.6923076923076923076923076923m);
        }

        [Fact]
        public async Task GivenEmployee_HasDependents_DependentDiscount_ReturnsExpectedCosts()
        {
            var foundEmployee = new Models.Data.Employee()
            {
                AnnualSalary = 5200,
                CompanyId = 1,
                EmployeeId = 1,
                FirstName = "Isaac",
                LastName = "Nicolay"
            };

            var foundDependents = new List<Models.Data.Dependent>()
            {
                new Models.Data.Dependent()
                {
                    EmployeeId = 1,
                    DependentId = "d9e11fab-0306-4ade-82a4-4250b29c5178",
                    FirstName = "Momathan",
                    LastName = "Jonbonavich"
                },
                new Models.Data.Dependent()
                {
                    EmployeeId = 1,
                    DependentId = "x4e11fab-0306-4ade-82a4-4250b29c5178",
                    FirstName = "Amberly",
                    LastName = "Hampsterson"
                }
            };

            _dynamoRepo.Setup(x => x.GetItemAsync<Models.Data.Employee>(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(foundEmployee);
            _dynamoRepo.Setup(x => x.GetAllItemsAsync<Models.Data.Dependent>(It.IsAny<IEnumerable<ScanCondition>>(),
                It.IsAny<DynamoDBOperationConfig>())).ReturnsAsync(foundDependents);

            var result = await _sut.GetBenefitsCostAsync(1, 1);
            Assert.True(result.CostAnnualBeforeDiscount == 2000m);
            Assert.True(result.CostAnnualAfterDiscount == 1800m);
            Assert.True(result.CostPayPeriodBeforeDiscount == 76.923076923076923076923076923m);
            Assert.True(result.CostPerPayPeriodAfterDiscount == 69.230769230769230769230769231m);
            Assert.True(result.DiscountAnnual == 200m);
            Assert.True(result.DiscountPerPayPeriod == 7.6923076923076923076923076923m);
        }

        [Fact]
        public async Task GivenEmployee_HasDependents_MultipleDiscountMatch_DoesNotStack()
        {
            var foundEmployee = new Models.Data.Employee()
            {
                AnnualSalary = 5200,
                CompanyId = 1,
                EmployeeId = 1,
                FirstName = "Anton",
                LastName = "Nicolay"
            };

            var foundDependents = new List<Models.Data.Dependent>()
            {
                new Models.Data.Dependent()
                {
                    EmployeeId = 1,
                    DependentId = "d9e11fab-0306-4ade-82a4-4250b29c5178",
                    FirstName = "Aaaron",
                    LastName = "Jonbonavich"
                },
                new Models.Data.Dependent()
                {
                    EmployeeId = 1,
                    DependentId = "x4e11fab-0306-4ade-82a4-4250b29c5178",
                    FirstName = "Austina",
                    LastName = "Hampsterson"
                }
            };

            _dynamoRepo.Setup(x => x.GetItemAsync<Models.Data.Employee>(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(foundEmployee);
            _dynamoRepo.Setup(x => x.GetAllItemsAsync<Models.Data.Dependent>(It.IsAny<IEnumerable<ScanCondition>>(),
                It.IsAny<DynamoDBOperationConfig>())).ReturnsAsync(foundDependents);

            var result = await _sut.GetBenefitsCostAsync(1, 1);
            Assert.True(result.CostAnnualBeforeDiscount == 2000m);
            Assert.True(result.CostAnnualAfterDiscount == 1800m);
            Assert.True(result.CostPayPeriodBeforeDiscount == 76.923076923076923076923076923m);
            Assert.True(result.CostPerPayPeriodAfterDiscount == 69.230769230769230769230769231m);
            Assert.True(result.DiscountAnnual == 200m);
            Assert.True(result.DiscountPerPayPeriod == 7.6923076923076923076923076923m);
        }

        [Fact]
        public async Task LowercaseADoesNotDiscount_ReturnsExpectedCosts()
        {
            var foundEmployee = new Models.Data.Employee()
            {
                AnnualSalary = 5200,
                CompanyId = 1,
                EmployeeId = 1,
                FirstName = "aaron",
                LastName = "Nicolay"
            };

            var foundDependents = new List<Models.Data.Dependent>()
            {
                new Models.Data.Dependent()
                {
                    EmployeeId = 1,
                    DependentId = "d9e11fab-0306-4ade-82a4-4250b29c5178",
                    FirstName = "alan",
                    LastName = "Jonbonavich"
                },
                new Models.Data.Dependent()
                {
                    EmployeeId = 1,
                    DependentId = "d9e11fab-0306-4ade-82a4-4250b29c5178",
                    FirstName = "alana",
                    LastName = "Jonbonavich"
                }
            };

            _dynamoRepo.Setup(x => x.GetItemAsync<Models.Data.Employee>(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(foundEmployee);
            _dynamoRepo.Setup(x => x.GetAllItemsAsync<Models.Data.Dependent>(It.IsAny<IEnumerable<ScanCondition>>(),
                It.IsAny<DynamoDBOperationConfig>())).ReturnsAsync(foundDependents);

            var result = await _sut.GetBenefitsCostAsync(1, 1);
            Assert.True(result.CostAnnualBeforeDiscount == 2000m);
            Assert.True(result.CostAnnualAfterDiscount == 2000m);
            Assert.True(result.CostPayPeriodBeforeDiscount == 76.923076923076923076923076923m);
            Assert.True(result.CostPerPayPeriodAfterDiscount == 76.923076923076923076923076923m);
            Assert.True(result.DiscountAnnual == 0m);
            Assert.True(result.DiscountPerPayPeriod == 0m);
        }
    }
}