using System;
using System.Threading;
using System.Threading.Tasks;
using PerfMonitorCS.Core;

namespace PerfMonitorCS.Examples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Running Performance Monitor Example...");
            Console.WriteLine(new string('=', 50));

            await RunExample();
        }

        static async Task RunExample()
        {
            // Run quick function multiple times
            Console.WriteLine("Running QuickFunction 10 times...");
            for (int i = 0; i < 10; i++)
            {
                await QuickFunction();
            }

            // Run slow function a few times
            Console.WriteLine("Running SlowFunction 3 times...");
            for (int i = 0; i < 3; i++)
            {
                await SlowFunction();
            }

            // Run variable time function multiple times
            Console.WriteLine("Running VariableTimeFunction 5 times...");
            for (int i = 0; i < 5; i++)
            {
                await VariableTimeFunction();
            }

            // Run fibonacci with different inputs
            Console.WriteLine("Running Fibonacci with different inputs...");
            await Fibonacci(5);
            await Fibonacci(8);
            await Fibonacci(10);

            // Test class methods
            Console.WriteLine("Testing Calculator class methods...");
            var calc = new Calculator();

            // Run calculator methods
            for (int i = 0; i < 3; i++)
            {
                await calc.AddAsync(i, i + 1);
                await calc.MultiplyAsync(i, 2);
            }

            await calc.ComplexCalculationAsync(1000);
            await calc.ComplexCalculationAsync(5000);

            // Print summary
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("PERFORMANCE SUMMARY");
            Console.WriteLine(new string('=', 50));

            PerformanceMonitorStatic.GlobalMonitor.PrintSummary();

            // Export to JSON
            Console.WriteLine("\nExporting performance data to JSON...");
            await PerformanceMonitorStatic.GlobalMonitor.ExportToJsonAsync("performance_data.json");
            Console.WriteLine("Performance data exported to 'performance_data.json'");
        }

        /// <summary>
        /// A quick function that does minimal work.
        /// </summary>
        static async Task<int> QuickFunction()
        {
            return await PerformanceMonitorStatic.MeasureExecution(async () =>
            {
                await Task.Delay(1); // Minimal delay
                var sum = 0;
                for (int i = 0; i < 100; i++)
                {
                    sum += i;
                }
                return sum;
            }, nameof(QuickFunction));
        }

        /// <summary>
        /// A slower function that simulates some work.
        /// </summary>
        static async Task<int> SlowFunction()
        {
            return await PerformanceMonitorStatic.MeasureExecution(async () =>
            {
                await Task.Delay(100); // Simulate 100ms of work
                var sum = 0;
                for (int i = 0; i < 1000; i++)
                {
                    sum += i;
                }
                return sum;
            }, nameof(SlowFunction));
        }

        /// <summary>
        /// A function with variable execution time.
        /// </summary>
        static async Task<int> VariableTimeFunction()
        {
            return await PerformanceMonitorStatic.MeasureExecution(async () =>
            {
                var random = new Random();
                var sleepTime = random.Next(10, 50); // Random sleep between 10-50ms
                await Task.Delay(sleepTime);
                return random.Next(1, 100);
            }, nameof(VariableTimeFunction));
        }

        /// <summary>
        /// Calculate fibonacci number (inefficient recursive version for demo).
        /// </summary>
        static async Task<long> Fibonacci(int n)
        {
            return await PerformanceMonitorStatic.MeasureExecution(async () =>
            {
                await Task.Delay(1); // Small delay to make it measurable
                return FibonacciRecursive(n);
            }, $"Fibonacci({n})");
        }

        static long FibonacciRecursive(int n)
        {
            if (n <= 1) return n;
            return FibonacciRecursive(n - 1) + FibonacciRecursive(n - 2);
        }
    }

    /// <summary>
    /// Example class with monitored methods.
    /// </summary>
    public class Calculator
    {
        private readonly PerformanceMonitor _monitor = new PerformanceMonitor();

        /// <summary>
        /// Add two numbers with some artificial delay.
        /// </summary>
        public async Task<int> AddAsync(int a, int b)
        {
            return await PerformanceMonitorStatic.MeasureExecution(async () =>
            {
                await Task.Delay(1); // 1ms delay
                return a + b;
            }, $"{nameof(Calculator)}.{nameof(AddAsync)}");
        }

        /// <summary>
        /// Multiply two numbers with some artificial delay.
        /// </summary>
        public async Task<int> MultiplyAsync(int a, int b)
        {
            return await PerformanceMonitorStatic.MeasureExecution(async () =>
            {
                await Task.Delay(2); // 2ms delay
                return a * b;
            }, $"{nameof(Calculator)}.{nameof(MultiplyAsync)}");
        }

        /// <summary>
        /// Perform a more complex calculation.
        /// </summary>
        public async Task<long> ComplexCalculationAsync(int n)
        {
            return await PerformanceMonitorStatic.MeasureExecution(async () =>
            {
                await Task.Delay(1);
                long result = 0;
                for (int i = 0; i < n; i++)
                {
                    result += (long)i * i;
                }
                return result;
            }, $"{nameof(Calculator)}.{nameof(ComplexCalculationAsync)}");
        }
    }
}
