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
| Method                      | Mean     | Error    | StdDev   | Median   | Gen0   | Gen1   | Allocated |
|---------------------------- |---------:|---------:|---------:|---------:|-------:|-------:|----------:|
| ProcessSummarizeAndSanitize | 19.76 μs | 1.635 μs | 4.822 μs | 17.21 μs | 1.5869 | 0.0305 |  15.87 KB |
| ProcessSummarizeOnly        | 13.69 μs | 0.270 μs | 0.420 μs | 13.51 μs | 1.5106 | 0.0305 |   14.9 KB |
| ProcessSanitizeOnly         | 16.89 μs | 0.478 μs | 1.342 μs | 16.82 μs | 1.5869 | 0.0305 |   15.7 KB |
