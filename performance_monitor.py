import time
import functools
from collections import defaultdict
from typing import Dict, List, Any, Callable
import statistics
import json


class PerformanceMonitor:
    """
    A performance monitoring class that tracks method execution times and call counts.
    """
    
    def __init__(self):
        self.metrics = defaultdict(list)
        self.call_counts = defaultdict(int)
    
    def monitor(self, func: Callable) -> Callable:
        """
        Decorator to monitor the performance of a function/method.
        
        Args:
            func: The function to monitor
            
        Returns:
            Wrapped function with performance monitoring
        """
        @functools.wraps(func)
        def wrapper(*args, **kwargs):
            start_time = time.perf_counter()
            
            try:
                result = func(*args, **kwargs)
                return result
            finally:
                end_time = time.perf_counter()
                execution_time = end_time - start_time
                
                func_name = f"{func.__module__}.{func.__qualname__}"
                self.metrics[func_name].append(execution_time)
                self.call_counts[func_name] += 1
        
        return wrapper
    
    def get_stats(self, func_name: str = None) -> Dict[str, Any]:
        """
        Get performance statistics for a specific function or all functions.
        
        Args:
            func_name: Optional function name to get stats for
            
        Returns:
            Dictionary containing performance statistics
        """
        if func_name:
            if func_name not in self.metrics:
                return {}
            
            times = self.metrics[func_name]
            return {
                'function': func_name,
                'call_count': self.call_counts[func_name],
                'total_time': sum(times),
                'average_time': statistics.mean(times),
                'min_time': min(times),
                'max_time': max(times),
                'median_time': statistics.median(times),
                'std_dev': statistics.stdev(times) if len(times) > 1 else 0
            }
        else:
            # Return stats for all functions
            all_stats = {}
            for func_name in self.metrics.keys():
                all_stats[func_name] = self.get_stats(func_name)
            return all_stats
    
    def reset(self):
        """Reset all performance metrics."""
        self.metrics.clear()
        self.call_counts.clear()
    
    def export_to_json(self, filename: str = None) -> str:
        """
        Export performance statistics to JSON format.
        
        Args:
            filename: Optional filename to save JSON to
            
        Returns:
            JSON string of performance statistics
        """
        stats = self.get_stats()
        json_data = json.dumps(stats, indent=2, default=str)
        
        if filename:
            with open(filename, 'w') as f:
                f.write(json_data)
        
        return json_data
    
    def print_summary(self):
        """Print a formatted summary of performance statistics."""
        stats = self.get_stats()
        
        if not stats:
            print("No performance data available.")
            return
        
        print("\n" + "="*80)
        print("PERFORMANCE MONITORING SUMMARY")
        print("="*80)
        
        for func_name, func_stats in stats.items():
            print(f"\nFunction: {func_name}")
            print(f"  Call Count: {func_stats['call_count']}")
            print(f"  Total Time: {func_stats['total_time']:.6f}s")
            print(f"  Average Time: {func_stats['average_time']:.6f}s")
            print(f"  Min Time: {func_stats['min_time']:.6f}s")
            print(f"  Max Time: {func_stats['max_time']:.6f}s")
            print(f"  Median Time: {func_stats['median_time']:.6f}s")
            print(f"  Std Deviation: {func_stats['std_dev']:.6f}s")


# Global instance for easy usage
perf_monitor = PerformanceMonitor()

# Convenience decorator
def monitor_performance(func: Callable) -> Callable:
    """
    Convenience decorator using the global performance monitor instance.
    
    Usage:
        @monitor_performance
        def my_function():
            pass
    """
    return perf_monitor.monitor(func) 