// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

#if NETFRAMEWORK
using System.Net.Http;
#endif
using System.Text;
using DotNet.Testcontainers.Builders;
using Kusto.Data;
using Testcontainers.Kusto;
using Xunit;

namespace OpenTelemetry.Instrumentation.Kusto.Tests;

public sealed class KustoIntegrationTestsFixture : IAsyncLifetime
{
    private static readonly string KustoImage = GetKustoImage();

    public KustoContainer DatabaseContainer { get; } = CreateKusto();

    public KustoConnectionStringBuilder ConnectionStringBuilder => new(this.DatabaseContainer.GetConnectionString());

    public Task InitializeAsync() => this.DatabaseContainer.StartAsync();

    public Task DisposeAsync() => this.DatabaseContainer.DisposeAsync().AsTask();

    private static KustoContainer CreateKusto()
        => new KustoBuilder()
        .WithImage(KustoImage)

        // This wait strategy can be removed once
        // https://github.com/testcontainers/testcontainers-dotnet/pull/1567
        // is merged and a new version is released.
        .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request
                .WithMethod(HttpMethod.Post)
                .ForPort(KustoBuilder.KustoPort)
                .ForPath("/v1/rest/mgmt")
                .WithContent(() => new StringContent("{\"csl\":\".show cluster\"}", Encoding.UTF8, "application/json"))))
        .Build();

    private static string GetKustoImage()
    {
        var assembly = typeof(KustoIntegrationTestsFixture).Assembly;

        using var stream = assembly.GetManifestResourceStream("kusto.Dockerfile");
        using var reader = new StreamReader(stream!);

        var raw = reader.ReadToEnd();

        // Exclude FROM
        return raw.Substring(4).Trim();
    }
}
