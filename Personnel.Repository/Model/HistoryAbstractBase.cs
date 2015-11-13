using Personnel.Repository.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Serialization;
using System.Xml.Serialization;
using System.Linq.Expressions;

namespace Personnel.Repository.Model
{
    [Serializable]
    [XmlRoot("id")]
    public class HistoryKeyValue
    {
        [XmlAttribute("key")]
        public string Property { get; set; }
        [XmlAttribute("val")]
        public string Value { get; set; }

        public HistoryKeyValue() { }
    }

    public abstract class HistoryAbstractBase : IHistoryRecord
    {
        protected static System.Reflection.PropertyInfo GetPropertyAndCheckRule(Type entityType)
        {
            var validProperties = entityType.GetProperties()
                    .Where(p => p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true)?.Length > 0)
                    .ToArray();

            if (validProperties.Length > 1)
                throw new Exception(string.Format(Properties.Resources.HISTORYRECORD_EntityBreakOntToOneRule, entityType.FullName, typeof(IHistoryRecord).Name));

            if (validProperties.Length == 1)
                return validProperties[0];

            return null;
        }

        protected virtual string GetSourceId()
        {
            var prop = GetPropertyAndCheckRule(GetType());
            return (prop != null) ? prop.GetValue(this)?.ToString() : null;
        }
        protected virtual string GetSourceName()
        {
            var t = GetType();
            while (t != null && t.BaseType != typeof(HistoryAbstractBase))
                t = t.BaseType;
            return t?.Name ?? GetType().Name;
        }

        #region IHistoryRecord

        string IHistoryRecord.SourceId => GetSourceId();
        string IHistoryRecord.SourceName => GetSourceName();

        #endregion
        #region ToString()

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }

        #endregion
    }

    public abstract class HistoryAbstractBase<TKey, TEntity> : HistoryAbstractBase
        where TEntity : class
    {
        protected override string GetSourceName()
        {
            var parentType = GetType();
            while (parentType != null && parentType.BaseType != typeof(HistoryAbstractBase<TKey, TEntity>))
                parentType = parentType.BaseType;
            return parentType?.Name;
        }

        private static System.Reflection.MethodBase GetGenericMethod(Type type, string name, Type[] typeArgs, Type[] argTypes, System.Reflection.BindingFlags flags)
        {
            int typeArity = typeArgs.Length;
            var methods = type.GetMethods()
                .Where(m => m.Name == name)
                .Where(m => m.GetGenericArguments().Length == typeArity)
                .Select(m => m.MakeGenericMethod(typeArgs));
            return Type.DefaultBinder.SelectMethod(flags, methods.ToArray(), argTypes, null);
        }

        private static bool IsIEnumerable(Type type)
        {
            return type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        private static Type GetIEnumerableImpl(Type type)
        {
            // Get IEnumerable implementation. Either type is IEnumerable<T> for some T, 
            // or it implements IEnumerable<T> for some T. We need to find the interface.
            if (IsIEnumerable(type))
                return type;
            Type[] t = type.FindInterfaces((m, o) => IsIEnumerable(m), null);
            return t[0];
        }

        private static System.Linq.Expressions.Expression<Func<TEntity, bool>> GetHistoryFilter(IEnumerable<TKey> identifiers)
        {
            var prop = GetPropertyAndCheckRule(typeof(TEntity));
            if (prop != null)
            {
                var keyPropertyName = prop.Name;

                var pe = Expression.Parameter(typeof(TEntity));
                var me = Expression.Property(pe, keyPropertyName);
                var ce = Expression.Constant(identifiers);

                Type cType = GetIEnumerableImpl(ce.Type);
                Type elemType = cType.GetGenericArguments()[0];
                //Type predType = typeof(Func<,>).MakeGenericType(me.Type, typeof(bool));

                var method = (System.Reflection.MethodInfo)GetGenericMethod(
                    typeof(Enumerable), "Contains", new[] { elemType }, new[] { cType, elemType }, System.Reflection.BindingFlags.Static);

                //var method = (System.Reflection.MethodInfo)GetGenericMethod(
                //    typeof(Enumerable), "Any", new[] { elemType }, new[] { cType, predType }, System.Reflection.BindingFlags.Static);

                var call = Expression.Call(method, ce, me);

                //var method = typeof(Enumerable).GetMethod("Contains", new[] { me.Type });
                //var call = Expression.Call(typeof(Enumerable), "Contains", new[] { me.Type }, ce, me);
                return Expression.Lambda<Func<TEntity, bool>>(call, pe);
            }

            return (i) => false;
        }

        #region History resolver

        [HistoryResolver]
        internal static TKey[] GetIdentifiers(Logic.Repository rep, string[] identifiers)
        {
            if (typeof(TKey) == typeof(Guid))
            {
                var res = identifiers.Select(id => (TKey)(object)Guid.Parse(id))
                    .ToArray();
                return res;
            }
            else
            {
                var res = identifiers.Select(id => (TKey)Convert.ChangeType(id, typeof(TKey)))
                    .ToArray();
                return res;
            }
        }

        [HistoryResolver]
        internal static TEntity[] GetItemsToDelete(Logic.Repository rep, string[] identifiers)
        {
            var ids = GetIdentifiers(rep, identifiers);
            return rep.Get<TEntity>(GetHistoryFilter(ids), asNoTracking: true).ToArray();
        }

        #endregion
    }
}
