using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace Common
{
    public static class EnumHelper
    {
        /// <summary>
        /// Get the enum value description, or return the enum string if no description
        /// </summary>
        /// <param name="EnumType"></param>
        /// <param name="EnumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription(string EnumType, object EnumValue)
        {
            return GetEnumDescription(GetEnumTypeFromName(EnumType), EnumValue);
        }

        /// <summary>
        /// Get the enum value description, or return the enum string if no description
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="EnumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription<T>(object EnumValue) where T : struct, IConvertible
        {
            return GetEnumDescription(typeof(T), EnumValue);
        }

        public static string GetEnumDescription<T>(T EnumValue) where T : struct, IConvertible
        {
            return GetEnumDescription(typeof(T), EnumValue);
        }

        /// <summary>
        /// Get the enum value description, or return the enum string if no description
        /// </summary>
        /// <param name="EnumType"></param>
        /// <param name="EnumValue"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Type EnumType, object EnumValue)
        {
            if ((EnumType == null) || (EnumValue == null))
                return string.Empty;

            var EnumValueString = EnumValue.ToString().Trim();
            if (EnumValueString == string.Empty)
                return string.Empty;

            EnumType = Nullable.GetUnderlyingType(EnumType) ?? EnumType;

            if (!EnumType.IsEnum)
                return string.Empty;

            //parse the value into an enum. this can be a number, or a string 
            //saves the value into the base type of the enum (int, etc)
            object e;
            try
            {
                e = Enum.Parse(EnumType, EnumValueString);
            }
            catch
            {
                return string.Empty;
            }

            if (e == null)
                return string.Empty;

            //check if the value of the base type is defined in the enum
            if (!Enum.IsDefined(EnumType, e))
                return string.Empty;

            //get the name of the enum entry from the base type
            string s = Enum.GetName(EnumType, e);
            if (s == null)
                return string.Empty;

            //try to get the description attribute if it exists, 
            //otherwise return the name of the enum entry
            var memInfo = EnumType.GetMember(s);
            if ((memInfo == null) || (memInfo.Count() == 0))
                return s;

            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((attributes == null) || (attributes.Length == 0))
                return s;

            return ((DescriptionAttribute)attributes[0]).Description;
        }


        /// <summary>
        /// Get the enum value as the type defined by the enum (int, byte, etc)
        /// </summary>
        /// <param name="EnumType"></param>
        /// <param name="EnumValue"></param>
        /// <returns></returns>
        public static object GetUnderlyingEnumValue(string EnumType, object EnumValue)
        {
            return GetUnderlyingEnumValue(GetEnumTypeFromName(EnumType), EnumValue);
        }

        /// <summary>
        /// Get the enum value as the type defined by the enum (int, byte, etc)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="EnumValue"></param>
        /// <returns></returns>
        public static object GetUnderlyingEnumValue<T>(object EnumValue) where T : struct, IConvertible
        {
            return GetUnderlyingEnumValue(typeof(T), EnumValue);
        }

        /// <summary>
        /// Get the enum value as the type defined by the enum (int, byte, etc)
        /// </summary>
        /// <param name="EnumType"></param>
        /// <param name="EnumValue"></param>
        /// <returns></returns>
        public static object GetUnderlyingEnumValue(Type EnumType, object EnumValue)
        {
            if ((EnumType == null) || (EnumValue == null))
                return null;

            EnumType = Nullable.GetUnderlyingType(EnumType) ?? EnumType;

            if (!EnumType.IsEnum)
                return null;

            Type utype = Enum.GetUnderlyingType(EnumType);
            if (utype == null)
                return null;

            return Convert.ChangeType(EnumValue, utype);
        }

        /// <summary>
        /// Check if an enum value is inside the enum
        /// </summary>
        /// <param name="EnumType"></param>
        /// <param name="EnumValue"></param>
        /// <returns></returns>
        public static bool CheckEnumValueValid(string EnumType, object EnumValue)
        {
            return CheckEnumValueValid(GetEnumTypeFromName(EnumType), EnumValue);
        }

        /// <summary>
        /// Check if an enum value is inside the enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="EnumValue"></param>
        /// <returns></returns>
        public static bool CheckEnumValueValid<T>(object EnumValue) where T : struct, IConvertible
        {
            return CheckEnumValueValid(typeof(T), EnumValue);
        }

        /// <summary>
        /// Check if an enum value is inside the enum
        /// </summary>
        /// <param name="EnumType"></param>
        /// <param name="EnumValue"></param>
        /// <returns></returns>
        public static bool CheckEnumValueValid(Type EnumType, object EnumValue)
        {
            if ((EnumType == null) || (EnumValue == null))
                return false;

            var EnumValueString = EnumValue.ToString().Trim();
            if (EnumValueString == string.Empty)
                return false;

            EnumType = Nullable.GetUnderlyingType(EnumType) ?? EnumType;

            if (!EnumType.IsEnum)
                return false;

            //parse the value into an enum. this can be a number, or a string 
            //saves the value into the base type of the enum (int, etc)
            object e;
            try
            {
                e = Enum.Parse(EnumType, EnumValueString);
            }
            catch
            {
                return false;
            }

            if (e == null)
                return false;

            //check if the value of the base type is defined in the enum
            if (Enum.IsDefined(EnumType, e))
                return true;

            return false;
        }


        /// <summary>
        /// Get list of enum values and descriptions
        /// </summary>
        /// <typeparam name="T">Enum underlying type for conversion</typeparam>
        /// <param name="EnumType"></param>
        /// <returns></returns>
        public static List<Tuple<T, string>> GetEnumPairs<T>(string EnumType, bool UseDeclarationOrder)
        {
            return GetEnumPairs<T>(GetEnumTypeFromName(EnumType), UseDeclarationOrder);
        }

        /// <summary>
        /// Get list of enum values and descriptions
        /// </summary>
        /// <typeparam name="T">Enum underlying type for conversion</typeparam>
        /// <param name="EnumType"></param>
        /// <returns></returns>
        public static List<Tuple<T, string>> GetEnumPairs<T>(bool UseDeclarationOrder) where T : struct, IConvertible
        {
            return GetEnumPairs<T>(typeof(T), UseDeclarationOrder);
        }

        /// <summary>
        /// Get list of enum values and descriptions
        /// </summary>
        /// <typeparam name="T">Enum underlying type for conversion</typeparam>
        /// <param name="EnumType"></param>
        /// <returns></returns>
        public static List<Tuple<T, string>> GetEnumPairs<T>(Type EnumType, bool UseDeclarationOrder)
        {
            var Items = new List<Tuple<T, string>>();

            if (EnumType == null)
                return Items;

            EnumType = Nullable.GetUnderlyingType(EnumType) ?? EnumType;
            if (!EnumType.IsEnum)
                return Items;

            var names = new List<String>();
            if (UseDeclarationOrder)
                foreach (System.Reflection.FieldInfo fi in EnumType.GetFields().Where(fi => fi.IsStatic).OrderBy(fi => fi.MetadataToken))
                    names.Add(fi.Name);
            else
                names = Enum.GetNames(EnumType).ToList();

            foreach (var name in names)
            {
                string s;

                var memInfo = EnumType.GetMember(name);
                if ((memInfo == null) || (memInfo.Count() == 0))
                {
                    s = name;
                }
                else
                {
                    var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if ((attributes == null) || (attributes.Length == 0))
                    {
                        s = name;
                    }
                    else
                    {
                        s = ((DescriptionAttribute)attributes[0]).Description;
                    }
                }

                object e = Enum.Parse(EnumType, name);
                if (e == null)
                    continue;

                T v = (T)Convert.ChangeType(e, typeof(T));

                Items.Add(new Tuple<T, string>(v, s));
            }

            return Items;
        }

        /// <summary>
        /// Return a type from an enum name
        /// </summary>
        /// <param name="enumName"></param>
        /// <returns></returns>
        public static Type GetEnumTypeFromName(string enumName)
        {
            if (string.IsNullOrEmpty(enumName))
                return null;

            //check for enum name in all assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(enumName);
                if (type == null)
                    continue;
                if (type.IsEnum)
                    return type;
            }

            //prepend assembly name in front of enum, then check for enum name in all assemblies
            var AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            enumName = AssemblyName + "." + enumName;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(enumName);
                if (type == null)
                    continue;
                if (type.IsEnum)
                    return type;
            }

            return null;
        }
    }
}
