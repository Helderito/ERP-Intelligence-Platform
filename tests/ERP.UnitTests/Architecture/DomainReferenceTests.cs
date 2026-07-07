namespace ERP.UnitTests.Architecture;

public sealed class DomainReferenceTests
{
    [Fact]
    public void DomainAssembly_ShouldNotReferenceOtherProjectAssemblies()
    {
        var referencedAssemblies = typeof(ERP.Domain.AssemblyReference).Assembly
            .GetReferencedAssemblies()
            .Select(assembly => assembly.Name)
            .Where(name => name is not null && name.StartsWith("ERP.", StringComparison.Ordinal))
            .ToArray();

        Assert.Empty(referencedAssemblies);
    }
}
