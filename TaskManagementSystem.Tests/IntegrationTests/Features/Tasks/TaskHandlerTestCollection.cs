using TaskManagementSystem.Tests.Fixtures;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Tasks;

[CollectionDefinition(nameof(TaskHandlerTestCollection))]
public sealed class TaskHandlerTestCollection : ICollectionFixture<TestDatabaseFixture>
{
}