using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DMC.IO
{
    public static class ExtensionMethods
    {
        public static string ToINIString(this object data)
        {
            return INIHelper.SerializeToINIString(data, "").ToString();
        }

        public static object FromINIString(this string iniString, Type dataType)
        {
            Dictionary<string, string> iniDictionary = INIHelper.BuildINIDictionary(iniString);

            return INIHelper.SetPropertyValues(dataType, dataType, "", iniDictionary, true);
        }

        /// <summary>
        /// Get the display name (from the LabVIEWNameAttribute, if present) for a class/property.
        /// Otherwise, returns the string name of the class/property.
        /// </summary>
        /// <param name="propertyInfo">MemberInfo object for the class/property in question.</param>
        /// <returns>Display name or class/property name.</returns>
        public static string GetDisplayName(this MemberInfo propertyInfo)
        {
            object[] atts = propertyInfo.GetCustomAttributes(typeof(INIDisplayNameAttribute), false);

            if (atts.Length > 0)
            {
                return (atts[0] as INIDisplayNameAttribute).DisplayName;
            }
            else
            {
                return propertyInfo.Name;
            }
        }

        public static string GetDisplayName(this Type enumType, object theValue)
        {
            foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
            {
                if ((int)theValue == (int)field.GetValue(null))
                {
                    return field.GetDisplayName();
                }
            }

            return theValue.ToString();
        }

        public static bool IsBasicType(this Type type)
        {
            return type.IsPrimitive ||
                    (type == typeof(string)) ||
                    (type == typeof(DateTime));
        }
    }
}
