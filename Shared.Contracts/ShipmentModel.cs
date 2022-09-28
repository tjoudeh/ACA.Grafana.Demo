namespace Shared.Contracts
{
    public class ShipmentModel
    {
        public int Id { get; set; }
        public Guid ShipmentId { get; set; }
        public string WaybillNo { get; set; } = string.Empty;
        public string RecipientName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime TimeStamp { get; set; }

    }
}