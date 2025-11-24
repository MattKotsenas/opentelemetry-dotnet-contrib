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

```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26100.7092/24H2/2024Update/HudsonValley)
Intel Core i9-10940X CPU 3.30GHz (Max: 3.31GHz), 1 CPU, 28 logical and 14 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v4


```
| Method                      | Mean     | Error    | StdDev   | Gen0   | Gen1   | Allocated |
|---------------------------- |---------:|---------:|---------:|-------:|-------:|----------:|
| ProcessSummarizeAndSanitize | 29.53 μs | 0.587 μs | 0.603 μs | 4.8523 | 0.4578 |  47.91 KB |
| ProcessSummarizeOnly        | 26.75 μs | 0.532 μs | 1.190 μs | 4.7607 | 0.3662 |  46.77 KB |
| ProcessSanitizeOnly         | 25.35 μs | 0.481 μs | 0.426 μs | 4.8523 |      - |  47.74 KB |
