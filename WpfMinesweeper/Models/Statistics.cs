namespace WpfMinesweeper.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using JonUtility;

    public enum Statistic
    {
        [Statistics(typeof(BoardSize), Description="The width, height, and mine count of the board.")] //, DisplayText="Board Size")]
        BoardSize = 0,

        [Statistics(typeof(int), Description = "The number of seconds that have elapsed since the game began.", DisplayText="Time Elapsed")]
        TimeElapsed,

        [Statistics(typeof(int), Description = "The total number of unflagged mines on the board. A negative number indicates that more flags have been placed than there are mines.", DisplayText="Mines Remaining")]
        MinesRemaining,

        [Statistics(typeof(bool), Description = "The final state of the game, which is either Incomplete, Victory, or Gameover.", DisplayText="Result")]
        GameState,

        [Statistics(typeof(int), Description = "Total number of flages placed at the end of the game.", DisplayText="Flags Placed")]
        FlagsPlaced,

        [Statistics(typeof(int), Description = "Total number of question marks placed on the board, including question marks later removed.", DisplayText="Question Marks Placed")]
        QuestionMarksPlacedTotal,

        [Statistics(typeof(int), Description = "Total number of moves resulting in tiles being revealed.", DisplayText = "Moves")]
        Moves,

        [Statistics(typeof(DateTime), Description = "The time that the game began.", DisplayText = "Game Start Time")]
        GameStartTime,

        [Statistics(typeof(DateTime), Description = "The time that the game ended.", DisplayText = "Game End Time")]
        GameEndTime
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class StatisticsAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        private readonly Type statType;
        private string description = string.Empty;
        private string displayText = string.Empty;

        public StatisticsAttribute(Type statType)
        {
            this.statType = statType;

            // TODO: Implement code here
            throw new NotImplementedException();
        }

        public Type Type
        {
            get
            {
                return statType;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (value == null)
                {
                    this.description = string.Empty;
                }
                else
                {
                    this.description = value;
                }
            }
        }

        public string DisplayText
        {
            get
            {
                return this.displayText;
            }
            set
            {
                if (value == null)
                {
                    this.displayText = string.Empty;
                }
                else
                {
                    this.displayText = value;
                }
            }
        }
    }
    
    public enum GameState
    {
        Unfinished,
        Victory,
        GameOver
    }

    public interface IStatisticsModule : IEnumerable<IStatisticsModule>
    {
        object this[Statistic statistic] { get; set; }

        string GetDescription(Statistic statistic);

        void AddStatisticsModule(IStatisticsModule statisticsModule);
    }

    public class StatisticsModule : IStatisticsModule
    {
        private static Dictionary<Statistic, Tuple<object, string>> statData;
        private Dictionary<Statistic, object> stats;
        private List<IStatisticsModule> containedModules;

        static StatisticsModule()
        {
            var fields = typeof(Statistic).GetFields().Skip(1);
            statData = new Dictionary<Statistic, Tuple<object, string>>();

            foreach (var field in fields)
            {
                var statEnum = (Statistic)Enum.Parse(typeof(Statistic), field.Name);
                var statType = typeof(object);
                var statDescription = string.Empty;
                foreach (var customAttribute in field.CustomAttributes)
                {
                    if (customAttribute.AttributeType != typeof(StatisticsAttribute))
                    {
                        continue;
                    }

                    statType = (Type)customAttribute.ConstructorArguments[0].Value;
                    foreach (var namedArgument in customAttribute.NamedArguments)
                    {
                        if (namedArgument.MemberName == "Description")
                        {
                            statDescription = namedArgument.TypedValue.Value.ToString();
                        }
                    }
                }

                statData.Add(statEnum, new Tuple<object, string>(statType.GetDefaultValue(), statDescription));
            }
        }

        public static IStatisticsModule Create()
        {
            return new StatisticsModule();
        }

        public StatisticsModule()
        {
            this.stats = new Dictionary<Statistic, object>();
            foreach (var keyPair in statData)
            {
                this.stats.Add(keyPair.Key, keyPair.Value.Item1);
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

        public void AddStatisticsModule(IStatisticsModule statisticsModule)
        {
            if (this.containedModules == null)
            {
                this.containedModules = new List<IStatisticsModule> { statisticsModule };
            }
            else
            {
                this.containedModules.Add(statisticsModule);
            }
        }

        public string GetDescription(Statistic statistic)
        {
            return statData[statistic].Item2;
        }

        public IEnumerator<IStatisticsModule> GetEnumerator()
        {
            if (this.containedModules == null)
            {
                yield break;
            }

            foreach (var module in this.containedModules)
            {
                yield return module;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
