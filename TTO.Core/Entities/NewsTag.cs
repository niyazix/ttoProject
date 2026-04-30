namespace TTO.Core.Entities;

public class NewsTag
{
    public int NewsId { get; set; }
    public int TagId { get; set; }

    public News News { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
