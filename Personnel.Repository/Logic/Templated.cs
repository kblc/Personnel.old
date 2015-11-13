using Personnel.Repository.Additional;
using Personnel.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using System.Data.Entity;

namespace Personnel.Repository.Logic
{
    public partial class Repository
    {
        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private System.Data.Entity.DbSet<T> GetDbSet<T>()
            where T : class
        {
            var DBSet = TryGetDbSet<T>();
            if (DBSet == null)
                throw new Exception($"DbSet not found for type: '{typeof(T).Name}'");
            return DBSet;
        }

        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private System.Data.Entity.DbSet<T> TryGetDbSet<T>()
            where T : class
        {
            var type = typeof(System.Data.Entity.DbSet<T>);
            var checkType = new Func<Type, Type, bool>((sourceType, baseType) =>
            {
                var t = sourceType;
                while (t != null)
                {
                    if (t == baseType)
                        return true;
                    t = t.BaseType;
                }
                return false;
            });

            var DBSet = Context.GetType()
                .GetProperties()
                .Where(p => p.CanRead && checkType(p.PropertyType, type))
                .Select(p => p.GetValue(Context) as System.Data.Entity.DbSet<T>)
                .FirstOrDefault(p => p != null);

            return DBSet;
        }

        /// <summary>
        /// Add item to database
        /// </summary>
        /// <param name="instance">File instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void Add<T>(T instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
            where T : class
        {
            AddRange(new T[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        
        /// <summary>
        /// Add items to database
        /// </summary>
        /// <param name="instances">Instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void AddRange<T>(IEnumerable<T> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
            where T : class
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    GetDbSet<T>().AddRange(instances);
                    if (saveAfterInsert)
                        this.SaveChanges(waitUntilSaving);
                }
                catch (Exception ex)
                {
                    var e = new Exception(ex.Message, ex);
                    for (int i = 0; i < instances.Count(); i++)
                        e.Data.Add(string.Format("instance_{0}", i), instances.ElementAt(i).ToString());
                    throw e;
                }
            }
            catch (Exception ex)
            {
                RaiseDatabaseLog(ex.GetExceptionText($"{nameof(Repository)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}<{typeof(T).Name}>(instances=[{(instances == null ? "NULL" : instances.Count().ToString())}],saveAfterRemove={saveAfterInsert},waitUntilSaving={waitUntilSaving})"));
                throw;
            }
        }
        
        /// <summary>
        /// Remove item from database
        /// </summary>
        /// <param name="instance">Instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void Remove<T>(T instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
            where T : class
        {
            RemoveRange(new T[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        
        /// <summary>
        /// Remove items from database
        /// </summary>
        /// <param name="instances">Instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void RemoveRange<T>(IEnumerable<T> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
            where T : class
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    GetDbSet<T>().RemoveRange(instances);
                    if (saveAfterRemove)
                        this.SaveChanges(waitUntilSaving);
                }
                catch (Exception ex)
                {
                    var e = new Exception(ex.Message, ex);
                    for (int i = 0; i < instances.Count(); i++)
                        e.Data.Add(string.Format("instance_{0}", i), instances.ElementAt(i).ToString());
                    throw e;
                }
            }
            catch (Exception ex)
            {
                RaiseDatabaseLog(ex.GetExceptionText($"{nameof(Repository)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}<{typeof(T).Name}>(instances=[{(instances == null ? "NULL" : instances.Count().ToString())}],saveAfterRemove={saveAfterRemove},waitUntilSaving={waitUntilSaving})"));
                throw;
            }
        }
        
        /// <summary>
        /// Get new instance without any link to database
        /// </summary>
        /// <returns>File instance</returns>
        public T New<T>(object anonymousFiller = null)
            where T : class
        {
            try
            {
                var constructor = typeof(T).GetConstructor(new Type[] { });
                if (constructor != null)
                {
                    var res = (T)constructor.Invoke(null);
                    if (anonymousFiller != null)
                        res.FillFromAnonymousType(anonymousFiller);
                    return res;
                }
                else
                    throw new Exception(string.Format(Properties.Resources.MODEL_ENTITYBASE_CanFindDefaultConstructor, typeof(T).FullName));
            }
            catch (Exception ex)
            {
                RaiseDatabaseLog(ex.GetExceptionText($"{nameof(Repository)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}<{typeof(T).Name}>()"));
                throw;
            }
        }

