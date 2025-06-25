# Performance Monitor

A simple Python performance monitoring system that tracks execution times and call counts for specific methods using decorators.

## Features

- **Easy-to-use decorator**: Simply add `@monitor_performance` to any function
- **Comprehensive statistics**: Track execution time, call count, min/max/average times, and standard deviation
- **JSON export**: Export performance data to JSON format for analysis
- **Reset functionality**: Clear metrics when needed
- **Exception handling**: Metrics are tracked even if functions raise exceptions
- **Class method support**: Works with instance methods, class methods, and static methods

## Quick Start

### Basic Usage

```python
from performance_monitor import monitor_performance

@monitor_performance
def my_function():
    # Your code here
    return "result"

# Call your function
result = my_function()

# View performance summary
from performance_monitor import perf_monitor
perf_monitor.print_summary()
```

### Advanced Usage

```python
from performance_monitor import PerformanceMonitor

# Create a custom monitor instance
monitor = PerformanceMonitor()

@monitor.monitor
def custom_function():
    return "custom result"

# Get specific statistics
stats = monitor.get_stats("__main__.custom_function")
print(f"Average execution time: {stats['average_time']:.6f}s")

# Export to JSON
json_data = monitor.export_to_json("performance_data.json")
```

## API Reference

### PerformanceMonitor Class

#### Methods

- `monitor(func)`: Decorator to monitor a function's performance
- `get_stats(func_name=None)`: Get performance statistics for a specific function or all functions
- `reset()`: Clear all performance metrics
- `export_to_json(filename=None)`: Export statistics to JSON format
- `print_summary()`: Print a formatted summary of all performance data

#### Statistics Provided

- `call_count`: Number of times the function was called
- `total_time`: Total execution time across all calls
- `average_time`: Average execution time per call
- `min_time`: Minimum execution time
- `max_time`: Maximum execution time
- `median_time`: Median execution time
- `std_dev`: Standard deviation of execution times

### Global Convenience Functions

- `monitor_performance`: Decorator using the global monitor instance
- `perf_monitor`: Global PerformanceMonitor instance

## File Structure

```
PerfMonitor/
├── performance_monitor.py    # Main performance monitoring module
├── example_usage.py         # Example usage demonstrations
├── test_performance_monitor.py  # Comprehensive test suite
├── requirements.txt         # Project dependencies
└── README.md               # This file
```

## Running Examples

### Basic Example
```bash
python example_usage.py
```

This will demonstrate the performance monitor with various types of functions and print a comprehensive summary.

### Running Tests
```bash
python test_performance_monitor.py
```

Or using pytest (if installed):
```bash
pytest test_performance_monitor.py -v
```

## Example Output

```
================================================================================
PERFORMANCE MONITORING SUMMARY
================================================================================

Function: __main__.quick_function
  Call Count: 10
  Total Time: 0.000123s
  Average Time: 0.000012s
  Min Time: 0.000010s
  Max Time: 0.000015s
  Median Time: 0.000012s
  Std Deviation: 0.000002s

Function: __main__.slow_function
  Call Count: 3
  Total Time: 0.301234s
  Average Time: 0.100411s
  Min Time: 0.100123s
  Max Time: 0.100987s
  Median Time: 0.100234s
  Std Deviation: 0.000432s
```

## Use Cases

- **Performance optimization**: Identify slow functions in your codebase
- **Benchmarking**: Compare performance of different implementations
- **Monitoring**: Track performance changes over time
- **Debugging**: Find performance bottlenecks
- **Testing**: Validate that performance requirements are met

## Technical Details

- Uses `time.perf_counter()` for high-precision timing
- Thread-safe for basic usage
- Minimal overhead - typically less than 1μs per function call
- Preserves function metadata using `functools.wraps`
- Handles exceptions gracefully - metrics are recorded even if functions fail

## Requirements

- Python 3.6+
- No external dependencies (uses only Python standard library)

## License

This project is provided as-is for educational and development purposes. 