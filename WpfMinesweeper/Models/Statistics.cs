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
    using System.Runtime.Serialization;
    using System.IO;

    [Serializable, DataContract]
    public enum Statistic
    {
        [Statistics(typeof(BoardSize), Description = "The width, height, and mine count of the board.", DisplayText = "Board Size")]
        BoardSize = 0,

        [Statistics(typeof(int), Description = "The number of seconds that have elapsed since the game began.", DisplayText = "Time Elapsed")]
        TimeElapsed = 1,

        [Statistics(typeof(int), Description = "The total number of unflagged mines on the board. A negative number indicates that more flags have been placed than there are mines.", DisplayText = "Mines Remaining")]
        MinesRemaining = 2,

        [Statistics(typeof(GameState), Description = "The final state of the game, which is either Incomplete, Victory, or Gameover.", DisplayText = "Result")]
        GameState = 3,

        [Statistics(typeof(int), Description = "Total number of flages placed at the end of the game.", DisplayText = "Flags Placed")]
        FlagsPlaced = 4,

        [Statistics(typeof(int), Description = "Total number of question marks placed on the board, including question marks later removed.", DisplayText = "Question Marks Placed")]
        QuestionMarksPlacedTotal = 5,

        [Statistics(typeof(int), Description = "Total number of moves resulting in tiles being revealed.", DisplayText = "Moves")]
        Moves = 6,

        [Statistics(typeof(DateTime), Description = "The time that the game began.", DisplayText = "Game Start Time")]
        GameStartTime = 7,

        [Statistics(typeof(DateTime), Description = "The time that the game ended.", DisplayText = "Game End Time")]
        GameEndTime = 8
    }

    public sealed class StatisticHelper
    {
        private static Dictionary<Statistic, Tuple<Type, string, string>> statData;

        static StatisticHelper()
        {
            var fields = typeof(Statistic).GetFields().Skip(1);
            statData = new Dictionary<Statistic, Tuple<Type, string, string>>();

            foreach (var field in fields)
            {
                var statEnum = (Statistic)Enum.Parse(typeof(Statistic), field.Name);
                var statType = typeof(object);
                var statDescription = string.Empty;
                var statDisplayText = string.Empty;

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
                        else if (namedArgument.MemberName == "DisplayText")
                        {
                            statDisplayText = namedArgument.TypedValue.Value.ToString();
                        }
                    }
                }

                statData.Add(statEnum, new Tuple<Type, string, string>(statType, statDescription, statDisplayText));
            }
        }

        public static Type GetStatType(Statistic stat)
        {
            return statData[stat].Item1;
        }

        public static string GetStatDescription(Statistic stat)
        {
            return statData[stat].Item2;
        }

        public static string GetStatDisplayText(Statistic stat)
        {
            return statData[stat].Item3;
        }

        public static IEnumerable<KeyValuePair<Statistic, object>> GetDefaultValues()
        {
            foreach (var pair in statData)
            {
                yield return new KeyValuePair<Statistic, object>(pair.Key, pair.Value.Item1.GetDefaultValue());
            }
        }
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

    [Serializable, DataContract(Name = "GameState")]
    public enum GameState
    {
        Unfinished,
        Victory,
        GameOver
    }

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

            foreach (var pair in  StatisticHelper.GetDefaultValues())
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
