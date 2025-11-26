# OpenTelemetry.Instrumentation.Kusto.Benchmarks

This project contains benchmarks for the OpenTelemetry Kusto instrumentation library.

## Running the Benchmarks

To run all benchmarks:

```bash
dotnet run --configuration Release --framework net10.0 --project test\OpenTelemetry.Instrumentation.Kusto.Benchmarks
```

Then choose the benchmark class that you want to run by entering the required
option number from the list of options shown on the Console window.

> [!TIP]
> The Profiling benchmarks are designed to run quickly and use the Visual Studio diagnosers to gather performance data.

## Results

// TODO: Includes private fixes in Kusto-Query-Langugage

```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.7093)
Intel Core Ultra 7 165H 3.80GHz, 1 CPU, 22 logical and 16 physical cores
.NET SDK 10.0.100-rc.2.25502.107
  [Host]     : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3


```
| Method                      | Mean     | Error    | StdDev   | Gen0   | Gen1   | Allocated |
|---------------------------- |---------:|---------:|---------:|-------:|-------:|----------:|
| ProcessSummarizeAndSanitize | 10.18 μs | 0.175 μs | 0.383 μs | 1.0834 | 0.0153 |  13.29 KB |
| ProcessSummarizeOnly        | 10.03 μs | 0.190 μs | 0.440 μs | 0.9918 | 0.0153 |  12.23 KB |
| ProcessSanitizeOnly         | 10.26 μs | 0.204 μs | 0.250 μs | 1.0529 | 0.0153 |  13.08 KB |
