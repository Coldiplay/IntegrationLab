namespace BaseLibrary.TestDB;

public partial class Chat
{
    public ulong Id { get; set; }

    public string? Name { get; set; }

    public bool? IsPrivateChat { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
