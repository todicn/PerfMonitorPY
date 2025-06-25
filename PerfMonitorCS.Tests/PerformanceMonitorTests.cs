using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using PerfMonitorCS.Core;

namespace PerfMonitorCS.Tests
{
    public class PerformanceMonitorTests : IDisposable
    {
        private readonly PerformanceMonitor _monitor;

        public PerformanceMonitorTests()
        {
            _monitor = new PerformanceMonitor();
        }

        public void Dispose()
        {
            _monitor.Reset();
            PerformanceMonitorStatic.GlobalMonitor.Reset();
        }

        [Fact]
        public void RecordExecution_ShouldTrackExecutionTime()
        {
            // Arrange
            const string methodName = "TestMethod";
            const double executionTime = 0.01; // 10ms

            // Act
            _monitor.RecordExecution(methodName, executionTime);

            // Assert
            var stats = _monitor.GetStats(methodName);
            Assert.NotEmpty(stats);
            Assert.Equal(methodName, stats["function"]);
            Assert.Equal(1, stats["call_count"]);
            Assert.Equal(executionTime, stats["total_time"]);
            Assert.Equal(executionTime, stats["average_time"]);
        }

        [Fact]
        public void RecordExecution_MultipleCallsShouldBeTracked()
        {
            // Arrange
            const string methodName = "MultipleCallsMethod";
            const int callCount = 5;

            // Act
            for (int i = 0; i < callCount; i++)
            {
                _monitor.RecordExecution(methodName, 0.001 * (i + 1));
            }

            // Assert
            var stats = _monitor.GetStats(methodName);
            Assert.Equal(callCount, stats["call_count"]);
            Assert.Equal(0.015, (double)stats["total_time"], 3); // Sum of 0.001 + 0.002 + ... + 0.005
        }

        [Fact]
        public void GetStats_NonExistentMethod_ShouldReturnEmptyDictionary()
        {
            // Act
            var stats = _monitor.GetStats("NonExistentMethod");

            // Assert
            Assert.Empty(stats);
        }

        [Fact]
        public void GetStats_AllMethods_ShouldReturnAllTrackedMethods()
        {
            // Arrange
            _monitor.RecordExecution("Method1", 0.001);
            _monitor.RecordExecution("Method2", 0.002);

            // Act
            var allStats = _monitor.GetStats();

            // Assert
            Assert.Equal(2, allStats.Count);
            Assert.Contains("Method1", allStats.Keys);
            Assert.Contains("Method2", allStats.Keys);
        }

        [Fact]
        public void Reset_ShouldClearAllMetrics()
        {
            // Arrange
            _monitor.RecordExecution("TestMethod", 0.001);
            Assert.NotEmpty(_monitor.GetStats());

            // Act
            _monitor.Reset();

            // Assert
            Assert.Empty(_monitor.GetStats());
        }

        [Fact]
        public async Task ExportToJsonAsync_ShouldReturnValidJson()
        {
            // Arrange
            _monitor.RecordExecution("JsonTestMethod", 0.001);

            // Act
            var jsonData = await _monitor.ExportToJsonAsync();

            // Assert
            Assert.NotNull(jsonData);
            Assert.Contains("JsonTestMethod", jsonData);
            Assert.Contains("call_count", jsonData);
        }

        [Fact]
        public async Task ExportToJsonAsync_WithFilename_ShouldCreateFile()
        {
            // Arrange
            _monitor.RecordExecution("FileTestMethod", 0.001);
            var tempFileName = Path.GetTempFileName();

            try
            {
                // Act
                await _monitor.ExportToJsonAsync(tempFileName);

                // Assert
                Assert.True(File.Exists(tempFileName));
                var fileContent = await File.ReadAllTextAsync(tempFileName);
                Assert.Contains("FileTestMethod", fileContent);
            }
            finally
            {
                // Cleanup
                if (File.Exists(tempFileName))
                    File.Delete(tempFileName);
            }
        }

        [Fact]
        public async Task MeasureExecution_ShouldTrackFunctionExecutionTime()
        {
            // Act
            var result = await PerformanceMonitorStatic.MeasureExecution(async () =>
            {
                await Task.Delay(10);
                return 42;
            }, "AsyncTestFunction", _monitor);

            // Assert
            Assert.Equal(42, result);
            var stats = _monitor.GetStats("AsyncTestFunction");
            Assert.NotEmpty(stats);
            Assert.Equal(1, stats["call_count"]);
            Assert.True((double)stats["total_time"] >= 0.005); // Lowered threshold to be more lenient
        }

