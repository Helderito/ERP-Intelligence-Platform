using ERP.Application.MasterData.Abstractions;
using ERP.Application.MasterData.Commands;
using ERP.Application.MasterData.Exceptions;
using ERP.Application.MasterData.Queries;
using ERP.Application.MasterData.Services;
using ERP.Domain.MasterData;

namespace ERP.UnitTests.MasterData;

public sealed class CustomerManagementServiceTests
{
    [Fact]
    public async Task CreateCustomerAsync_ShouldCreateCustomerWithContactsAndAddresses_WhenCodeIsUnique()
    {
        var customerRepository = new FakeCustomerRepository();
        var service = new CustomerManagementService(customerRepository);

        var customer = await service.CreateCustomerAsync(
            new CreateCustomerCommand(
                "cus-001",
                "Sample Customer",
                [new CustomerContactCommand("Ana Silva", "ana@example.com", "+244 900 000 000")],
                [new CustomerAddressCommand("Rua Principal", null, "Luanda", "1000", "Angola")]));

        Assert.Equal("CUS-001", customer.Code);
        Assert.Single(customer.Contacts);
        Assert.Single(customer.Addresses);
        Assert.Single(customerRepository.Customers);
    }

    [Fact]
    public async Task CreateCustomerAsync_ShouldThrow_WhenCodeAlreadyExists()
    {
        var customerRepository = new FakeCustomerRepository();
        var service = new CustomerManagementService(customerRepository);

        await service.CreateCustomerAsync(new CreateCustomerCommand("cus-001", "Sample Customer", [], []));

        await Assert.ThrowsAsync<CustomerCodeAlreadyExistsException>(
            () => service.CreateCustomerAsync(new CreateCustomerCommand("CUS-001", "Duplicate", [], [])));
    }

    [Fact]
    public async Task UpdateCustomerAsync_ShouldReplaceContactsAndAddresses()
    {
        var customerRepository = new FakeCustomerRepository();
        var service = new CustomerManagementService(customerRepository);
        var created = await service.CreateCustomerAsync(
            new CreateCustomerCommand(
                "cus-001",
                "Sample Customer",
                [new CustomerContactCommand("Ana Silva", "ana@example.com", null)],
                [new CustomerAddressCommand("Rua Principal", null, "Luanda", "1000", "Angola")]));

        var updated = await service.UpdateCustomerAsync(
            new UpdateCustomerCommand(
                created.Id,
                "Updated Customer",
                [new CustomerContactCommand("Carlos Lopes", "carlos@example.com", null)],
                [new CustomerAddressCommand("Avenida Nova", "2A", "Lisboa", "1200-001", "Portugal")]));

        Assert.Equal("Updated Customer", updated.Name);
        Assert.Single(updated.Contacts);
        Assert.Equal("Carlos Lopes", updated.Contacts.Single().Name);
        Assert.Single(updated.Addresses);
        Assert.Equal("Lisboa", updated.Addresses.Single().City);
    }

    [Fact]
    public async Task UpdateCustomerAsync_ShouldThrow_WhenCustomerDoesNotExist()
    {
        var service = new CustomerManagementService(new FakeCustomerRepository());

        await Assert.ThrowsAsync<CustomerNotFoundException>(
            () => service.UpdateCustomerAsync(new UpdateCustomerCommand(Guid.NewGuid(), "Missing", [], [])));
    }

    [Fact]
    public async Task SearchCustomersAsync_ShouldReturnPagedResults()
    {
        var customerRepository = new FakeCustomerRepository();
        var service = new CustomerManagementService(customerRepository);

        await service.CreateCustomerAsync(new CreateCustomerCommand("cus-001", "Sample Customer", [], []));

        var result = await service.SearchCustomersAsync(new SearchCustomersQuery("sample", 1, 20));

        Assert.Equal(1, result.TotalRecords);
        Assert.Single(result.Items);
    }

    private sealed class FakeCustomerRepository : ICustomerRepository
    {
        public List<Customer> Customers { get; } = [];

        public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            Customers.Add(customer);
            return Task.CompletedTask;
        }

        public Task<int> CountAsync(string? search, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ApplySearch(search).Count());
        }

        public Task<Customer?> GetByCodeAsync(CustomerCode code, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Customers.FirstOrDefault(customer => customer.Code.Equals(code)));
        }

        public Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Customers.FirstOrDefault(customer => customer.Id == customerId));
        }

        public Task<IReadOnlyCollection<Customer>> SearchAsync(
            string? search,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var customers = ApplySearch(search)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArray();

            return Task.FromResult<IReadOnlyCollection<Customer>>(customers);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        private IEnumerable<Customer> ApplySearch(string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return Customers;
            }

            return Customers.Where(customer =>
                customer.Code.Value.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                customer.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        }
    }
}
