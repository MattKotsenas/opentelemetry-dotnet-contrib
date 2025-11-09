// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics;
using Kusto.Data;
using Kusto.Data.Net.Client;
using OpenTelemetry.Metrics;
using OpenTelemetry.Tests;
using OpenTelemetry.Trace;
using Xunit;

namespace OpenTelemetry.Instrumentation.Kusto.Tests;

[Trait("CategoryName", "KustoIntegrationTests")]
public sealed class KustoIntegrationTests : IClassFixture<KustoIntegrationTestsFixture>
{
    private const string DatabaseName = "NetDefaultDB";

    private readonly KustoIntegrationTestsFixture fixture;

    public KustoIntegrationTests(KustoIntegrationTestsFixture fixture)
    {
        this.fixture = fixture;
    }

    [EnabledOnDockerPlatformTheory(DockerPlatform.Linux)]
    [InlineData(".show version")]
    [InlineData(".show databases")]
    public void SuccessfulQueryTest(string query)
    {
        var activities = new List<Activity>();
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddInMemoryExporter(activities)
            .AddKustoInstrumentation()
            .Build();

        var kcsb = new KustoConnectionStringBuilder(this.fixture.DatabaseContainer.GetConnectionString());

        using var queryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb);

        var reader = queryProvider.ExecuteQuery(DatabaseName, query, null);

        // Verify results can be read
        Assert.NotNull(reader);
        while (reader.Read())
        {
            // Read through results
        }

        // Give some time for async operations to complete
        Task.Delay(TimeSpan.FromSeconds(2)).Wait();

        Assert.NotEmpty(activities);
        var activity = activities.FirstOrDefault(a => a.OperationName.Contains("Query") || a.OperationName.Contains("Management"));
        Assert.NotNull(activity);

        // Verify activity tags
        Assert.Contains(activity.Tags, t => t.Key == "db.system" && t.Value == "kusto");
        Assert.Contains(activity.Tags, t => t.Key == "db.query.text");
        Assert.Contains(activity.Tags, t => t.Key == "url.full");
    }

    [EnabledOnDockerPlatformTheory(DockerPlatform.Linux)]
    [InlineData(true)]
    [InlineData(false)]
    public void QueryWithOptionsTest(bool enableInstrumentation)
    {
        var activities = new List<Activity>();
        var options = new KustoInstrumentationOptions();

        using var tracerProvider = enableInstrumentation
            ? Sdk.CreateTracerProviderBuilder()
                .AddInMemoryExporter(activities)
                .AddKustoInstrumentation(options)
                .Build()
            : null;

        var kcsb = new KustoConnectionStringBuilder(this.fixture.DatabaseContainer.GetConnectionString());

        using var queryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb);

        var reader = queryProvider.ExecuteQuery(DatabaseName, ".show version", null);

        Assert.NotNull(reader);
        while (reader.Read())
        {
            // Read through results
        }

        Task.Delay(TimeSpan.FromSeconds(2)).Wait();

        if (enableInstrumentation)
        {
            Assert.NotEmpty(activities);
        }
        else
        {
            Assert.Empty(activities);
        }
    }

    [EnabledOnDockerPlatformTheory(DockerPlatform.Linux)]
    [InlineData(".show databases")]
    public void MetricsAreRecorded(string query)
    {
        var exportedItems = new List<Metric>();

        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddInMemoryExporter(exportedItems)
            .AddMeter("Kusto.Client")
            .Build();

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddKustoInstrumentation()
            .Build();

        var kcsb = new KustoConnectionStringBuilder(this.fixture.DatabaseContainer.GetConnectionString());

        using var queryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb);

        var reader = queryProvider.ExecuteQuery(DatabaseName, query, null);

        Assert.NotNull(reader);
        while (reader.Read())
        {
            // Read through results
        }

        meterProvider.ForceFlush();
        Task.Delay(TimeSpan.FromSeconds(2)).Wait();

        Assert.NotEmpty(exportedItems);
        var durationMetric = exportedItems.FirstOrDefault(m => m.Name == "db.client.operation.duration");
        Assert.NotNull(durationMetric);

        var countMetric = exportedItems.FirstOrDefault(m => m.Name == "db.client.operation.count");
        Assert.NotNull(countMetric);
    }

    [EnabledOnDockerPlatformTheory(DockerPlatform.Linux)]
    [InlineData(".create table InvalidTable (Col1:string)")]
    public void ErrorQueryTest(string query)
    {
        var activities = new List<Activity>();
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddInMemoryExporter(activities)
            .AddKustoInstrumentation()
            .Build();

        var kcsb = new KustoConnectionStringBuilder(this.fixture.DatabaseContainer.GetConnectionString());

        using var queryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb);

        try
        {
            var reader = queryProvider.ExecuteQuery(DatabaseName, query, null);
            while (reader.Read())
            {
                // Read through results
            }
        }
        catch
        {
            // Expected to fail
        }

        Task.Delay(TimeSpan.FromSeconds(2)).Wait();

        Assert.NotEmpty(activities);
        var errorActivity = activities.FirstOrDefault(a => a.Status == ActivityStatusCode.Error);
        Assert.NotNull(errorActivity);
    }

    [EnabledOnDockerPlatformTheory(DockerPlatform.Linux)]
    [InlineData("print message='Hello, World!'")]
    [InlineData("print number=42")]
    public void SimpleQueryTest(string query)
    {
        var activities = new List<Activity>();
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddInMemoryExporter(activities)
            .AddKustoInstrumentation()
            .Build();

        var kcsb = new KustoConnectionStringBuilder(this.fixture.DatabaseContainer.GetConnectionString());

        using var queryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb);

        var reader = queryProvider.ExecuteQuery(DatabaseName, query, null);

        Assert.NotNull(reader);
        var hasResults = false;
        while (reader.Read())
        {
            hasResults = true;
        }

        Assert.True(hasResults);

        Task.Delay(TimeSpan.FromSeconds(2)).Wait();

        Assert.NotEmpty(activities);
        var activity = activities.First();
        Assert.Equal(ActivityStatusCode.Unset, activity.Status);
        Assert.Contains(activity.Tags, t => t.Key == "db.query.text" && t.Value?.ToString()?.Contains(query.Split(' ')[0]) == true);
    }

    [EnabledOnDockerPlatformFact(DockerPlatform.Linux)]
    public void MultipleQueriesTest()
    {
        var activities = new List<Activity>();
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddInMemoryExporter(activities)
            .AddKustoInstrumentation()
            .Build();

        var kcsb = new KustoConnectionStringBuilder(this.fixture.DatabaseContainer.GetConnectionString());

        using var queryProvider = KustoClientFactory.CreateCslQueryProvider(kcsb);

        // Execute multiple queries
        for (int i = 0; i < 3; i++)
        {
            var reader = queryProvider.ExecuteQuery(DatabaseName, $"print iteration={i}", null);
            while (reader.Read())
            {
                // Read through results
            }
        }

        Task.Delay(TimeSpan.FromSeconds(2)).Wait();

        Assert.NotEmpty(activities);
        Assert.True(activities.Count >= 3, $"Expected at least 3 activities, got {activities.Count}");
    }
}
