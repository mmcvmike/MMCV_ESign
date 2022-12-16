﻿using System;
using System.Web;

namespace MMCV_Common.Helper
{
    static public class CacheHelper
    {
        public static object Get(string key)
        {
            return HttpContext.Current.Cache[key];
        }

        public static void Set(string key, object value)
        {
            HttpContext.Current.Cache.Insert(key, value, null,
                DateTime.Now.AddMinutes(40),
                TimeSpan.Zero,
                System.Web.Caching.CacheItemPriority.Default, null
                );
        }

        public static void Set(string key, object value, int minutes)
        {
            HttpContext.Current.Cache.Insert(key, value, null,
                DateTime.Now.AddMinutes(minutes),
                TimeSpan.Zero,
                System.Web.Caching.CacheItemPriority.Default, null
                );
        }

        public static void Set(string key, object value, int minutes, System.Web.Caching.CacheDependency dependence)
        {
            HttpContext.Current.Cache.Insert(key, value, dependence,
                DateTime.Now.AddMinutes(minutes),
                TimeSpan.Zero,
                System.Web.Caching.CacheItemPriority.Default, null
                );
        }

        public static void SetCacheDay(string key, object value, int day)
        {
            HttpContext.Current.Cache.Insert(key, value, null,
                DateTime.Now.AddDays(day),
                TimeSpan.Zero,
                System.Web.Caching.CacheItemPriority.Default, null
                );
        }

        public static void Remove(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }
    }
}