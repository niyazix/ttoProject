namespace TTO.Core.Entities;

public class SiteSetting
{
    public int Id { get; set; }
    public string SettingKey { get; set; } = null!;
    public string? SettingValue { get; set; }
    public string? Description { get; set; }
    public string SettingGroup { get; set; } = "general"; // general | social | seo | contact
    public bool IsPublic { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}
