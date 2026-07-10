using ERP.Application.Common.Models;

namespace ERP.Api.Contracts.MasterData;

public sealed record PagedResultResponse<TItem>(
    int Page,
    int PageSize,
    int TotalRecords,
    IReadOnlyCollection<TItem> Items)
{
    public static PagedResultResponse<TItem> FromDto<TDto>(
        PagedResultDto<TDto> result,
        Func<TDto, TItem> map)
    {
        return new PagedResultResponse<TItem>(
            result.Page,
            result.PageSize,
            result.TotalRecords,
            result.Items.Select(map).ToArray());
    }
}
