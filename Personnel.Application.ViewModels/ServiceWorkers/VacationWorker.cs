using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers;
using Helpers.Linq;

namespace Personnel.Application.ViewModels.ServiceWorkers
{
    public class VacationWorker : AbstractBaseWorker
    {
        private readonly List<VacationService.Vacation> Vacations = new List<VacationService.Vacation>();
        private readonly List<VacationService.VacationBalance> VacationBalances = new List<VacationService.VacationBalance>();
        private readonly List<VacationService.VacationLevel> VacationLevels = new List<VacationService.VacationLevel>();

        public void setPeriod(DateTime? from, DateTime? to)
        {
            bool needToReload = this.from != from || this.to != to;
            this.from = from;
            this.to = to;
            if (needToReload && (initThread != null || IsLoaded))
            {
                DoStop();
                DoStart();
            }
        }

        private DateTime? from = null;
        public DateTime? From
        {
            get { return from; }
            set { setPeriod(value, To); }
        }

        private DateTime? to = null;
        public DateTime? To
        {
            get { return to; }
            set { setPeriod(From, value); }
        }

        private IList<T> GetList<T>()
        {
            if (typeof(T) == typeof(VacationService.Vacation))
                return (IList<T>)Vacations;
            if (typeof(T) == typeof(VacationService.VacationBalance))
                return (IList<T>)VacationBalances;
            if (typeof(T) == typeof(VacationService.VacationLevel))
                return (IList<T>)VacationLevels;
            return null;
        }
        private Action<IList<T>, ChangeAction> GetEvent<T>()
        {
            if (typeof(T) == typeof(VacationService.Vacation))
                return new Action<IList<T>, ChangeAction>((items, action) => this.RaiseOnVacationChanged(items.Cast<VacationService.Vacation>(), action));
            if (typeof(T) == typeof(VacationService.VacationBalance))
                return new Action<IList<T>, ChangeAction>((items, action) => this.RaiseOnVacationBalanceChanged(items.Cast<VacationService.VacationBalance>(), action));
            if (typeof(T) == typeof(VacationService.VacationLevel))
                return new Action<IList<T>, ChangeAction>((items, action) => this.RaiseOnVacationLevelChanged(items.Cast<VacationService.VacationLevel>(), action));
            return null;
        }
        private void RaiseItemsInitialize<T>(T[] items)
        {
            IList<T> list = GetList<T>();
            var evnt = GetEvent<T>();
            if (list != null)
            {
                var remItems = list.ToArray();
                list.Clear();
                evnt?.Invoke(remItems, ChangeAction.Remove);
                foreach (var item in items)
                    list.Add(item);
                evnt?.Invoke(list, ChangeAction.Add);
            }
        }

        public void ApplyHistoryChanges(HistoryService.History e)
        {
            if (!IsLoaded)
                return;

            ApplyHistoryChange(e?.Add);
            ApplyHistoryChange(e?.Change);
            ApplyHistoryRemove(e?.Remove);
        }

