using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace PerfMonitorCS.Core
{
    /// <summary>
    /// A performance monitoring class that tracks method execution times and call counts.
    /// Thread-safe implementation using concurrent collections.
    /// </summary>
    public class PerformanceMonitor
    {
        private readonly ConcurrentDictionary<string, List<double>> _metrics;
        private readonly ConcurrentDictionary<string, int> _callCounts;
        private readonly object _lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the PerformanceMonitor class.
        /// </summary>
        public PerformanceMonitor()
        {
            _metrics = new ConcurrentDictionary<string, List<double>>();
            _callCounts = new ConcurrentDictionary<string, int>();
        }

        /// <summary>
        /// Records the execution time for a method call.
        /// </summary>
        /// <param name="methodName">The name of the method</param>
        /// <param name="executionTime">The execution time in seconds</param>
        public void RecordExecution(string methodName, double executionTime)
        {
            lock (_lockObject)
            {
                _metrics.AddOrUpdate(methodName,
                    new List<double> { executionTime },
                    (key, existing) =>
                    {
                        existing.Add(executionTime);
                        return existing;
                    });

                _callCounts.AddOrUpdate(methodName, 1, (key, existing) => existing + 1);
            }
        }

        /// <summary>
        /// Gets performance statistics for a specific method or all methods.
        /// </summary>
        /// <param name="methodName">Optional method name to get stats for</param>
        /// <returns>Dictionary containing performance statistics</returns>
        public Dictionary<string, object> GetStats(string? methodName = null)
        {
            if (!string.IsNullOrEmpty(methodName))
            {
                if (!_metrics.ContainsKey(methodName))
                    return new Dictionary<string, object>();

                return GetMethodStats(methodName);
            }

            var allStats = new Dictionary<string, object>();
            foreach (var method in _metrics.Keys)
            {
                allStats[method] = GetMethodStats(method);
            }
            return allStats;
        }

        private Dictionary<string, object> GetMethodStats(string methodName)
        {
            if (!_metrics.TryGetValue(methodName, out var times) || times.Count == 0)
                return new Dictionary<string, object>();

            var sortedTimes = times.OrderBy(x => x).ToList();
            var sum = times.Sum();
            var count = times.Count;
            var mean = sum / count;

            // Calculate standard deviation
            var variance = times.Sum(x => Math.Pow(x - mean, 2)) / count;
            var stdDev = count > 1 ? Math.Sqrt(variance) : 0;

            // Calculate median
            var median = count % 2 == 0
                ? (sortedTimes[count / 2 - 1] + sortedTimes[count / 2]) / 2
                : sortedTimes[count / 2];

            return new Dictionary<string, object>
            {
                ["function"] = methodName,
                ["call_count"] = _callCounts.GetValueOrDefault(methodName, 0),
                ["total_time"] = sum,
                ["average_time"] = mean,
                ["min_time"] = sortedTimes.First(),
                ["max_time"] = sortedTimes.Last(),
                ["median_time"] = median,
                ["std_dev"] = stdDev
            };
        }

        /// <summary>
        /// Resets all performance metrics.
        /// </summary>
        public void Reset()
        {
            lock (_lockObject)
            {
                _metrics.Clear();
                _callCounts.Clear();
            }
        }

        /// <summary>
        /// Exports performance statistics to JSON format.
        /// </summary>
        /// <param name="filename">Optional filename to save JSON to</param>
        /// <returns>JSON string of performance statistics</returns>
        public async Task<string> ExportToJsonAsync(string? filename = null)
        {
            var stats = GetStats();
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonData = JsonSerializer.Serialize(stats, options);

            if (!string.IsNullOrEmpty(filename))
            {
                await File.WriteAllTextAsync(filename, jsonData);
            }

            return jsonData;
        }

        /// <summary>
        /// Synchronous version of ExportToJsonAsync.
        /// </summary>
        /// <param name="filename">Optional filename to save JSON to</param>
        /// <returns>JSON string of performance statistics</returns>
        public string ExportToJson(string? filename = null)
        {
            return ExportToJsonAsync(filename).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Prints a formatted summary of performance statistics to the console.
        /// </summary>
        public void PrintSummary()
        {
            var stats = GetStats();

            if (stats.Count == 0)
            {
                Console.WriteLine("No performance data available.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine(new string('=', 80));
            Console.WriteLine("PERFORMANCE MONITORING SUMMARY");
            Console.WriteLine(new string('=', 80));

            foreach (var kvp in stats)
            {
                var methodName = kvp.Key;
                var methodStats = (Dictionary<string, object>)kvp.Value;

                Console.WriteLine($"\nFunction: {methodName}");
                Console.WriteLine($"  Call Count: {methodStats["call_count"]}");
                Console.WriteLine($"  Total Time: {methodStats["total_time"]:F6}s");
                Console.WriteLine($"  Average Time: {methodStats["average_time"]:F6}s");
                Console.WriteLine($"  Min Time: {methodStats["min_time"]:F6}s");
                Console.WriteLine($"  Max Time: {methodStats["max_time"]:F6}s");
                Console.WriteLine($"  Median Time: {methodStats["median_time"]:F6}s");
                Console.WriteLine($"  Std Deviation: {methodStats["std_dev"]:F6}s");
            }
        }
    }
} 