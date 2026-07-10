namespace ERP.Application.MasterData.Queries;

public sealed record SearchProductsQuery(string? Search, int Page = 1, int PageSize = 20);
