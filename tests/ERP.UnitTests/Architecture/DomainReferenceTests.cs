namespace ERP.UnitTests.Architecture;

public sealed class DomainReferenceTests
{
    [Fact]
    public void DomainAssembly_ShouldOnlyReferenceSharedKernel()
    {
        // The Shared Kernel holds cross-cutting DDD building blocks (Entity, ValueObject,
        // IDomainEvent) shared across Bounded Contexts. It is not an architectural layer,
        // so Domain may depend on it — Domain must never depend on Application,
        // Infrastructure or Api.
        var referencedAssemblies = typeof(ERP.Domain.AssemblyReference).Assembly
            .GetReferencedAssemblies()
            .Select(assembly => assembly.Name)
            .OfType<string>()
            .Where(name => name.StartsWith("ERP.", StringComparison.Ordinal))
            .ToArray();

        Assert.Equal(["ERP.SharedKernel"], referencedAssemblies);
    }
}
