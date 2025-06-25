# Performance Monitor CS

A comprehensive C# performance monitoring system that tracks execution times and call counts for methods using various monitoring approaches.

## Features

* **Easy-to-use static methods**: Simply wrap your code with `PerformanceMonitorStatic.MeasureExecution`
* **Extension methods**: Use fluent syntax with `.WithPerformanceMonitoring()`
* **Comprehensive statistics**: Track execution time, call count, min/max/average times, and standard deviation
* **JSON export**: Export performance data to JSON format for analysis
* **Reset functionality**: Clear metrics when needed
* **Exception handling**: Metrics are tracked even if methods throw exceptions
* **Thread-safe**: Concurrent access support using thread-safe collections
* **Async support**: Full support for async/await patterns

## Quick Start

### Basic Usage

```csharp
using PerfMonitorCS.Core;

// Measure a function
var result = PerformanceMonitorStatic.MeasureExecution(() =>
{
    // Your code here
    return SomeExpensiveOperation();
}, "MyOperation");

// Measure an async function
var result = await PerformanceMonitorStatic.MeasureExecution(async () =>
{
    // Your async code here
    return await SomeAsyncOperation();
}, "MyAsyncOperation");

// View performance summary
PerformanceMonitorStatic.GlobalMonitor.PrintSummary();
```

### Advanced Usage

```csharp
using PerfMonitorCS.Core;

// Create a custom monitor instance
var monitor = new PerformanceMonitor();

// Measure with custom monitor
var result = PerformanceMonitorStatic.MeasureExecution(() =>
{
    return CustomOperation();
}, "CustomOperation", monitor);

// Get specific statistics
var stats = monitor.GetStats("CustomOperation");
Console.WriteLine($"Average execution time: {stats["average_time"]:F6}s");

// Export to JSON
var json = await monitor.ExportToJsonAsync("performance_data.json");
```

### Extension Methods

```csharp
using PerfMonitorCS.Core;

// Use extension methods for fluent syntax
Func<int> myFunction = () => SomeComputation();
var monitoredFunction = myFunction.WithPerformanceMonitoring("MyFunction");
var result = monitoredFunction();

// For actions
Action myAction = () => DoSomething();
var monitoredAction = myAction.WithPerformanceMonitoring("MyAction");
monitoredAction();
```

## API Reference

### PerformanceMonitor Class

#### Methods

* `RecordExecution(methodName, executionTime)`: Manually record execution time
* `GetStats(methodName = null)`: Get performance statistics for a specific method or all methods
* `Reset()`: Clear all performance metrics
* `ExportToJsonAsync(filename = null)`: Export statistics to JSON format
* `ExportToJson(filename = null)`: Synchronous version of JSON export
* `PrintSummary()`: Print a formatted summary of all performance data

#### Statistics Provided

* `call_count`: Number of times the method was called
* `total_time`: Total execution time across all calls
* `average_time`: Average execution time per call
* `min_time`: Minimum execution time
* `max_time`: Maximum execution time
* `median_time`: Median execution time
* `std_dev`: Standard deviation of execution times

### Static Helper Methods

* `PerformanceMonitorStatic.MeasureExecution<T>(Func<T>, methodName, monitor)`: Measure function execution
* `PerformanceMonitorStatic.MeasureExecution(Action, methodName, monitor)`: Measure action execution
* `PerformanceMonitorStatic.GlobalMonitor`: Access to global monitor instance

## Project Structure

```
PerfMonitorCS/
├── PerfMonitorCS.Core/          # Core performance monitoring library
│   ├── PerformanceMonitor.cs    # Main performance monitoring class
│   └── MonitorPerformanceAttribute.cs  # Attribute and static helpers
├── PerfMonitorCS.Examples/      # Example usage demonstrations
│   └── Program.cs               # Example applications
├── PerfMonitorCS.Tests/         # Comprehensive test suite
│   └── PerformanceMonitorTests.cs  # xUnit test cases
├── PerfMonitorCS.sln           # Visual Studio solution file
└── README.md                   # This file
```

## Running Examples

### Console Application

```bash
dotnet run --project PerfMonitorCS.Examples
```

This will demonstrate the performance monitor with various types of functions and print a comprehensive summary.

### Running Tests

```bash
dotnet test PerfMonitorCS.Tests
```

Or run with detailed output:

```bash
dotnet test PerfMonitorCS.Tests --logger "console;verbosity=detailed"
```

## Example Output

```
================================================================================
PERFORMANCE MONITORING SUMMARY
================================================================================

Function: QuickFunction
  Call Count: 10
  Total Time: 0.012340s
  Average Time: 0.001234s
  Min Time: 0.001000s
  Max Time: 0.001500s
  Median Time: 0.001200s
  Std Deviation: 0.000200s

Function: SlowFunction
  Call Count: 3
  Total Time: 0.301234s
  Average Time: 0.100411s
  Min Time: 0.100123s
  Max Time: 0.100987s
  Median Time: 0.100234s
  Std Deviation: 0.000432s
```

## Use Cases

* **Performance optimization**: Identify slow methods in your codebase
* **Benchmarking**: Compare performance of different implementations
* **Monitoring**: Track performance changes over time in production
* **Debugging**: Find performance bottlenecks
* **Testing**: Validate that performance requirements are met
* **Profiling**: Detailed analysis of method execution patterns

## Technical Details

* Uses `System.Diagnostics.Stopwatch` for high-precision timing
* Thread-safe using `ConcurrentDictionary` and locking mechanisms
* Minimal overhead - typically less than 1μs per method call
* Preserves exception behavior - metrics recorded even when methods throw
* Full async/await support with proper task handling
* JSON serialization using `System.Text.Json`

## Requirements

* .NET 6.0 or later
* No external dependencies beyond .NET base class library

## NuGet Packages Used

* `System.Text.Json` (built-in)
* `xunit` (testing)
* `xunit.runner.visualstudio` (testing)

## License

This project is provided as-is for educational and development purposes.

## Migration from Python

This C# version provides equivalent functionality to the original Python PerfMonitorPY project:

* **Python decorators** → **C# extension methods and static helpers**
* **Python context managers** → **C# using statements and try/finally blocks**
* **Python `time.perf_counter()`** → **C# `Stopwatch`**
* **Python `statistics` module** → **C# LINQ and manual calculations**
* **Python `json` module** → **C# `System.Text.Json`**

The API has been adapted to follow C# conventions while maintaining the same core functionality and ease of use. 