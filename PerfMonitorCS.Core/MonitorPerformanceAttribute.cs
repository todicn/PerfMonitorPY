using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PerfMonitorCS.Core
{
    /// <summary>
    /// Attribute to mark methods for performance monitoring.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class MonitorPerformanceAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the performance monitor instance to use.
        /// If null, uses the global monitor.
        /// </summary>
        public PerformanceMonitor? Monitor { get; set; }

        /// <summary>
        /// Gets or sets a custom name for the method being monitored.
        /// </summary>
        public string? MethodName { get; set; }

        /// <summary>
        /// Initializes a new instance of the MonitorPerformanceAttribute class.
        /// </summary>
        /// <param name="methodName">Optional custom method name</param>
        public MonitorPerformanceAttribute(string? methodName = null)
        {
            MethodName = methodName;
        }
    }

    /// <summary>
    /// Static class providing performance monitoring functionality.
    /// </summary>
    public static class PerformanceMonitorStatic
    {
        /// <summary>
        /// Global performance monitor instance.
        /// </summary>
        public static PerformanceMonitor GlobalMonitor { get; } = new PerformanceMonitor();

        /// <summary>
        /// Measures the execution time of an action and records it.
        /// </summary>
        /// <param name="action">The action to measure</param>
        /// <param name="methodName">The name of the method being measured</param>
        /// <param name="monitor">Optional specific monitor instance</param>
        /// <returns>The result of the action</returns>
        public static T MeasureExecution<T>(Func<T> action, string? methodName = null, PerformanceMonitor? monitor = null)
        {
            monitor ??= GlobalMonitor;
            methodName ??= GetCallerMethodName();

            var stopwatch = Stopwatch.StartNew();
            try
            {
                return action();
            }
            finally
            {
                stopwatch.Stop();
                var executionTime = stopwatch.Elapsed.TotalSeconds;
                monitor.RecordExecution(methodName, executionTime);
            }
        }

        /// <summary>
        /// Measures the execution time of an async action and records it.
        /// </summary>
        /// <param name="action">The async action to measure</param>
        /// <param name="methodName">The name of the method being measured</param>
        /// <param name="monitor">Optional specific monitor instance</param>
        /// <returns>The result of the action</returns>
        public static async Task<T> MeasureExecution<T>(Func<Task<T>> action, string? methodName = null, PerformanceMonitor? monitor = null)
        {
            monitor ??= GlobalMonitor;
            methodName ??= GetCallerMethodName();

            var stopwatch = Stopwatch.StartNew();
            try
            {
                return await action();
            }
            finally
            {
                stopwatch.Stop();
                var executionTime = stopwatch.Elapsed.TotalSeconds;
                monitor.RecordExecution(methodName, executionTime);
            }
        }

        /// <summary>
        /// Measures the execution time of an action and records it.
        /// </summary>
        /// <param name="action">The action to measure</param>
        /// <param name="methodName">The name of the method being measured</param>
        /// <param name="monitor">Optional specific monitor instance</param>
        public static void MeasureExecution(Action action, string? methodName = null, PerformanceMonitor? monitor = null)
        {
            monitor ??= GlobalMonitor;
            methodName ??= GetCallerMethodName();

            var stopwatch = Stopwatch.StartNew();
            try
            {
                action();
            }
            finally
            {
                stopwatch.Stop();
                var executionTime = stopwatch.Elapsed.TotalSeconds;
                monitor.RecordExecution(methodName, executionTime);
            }
        }

        /// <summary>
        /// Measures the execution time of an async action and records it.
        /// </summary>
        /// <param name="action">The async action to measure</param>
        /// <param name="methodName">The name of the method being measured</param>
        /// <param name="monitor">Optional specific monitor instance</param>
        public static async Task MeasureExecution(Func<Task> action, string? methodName = null, PerformanceMonitor? monitor = null)
        {
            monitor ??= GlobalMonitor;
            methodName ??= GetCallerMethodName();

            var stopwatch = Stopwatch.StartNew();
            try
            {
                await action();
            }
            finally
            {
                stopwatch.Stop();
                var executionTime = stopwatch.Elapsed.TotalSeconds;
                monitor.RecordExecution(methodName, executionTime);
            }
        }

        /// <summary>
        /// Gets the name of the calling method.
        /// </summary>
        /// <param name="memberName">Automatically filled by the compiler</param>
        /// <param name="sourceFilePath">Automatically filled by the compiler</param>
        /// <returns>The formatted method name</returns>
        private static string GetCallerMethodName(
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "")
        {
            var fileName = System.IO.Path.GetFileNameWithoutExtension(sourceFilePath);
            return $"{fileName}.{memberName}";
        }
    }

    /// <summary>
    /// Extension methods for performance monitoring.
    /// </summary>
    public static class PerformanceMonitorExtensions
    {
        /// <summary>
        /// Wraps a function with performance monitoring.
        /// </summary>
        /// <typeparam name="T">Return type</typeparam>
        /// <param name="func">Function to monitor</param>
        /// <param name="methodName">Method name for tracking</param>
        /// <param name="monitor">Monitor instance</param>
        /// <returns>Wrapped function</returns>
        public static Func<T> WithPerformanceMonitoring<T>(
            this Func<T> func, 
            string? methodName = null, 
            PerformanceMonitor? monitor = null)
        {
            return () => PerformanceMonitorStatic.MeasureExecution(func, methodName, monitor);
        }

        /// <summary>
        /// Wraps an action with performance monitoring.
        /// </summary>
        /// <param name="action">Action to monitor</param>
        /// <param name="methodName">Method name for tracking</param>
        /// <param name="monitor">Monitor instance</param>
        /// <returns>Wrapped action</returns>
        public static Action WithPerformanceMonitoring(
            this Action action, 
            string? methodName = null, 
            PerformanceMonitor? monitor = null)
        {
            return () => PerformanceMonitorStatic.MeasureExecution(action, methodName, monitor);
        }
    }
} 