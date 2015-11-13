using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Personnel.Repository.Additional
{
    public static class Extensions
    {
        public static string GetColumnPropertiesForEntity(this object obj)
        {
            string properties = string.Empty;
            var type = obj.GetType();
            var columnType = typeof(ColumnAttribute);
            foreach (var i in type.GetProperties()
                                .Where(pi => pi.GetCustomAttributes(columnType, false).Any())
                                .Select(pi => new
                                {
                                    Attr = (pi.GetCustomAttributes(columnType, false).FirstOrDefault() as ColumnAttribute),
                                    Value = pi.GetValue(obj)
                                })
                                .Select(i => new
                                {
                                    Name = i.Attr == null ? "<err>" : i.Attr.Name,
                                    i.Value
                                })
                                )
                properties += (string.IsNullOrWhiteSpace(properties) ? string.Empty : ",") + string.Format("{0}='{1}'", i.Name, i.Value == null ? "NULL" : i.Value.ToString());
            return string.Format("{0}:[{1}]", type.Name, properties);
        }

        public static void FillFromAnonymousType(this object obj, object anonymousObject)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (anonymousObject == null)
                throw new ArgumentNullException("anonymousObject");

            Type type = obj.GetType();
            Type typeAn = anonymousObject.GetType();
            foreach (var pi in typeAn.GetProperties())
            {
                var value = pi.GetValue(anonymousObject, null);
                var setProp = type.GetProperty(pi.Name);
                if (setProp != null)
                    setProp.SetValue(obj, value);
            }
        }

        public static string GetResourceNameFromType(this Enum type)
        {
            var descrValue = type.GetResourceDescription();
            var obj = Properties.Resources.ResourceManager.GetObject(descrValue);
            return obj == null
                ? descrValue
                : obj.ToString();
        }

        public static string GetResourceDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            return (Attribute.GetCustomAttribute(field, typeof(ResourceDescriptionAttribute)) as ResourceDescriptionAttribute)?.Description
                ?? (Properties.Resources.ResourceManager.GetObject(value.ToString())?.ToString() ?? value.ToString());
        }

        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute 
                    = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))
                        as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static T GetAttribute<T>(this object value, T attributeType)
            where T : Attribute
        {
            var val = value.GetType();
            var attribute = Attribute.GetCustomAttribute(value.GetType(), typeof(T)) as T;
            return attribute;
        }

        public static T GetEnumValueByName<T>(string name)
        {
            return typeof(T).GetEnumValues()
                .Cast<T>()
                .FirstOrDefault(ct => string.Compare(ct.ToString(), name, true) == 0);
        }


        public static Encoding GetEncodingFromName(string encodingName)
        {
            if (!string.IsNullOrWhiteSpace(encodingName))
                try
                {
                    return Encoding.GetEncoding(encodingName);
                } catch { }
            return Encoding.Default;
        }
    }
}
