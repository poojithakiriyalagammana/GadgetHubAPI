namespace GadgetHubAPI.Models
{
    public class Quotation
    {
        public int QuotationId { get; set; }
        public int ProductId { get; set; }

        public int DistributorId { get; set; }
        public decimal PricePerUnit { get; set; }
        public int Availability { get; set; }
        public int EstimatedDeliveryDays { get; set; }

        public Product? Product { get; set; }
        public Distributor? Distributor { get; set; }
    }
}
