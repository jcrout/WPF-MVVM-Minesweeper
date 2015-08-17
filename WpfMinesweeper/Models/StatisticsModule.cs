namespace WpfMinesweeper.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using JonUtility;

    [Serializable, DataContract]
    public class StatisticsModule : IStatisticsModule
    {
        /// <summary>
        ///     The collection of stat/value pairs for the module.
        /// </summary>
        [DataMember(Name = "Stats")]
        private Dictionary<Statistic, object> stats;

        /// <summary>
        ///     Initializes a new instance of the StatisticsModule class.
        /// </summary>
        private StatisticsModule()
        {
            var defaultValues = StatisticHelper.GetGameStatistics();
            this.stats = new Dictionary<Statistic, object>(defaultValues.Count());

            foreach (var stat in StatisticHelper.GetGameStatistics())
            {
                this.stats.Add(stat, StatisticHelper.GetType(stat).GetDefaultValue());
            }
        }

        /// <summary>
        ///     Gets or sets the value of the <paramref name="statistic" /> for this module.
        /// </summary>
        /// <param name="statistic">The statistic to use as an indexer to retrieve an associated value.</param>
        /// <returns>Returns the value associated with the statistic.</returns>
        public object this[Statistic statistic]
        {
            get
            {
                return this.stats[statistic];
            }
            set
            {
                if (this.stats.ContainsKey(statistic))
                {
                    this.stats[statistic] = value;
                }
                else
                {
                    this.stats.Add(statistic, value);
                }
            }
        }

        /// <summary>
        ///     Creates and returns a new <see cref="IStatisticsModule" /> using the default implementation method.
        /// </summary>
        /// <returns>An enumerator that iterates through a collection.</returns>
        public static IStatisticsModule Create()
        {
            return new StatisticsModule();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An enumerator that iterates through a collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        ///     Returns an enumerator that iterates through a generic collection.
        /// </summary>
        public IEnumerator<KeyValuePair<Statistic, object>> GetEnumerator()
        {
            foreach (var pair in this.stats)
            {
                yield return pair;
            }
        }
    }
}
