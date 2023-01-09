namespace Benefits.Api.Settings
{
    public class BenefitsSettings
    {
        public decimal EmployeeCostAnnual { get; set; }
        public decimal PerDependentCostAnnual { get; set; }
        public float DiscountPercentage { get; set; }
        public int PayPeriodsAnnual { get; set; }
    }
}