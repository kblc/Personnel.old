using Personnel.Application.ViewModels.AdditionalModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Helpers;

namespace Personnel.Application.ViewModels.Staffing
{
    public class ViewModel : Additional.AbstractBaseViewModel
    {
        public ObservableCollection<StaffingService.Right> Rights { get; }
            = new ObservableCollection<StaffingService.Right>();

        public ObservableCollection<StaffingService.Staffing> Staffing { get; }
            = new ObservableCollection<StaffingService.Staffing>();

        public ObservableCollection<StaffingService.Employee> Employee { get; }
            = new ObservableCollection<StaffingService.Employee>();

        public ObservableCollection<StaffingService.Department> Department { get; }
            = new ObservableCollection<StaffingService.Department>();

        public ObservableCollection<StaffingService.Appoint> Appoint { get; }
            = new ObservableCollection<StaffingService.Appoint>();

        private StaffingService.Employee current = null;
        public StaffingService.Employee Current
        {
            get { return current; }
            private set
            {
                if (current == value)
                    return;
                current = value;
                RaisePropertyChanged(() => Current);
            }
        }

        private void RaiseExceptionCatched(Type t, Exception ex) => Static.Notifications.AddExceptionNotification(ex, t?.Name);
        private void RaiseIsLoaded(bool value)
        {
            IsLoaded = value;
        }
        private void RaiseCurrentChanged(StaffingService.Employee current)
        {
            Current = current;
        }
        private void RaiseItemsInitialize<T>(T[] items)
        {
            if (typeof(T) == typeof(StaffingService.Right))
                foreach (var item in items.Cast<StaffingService.Right>())
                    Rights.Add(item);
            if (typeof(T) == typeof(StaffingService.Staffing))
                foreach (var item in items.Cast<StaffingService.Staffing>())
                    Staffing.Add(item);
            if (typeof(T) == typeof(StaffingService.Employee))
                foreach (var item in items.Cast<StaffingService.Employee>())
                    Employee.Add(item);
            if (typeof(T) == typeof(StaffingService.Department))
                foreach (var item in items.Cast<StaffingService.Department>())
                    Department.Add(item);
            if (typeof(T) == typeof(StaffingService.Appoint))
                foreach (var item in items.Cast<StaffingService.Appoint>())
                    Appoint.Add(item);
        }

        private void OnHistoryChanged(object sender, HistoryService.History e)
        {
            var upds = new[] { e.Add, e.Change };
            foreach(var upd in upds.Where(i => i != null))
            {
                upd.GetType().GetProperties(System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                    .Where(p => p.PropertyType.IsArray && p.CanRead)
                    .Select(p => new { Values = p.GetValue(upd), ElementType = p.PropertyType.GetElementType() })
                    .Where(p => p.Values != null)
                    .ToList()
                    .ForEach(i =>
                    {
                        //
                    });
            }

            RaiseOnChange();
        }

        private Thread initThread = null;
        protected override void Init()
        {
            initThread = new Thread(new ParameterizedThreadStart(InitThread));
            initThread.IsBackground = true;
            initThread.Start(Context);
            Static.History.Changed += OnHistoryChanged;
        }
        protected override void OnDisposing()
        {
            if (initThread != null && initThread.IsAlive)
            {
                initThread.Abort();
                initThread = null;
            }
        }

        private void InitThread(object context)
        {
            var modelLevelThContext = (System.Runtime.Remoting.Contexts.Context)context;
            bool inited = false;
            #region Infinity try to connect then init and exit

            var checkAggregateExceptions = new Action<Type, AggregateException>((t, ex) =>
            {
                foreach(var e in ex.InnerExceptions)
                    modelLevelThContext.DoCallBack(() => RaiseExceptionCatched(t, e));
                throw ex;
            });

            do
            {
                try
                {
                    using (var sClient = new StaffingService.StaffingServiceClient())
                    {
                        var resCurrent = sClient.EmployeeGetCurrentAsync().ContinueWith(t =>
                        {
                            checkAggregateExceptions(typeof(StaffingService.Employee), t.Exception);
                            modelLevelThContext.DoCallBack(() => RaiseCurrentChanged(t.Result.Value));
                        });
                        var resRights = sClient.RightsGetAsync().ContinueWith(t =>
                        {
                            checkAggregateExceptions(typeof(StaffingService.Right), t.Exception);
                            modelLevelThContext.DoCallBack(() => RaiseItemsInitialize(t.Result.Values));
                        });
                        var resStaffing = sClient.StaffingsGetAsync().ContinueWith(t =>
                        {
                            checkAggregateExceptions(typeof(StaffingService.Staffing), t.Exception);
                            modelLevelThContext.DoCallBack(() => RaiseItemsInitialize(t.Result.Values));
                        });
                        var resEmployees = sClient.EmployeesGetAsync().ContinueWith(t =>
                        {
                            checkAggregateExceptions(typeof(StaffingService.Employee), t.Exception);
                            modelLevelThContext.DoCallBack(() => RaiseItemsInitialize(t.Result.Values));
                        });
                        var resDeps = sClient.DepartmentsGetAsync().ContinueWith(t =>
                        {
                            checkAggregateExceptions(typeof(StaffingService.Department), t.Exception);
                            modelLevelThContext.DoCallBack(() => RaiseItemsInitialize(t.Result.Values));
                        });
                        var resAppoints = sClient.AppointsGetAsync().ContinueWith(t =>
                        {
                            checkAggregateExceptions(typeof(StaffingService.Appoint), t.Exception);
                            modelLevelThContext.DoCallBack(() => RaiseItemsInitialize(t.Result.Values));
                        });

                        Task.WaitAll(resCurrent, resRights, resStaffing, resEmployees, resDeps, resAppoints);
                        (new Task[] { resCurrent, resRights, resStaffing, resEmployees, resDeps, resAppoints }).ToList()
                            .ForEach(t => checkAggregateExceptions(null, t.Exception));

                        inited = true;
                    }
                }
                catch (Exception ex)
                {
                    modelLevelThContext.DoCallBack(() => RaiseExceptionCatched(null, ex));
                    Thread.Sleep(200);
                }
            } while (!inited);

            modelLevelThContext.DoCallBack(() => RaiseIsLoaded(true));

            #endregion
        }

        private void RaiseOnChange()
        {
            Changed?.Invoke(this, new EventArgs());
        }
        public event EventHandler Changed;
    }
}
