namespace WpfMinesweeper.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    public interface IStatisticsModule : IEnumerable<KeyValuePair<Statistic, object>>
    {
        object this[Statistic statistic] { get; set; }

        string GetDescription(Statistic statistic);

    }

    [DataContract]
    public class StatisticsModule : IStatisticsModule
    {
        [DataMember(Name = "Stats")]
        private Dictionary<Statistic, object> stats;

        public static IStatisticsModule Create()
        {
            return new StatisticsModule();
        }

        public StatisticsModule()
        {
            var defaultValues = StatisticHelper.GetDefaultValues();
            this.stats = new Dictionary<Statistic, object>(defaultValues.Count());

            foreach (var pair in StatisticHelper.GetDefaultValues())
            {
                this.stats.Add(pair.Key, pair.Value);
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
                    this.stats.Add(statistic, value);
                }
            }
        }

        public string GetDescription(Statistic statistic)
        {
            return StatisticHelper.GetStatDescription(statistic);
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
