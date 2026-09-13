namespace ERP.Application.Tenancy.Queries;

public sealed record GetCompaniesQuery;

public sealed record GetCompanyByIdQuery(Guid CompanyId);
