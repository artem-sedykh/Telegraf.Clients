using System;
using System.Threading.Tasks;
using Telegraf.Infrastructure;

namespace Telegraf.Statsd.Client
{
    public interface ITelegrafStatsdClient
    {
        /// <summary>
        /// Increments counter <paramref name="measurement"/> by <paramref name="value"/>.
        /// </summary>
        /// <remarks>A Counter is a Gauge calculated at the server.</remarks>
        void Counter(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null);

        /// <summary>
        /// Increments counter <paramref name="measurement"/> by <paramref name="value"/>.
        /// </summary>
        /// <remarks>A Counter is a Gauge calculated at the server.</remarks>
        Task CounterAsync(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null);

        /// <summary>
        /// Increments counter <paramref name="measurement"/> by <paramref name="value"/>.
        /// </summary>
        /// <remarks>A Counter is a Gauge calculated at the server.</remarks>
        void Counter(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder);

        /// <summary>
        /// Increments counter <paramref name="measurement"/> by <paramref name="value"/>.
        /// </summary>
        /// <remarks>A Counter is a Gauge calculated at the server.</remarks>
        Task CounterAsync(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder);

        /// <summary>
        /// Records an arbitrary <paramref name="value"/> for gauge <paramref name="measurement"/>.
        /// </summary>
        /// <remarks>
        /// A Gauge is an instantaneous measurement of a value, like the gas gauge in a car. It differs from a counter by being calculated at the client rather than the server.
        /// </remarks>
        void Gauge(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null);

        /// <summary>
        /// Records an arbitrary <paramref name="value"/> for gauge <paramref name="measurement"/>.
        /// </summary>
        /// <remarks>
        /// A Gauge is an instantaneous measurement of a value, like the gas gauge in a car. It differs from a counter by being calculated at the client rather than the server.
        /// </remarks>
        Task GaugeAsync(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null);

        /// <summary>
        /// Records an arbitrary <paramref name="value"/> for gauge <paramref name="measurement"/>.
        /// </summary>
        /// <remarks>
        /// A Gauge is an instantaneous measurement of a value, like the gas gauge in a car. It differs from a counter by being calculated at the client rather than the server.
        /// </remarks>
        void Gauge(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder);

        /// <summary>
        /// Records an arbitrary <paramref name="value"/> for gauge <paramref name="measurement"/>.
        /// </summary>
        /// <remarks>
        /// A Gauge is an instantaneous measurement of a value, like the gas gauge in a car. It differs from a counter by being calculated at the client rather than the server.
        /// </remarks>
        Task GaugeAsync(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder);

        /// <summary>
        /// Records the measurement for timer <paramref name="measurement"/>.
        /// </summary>
        /// <remarks>
        /// A Timer is a measure of the number of milliseconds elapsed between a start and end time, for example the time to complete rendering of a web page for a user.
        /// </remarks>
        void Time(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null);

        /// <summary>
        /// Records the measurement for timer <paramref name="measurement"/>.
        /// </summary>
        /// <remarks>
        /// A Timer is a measure of the number of milliseconds elapsed between a start and end time, for example the time to complete rendering of a web page for a user.
        /// </remarks>
        Task TimeAsync(string measurement, double value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null);

        /// <summary>
        /// Records the measurement for timer <paramref name="measurement"/>.
        /// </summary>
        /// <remarks>
        /// A Timer is a measure of the number of milliseconds elapsed between a start and end time, for example the time to complete rendering of a web page for a user.
        /// </remarks>
        void Time(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder);

        /// <summary>
        /// Records the measurement for timer <paramref name="measurement"/>.
        /// </summary>
        /// <remarks>
        /// A Timer is a measure of the number of milliseconds elapsed between a start and end time, for example the time to complete rendering of a web page for a user.
        /// </remarks>
        Task TimeAsync(string measurement, double value, Func<ITagBuilder, ITagBuilder> builder);

        /// <summary>
        /// Records a unique occurence of <paramref name="value"/> between flushes.
        /// </summary>
        void Set(string measurement, string value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null);

        /// <summary>
        /// Records a unique occurence of <paramref name="value"/> between flushes.
        /// </summary>
        Task SetAsync(string measurement, string value, double sample = 1, Func<ITagBuilder, ITagBuilder> builder = null);

        /// <summary>
        /// Records a unique occurence of <paramref name="value"/>.
        /// </summary>
        void Set(string measurement, string value, Func<ITagBuilder, ITagBuilder> builder);

        /// <summary>
        /// Records a unique occurence of <paramref name="value"/>.
        /// </summary>
        Task SetAsync(string measurement, string value, Func<ITagBuilder, ITagBuilder> builder);
    }
}
