namespace BaseLibrary.TestDB;

public partial class Message
{
    public ulong Id { get; set; }

    public string Content { get; set; } = null!;

    public DateTime Date { get; set; }

    public ulong SenderId { get; set; }

    public ulong ChatId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
