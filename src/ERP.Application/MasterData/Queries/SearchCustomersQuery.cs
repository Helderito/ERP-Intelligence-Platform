namespace ERP.Application.MasterData.Queries;

public sealed record SearchCustomersQuery(string? Search, int Page, int PageSize);
