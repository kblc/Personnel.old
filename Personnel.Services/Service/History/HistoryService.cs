using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Personnel.Services.Model;
using System.ServiceModel;
using System.Threading;
using Helpers;

namespace Personnel.Services.Service.History
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class HistoryService : Base.BaseService, IHistoryService, IHistoryServiceREST
    {
        public HistoryService()
        {
            try
            {
                var cfg = new Configuration();
                cfg.CopyObjectTo(this);
            }
            catch { }
        }

        /// <summary>
        /// Max history items per request
        /// </summary>
        public int MaxHistoryItemsPerRequest { get; set; } = Config.ServicesConfigSection.DefaultMaxHistoryItemsPerRequest;

        /// <summary>
        /// Get items from repository
        /// </summary>
        /// <param name="resultElementType">Result element array item type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="returnArrayElementType">Entity return array element type</param>
        /// <param name="actionType">Action type</param>
        /// <param name="sourceIds">Identifiers</param>
        /// <param name="rep">Repository</param>
        /// <returns>Values to set for history property</returns>
        private object GetFromRepository(Type resultElementType, Type entityType, Type returnArrayElementType, Repository.Model.HistoryChangeType actionType, IEnumerable<string> sourceIds, Repository.Logic.Repository rep)
        {
            var res = rep.GetHistoryItems(returnArrayElementType, entityType, sourceIds);

            if (res == null)
                return null;

            if (actionType == Repository.Model.HistoryChangeType.Remove)
                return res;

            var map = AutoMapper.Mapper.GetAllTypeMaps()
                .Where(m => m.SourceType == returnArrayElementType && m.DestinationType == resultElementType)
                .FirstOrDefault();

            if (map != null)
            {
                var sourceArray = ((Array)res).Cast<object>()
                    .Select(i => AutoMapper.Mapper.Map(i, returnArrayElementType, resultElementType))
                    .ToArray();

                if (sourceArray.Length == 0)
                    return null;

                var result = Array.CreateInstance(resultElementType, sourceArray.Length);
                Array.Copy(sourceArray, result, sourceArray.Length);
                return result;
            }

            return null;
        }

        /// <summary>
        /// Get history long polling request
        /// </summary>
        /// <param name="eventId">Start event from</param>
        public Model.HistoryExecutionResult Get()
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ViewChanges);

                        var historyData = rep.Get<Repository.Model.History>(asNoTracking: true)
                            .OrderByDescending(h => h.HistoryId)
                            .Take(1)
                            .FirstOrDefault();

                        return new Model.HistoryExecutionResult(new Model.History()
                        {
                            EventId = historyData?.HistoryId ?? 0
                        });
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.HistoryExecutionResult(ex);
                }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Get history long polling request
        /// </summary>
        /// <param name="eventId">Start event from</param>
        public Model.HistoryExecutionResult RESTGet() => Get();

        /// <summary>
        /// Get history long polling request
        /// </summary>
        /// <param name="eventId">Start event from</param>
        public Model.HistoryExecutionResult GetFrom(long eventId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = new Model.History();
                    while (true)
                        using (var rep = GetNewRepository(logSession))
                        {
                            SRVCCheckCredentials(logSession, rep, Repository.Model.RightType.Login, Repository.Model.RightType.ViewChanges);

                            var historyData = rep.Get<Repository.Model.History>(h => h.HistoryId > eventId, asNoTracking: true)
                                .OrderBy(h => h.HistoryId)
                                .Take(MaxHistoryItemsPerRequest)
                                .ToArray();

                            if (historyData.Length > 0)
                            {
                                res.EventId = historyData.Last().HistoryId;
                                var itemsToProcess = historyData
                                    .Distinct();

                                var getData = new Func<Repository.Model.HistoryChangeType, object>((t) =>
                                {
                                    bool hasChanged = false;
                                    object resD = t == Repository.Model.HistoryChangeType.Remove
                                        ? new Model.HistoryRemoveInfo() as object
                                        : new Model.HistoryUpdateInfo() as object;

                                    resD.GetType()
                                        .GetProperties()
                                        .Select(p => new { Property = p, Attr = p.GetCustomAttributes(typeof(Model.RepositoryResolvingAttribute), true).Cast<Model.RepositoryResolvingAttribute>().FirstOrDefault() })
                                        .Where(p => p.Attr != null)
                                        .Select(p => new { p.Property, p.Attr.EntityType, p.Attr.ReturnArrayElementType })
                                        .ToList()
                                        .ForEach(p =>
                                        {
                                            var sourceIds = itemsToProcess.Where(h => h.ChangeType == t && string.Compare(h.Source, p.EntityType.Name, true) == 0).OrderBy(h => h.Date).Select(h => h.SourceId).ToArray();
                                            if (sourceIds.Length > 0)
                                            { 
                                                var val = GetFromRepository(p.Property.PropertyType.IsArray ? p.Property.PropertyType.GetElementType() : p.Property.PropertyType, p.EntityType, p.ReturnArrayElementType, t, sourceIds, rep);
                                                if (val != null)
                                                {
                                                    p.Property.SetValue(resD, val);
                                                    hasChanged = true;
                                                }
                                            }
                                        });

                                    return hasChanged ? resD : null;
                                });

                                res.Add = getData(Repository.Model.HistoryChangeType.Add) as Model.HistoryUpdateInfo;
                                res.Change = getData(Repository.Model.HistoryChangeType.Change) as Model.HistoryUpdateInfo;
                                res.Remove = getData(Repository.Model.HistoryChangeType.Remove) as Model.HistoryRemoveInfo;

                                break;
                            }

                            Thread.Sleep(300);
                        }
                    return new Model.HistoryExecutionResult(res);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(eventId), eventId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Model.HistoryExecutionResult(ex);
                }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Get history long polling request
        /// </summary>
        /// <param name="eventId">Start event from</param>
        public Model.HistoryExecutionResult RESTGetFrom(long eventId) => GetFrom(eventId);
    }
}