        private void ApplyHistoryChange(HistoryService.HistoryUpdateInfo change)
        {
            if (change == null)
                return;

            #region Vacation

            if (change.Vacation != null)
            {
                var vacationsToUpdate = new List<VacationService.Vacation>();
                var vacationsToInsert = new List<VacationService.Vacation>();

                lock (Vacations)
                {
                    var upd = Vacations.FullOuterJoin(change.Vacation, a => a.Id, a => a.Id,
                        (Existed, New) => new
                        {
                            Existed,
                            New = AutoMapper.Mapper.Map<VacationService.Vacation>(New)
                        }).ToArray();

                    foreach (var i in upd)
                    {
                        if (i.Existed == null)
                        {
                            Vacations.Add(i.New);
                            vacationsToInsert.Add(i.New);
                        }
                        else
                        {
                            i.Existed.CopyObjectFrom(i.New);
                            vacationsToUpdate.Add(i.Existed);
                        }
                    }
                }

                RaiseOnVacationChanged(vacationsToUpdate, ChangeAction.Change);
                RaiseOnVacationChanged(vacationsToInsert, ChangeAction.Add);
            }

            #endregion
            #region VacationBalance

            if (change.VacationBalance != null)
            {
                var vacationBalancesToUpdate = new List<VacationService.VacationBalance>();
                var vacationBalancesToInsert = new List<VacationService.VacationBalance>();

                lock (VacationBalances)
                {
                    var upd = VacationBalances.FullOuterJoin(change.VacationBalance, a => a.Id, a => a.Id,
                        (Existed, New) => new
                        {
                            Existed,
                            New = AutoMapper.Mapper.Map<VacationService.VacationBalance>(New)
                        }).ToArray();

                    foreach (var i in upd)
                    {
                        if (i.Existed == null)
                        {
                            VacationBalances.Add(i.New);
                            vacationBalancesToInsert.Add(i.New);
                        }
                        else
                        {
                            i.Existed.CopyObjectFrom(i.New);
                            vacationBalancesToUpdate.Add(i.Existed);
                        }
                    }
                }

                RaiseOnVacationBalanceChanged(vacationBalancesToUpdate, ChangeAction.Change);
                RaiseOnVacationBalanceChanged(vacationBalancesToInsert, ChangeAction.Add);
            }

            #endregion
            #region VacationAgreement

            if (change.VacationAgreement != null)
            {
                var vacationToUpdate = new List<VacationService.Vacation>();

                lock (VacationBalances)
                {
                    var upd = Vacations.Join(change.VacationAgreement, a => a.Id, a => a.VacationId,
                        (Existed, New) => new
                        {
                            Vacation = Existed,
                            VacationAgreement = AutoMapper.Mapper.Map<VacationService.VacationAgreement>(New)
                        }).ToArray();

                    foreach (var i in upd)
                    {
                        var existedAgreement = i.Vacation.Agreements.FirstOrDefault(va => va.Id == i.VacationAgreement.Id);
                        if (existedAgreement != null)
                        {
                            existedAgreement.CopyObjectFrom(i.VacationAgreement);
                        } else
                        {
                            i.Vacation.Agreements = i.Vacation.Agreements.Union(new[] { i.VacationAgreement }).ToArray();
                        }
                        vacationToUpdate.Add(i.Vacation);
                    }
                }

                RaiseOnVacationChanged(vacationToUpdate, ChangeAction.Change);
            }

            #endregion
            #region VacationLevel

            if (change.VacationLevel != null)
            {
                var vacationLevelsToUpdate = new List<VacationService.VacationLevel>();
                var vacationLevelsToInsert = new List<VacationService.VacationLevel>();

                lock (VacationLevels)
                {
                    var upd = VacationLevels.FullOuterJoin(change.VacationLevel, a => a.Id, a => a.Id,
                        (Existed, New) => new
                        {
                            Existed,
                            New = AutoMapper.Mapper.Map<VacationService.VacationLevel>(New)
                        }).ToArray();

                    foreach (var i in upd)
                    {
                        if (i.Existed == null)
                        {
                            VacationLevels.Add(i.New);
                            vacationLevelsToInsert.Add(i.New);
                        }
                        else
                        {
                            i.Existed.CopyObjectFrom(i.New);
                            vacationLevelsToUpdate.Add(i.Existed);
                        }
                    }
                }

                RaiseOnVacationLevelChanged(vacationLevelsToUpdate, ChangeAction.Change);
                RaiseOnVacationLevelChanged(vacationLevelsToInsert, ChangeAction.Add);
            }

            #endregion
        }
        private void ApplyHistoryRemove(HistoryService.HistoryRemoveInfo remove)
        {
            if (remove == null)
                return;

            #region Vacation

            if (remove.Vacation != null)
            {
                var removedVacations = new List<VacationService.Vacation>();
                lock (Vacations)
                {
                    var del = Vacations.Join(remove.Vacation, a => a.Id, id => id, (a, id) => a).ToArray();
                    foreach (var i in del) {
                        removedVacations.Add(i);
                        Vacations.Remove(i);
                    }
                }
                RaiseOnVacationChanged(removedVacations, ChangeAction.Remove);
            }

            #endregion
            #region VacationBalance

            if (remove.VacationBalance != null)
            {
                var removedVacationBalances = new List<VacationService.VacationBalance>();
                lock (VacationBalances)
                {
                    var del = VacationBalances.Join(remove.VacationBalance, a => a.Id, id => id, (a, id) => a).ToArray();
                    foreach (var i in del)
                    {
                        removedVacationBalances.Add(i);
                        VacationBalances.Remove(i);
                    }
                }
                RaiseOnVacationBalanceChanged(removedVacationBalances, ChangeAction.Remove);
            }

            #endregion
            #region VacationAgreement

            if (remove.VacationAgreement != null)
            {
                var updatedVacations = new List<VacationService.Vacation>();
                lock (Vacations)
                {
                    var del = Vacations
                        .SelectMany(vacation => vacation.Agreements.Select(agreement => new { vacation, agreement }))
                        .Join(remove.VacationAgreement, a => a.agreement.Id, id => id, (a, id) => new { a.agreement, a.vacation }).ToArray();

                    foreach (var i in del)
                    {
                        updatedVacations.Add(i.vacation);
                        i.vacation.Agreements = i.vacation.Agreements.Except(new[] { i.agreement }).ToArray();
                    }
                }
                RaiseOnVacationChanged(updatedVacations, ChangeAction.Change);
            }

            #endregion
        }

