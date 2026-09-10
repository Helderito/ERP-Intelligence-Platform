namespace ERP.Application.MasterData.Queries;

public sealed record ListReferenceItemsQuery(string? Search, bool IncludeInactive = false);
