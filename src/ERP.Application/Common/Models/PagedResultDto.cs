namespace ERP.Application.Common.Models;

public sealed record PagedResultDto<TItem>(
    int Page,
    int PageSize,
    int TotalRecords,
    IReadOnlyCollection<TItem> Items);
