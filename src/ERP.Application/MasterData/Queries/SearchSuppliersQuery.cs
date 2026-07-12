namespace ERP.Application.MasterData.Queries;

public sealed record SearchSuppliersQuery(string? Search, int Page, int PageSize);
