using ERP.Application.MasterData.Abstractions;
using ERP.Application.MasterData.Models;

namespace ERP.Application.MasterData.Services;

public sealed class SharedReferenceDataService
{
    private readonly ICountryRepository _countryRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IPaymentTermRepository _paymentTermRepository;

    public SharedReferenceDataService(
        ICountryRepository countryRepository,
        ICurrencyRepository currencyRepository,
        IPaymentTermRepository paymentTermRepository)
    {
        _countryRepository = countryRepository;
        _currencyRepository = currencyRepository;
        _paymentTermRepository = paymentTermRepository;
    }

    public async Task<IReadOnlyCollection<CountryDto>> ListCountriesAsync(
        CancellationToken cancellationToken = default)
    {
        var countries = await _countryRepository.ListAsync(cancellationToken);
        return countries.Select(country => new CountryDto(country.Id, country.Code, country.Name)).ToArray();
    }

    public async Task<IReadOnlyCollection<CurrencyDto>> ListCurrenciesAsync(
        CancellationToken cancellationToken = default)
    {
        var currencies = await _currencyRepository.ListAsync(cancellationToken);
        return currencies.Select(currency => new CurrencyDto(currency.Id, currency.Code, currency.Name)).ToArray();
    }

    public async Task<IReadOnlyCollection<PaymentTermDto>> ListPaymentTermsAsync(
        CancellationToken cancellationToken = default)
    {
        var paymentTerms = await _paymentTermRepository.ListAsync(cancellationToken);
        return paymentTerms
            .Select(term => new PaymentTermDto(term.Id, term.Code, term.Name, term.NetDays))
            .ToArray();
    }
}
