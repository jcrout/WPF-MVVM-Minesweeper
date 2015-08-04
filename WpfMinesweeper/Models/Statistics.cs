namespace WpfMinesweeper.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using JonUtility;

    [Serializable, DataContract]
    public enum Statistic
    {
        [Statistics(typeof (BoardSize), Description = "The width, height, and mine count of the board.", DisplayText = "Board Size")]
        BoardSize = 0,

        [Statistics(typeof (int), Description = "The number of seconds that have elapsed since the game began.", DisplayText = "Time Elapsed")]
        TimeElapsed = 1,

        [Statistics(typeof (int), Description = "The total number of unflagged mines on the board. A negative number indicates that more flags have been placed than there are mines.", DisplayText = "Mines Remaining")]
        MinesRemaining = 2,

        [Statistics(typeof (GameState), Description = "The final state of the game, which is either Incomplete, Victory, or Gameover.", DisplayText = "Result")]
        GameState = 3,

        [Statistics(typeof (int), Description = "Total number of flages placed at the end of the game.", DisplayText = "Flags Placed")]
        FlagsPlaced = 4,

        [Statistics(typeof (int), Description = "Total number of question marks placed on the board, including question marks later removed.", DisplayText = "Question Marks Placed")]
        QuestionMarksPlacedTotal = 5,

        [Statistics(typeof (int), Description = "Total number of moves resulting in tiles being revealed.", DisplayText = "Moves")]
        Moves = 6,

        [Statistics(typeof (DateTime), Description = "The time that the game began.", DisplayText = "Game Start Time")]
        GameStartTime = 7,

        [Statistics(typeof (DateTime), Description = "The time that the game ended.", DisplayText = "Game End Time")]
        GameEndTime = 8,

        [Statistics(typeof (int), Description = "The total number of matches.", DisplayText = "Count", IsSingleGameStatistic = false)]
        MatchCount = 9
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class StatisticsAttribute : Attribute
    {
        private readonly Type statType;
        private string description = string.Empty;
        private string displayText = string.Empty;
        private bool isSingleGameStatistic = true;

        public StatisticsAttribute(Type statType)
        {
            this.statType = statType;
        }

        public Type Type
        {
            get
            {
                return this.statType;
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

        public bool IsSingleGameStatistic
        {
            get
            {
                return this.isSingleGameStatistic;
            }
            set
            {
                this.isSingleGameStatistic = value;
            }
        }
    }

    public static class StatisticHelper
    {
        private static readonly Dictionary<Statistic, StatisticsAttribute> statData;

        static StatisticHelper()
        {
            var fields = typeof (Statistic).GetFields().Skip(1);
            StatisticHelper.statData = new Dictionary<Statistic, StatisticsAttribute>();

            foreach (var field in fields)
            {
                var statEnum = (Statistic)Enum.Parse(
                    typeof (Statistic),
                    field.Name);
                var statType = typeof (object);
                var statDescription = string.Empty;
                var statDisplayText = string.Empty;
                var statIsSingleGame = true;

                foreach (var customAttribute in field.CustomAttributes)
                {
                    if (customAttribute.AttributeType != typeof (StatisticsAttribute))
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
                        else if (namedArgument.MemberName == "IsSingleGameStatistic")
                        {
                            statIsSingleGame = (bool)namedArgument.TypedValue.Value;
                        }
                    }
                }

                StatisticHelper.statData.Add(
                    statEnum,
                    new StatisticsAttribute(statType)
                    {
                        Description = statDescription,
                        DisplayText = statDisplayText,
                        IsSingleGameStatistic = statIsSingleGame
                    });
            }
        }

        public static StatisticsAttribute GetAttribute(Statistic stat)
        {
            var attribute = StatisticHelper.statData[stat];
            return new StatisticsAttribute(attribute.Type)
            {
                Description = attribute.Description,
                DisplayText = attribute.DisplayText,
                IsSingleGameStatistic = attribute.IsSingleGameStatistic
            };
        }

        public static Statistic FromDisplayText(string displayText)
        {
            foreach (var pair in StatisticHelper.statData)
            {
                if (string.Equals(
                    pair.Value.DisplayText,
                    displayText))
                {
                    return pair.Key;
                }
            }

            return default(Statistic);
        }

        public static Type GetType(Statistic stat)
        {
            return StatisticHelper.statData[stat].Type;
        }

        public static string GetDescription(Statistic stat)
        {
            return StatisticHelper.statData[stat].Description;
        }

        public static string GetDisplayText(Statistic stat)
        {
            return StatisticHelper.statData[stat].DisplayText;
        }

        public static IEnumerable<KeyValuePair<Statistic, object>> GetDefaultValues()
        {
            foreach (var pair in StatisticHelper.statData)
            {
                yield return new KeyValuePair<Statistic, object>(pair.Key,
                    pair.Value.Type.GetDefaultValue());
            }
        }

        public static IEnumerable<Statistic> GetGameStatistics()
        {
            foreach (var pair in StatisticHelper.statData)
            {
                if (pair.Value.IsSingleGameStatistic)
                {
                    yield return pair.Key;
                }
            }
        }

        public static IEnumerable<Statistic> GetGlobalStatistics()
        {
            foreach (var pair in StatisticHelper.statData)
            {
                if (!pair.Value.IsSingleGameStatistic)
                {
                    yield return pair.Key;
                }
            }
        }
    }

    public class StatisticComparer : IComparer<object>
    {
        private static readonly StatisticComparer defaultComparer = new StatisticComparer();

        public static StatisticComparer Default
        {
            get
            {
                return StatisticComparer.defaultComparer;
            }
        }

        public int Compare(object x, object y)
        {
            var objType = x.GetType();
            if (objType.IsPrimitive)
            {
                var d1 = (double)Convert.ChangeType(
                    x,
                    typeof (double));
                var d2 = (double)Convert.ChangeType(
                    y,
                    typeof (double));
                return (d1 > d2 ? 1 : d1 < d2 ? -1 : 0);
            }
            if (objType == typeof (BoardSize))
            {
                return ((BoardSize)x).CompareTo(
                    (BoardSize)y);
            }
            if (objType.IsEnum)
            {
                var value1 = x.ToString();
                var value2 = y.ToString();

                return value1.CompareTo(
                    value2);
            }
            return 0;
        }
    }
}
