using TaskManagementSystem.Tests.Fixtures;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Categories;

[CollectionDefinition(nameof(CategoryHandlerTestCollection))]
public sealed class CategoryHandlerTestCollection : ICollectionFixture<TestDatabaseFixture>
{

}