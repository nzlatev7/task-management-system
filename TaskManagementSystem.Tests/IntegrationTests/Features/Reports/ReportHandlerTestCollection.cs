using TaskManagementSystem.Tests.Fixtures;

namespace TaskManagementSystem.Tests.IntegrationTests.Features.Reports;

[CollectionDefinition(nameof(ReportHandlerTestCollection))]
public sealed class ReportHandlerTestCollection : ICollectionFixture<TestDatabaseFixture>
{
}