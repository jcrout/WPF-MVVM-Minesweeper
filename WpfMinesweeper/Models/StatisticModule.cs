namespace WpfMinesweeper.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using JonUtility;

    public interface IStatisticsModule : IEnumerable<KeyValuePair<Statistic, object>>
    {
        object this[Statistic statistic] { get; set; }

        string GetDescription(Statistic statistic);
    }

    [Serializable, DataContract]
    public class StatisticsModule : IStatisticsModule
    {
        [DataMember(Name = "Stats")] private Dictionary<Statistic, object> stats;

        public static IStatisticsModule Create()
        {
            return new StatisticsModule();
        }

        public StatisticsModule()
        {
            var defaultValues = StatisticHelper.GetGameStatistics();
            this.stats = new Dictionary<Statistic, object>(defaultValues.Count());

            foreach (var stat in StatisticHelper.GetGameStatistics())
            {
                this.stats.Add(stat,
                    StatisticHelper.GetType(stat).GetDefaultValue());
            }
        }

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
                    this.stats.Add(statistic,
                        value);
                }
            }
        }

        public string GetDescription(Statistic statistic)
        {
            return StatisticHelper.GetDescription(statistic);
        }

        public IEnumerator<KeyValuePair<Statistic, object>> GetEnumerator()
        {
            foreach (var pair in this.stats)
            {
                yield return pair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}