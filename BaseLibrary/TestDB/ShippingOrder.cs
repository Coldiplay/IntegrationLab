namespace BaseLibrary.TestDB;

public partial class ShippingOrder
{
    public ulong Id { get; set; }

    public DateTime OrderDate { get; set; }

    public string ReceiverFio { get; set; } = null!;

    public string ReceiverPhone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime ShippingDate { get; set; }

    public DateTime? SentDate { get; set; }

    public DateTime? ReceivedDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