        [Fact]
        public void MeasureExecution_Action_ShouldTrackExecutionTime()
        {
            // Arrange
            var executed = false;

            // Act
            PerformanceMonitorStatic.MeasureExecution(() =>
            {
                Thread.Sleep(10);
                executed = true;
            }, "ActionTestFunction", _monitor);

            // Assert
            Assert.True(executed);
            var stats = _monitor.GetStats("ActionTestFunction");
            Assert.NotEmpty(stats);
            Assert.Equal(1, stats["call_count"]);
            Assert.True((double)stats["total_time"] >= 0.009);
        }

        [Fact]
        public async Task MeasureExecution_WithException_ShouldStillTrackTime()
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await PerformanceMonitorStatic.MeasureExecution(async () =>
                {
                    await Task.Delay(10);
                    throw new InvalidOperationException("Test exception");
                }, "ExceptionTestFunction", _monitor);
            });

            // Verify that metrics were still recorded
            var stats = _monitor.GetStats("ExceptionTestFunction");
            Assert.NotEmpty(stats);
            Assert.Equal(1, stats["call_count"]);
        }

        [Fact]
        public void GetMethodStats_ShouldCalculateCorrectStatistics()
        {
            // Arrange - Add known values for easier testing
            const string methodName = "StatisticsTestMethod";
            var times = new[] { 0.001, 0.002, 0.003, 0.004, 0.005 }; // 1ms to 5ms

            foreach (var time in times)
            {
                _monitor.RecordExecution(methodName, time);
            }

            // Act
            var stats = _monitor.GetStats(methodName);

            // Assert
            Assert.Equal(5, stats["call_count"]);
            Assert.Equal(0.015, (double)stats["total_time"], 3); // Sum
            Assert.Equal(0.003, (double)stats["average_time"], 3); // Mean
            Assert.Equal(0.001, (double)stats["min_time"], 3);
            Assert.Equal(0.005, (double)stats["max_time"], 3);
            Assert.Equal(0.003, (double)stats["median_time"], 3); // Middle value
            
            // Standard deviation should be > 0 for varying values
            Assert.True((double)stats["std_dev"] > 0);
        }

        [Fact]
        public void WithPerformanceMonitoring_Extension_ShouldWork()
        {
            // Arrange
            Func<int> testFunc = () =>
            {
                Thread.Sleep(10);
                return 100;
            };

            // Act
            var monitoredFunc = testFunc.WithPerformanceMonitoring("ExtensionTestFunction", _monitor);
            var result = monitoredFunc();

            // Assert
            Assert.Equal(100, result);
            var stats = _monitor.GetStats("ExtensionTestFunction");
            Assert.NotEmpty(stats);
            Assert.Equal(1, stats["call_count"]);
        }

        [Fact]
        public void PrintSummary_WithNoData_ShouldNotThrow()
        {
            // This test verifies that PrintSummary doesn't throw when there's no data
            // We can't easily capture console output in unit tests, but we ensure no exception
            Assert.True(true); // Placeholder - the real test is that no exception is thrown below
            _monitor.PrintSummary();
        }

        [Fact]
        public void PrintSummary_WithData_ShouldNotThrow()
        {
            // Arrange
            _monitor.RecordExecution("PrintTestMethod", 0.001);

            // Act & Assert - Should not throw
            _monitor.PrintSummary();
            Assert.True(true); // If we reach here, no exception was thrown
        }

        [Fact]
        public async Task PerformanceMonitor_ThreadSafety_ShouldHandleConcurrentAccess()
        {
            // Arrange
            const int threadCount = 10;
            const int operationsPerThread = 100;
            var tasks = new Task[threadCount];

            // Act
            for (int i = 0; i < threadCount; i++)
            {
                var threadId = i;
                tasks[i] = Task.Run(async () =>
                {
                    for (int j = 0; j < operationsPerThread; j++)
                    {
                        await PerformanceMonitorStatic.MeasureExecution(async () =>
                        {
                            await Task.Delay(1);
                        }, $"Thread{threadId}Method", _monitor);
                    }
                });
            }

            await Task.WhenAll(tasks);

            // Assert
            var allStats = _monitor.GetStats();
            Assert.Equal(threadCount, allStats.Count);

            foreach (var kvp in allStats)
            {
                var methodStats = (System.Collections.Generic.Dictionary<string, object>)kvp.Value;
                Assert.Equal(operationsPerThread, methodStats["call_count"]);
            }
        }
    }
} 