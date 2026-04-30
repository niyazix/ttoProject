using TTO.Core.Enums;

namespace TTO.API.DTOs;

public record PageListItem(int Id, string Title, string Slug, bool IsPublished, bool IsSystem, PageType PageType, int SortOrder, int? ParentId, DateTime UpdatedAt);

public record PageDetail(int Id, string Title, string Slug, string? MetaTitle, string? MetaDescription, bool IsPublished, bool IsSystem, PageType PageType, int SortOrder, int? ParentId, List<ContentBlockDto> ContentBlocks);

public record ContentBlockDto(int Id, string BlockType, string? Content, int SortOrder, bool IsActive);

public record CreatePageRequest(string Title, string Slug, string? MetaTitle, string? MetaDescription, PageType PageType, int? ParentId = null, int SortOrder = 0);

public record UpdatePageRequest(string Title, string? MetaTitle, string? MetaDescription, int SortOrder, int? ParentId);

public record UpsertContentBlockRequest(string BlockType, string? Content, int SortOrder, bool IsActive = true);
