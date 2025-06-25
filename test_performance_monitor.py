import unittest
import time
import json
import os
import tempfile
from performance_monitor import PerformanceMonitor, monitor_performance, perf_monitor


class TestPerformanceMonitor(unittest.TestCase):
    """Test cases for the PerformanceMonitor class."""
    
    def setUp(self):
        """Set up test fixtures before each test method."""
        self.monitor = PerformanceMonitor()
    
    def tearDown(self):
        """Clean up after each test method."""
        self.monitor.reset()
        perf_monitor.reset()
    
    def test_monitor_decorator_tracks_execution_time(self):
        """Test that the monitor decorator tracks execution time."""
        @self.monitor.monitor
        def test_function():
            time.sleep(0.01)  # Sleep for 10ms
            return "test"
        
        # Call the function
        result = test_function()
        
        # Check that the function returned the expected result
        self.assertEqual(result, "test")
        
        # Check that metrics were recorded
        func_name = f"{test_function.__module__}.{test_function.__qualname__}"
        self.assertIn(func_name, self.monitor.metrics)
        self.assertEqual(self.monitor.call_counts[func_name], 1)
        
        # Check that execution time is reasonable (should be >= 10ms)
        execution_time = self.monitor.metrics[func_name][0]
        self.assertGreaterEqual(execution_time, 0.009)  # Allow for some timing variance
    
    def test_multiple_calls_tracking(self):
        """Test that multiple calls to the same function are tracked correctly."""
        @self.monitor.monitor
        def test_function():
            time.sleep(0.001)  # Sleep for 1ms
            return 42
        
        # Call the function multiple times
        for _ in range(5):
            test_function()
        
        func_name = f"{test_function.__module__}.{test_function.__qualname__}"
        
        # Check call count
        self.assertEqual(self.monitor.call_counts[func_name], 5)
        
        # Check that we have 5 execution times recorded
        self.assertEqual(len(self.monitor.metrics[func_name]), 5)
    
    def test_get_stats_for_specific_function(self):
        """Test getting statistics for a specific function."""
        @self.monitor.monitor
        def test_function():
            time.sleep(0.001)
            return True
        
        # Call function multiple times
        for _ in range(3):
            test_function()
        
        func_name = f"{test_function.__module__}.{test_function.__qualname__}"
        stats = self.monitor.get_stats(func_name)
        
        # Validate stats structure and values
        self.assertEqual(stats['function'], func_name)
        self.assertEqual(stats['call_count'], 3)
        self.assertGreater(stats['total_time'], 0)
        self.assertGreater(stats['average_time'], 0)
        self.assertGreater(stats['min_time'], 0)
        self.assertGreater(stats['max_time'], 0)
        self.assertGreaterEqual(stats['median_time'], 0)
        self.assertGreaterEqual(stats['std_dev'], 0)
    
    def test_get_stats_all_functions(self):
        """Test getting statistics for all monitored functions."""
        @self.monitor.monitor
        def function_a():
            time.sleep(0.001)
            return "A"
        
        @self.monitor.monitor
        def function_b():
            time.sleep(0.002)
            return "B"
        
        # Call both functions
        function_a()
        function_b()
        
        # Get stats for all functions
        all_stats = self.monitor.get_stats()
        
        # Should have stats for both functions
        self.assertEqual(len(all_stats), 2)
        
        func_a_name = f"{function_a.__module__}.{function_a.__qualname__}"
        func_b_name = f"{function_b.__module__}.{function_b.__qualname__}"
        
        self.assertIn(func_a_name, all_stats)
        self.assertIn(func_b_name, all_stats)
    
    def test_reset_functionality(self):
        """Test that reset clears all metrics."""
        @self.monitor.monitor
        def test_function():
            return "test"
        
        # Call function and verify metrics exist
        test_function()
        func_name = f"{test_function.__module__}.{test_function.__qualname__}"
        self.assertIn(func_name, self.monitor.metrics)
        self.assertEqual(self.monitor.call_counts[func_name], 1)
        
        # Reset and verify metrics are cleared
        self.monitor.reset()
        self.assertEqual(len(self.monitor.metrics), 0)
        self.assertEqual(len(self.monitor.call_counts), 0)
    
    def test_export_to_json(self):
        """Test exporting performance data to JSON."""
        @self.monitor.monitor
        def test_function():
            time.sleep(0.001)
            return "json_test"
        
        # Call function
        test_function()
        
        # Test JSON export without file
        json_data = self.monitor.export_to_json()
        self.assertIsInstance(json_data, str)
        
        # Parse JSON to verify it's valid
        parsed_data = json.loads(json_data)
        self.assertIsInstance(parsed_data, dict)
        
        # Test JSON export with file
        with tempfile.NamedTemporaryFile(mode='w', delete=False, suffix='.json') as temp_file:
            temp_filename = temp_file.name
        
        try:
            self.monitor.export_to_json(temp_filename)
            
            # Verify file was created and contains valid JSON
            self.assertTrue(os.path.exists(temp_filename))
            
            with open(temp_filename, 'r') as f:
                file_data = json.load(f)
            
            self.assertIsInstance(file_data, dict)
        finally:
            # Clean up temp file
            if os.path.exists(temp_filename):
                os.unlink(temp_filename)
    
    def test_convenience_decorator(self):
        """Test the convenience decorator using global monitor."""
        @monitor_performance
        def test_global_function():
            time.sleep(0.001)
            return "global_test"
        
        # Call function
        result = test_global_function()
        self.assertEqual(result, "global_test")
        
        # Check that global monitor recorded the metrics
        func_name = f"{test_global_function.__module__}.{test_global_function.__qualname__}"
        self.assertIn(func_name, perf_monitor.metrics)
        self.assertEqual(perf_monitor.call_counts[func_name], 1)
    
    def test_function_with_exception(self):
        """Test that performance is still tracked even if function raises exception."""
        @self.monitor.monitor
        def failing_function():
            time.sleep(0.001)
            raise ValueError("Test exception")
        
        # Call function and expect exception
        with self.assertRaises(ValueError):
            failing_function()
        
        # Verify that metrics were still recorded
        func_name = f"{failing_function.__module__}.{failing_function.__qualname__}"
        self.assertIn(func_name, self.monitor.metrics)
        self.assertEqual(self.monitor.call_counts[func_name], 1)
    
    def test_class_method_monitoring(self):
        """Test monitoring class methods."""
        class TestClass:
            def __init__(self, monitor):
                self.monitor = monitor
            
            @monitor_performance
            def instance_method(self):
                time.sleep(0.001)
                return "instance"
            
            @classmethod
            @monitor_performance
            def class_method(cls):
                time.sleep(0.001)
                return "class"
            
            @staticmethod
            @monitor_performance
            def static_method():
                time.sleep(0.001)
                return "static"
        
        # Test instance method
        test_obj = TestClass(self.monitor)
        result = test_obj.instance_method()
        self.assertEqual(result, "instance")
        
        # Test class method
        result = TestClass.class_method()
        self.assertEqual(result, "class")
        
        # Test static method
        result = TestClass.static_method()
        self.assertEqual(result, "static")
        
        # Verify all methods were tracked
        all_stats = perf_monitor.get_stats()
        self.assertGreaterEqual(len(all_stats), 3)
    
    def test_stats_for_nonexistent_function(self):
        """Test getting stats for a function that doesn't exist."""
        stats = self.monitor.get_stats("nonexistent.function")
        self.assertEqual(stats, {})
    
    def test_statistics_calculations(self):
        """Test that statistical calculations are correct."""
        execution_times = [0.001, 0.002, 0.003, 0.004, 0.005]
        
        @self.monitor.monitor
        def controlled_function(delay):
            time.sleep(delay)
            return delay
        
        # Call function with known delays
        for delay in execution_times:
            controlled_function(delay)
        
        func_name = f"{controlled_function.__module__}.{controlled_function.__qualname__}"
        stats = self.monitor.get_stats(func_name)
        
        # Verify statistics (allowing for small timing variations)
        self.assertEqual(stats['call_count'], 5)
        self.assertAlmostEqual(stats['total_time'], sum(execution_times), delta=0.005)
        self.assertAlmostEqual(stats['average_time'], sum(execution_times) / len(execution_times), delta=0.005)


