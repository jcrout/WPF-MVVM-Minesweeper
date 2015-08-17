namespace WpfMinesweeper.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JonUtility;

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

                StatisticHelper.statData.Add(statEnum, new StatisticsAttribute(statType) {Description = statDescription, DisplayLabel = statDisplayText, IsAggregate = statIsAggregate});
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
}
