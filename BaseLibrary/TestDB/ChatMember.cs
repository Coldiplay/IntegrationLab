namespace BaseLibrary.TestDB;

public partial class ChatMember
{
    public ulong UserId { get; set; }

    public ulong ChatId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
