using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MMCV_Common.Helper
{
    public static class Extension
    {
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }
            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(System.DayOfWeek))
                {
                    DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row[property.Name].ToString());
                    property.SetValue(item, day, null);
                }
                else
                {
                    //if (row[property.Name] == DBNull.Value)
                    //    property.SetValue(item, null, null);
                    //else
                    //    property.SetValue(item, row[property.Name], null);
                    try
                    {
                        if (property.PropertyType == typeof(System.String))
                            property.SetValue(item, StringHelper.TCVN3ToUnicode(row[property.Name].ToString()), null);
                        else
                            property.SetValue(item, row[property.Name], null);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            return item;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<List<T>> SplitList<T>(this List<T> source, int nSize = 30)
        {
            for (int i = 0; i < source.Count; i += nSize)
            {
                yield return source.GetRange(i, Math.Min(nSize, source.Count - i));
            }
        }

        /// <summary>
        /// TuyenNV: Just apply where search when field condition is not null or not empty
        /// Ex: source.WhereIf(str != null, x => x.Field == str)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition
                ? source.Where(predicate)
                : source;
        }
    }
}