        private Thread initThread = null;

        protected override bool DoStart()
        {
            try
            {
                if (initThread == null)
                {
                    initThread = new Thread(new ParameterizedThreadStart(InitThread));
                    initThread.IsBackground = true;
                    initThread.Start(Context);
                }
                return true;
            }
            catch(Exception ex)
            {
                SetError(ex);
                return false;
            }
        }
        protected override bool DoStop()
        {
            try
            {
                if (initThread != null && initThread.IsAlive)
                    initThread.Abort();
                return true;
            }
            catch (Exception ex)
            {
                SetError(ex);
                return false;
            }
            finally
            {
                initThread = null;
            }
        }

        private void InitThread(object context)
        {
            IsLoaded = false;

            var modelLevelThContext = (System.Runtime.Remoting.Contexts.Context)context;
            bool inited = false;
            #region Infinity try to connect then init and exit

            var checkAggregateExceptions = new Func<Type, AggregateException, bool>((t, ex) =>
            {
                var res = true;
                if (ex != null)
                    foreach (var e in ex.InnerExceptions)
                    { 
                        SetError(e);
                        res = false;
                    }
                return res;
            });

            do
            {
                try
                {
                    using (var sClient = new VacationService.VacationServiceClient())
                    {
                        var tasks = new List<Task<bool>>();

                        tasks.Add(sClient.VacationsGetAsync(From, To).ContinueWith<bool>(t =>
                            {
                                if (checkAggregateExceptions(typeof(VacationService.Vacation), t.Exception))
                                {
                                    if (t.Result.Error != null)
                                        throw new Exception(t.Result.Error);

                                    modelLevelThContext.DoCallBack(() => RaiseItemsInitialize(t.Result.Values));
                                    return true;
                                }
                                return false;
                            }));
                        tasks.Add(sClient.VacationBalanceGetAsync().ContinueWith<bool>(t =>
                        {
                            if (checkAggregateExceptions(typeof(VacationService.VacationBalance), t.Exception))
                            {
                                if (t.Result.Error != null)
                                    throw new Exception(t.Result.Error);

                                modelLevelThContext.DoCallBack(() => RaiseItemsInitialize(t.Result.Values));
                                return true;
                            }
                            return false;
                        }));
                        tasks.Add(sClient.GetLevelsAsync().ContinueWith<bool>(t =>
                        {
                            if (checkAggregateExceptions(typeof(VacationService.VacationLevel), t.Exception))
                            {
                                if (t.Result.Error != null)
                                    throw new Exception(t.Result.Error);

                                modelLevelThContext.DoCallBack(() => RaiseItemsInitialize(t.Result.Values));
                                return true;
                            }
                            return false;
                        }));

                        Task.WaitAll(tasks.ToArray());
                        tasks.ForEach(t => checkAggregateExceptions(null, t.Exception));
                        inited = tasks.All(t => t.Exception == null && t.Result);
                        if (inited)
                            SetError((string)null);
                    }
                }
                catch (Exception ex)
                {
                    SetError(ex);
                    Thread.Sleep(ConnectionTimeInterval);
                }
            } while (!inited);

            #endregion

            IsLoaded = true;
        }

        #region Events

        private void RaiseOnVacationChanged(IEnumerable<VacationService.Vacation> items, ChangeAction action)
            => Context.DoCallBack(() => 
            {
                if (items.Any())
                    OnVacationChanged?.Invoke(this, new ListItemsEventArgs<VacationService.Vacation>(items.ToArray(), action));
            });

        private void RaiseOnVacationBalanceChanged(IEnumerable<VacationService.VacationBalance> items, ChangeAction action)
         => Context.DoCallBack(() =>
            {
                if (items.Any())
                    OnVacationBalanceChanged?.Invoke(this, new ListItemsEventArgs<VacationService.VacationBalance>(items.ToArray(), action));
            });

        private void RaiseOnVacationLevelChanged(IEnumerable<VacationService.VacationLevel> items, ChangeAction action)
         => Context.DoCallBack(() =>
             {
                 if (items.Any())
                     OnVacationLevelChanged?.Invoke(this, new ListItemsEventArgs<VacationService.VacationLevel>(items.ToArray(), action));
             });

        public event EventHandler<ListItemsEventArgs<VacationService.VacationBalance>> OnVacationBalanceChanged;
        public event EventHandler<ListItemsEventArgs<VacationService.Vacation>> OnVacationChanged;
        public event EventHandler<ListItemsEventArgs<VacationService.VacationLevel>> OnVacationLevelChanged;

        #endregion
    }
}
