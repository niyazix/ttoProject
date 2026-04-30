namespace TTO.API.DTOs;

public record NewsListItem(int Id, string Title, string Slug, string? Summary, string? CoverImageUrl, string? Author, bool IsPublished, bool IsFeatured, DateTime? PublishDate, int ViewCount, string? CategoryName);

public record NewsDetail(int Id, string Title, string Slug, string? Summary, string Content, string? CoverImageUrl, string? Author, bool IsPublished, bool IsFeatured, DateTime? PublishDate, int ViewCount, int? CategoryId, string? CategoryName, List<string> Tags);

public record CreateNewsRequest(string Title, string Slug, string? Summary, string Content, string? CoverImageUrl, int? CategoryId, string? Author, DateTime? PublishDate, bool IsFeatured = false, List<string>? Tags = null);

public record UpdateNewsRequest(string Title, string? Summary, string Content, string? CoverImageUrl, int? CategoryId, string? Author, DateTime? PublishDate, bool IsFeatured, List<string>? Tags);
