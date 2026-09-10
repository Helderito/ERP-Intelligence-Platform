namespace ERP.Application.MasterData.Queries;

public sealed record SearchWarehousesQuery(string? Search, int Page, int PageSize);
