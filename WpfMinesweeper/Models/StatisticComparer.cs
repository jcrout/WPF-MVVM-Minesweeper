namespace WpfMinesweeper.Models
{
    using System;
    using System.Collections.Generic;

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
}
