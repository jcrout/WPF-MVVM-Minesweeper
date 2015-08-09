namespace WpfMinesweeper.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using JonUtility;
    using System.Reflection;

    [Serializable, DataContract]
    public enum Statistic
    {
        [Statistics(typeof(BoardSize), Description = "The width, height, and mine count of the board.", DisplayLabel = "Board Size")]
        BoardSize = 0,

        [Statistics(typeof(int), Description = "The number of seconds that have elapsed since the game began.", DisplayLabel = "Time Elapsed")]
        TimeElapsed = 1,

        [Statistics(typeof(int), Description = "The total number of unflagged mines on the board. A negative number indicates that more flags have been placed than there are mines.", DisplayLabel = "Mines Remaining")]
        MinesRemaining = 2,

        [Statistics(typeof(GameResult), Description = "The final state of the game, which is either Incomplete, Victory, or Gameover.", DisplayLabel = "Result")]
        GameState = 3,

        [Statistics(typeof(int), Description = "Total number of flages placed at the end of the game.", DisplayLabel = "Flags Placed")]
        FlagsPlaced = 4,

        [Statistics(typeof(int), Description = "Total number of question marks placed on the board, including question marks later removed.", DisplayLabel = "Question Marks Placed")]
        QuestionMarksPlacedTotal = 5,

        [Statistics(typeof(int), Description = "Total number of moves resulting in tiles being revealed.", DisplayLabel = "Moves")]
        Moves = 6,

        [Statistics(typeof(DateTime), Description = "The time that the game began.", DisplayLabel = "Game Start Time")]
        GameStartTime = 7,

        [Statistics(typeof(DateTime), Description = "The time that the game ended.", DisplayLabel = "Game End Time")]
        GameEndTime = 8,

        [Statistics(typeof(int), Description = "The total number of matches.", DisplayLabel = "Count", IsAggregate = true)]
        MatchCount = 9
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
                var d1 = (double)Convert.ChangeType(x, typeof(double));
                var d2 = (double)Convert.ChangeType(y, typeof(double));
                return (d1 > d2 ? 1 : d1 < d2 ? -1 : 0);
            }
            if (objType == typeof(BoardSize))
            {
                return ((BoardSize)x).CompareTo((BoardSize)y);
            }
            if (objType.IsEnum)
            {
                var value1 = x.ToString();
                var value2 = y.ToString();

                return String.Compare(value1, value2, StringComparison.Ordinal);
            }
            return 0;
        }
    }

    public static class StatisticHelper
    {
        private static readonly Dictionary<Statistic, StatisticsAttribute> statData;

        static StatisticHelper()
        {
            var fields = typeof(Statistic).GetFields().Skip(1);
            StatisticHelper.statData = new Dictionary<Statistic, StatisticsAttribute>();

            foreach (var field in fields)
            {
                var statEnum = (Statistic)Enum.Parse(typeof(Statistic), field.Name);
                var statType = typeof(object);
                var statDescription = string.Empty;
                var statDisplayText = string.Empty;
                var statIsAggregate = false;

                var statAttributes =
                    field.CustomAttributes.Where(
                        customAttribute => customAttribute.AttributeType == typeof(StatisticsAttribute));

                foreach (var customAttribute in statAttributes)
                {
                    statType = (Type)customAttribute.ConstructorArguments[0].Value;

                    foreach (var namedArgument in customAttribute.NamedArguments)
                    {
                        switch (namedArgument.MemberName)
                        {
                            case "Description":
                                statDescription = namedArgument.TypedValue.Value.ToString();
                                break;
                            case "DisplayLabel":
                                statDisplayText = namedArgument.TypedValue.Value.ToString();
                                break;
                            case "IsAggregate":
                                statIsAggregate = (bool)namedArgument.TypedValue.Value;
                                break;
                        }
                    }
                }

                StatisticHelper.statData.Add(statEnum, new StatisticsAttribute(statType) { Description = statDescription, DisplayLabel = statDisplayText, IsAggregate = statIsAggregate});
            }
        }

        public static StatisticsAttribute GetAttribute(Statistic stat)
        {
            var attribute = StatisticHelper.statData[stat];
            return new StatisticsAttribute(attribute.Type) {Description = attribute.Description, DisplayLabel = attribute.DisplayLabel, IsAggregate = attribute.IsAggregate};
        }

        public static Statistic FromDisplayText(string displayText)
        {
            foreach (var pair in StatisticHelper.statData)
            {
                if (string.Equals(pair.Value.DisplayLabel, displayText))
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
            return StatisticHelper.statData[stat].DisplayLabel;
        }

        public static IEnumerable<KeyValuePair<Statistic, object>> GetDefaultValues()
        {
            foreach (var pair in StatisticHelper.statData)
            {
                yield return new KeyValuePair<Statistic, object>(pair.Key, pair.Value.Type.GetDefaultValue());
            }
        }

        public static IEnumerable<Statistic> GetGameStatistics()
        {
            foreach (var pair in StatisticHelper.statData)
            {
                if (!pair.Value.IsAggregate)
                {
                    yield return pair.Key;
                }
            }
        }

        public static IEnumerable<Statistic> GetGlobalStatistics()
        {
            foreach (var pair in StatisticHelper.statData)
            {
                if (pair.Value.IsAggregate)
                {
                    yield return pair.Key;
                }
            }
        }
    }

    /// <summary>
    ///     Specifies meta-data for a <see cref="Statistic"/> enumeration field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class StatisticsAttribute : Attribute
    {
        private string description = string.Empty;
        private string displayLabel = string.Empty;

        public StatisticsAttribute(Type statType)
        {
            if (statType == null)
            {
                throw new ArgumentNullException("statType");
            }

            this.Type = statType;
        }

        /// <summary>
        ///     Gets or sets a description of the stat.
        /// </summary>
        /// <value>The description string. This value defaults to an empty string on <see langword="null"/>.</value>
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets or sets the display label of the stat.
        /// </summary>
        /// <value>The display label string. This value defaults to an empty string on <see langword="null"/>.</value>
        public string DisplayLabel
        {
            get
            {
                return this.displayLabel;
            }
            set
            {
                this.displayLabel = value ?? string.Empty;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Statistic"/> is recorded during individual games.
        /// </summary>
        /// <value><c>true</c> if this instance is single game statistic; otherwise, <c>false</c>.</value>
        public bool IsAggregate { get; set; }

        public Type Type { get; private set; }
    }
}
