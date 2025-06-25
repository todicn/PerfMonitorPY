import time
import random
from performance_monitor import PerformanceMonitor, monitor_performance


# Example functions to monitor
@monitor_performance
def quick_function():
    """A quick function that does minimal work."""
    return sum(range(100))


@monitor_performance
def slow_function():
    """A slower function that simulates some work."""
    time.sleep(0.1)  # Simulate 100ms of work
    return sum(range(1000))


@monitor_performance
def variable_time_function():
    """A function with variable execution time."""
    sleep_time = random.uniform(0.01, 0.05)  # Random sleep between 10-50ms
    time.sleep(sleep_time)
    return random.randint(1, 100)


@monitor_performance
def fibonacci(n):
    """Calculate fibonacci number (inefficient recursive version for demo)."""
    if n <= 1:
        return n
    return fibonacci(n-1) + fibonacci(n-2)


class Calculator:
    """Example class with monitored methods."""
    
    def __init__(self):
        self.monitor = PerformanceMonitor()
    
    @monitor_performance
    def add(self, a, b):
        """Add two numbers with some artificial delay."""
        time.sleep(0.001)  # 1ms delay
        return a + b
    
    @monitor_performance
    def multiply(self, a, b):
        """Multiply two numbers with some artificial delay."""
        time.sleep(0.002)  # 2ms delay
        return a * b
    
    @monitor_performance
    def complex_calculation(self, n):
        """Perform a more complex calculation."""
        result = 0
        for i in range(n):
            result += i ** 2
        return result


def run_example():
    """Run the example demonstration."""
    print("Running Performance Monitor Example...")
    print("="*50)
    
    # Run quick function multiple times
    print("Running quick_function 10 times...")
    for _ in range(10):
        quick_function()
    
    # Run slow function a few times
    print("Running slow_function 3 times...")
    for _ in range(3):
        slow_function()
    
    # Run variable time function multiple times
    print("Running variable_time_function 5 times...")
    for _ in range(5):
        variable_time_function()
    
    # Run fibonacci with different inputs
    print("Running fibonacci with different inputs...")
    for n in [5, 8, 10]:
        fibonacci(n)
    
    # Test class methods
    print("Testing Calculator class methods...")
    calc = Calculator()
    
    # Run calculator methods
    for i in range(3):
        calc.add(i, i+1)
        calc.multiply(i, 2)
    
    calc.complex_calculation(1000)
    calc.complex_calculation(5000)
    
    # Print summary
    print("\n" + "="*50)
    print("PERFORMANCE SUMMARY")
    print("="*50)
    
    from performance_monitor import perf_monitor
    perf_monitor.print_summary()
    
    # Export to JSON
    print("\nExporting performance data to JSON...")
    json_data = perf_monitor.export_to_json("performance_data.json")
    print("Performance data exported to 'performance_data.json'")


if __name__ == "__main__":
    run_example() 