        /// <summary>
        /// Get new instance without any link to database
        /// </summary>
        /// <returns>File instance</returns>
        public T New<T>(Action<T> filler)
            where T : class
        {
            try
            {
                var constructor = typeof(T).GetConstructor(new Type[] { });
                if (constructor != null)
                {
                    var res = (T)constructor.Invoke(null);
                    if (filler != null)
                        filler(res);
                    return res;
                }
                else
                    throw new Exception(string.Format(Properties.Resources.MODEL_ENTITYBASE_CanFindDefaultConstructor, typeof(T).FullName));
            }
            catch (Exception ex)
            {
                RaiseDatabaseLog(ex.GetExceptionText($"{nameof(Repository)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}<{typeof(T).Name}>()"));
                throw;
            }
        }

        /// <summary>
        /// Get items
        /// </summary>
        /// <returns>Items queriable collection</returns>
        public IQueryable<T> Get<T>(bool asNoTracking = false, IEnumerable<string> eagerLoad = null)
            where T: class
        {
            var res = (
                (asNoTracking) 
                ? GetDbSet<T>().AsNoTracking() 
                : GetDbSet<T>()
                ) as IQueryable<T>;
            if (eagerLoad != null)
                foreach (var el in eagerLoad)
                    res = res.Include(el);
            return res;
        }

        /// <summary>
        /// Get items
        /// </summary>
        /// <returns>Items queriable collection</returns>
        public IQueryable<T> Get<T, TProperty>(bool asNoTracking = false, IEnumerable<System.Linq.Expressions.Expression<Func<T, TProperty>>> eagerLoad = null)
            where T : class
        {
            var res = (
                (asNoTracking) 
                ? GetDbSet<T>().AsNoTracking() 
                : GetDbSet<T>()
                ) as IQueryable<T>;
            if (eagerLoad != null)
                foreach (var el in eagerLoad)
                    res = res.Include(el);
            return res;
        }

        /// <summary>
        /// Get items with where clause
        /// </summary>
        /// <returns>Items queriable collection</returns>
        public IQueryable<T> Get<T>(System.Linq.Expressions.Expression<Func<T, bool>> whereClause, bool asNoTracking = false, IEnumerable<string> eagerLoad = null)
            where T : class
        {
            return Get<T>(asNoTracking, eagerLoad).Where(whereClause);
        }

        ///// <summary>
        ///// Get items by identifiers
        ///// </summary>
        ///// <param name="instanceIds">Identifiers array</param>
        ///// <returns>Queriable collection</returns>
        //public IQueryable<T1> Get<T1, T2>(IEnumerable<T2> instanceIds, System.Linq.Expressions.Expression<Func<T1, T2>> getIdExpression, bool asNoTracking = false)
        //    where T1 : class
        //{
        //    try
        //    {
        //        var getId = getIdExpression.Compile();
        //        return Get<T1>(i => instanceIds.Contains(getId(i)), asNoTracking);
        //    }
        //    catch(Exception ex)
        //    {
        //        RaiseDatabaseLog(ex.GetExceptionText($"{nameof(Repository)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}<{typeof(T1).Name},{typeof(T2).Name}>()"));
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Get item by identifier
        ///// </summary>
        ///// <param name="instanceId">Identifier</param>
        ///// <returns>Item with specified identifier</returns>
        //public T1 Get<T1, T2>(T2 instanceId, System.Linq.Expressions.Expression<Func<T1, T2>> getIdExpression, bool asNoTracking = false)
        //    where T1 : class
        //{
        //    try
        //    {
        //        return Get<T1, T2>(new T2[] { instanceId }, getIdExpression, asNoTracking).FirstOrDefault();
        //    }
        //    catch(Exception ex)
        //    {
        //        RaiseDatabaseLog(ex.GetExceptionText($"{nameof(Repository)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}<{typeof(T1).Name},{typeof(T2).Name}>()"));
        //        throw;
        //    }
        //}
    }
}
