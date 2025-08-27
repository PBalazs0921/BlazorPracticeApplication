using Xunit;

namespace BlazorApp1.IntegrationTests.Tests.APITests;

[CollectionDefinition("ApiTestCollection")]
public class ApiTestCollection : ICollectionFixture<IntegrationTestWebAppFactory<Program>>
{
}