class TestIntegration(unittest.TestCase):
    """Integration tests for the performance monitoring system."""
    
    def setUp(self):
        perf_monitor.reset()
    
    def tearDown(self):
        perf_monitor.reset()
    
    def test_full_workflow(self):
        """Test the complete workflow of monitoring, getting stats, and exporting."""
        @monitor_performance
        def workflow_function(n):
            """Function for workflow testing."""
            total = 0
            for i in range(n):
                total += i
            time.sleep(0.001)  # Small delay for timing
            return total
        
        # Execute function multiple times with different parameters
        results = []
        for n in [10, 50, 100]:
            for _ in range(3):  # 3 calls for each parameter
                results.append(workflow_function(n))
        
        # Verify results
        self.assertEqual(len(results), 9)
        
        # Get and verify stats
        func_name = f"{workflow_function.__module__}.{workflow_function.__qualname__}"
        stats = perf_monitor.get_stats(func_name)
        
        self.assertEqual(stats['call_count'], 9)
        self.assertGreater(stats['total_time'], 0)
        self.assertGreater(stats['average_time'], 0)
        
        # Test JSON export
        json_data = perf_monitor.export_to_json()
        parsed_data = json.loads(json_data)
        
        self.assertIn(func_name, parsed_data)
        self.assertEqual(parsed_data[func_name]['call_count'], 9)


if __name__ == '__main__':
    # Run the tests
    unittest.main(verbosity=2) 