using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DMCBase;

namespace DMC.IO
{
    public static class INIHelper
    {
        public static StringBuilder SerializeToINIString(object eventData, string prefix)
        {
            StringBuilder iniString = new StringBuilder();

            if (eventData.GetType().IsBasicType())
            {
                iniString = iniString.Append(eventData);
            }
            else
            {
                foreach (PropertyInfo pi in eventData.GetType().GetProperties())
                {
                    object value = pi.GetValue(eventData, null);
                    string displayName = pi.GetDisplayName();
                    Type type = pi.PropertyType;

                    if (type.IsBasicType())
                    {
                        iniString.Append(string.Format("{0}{1}={2}\n", prefix, displayName, value));
                    }
                    else if (type.IsEnum)
                    {
                        iniString.Append(string.Format("{0}{1}={2}\n", prefix, displayName, type.GetDisplayName(value)));
                    }
                    else if (type.IsArray)
                    {
                        Type elementType = type.GetElementType();
                        Array arr = value as Array;

                        if (arr == null)
                        {
                            arr = Array.CreateInstance(type, new int[type.GetArrayRank()]);
                        }

                        iniString.Append(string.Format("{0}{1}=", prefix, displayName));

                        for (int i = 0; i < arr.Rank; i++)
                        {
                            string text = string.Format(
                                "{0}{1}{2}{3}",
                                i == 0 ? "<" : " ",
                                arr.GetLength(i),
                                i == arr.Rank - 1 ? ">" : "",
                                ((i == arr.Rank - 1) && !elementType.IsPrimitive) ? "\n" : "");

                            iniString.Append(text);
                        }

                        if (arr.Length == 0)
                        {
                            iniString.Append("\n");
                        }
                        else
                        {
                            int[] indices = new int[arr.Rank];

                            for (int i = 0; i < arr.Length; i++)
                            {
                                GetArrayIndices(i, arr, indices);

                                if (elementType.IsPrimitive)
                                {
                                    iniString.Append(
                                        string.Format(
                                            "{0}{1}",
                                            arr.GetValue(indices),
                                            i == arr.Length - 1 ? "\n" : ","));
                                }
                                else
                                {
                                    string arrayPrefix = BuildArrayPrefix(indices, displayName);

                                    if (elementType == typeof(string) ||
                                        elementType == typeof(bool) ||
                                        elementType == typeof(DateTime))
                                    {
                                        iniString.Append(string.Format("{0}{1}={2}\n", prefix, arrayPrefix, arr.GetValue(indices)));
                                    }
                                    else
                                    {
                                        object elementValue = arr.GetValue(indices);

                                        if (elementValue == null)
                                        {
                                            elementValue = Activator.CreateInstance(elementType);
                                        }

                                        iniString.Append(SerializeToINIString(elementValue, string.Format("{0}{1}.{2}.", prefix, arrayPrefix, elementType.GetDisplayName())));
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (value == null)
                        {
                            value = Activator.CreateInstance(pi.PropertyType);
                        }

                        iniString.Append(SerializeToINIString(value, string.Format("{0}{1}.", prefix, displayName)));
                    }
                }
            }

            return iniString;
        }

        public static Dictionary<string, string> BuildINIDictionary(string iniString)
        {
            Dictionary<string, string> returnDictionary = new Dictionary<string, string>();

            string[] tokens = iniString.Split(new char[] { '\n' });

            foreach (string token in tokens)
            {
                string[] strKeyAndValues = token.Split(new char[] { '=' });

                if (!returnDictionary.ContainsKey(strKeyAndValues[0]))
                {
                    returnDictionary.Add(strKeyAndValues[0], strKeyAndValues.Length > 1 ? strKeyAndValues[1] : "");
                }
            }

            return returnDictionary;
        }

        public static object SetPropertyValues(Type type, MemberInfo memberInfo, string prefix, Dictionary<string, string> iniDictionary, bool topLevel)
        {
            //Debug.WriteLine( "Start SetPropertyValues\t" + Environment.TickCount.ToString() );
            string displayName = (memberInfo is PropertyInfo) ? memberInfo.GetDisplayName() : type.GetDisplayName();

            string key = prefix + displayName;

            if (type.IsBasicType() || type.IsEnum)
            {
                if (iniDictionary.ContainsKey(key))
                {
                    return ParsePropertyValuePerType(type, iniDictionary[key]);
                }
                else
                {
                    return type.GetDefaultObject();
                }
            }
            //else if (type == typeof(LVImage))
            //{
            //    if (iniDictionary.ContainsKey(key))
            //    {
            //        return new LVImage(Encoding.ASCII.GetBytes(iniDictionary[key]));
            //    }
            //    else
            //    {
            //        return this.GetDefaultObject(type);
            //    }
            //}
            else if (type.IsArray)
            {
                Type elementType = type.GetElementType();

                if (iniDictionary.ContainsKey(key))
                {
                    string[] tokens = iniDictionary[key].Split(new char[] { '<', '>' });
                    string[] strDimensions = tokens[1].Split(new char[] { ' ' });
                    int[] dimensions = new int[strDimensions.Length];
                    for (int i = 0; i < strDimensions.Length; i++)
                    {
                        dimensions[i] = int.Parse(strDimensions[i]);
                    }

                    Array objValues = Array.CreateInstance(elementType, dimensions);
                    int[] indices = new int[strDimensions.Length];
                    string[] strPrimitiveValues = null;
                    object value = null;

                    for (int i = 0; i < objValues.Length; i++)
                    {
                        GetArrayIndices(i, objValues, indices);

                        if (elementType.IsPrimitive && tokens.Length == 3)
                        {
                            if (i == 0)
                            {
                                strPrimitiveValues = tokens[2].Split(new char[] { ',' });
                            }

                            value = ParsePropertyValuePerType(elementType, strPrimitiveValues[i]);
                        }
                        else
                        {
                            string arrayPrefix = BuildArrayPrefix(indices, displayName);

                            if (elementType == typeof(string) ||
                                elementType == typeof(bool) ||
                                elementType == typeof(DateTime))
                            {
                                value = ParsePropertyValuePerType(elementType, iniDictionary[arrayPrefix]);
                            }
                            else
                            {
                                value = Activator.CreateInstance(elementType);

                                foreach (PropertyInfo pi in elementType.GetProperties())
                                {
                                    object propertyValue = SetPropertyValues(pi.PropertyType, pi, string.Format("{0}{1}.{2}.", prefix, arrayPrefix, elementType.GetDisplayName()), iniDictionary, false);

                                    pi.SetValue(value, propertyValue, null);
                                }
                            }
                        }

                        objValues.SetValue(value, indices);
                    }

                    return objValues;
                }
                else
                {
                    return Array.CreateInstance(elementType, new int[type.GetArrayRank()]);
                }
            }
            else
            {
                object tempObject = Activator.CreateInstance(type);

                foreach (PropertyInfo pi in type.GetProperties())
                {
                    pi.SetValue(tempObject, SetPropertyValues(pi.PropertyType, pi, topLevel ? prefix : key + ".", iniDictionary, false), null);
                }

                return tempObject;
            }
        }

        public static void GetArrayIndices(int absoluteIndex, Array arr, int[] indeces)
        {
            for (int j = 0; j < arr.Rank; j++)
            {
                int product = 1;
                for (int k = arr.Rank - 1; k > j; k--)
                {
                    product *= arr.GetLength(k);
                }

                indeces[j] = (absoluteIndex / product) % arr.GetLength(j);
            }
        }

        public static string BuildArrayPrefix(int[] indices, string displayName)
        {
            string arrayPrefix = displayName;

            foreach (int index in indices)
            {
                arrayPrefix += string.Format(" {0}", index);
            }

            return arrayPrefix;
        }

        private static object ParsePropertyValuePerType(Type type, string strValue)
        {
            if (type == typeof(string))
            {
                return strValue;
            }
            else if (type.IsEnum)
            {
                foreach (FieldInfo field in type.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
                {
                    if (field.GetDisplayName() == strValue)
                    {
                        return field.GetValue(null);
                    }
                }

                return type.GetDefaultObject();
            }
            else
            {
                try
                {
                    MethodInfo mi = type.GetMethod("Parse", new Type[] { typeof(string) });

                    return mi.Invoke(null, new object[] { strValue });
                }
                catch (Exception)
                {
                    return Activator.CreateInstance(type);
                }
            }
        }
    }
}
