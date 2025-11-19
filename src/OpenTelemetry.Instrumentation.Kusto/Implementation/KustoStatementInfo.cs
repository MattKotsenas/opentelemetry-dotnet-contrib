// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

namespace OpenTelemetry.Instrumentation.Kusto.Implementation;

internal readonly struct KustoStatementInfo
{
    public KustoStatementInfo(string sanitizedQuery, string dbQuerySummary)
    {
        this.SanitizedQuery = sanitizedQuery;
        this.DbQuerySummary = dbQuerySummary;
    }

    public string SanitizedQuery { get; }

    public string DbQuerySummary { get; }
}
