using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using AutoMapper;
using Newtonsoft.Json;

namespace Medelit.Common
{
    public static class MedelitExtensions
    {
        public static string Serialize(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return JsonConvert.SerializeObject(str);
            }
            return str;
        }
        public static dynamic DeSerialize(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return JsonConvert.DeserializeObject(str);
            }
            return str;
        }

        public static string CLower(this string str)
        {
            if (!string.IsNullOrEmpty(str))
                return str.ToLower();
            return str;
        }

        public static string CString(this DateTime? str)
        {
            if (str.HasValue)
                return $"'{str}'";
            return "null";
        }

        public static string CString(this bool? str)
        {
            if (str.HasValue)
                return $"'{str}'";
            return "null";
        }

        public static string CString(this decimal? str)
        {
            if (str.HasValue)
                return $"{str}";
            return "null";
        }

        public static short ToInt(this Enum enumValue)
        {
            return (short)((object)enumValue);
        }

        public static string GetDescription(this Enum enumeration)
        {
            string value = enumeration.ToString();
            Type enumType = enumeration.GetType();
            var descAttribute = (DescriptionAttribute[])enumType
                .GetField(value)
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return descAttribute.Length > 0 ? descAttribute[0].Description : value;
        }

        public static string GetDisplayText<T>(T enumMember)
                   where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new Exception("Requires enum only");

            var a = enumMember
                    .GetType()
                    .GetField(enumMember.ToString())
                    .GetCustomAttribute<DescriptionAttribute>();
            return a == null ? enumMember.ToString() : a.Description;
        }

        public static Dictionary<int, string> ParseToDictionary<T>()
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new Exception("Requires enum only");

            Dictionary<int, string> dict = new Dictionary<int, string>();
            T _enum = default(T);
            foreach (var f in _enum.GetType().GetFields())
            {
                if (f.GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute i)
                    dict.Add((int)f.GetValue(_enum), i == null ? f.ToString() : i.Description);
            }
            return dict;
        }

        public static List<(int Value, string DisplayText)> ParseToTupleList<T>()
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new Exception("Requires enum only");

            List<(int, string)> tupleList = new List<(int, string)>();
            T _enum = default(T);
            foreach (var f in _enum.GetType().GetFields())
            {
                if (f.GetCustomAttribute<DescriptionAttribute>() is DescriptionAttribute i)
                    tupleList.Add(((int)f.GetValue(_enum), i == null ? f.ToString() : i.Description));
            }
            return tupleList;
        }
        public static string sReplace(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            return str.Trim().Replace("'", "''").Replace(",,","");
        }


    }
}
