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
    public enum StaffingListsAction
    {
        Add,
        Change,
        Remove
    }

    public class StaffingListItemsEventArgs<T> : EventArgs
    {
        public T[] Items { get; private set; }
        public StaffingListsAction Action { get; private set; }
        public StaffingListItemsEventArgs(T[] items, StaffingListsAction action)
        {
            Items = items;
            Action = action;
        }
    }

    public class StaffingWorker : AbstractBaseWorker
    {
        private readonly List<StaffingService.Right> Rights = new List<StaffingService.Right>();
        private readonly List<StaffingService.Staffing> Staffing = new List<StaffingService.Staffing>();
        private readonly List<StaffingService.Employee> Employees = new List<StaffingService.Employee>();
        private readonly List<StaffingService.Department> Departments = new List<StaffingService.Department>();
        private StaffingService.Employee current = null;
        public StaffingService.Employee Current
        {
            get { return current; }
            private set
            {
                //if (current == value)
                //    return;
                current = value;
                RaisePropertyChanged();
                RaiseOnCurrentChanged();
            }
        }

        private void RaiseCurrentInitialize(StaffingService.Employee current)
        {
            this.Current = current;
        }
        private IList<T> GetList<T>()
        {
            if (typeof(T) == typeof(StaffingService.Right))
                return (IList<T>)Rights;
            if (typeof(T) == typeof(StaffingService.Staffing))
                return (IList<T>)Staffing;
            if (typeof(T) == typeof(StaffingService.Employee))
                return (IList<T>)Employees;
            if (typeof(T) == typeof(StaffingService.Department))
                return (IList<T>)Departments;
            return null;
        }
        private Action<IList<T>, StaffingListsAction> GetEvent<T>()
        {
            if (typeof(T) == typeof(StaffingService.Right))
                return new Action<IList<T>, StaffingListsAction>((items, action) => this.RaiseOnRightsChanged(items.Cast<StaffingService.Right>(), action));
            if (typeof(T) == typeof(StaffingService.Staffing))
                return new Action<IList<T>, StaffingListsAction>((items, action) => this.RaiseOnStaffingChanged(items.Cast<StaffingService.Staffing>(), action));
            if (typeof(T) == typeof(StaffingService.Employee))
                return new Action<IList<T>, StaffingListsAction>((items, action) => this.RaiseOnEmployeesChanged(items.Cast<StaffingService.Employee>(), action));
            if (typeof(T) == typeof(StaffingService.Department))
                return new Action<IList<T>, StaffingListsAction>((items, action) => this.RaiseOnDepartmentsChanged(items.Cast<StaffingService.Department>(), action));
            return null;
        }
        private void RaiseItemsInitialize<T>(T[] items)
        {
            IList<T> list = GetList<T>();
            var evnt = GetEvent<T>();
            if (list != null)
            {
                list.Clear();
                evnt?.Invoke(list, StaffingListsAction.Remove);
                foreach (var item in items)
                    list.Add(item);
                evnt?.Invoke(list, StaffingListsAction.Add);
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

            if (change.Department != null)
            {
                var updStf = new List<StaffingService.Staffing>();
                var updEmp = new List<StaffingService.Employee>();
                var updApp = new List<StaffingService.Department>();
                var insApp = new List<StaffingService.Department>();
                lock (Departments)
                {
                    var upd = Departments.FullOuterJoin(change.Department,
                        a => a.Id,
                        a => a.Id,
                        (Existed, New) => new
                        {
                            Existed,
                            New = AutoMapper.Mapper.Map<StaffingService.Department>(New)
                        }).ToArray();
                    foreach (var i in upd)
                    {
                        if (i.Existed == null)
                        {
                            Departments.Add(i.New);
                            insApp.Add(i.New);
                        }
                        else
                        {
                            i.Existed.CopyObjectFrom(i.New);
                            updApp.Add(i.Existed);
                        }
                    }
                }

                lock(Staffing)
                {
                    var items = updApp.Union(insApp).Select(s => s.Id);
                    updStf.AddRange( Staffing.Where(s => items.Contains(s.DepartmentId)) );
                }
                lock (Employees)
                {
                    var items = Employees.Join(updStf, e => e.Stuffing?.Id, s => s.Id, (Employee, NewStaffing) => new { Employee, NewStaffing }).ToList();
                    items.ForEach(i => i.Employee.Stuffing = i.NewStaffing);
                    updEmp.AddRange(items.Select(i => i.Employee));
                }
                RaiseOnDepartmentsChanged(updApp, StaffingListsAction.Change);
                RaiseOnDepartmentsChanged(insApp, StaffingListsAction.Add);
                RaiseOnStaffingChanged(updStf, StaffingListsAction.Change);
                RaiseOnEmployeesChanged(updEmp, StaffingListsAction.Change);
            }
            if (change.Employee != null)
            {
                var updApp = new List<StaffingService.Employee>();
                var insApp = new List<StaffingService.Employee>();
                lock (Employees)
                { 
                    var upd = Employees.FullOuterJoin(change.Employee,
                        a => a.Id,
                        a => a.Id,
                        (Existed, New) => new
                        {
                            Existed,
                            New = AutoMapper.Mapper.Map<StaffingService.Employee>(New)
                        }).ToArray();
                    foreach (var i in upd)
                        {
                            if (i.Existed == null)
                            {
                                Employees.Add(i.New);
                                insApp.Add(i.New);
                            }
                            else
                            {
                                i.Existed.CopyObjectFrom(i.New);
                                updApp.Add(i.Existed);
                            }
                        }
                }
                RaiseOnEmployeesChanged(updApp, StaffingListsAction.Change);
                RaiseOnEmployeesChanged(insApp, StaffingListsAction.Add);
            }
            if (change.Stuffing != null)
            {
                var updEmp = new List<StaffingService.Employee>();
                var updApp = new List<StaffingService.Staffing>();
                var insApp = new List<StaffingService.Staffing>();
                lock (Staffing)
                { 
                    var upd = Staffing.FullOuterJoin(change.Stuffing,
                        a => a.Id,
                        a => a.Id,
                        (Existed, New) => new
                        {
                            Existed,
                            New = AutoMapper.Mapper.Map<StaffingService.Staffing>(New)
                        }).ToArray();
                    foreach (var i in upd)
                        {
                            if (i.Existed == null)
                            {
                                Staffing.Add(i.New);
                                insApp.Add(i.New);
                            }
                            else if (i.New != null)
                            {
                                i.Existed.CopyObjectFrom(i.New);
                                updApp.Add(i.Existed);
                            }
                        }
                }
                lock(Employees)
                {
                    var items = Employees.Join(updApp.Union(insApp), e => e.Stuffing?.Id, s => s.Id, (Employee, NewStaffing) => new { Employee, NewStaffing }).ToList();
                    items.ForEach(i => i.Employee.Stuffing = i.NewStaffing);
                    updEmp.AddRange(items.Select(i => i.Employee));
                }
                RaiseOnStaffingChanged(updApp, StaffingListsAction.Change);
                RaiseOnStaffingChanged(insApp, StaffingListsAction.Add);
                RaiseOnEmployeesChanged(updEmp, StaffingListsAction.Change);
            }

            if (change.EmployeeLogin != null)
            {
                var updEmp = new List<StaffingService.Employee>();
                lock (Employees)
                {
                    var table = change.EmployeeLogin
                        .Join(Employees, l => l.EmployeeId, e => e.Id, (Login, Employee) => new { Login, Employee })
                        .GroupBy(i => i.Employee)
                        .Select(g => new { Employee = g.Key, LoginsToUpdate = g.Select(i => i.Login) })
                        .ToList();

                    table.ForEach(i =>
                    {
                        var itemsTable = i.Employee.Logins.FullOuterJoin(i.LoginsToUpdate, l => l.Id, l => l.Id, (Existed, New) => new { Existed, New = AutoMapper.Mapper.Map<StaffingService.EmployeeLogin>(New) });
                        var chg = itemsTable.Where(n => n.Existed != null).ToList();
                        chg.ForEach(c => c.CopyObjectFrom(c.New));
                        var ins = itemsTable.Where(n => n.Existed == null).ToList().Select(n => n.New);
                        i.Employee.Logins = chg.Select(n => n.Existed).Union(ins).ToArray();
                    });

                    updEmp.AddRange(table.Select(i => i.Employee));
                }
                RaiseOnEmployeesChanged(updEmp, StaffingListsAction.Change);
            }

            if (change.EmployeeRight != null)
            {
                var updEmp = new List<StaffingService.Employee>();
                lock (Employees)
                {
                    var table = change.EmployeeRight
                        .Join(Employees, l => l.EmployeeId, e => e.Id, (Right, Employee) => new { Right, Employee })
                        .GroupBy(i => i.Employee)
                        .Select(g => new { Employee = g.Key, RightsToUpdate = g.Select(i => i.Right) })
                        .ToList();

                    table.ForEach(i =>
                    {
                        var itemsTable = i.Employee.Rights.FullOuterJoin(i.RightsToUpdate, l => l.Id, l => l.Id, (Existed, New) => new { Existed, New = AutoMapper.Mapper.Map<StaffingService.EmployeeRight>(New) });
                        var chg = itemsTable.Where(n => n.Existed != null).ToList();
                        chg.ForEach(c => c.CopyObjectFrom(c.New));
                        var ins = itemsTable.Where(n => n.Existed == null).ToList().Select(n => n.New);
                        i.Employee.Rights = chg.Select(n => n.Existed).Union(ins).ToArray();
                    });

                    updEmp.AddRange(table.Select(i => i.Employee));
                }
                RaiseOnEmployeesChanged(updEmp, StaffingListsAction.Change);
            }

            if (change.EmployeePhoto != null)
            {
                var updEmp = new List<StaffingService.Employee>();
                lock (Employees)
                {
                    var table = change.EmployeePhoto
                        .Join(Employees, l => l.EmployeeId, e => e.Id, (Photo, Employee) => new { Photo, Employee })
                        .GroupBy(i => i.Employee)
                        .Select(g => new { Employee = g.Key, PhotosToUpdate = g.Select(i => i.Photo) })
                        .ToList();

                    table.ForEach(i =>
                    {
                        var itemsTable = i.Employee.Photos.FullOuterJoin(i.PhotosToUpdate, l => l.FileId, l => l.FileId, (Existed, New) => new { Existed, New = AutoMapper.Mapper.Map<StaffingService.EmployeePhoto>(New) });
                        var chg = itemsTable.Where(n => n.Existed != null).ToList();
                        chg.ForEach(c => c.CopyObjectFrom(c.New));
                        var ins = itemsTable.Where(n => n.Existed == null).ToList().Select(n => n.New);
                        i.Employee.Photos = chg.Select(n => n.Existed).Union(ins).ToArray();
                    });

                    updEmp.AddRange(table.Select(i => i.Employee));
                }
                RaiseOnEmployeesChanged(updEmp, StaffingListsAction.Change);
            }
        }
        private void ApplyHistoryRemove(HistoryService.HistoryRemoveInfo remove)
        {
            if (remove == null)
                return;

            if (remove.Departments != null)
            { 
                lock(Departments)
                {
                    var stfDel = new List<StaffingService.Staffing>();
                    var del = Departments.Join(remove.Departments, a => a.Id, id => id, (a, id) => a).ToArray();
                    foreach (var i in del) Departments.Remove(i);
                    var delIds = del.Select(i => i.Id).ToArray();
                    lock(Staffing)
                    {
                        stfDel.AddRange(Staffing.Where(i => delIds.Contains(i.DepartmentId)));
                        foreach (var s in stfDel)
                            Staffing.Remove(s);
                    }
                    var empChg = Employees.Where(e => delIds.Contains(e.Stuffing?.DepartmentId ?? 0)).ToList();
                    RaiseOnStaffingChanged(stfDel, StaffingListsAction.Remove);
                    RaiseOnDepartmentsChanged(del, StaffingListsAction.Remove);
                    RaiseOnEmployeesChanged(empChg, StaffingListsAction.Change);
                }
            }
            if (remove.Employees != null)
            {
                var del = Employees.Join(remove.Employees, a => a.Id, id => id, (a, id) => a).ToArray();
                lock (Employees) foreach (var i in del) Employees.Remove(i);
                RaiseOnEmployeesChanged(del, StaffingListsAction.Remove);
            }
            if (remove.EmployeeRights != null)
            {
                var rightsTable = Employees.SelectMany(e => e.Rights.Select(r => new { Employee = e, Right = r }));
                var rightLines = rightsTable
                    .Join(remove.EmployeeRights, er => er.Right.Id, id => id, (er, r) => er)
                    .GroupBy(i => i.Employee)
                    .Select(g => new { Employee = g.Key, RightsToDelete = g.Select(i => i.Right).Select(r => r.Id) })
                    .ToList();
                lock (Employees) rightLines.ForEach(i => i.Employee.Rights = i.Employee.Rights.Where(r => !i.RightsToDelete.Contains(r.Id)).ToArray());
                RaiseOnEmployeesChanged(rightLines.Select(i => i.Employee), StaffingListsAction.Change);
            }
            if (remove.EmployeeLogins != null)
            {
                var loginsTable = Employees.SelectMany(e => e.Logins.Select(r => new { Employee = e, Login = r }));
                var loginLines = loginsTable
                    .Join(remove.EmployeeLogins, er => er.Login.Id, id => id, (er, r) => er)
                    .GroupBy(i => i.Employee)
                    .Select(g => new { Employee = g.Key, LoginsToDelete = g.Select(i => i.Login.Id) })
                    .ToList();
                lock (Employees) loginLines.ForEach(i => i.Employee.Logins = i.Employee.Logins.Where(r => !i.LoginsToDelete.Contains(r.Id)).ToArray());
                RaiseOnEmployeesChanged(loginLines.Select(i => i.Employee), StaffingListsAction.Change);
            }
            if (remove.EmployeePhotos != null)
            {
                var photosTable = Employees.SelectMany(e => e.Photos.Select(r => new { Employee = e, Photo = r }));
                var photoLines = photosTable
                    .Join(remove.EmployeePhotos, er => er.Photo.FileId, id => id, (er, r) => er)
                    .GroupBy(i => i.Employee)
                    .Select(g => new { Employee = g.Key, PhotosToDelete = g.Select(i => i.Photo.FileId) })
                    .ToList();
                lock (Employees) photoLines.ForEach(i => i.Employee.Photos = i.Employee.Photos.Where(r => !i.PhotosToDelete.Contains(r.FileId)).ToArray());
                RaiseOnEmployeesChanged(photoLines.Select(i => i.Employee), StaffingListsAction.Change);
            }
            if (remove.Stuffing != null)
            {
                var del = Staffing.Join(remove.Stuffing, a => a.Id, id => id, (a, id) => a).ToArray();
                var delIds = del.Select(i => i.Id);
                var empChg = new List<StaffingService.Employee>();
                lock (Employees)
                {
                    empChg.AddRange(Employees.Where(e => delIds.Contains(e.Stuffing?.Id ?? 0)));
                    empChg.ForEach(e => e.Stuffing = null);
                    foreach (var i in del)
                        Staffing.Remove(i);
                }
                RaiseOnStaffingChanged(del, StaffingListsAction.Remove);
                RaiseOnEmployeesChanged(empChg, StaffingListsAction.Change);
            }
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
                    using (var sClient = new StaffingService.StaffingServiceClient())
                    {
                        var tasks = new List<Task<bool>>();

                        tasks.Add(sClient.EmployeeGetCurrentAsync().ContinueWith<bool>(t =>
                            {
                                if (checkAggregateExceptions(typeof(StaffingService.Employee), t.Exception))
                                {
                                    if (t.Result.Error != null)
                                        throw new Exception(t.Result.Error);

                                    modelLevelThContext.DoCallBack(() => RaiseCurrentInitialize(t.Result.Value));
                                    return true;
                                }
                                return false;
                            }));

                        tasks.Add(sClient.RightsGetAsync().ContinueWith<bool>(t =>
                        {
                            if (checkAggregateExceptions(typeof(StaffingService.Right), t.Exception))
                            {
                                if (t.Result.Error != null)
                                    throw new Exception(t.Result.Error);

                                modelLevelThContext.DoCallBack(() => RaiseItemsInitialize(t.Result.Values));
                                return true;
                            }
                            return false;
                        }));

                        tasks.Add(sClient.StaffingsGetAsync().ContinueWith<bool>(t =>
                        {
                            if (checkAggregateExceptions(typeof(StaffingService.Staffing), t.Exception))
                            {
                                if (t.Result.Error != null)
                                    throw new Exception(t.Result.Error);

                                modelLevelThContext.DoCallBack(() => RaiseItemsInitialize(t.Result.Values));
                                return true;
                            }
                            return false;
                        }));

                        tasks.Add(sClient.EmployeesGetAsync().ContinueWith<bool>(t =>
                        {
                            if (checkAggregateExceptions(typeof(StaffingService.Employee), t.Exception))
                            {
                                if (t.Result.Error != null)
                                    throw new Exception(t.Result.Error);

                                modelLevelThContext.DoCallBack(() => RaiseItemsInitialize(t.Result.Values));
                                return true;
                            }
                            return false;
                        }));

                        tasks.Add(sClient.DepartmentsGetAsync().ContinueWith<bool>(t =>
                        {
                            if (checkAggregateExceptions(typeof(StaffingService.Employee), t.Exception))
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

            IsLoaded = true;

            #endregion
        }

        private void RaiseOnCurrentChanged() => Context.DoCallBack(() => OnCurrentChanged?.Invoke(this, Current));
        private void RaiseOnDepartmentsChanged(IEnumerable<StaffingService.Department> items, StaffingListsAction action)
            => Context.DoCallBack(() => {
                if (items.Any())
                    OnDepartmentsChanged?.Invoke(this, new StaffingListItemsEventArgs<StaffingService.Department>(items.ToArray(), action));
            });
        private void RaiseOnEmployeesChanged(IEnumerable<StaffingService.Employee> items, StaffingListsAction action)
        {
            Context.DoCallBack(() =>
            {
                if (items.Any())
                    OnEmployeesChanged?.Invoke(this, new StaffingListItemsEventArgs<StaffingService.Employee>(items.ToArray(), action));
            });

            var current = items.FirstOrDefault(i => i.Id == (Current?.Id ?? 0));
            if (current != null)
                Current = (action == StaffingListsAction.Remove) 
                    ? null 
                    : current;
        }
        private void RaiseOnStaffingChanged(IEnumerable<StaffingService.Staffing> items, StaffingListsAction action)
            => Context.DoCallBack(() => {
                if (items.Any())
                    OnStaffingChanged?.Invoke(this, new StaffingListItemsEventArgs<StaffingService.Staffing>(items.ToArray(), action));
            });
        private void RaiseOnRightsChanged(IEnumerable<StaffingService.Right> items, StaffingListsAction action)
            => Context.DoCallBack(() => {
                if (items.Any())
                    OnRightsChanged?.Invoke(this, new StaffingListItemsEventArgs<StaffingService.Right>(items.ToArray(), action));
            });

        public event EventHandler<StaffingService.Employee> OnCurrentChanged;

        public event EventHandler<StaffingListItemsEventArgs<StaffingService.Right>> OnRightsChanged;
        public event EventHandler<StaffingListItemsEventArgs<StaffingService.Staffing>> OnStaffingChanged;
        public event EventHandler<StaffingListItemsEventArgs<StaffingService.Employee>> OnEmployeesChanged;
        public event EventHandler<StaffingListItemsEventArgs<StaffingService.Department>> OnDepartmentsChanged;
    }
}
