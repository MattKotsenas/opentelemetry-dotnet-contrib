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

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26100.7092/24H2/2024Update/HudsonValley)
Intel Core i9-10940X CPU 3.30GHz (Max: 3.31GHz), 1 CPU, 28 logical and 14 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4


```
| Method                      | Mean     | Error    | StdDev   | Gen0   | Gen1   | Allocated |
|---------------------------- |---------:|---------:|---------:|-------:|-------:|----------:|
| ProcessSummarizeAndSanitize | 27.61 μs | 0.549 μs | 0.887 μs | 3.8452 | 0.2441 |  38.03 KB |
| ProcessSummarizeOnly        | 24.31 μs | 0.472 μs | 0.763 μs | 3.7537 | 0.3662 |   36.9 KB |
| ProcessSanitizeOnly         | 24.21 μs | 0.448 μs | 0.440 μs | 3.8452 | 0.2136 |  37.87 KB |
