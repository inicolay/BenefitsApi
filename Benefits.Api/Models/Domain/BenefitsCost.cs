using Amazon.DynamoDBv2.Model;

namespace Benefits.Api.Models.Domain
{
    public class BenefitsCost
    {
        public decimal CostAnnualBeforeDiscount { get; set; }
        public decimal CostPayPeriodBeforeDiscount { get; set; }
        public decimal DiscountAnnual { get; set; }
        public decimal DiscountPerPayPeriod { get; set; }
        public decimal CostAnnualAfterDiscount { get; set; }
        public decimal CostPerPayPeriodAfterDiscount { get; set; }
    }
}