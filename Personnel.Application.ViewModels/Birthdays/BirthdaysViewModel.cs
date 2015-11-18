using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Helpers.Linq;
using Helpers.WPF;
using Helpers;
using Personnel.Application.ViewModels.AdditionalModels;
using System.Windows;
using System.Globalization;

namespace Personnel.Application.ViewModels.Birthdays
{
    public class BirthdaysViewModel : DependencyObject
    {
        #region Period

        public static readonly DependencyProperty PeriodProperty = DependencyProperty.Register(nameof(Period), typeof(TimeSpan),
            typeof(BirthdaysViewModel), new PropertyMetadata(TimeSpan.FromDays(365.25 * 3), (s, e) => { }));

        public TimeSpan Period
        {
            get { return (TimeSpan)GetValue(PeriodProperty); }
            set { SetValue(PeriodProperty, value); }
        }
        #endregion
        #region Notifications

        public static readonly DependencyProperty NotificationsProperty = DependencyProperty.Register(nameof(Notifications), typeof(Notifications.NotificationsViewModel),
            typeof(BirthdaysViewModel), new PropertyMetadata(null, (s, e) => { }));

        public Notifications.NotificationsViewModel Notifications
        {
            get { return (Notifications.NotificationsViewModel)GetValue(NotificationsProperty); }
            set { SetValue(NotificationsProperty, value); }
        }

        #endregion
        #region Staffing

        public static readonly DependencyProperty StaffingProperty = DependencyProperty.Register(nameof(Staffing), typeof(Staffing.StaffingViewModel),
            typeof(BirthdaysViewModel), new PropertyMetadata(null, (s, e) => 
            {
                var model = s as BirthdaysViewModel;
                model?.Load(e.NewValue as Staffing.StaffingViewModel);
            }));

        public Staffing.StaffingViewModel Staffing
        {
            get { return (Staffing.StaffingViewModel)GetValue(StaffingProperty); }
            set { SetValue(StaffingProperty, value); }
        }

        #endregion
        #region IsLoaded

        private static readonly DependencyPropertyKey ReadOnlyIsLoadedPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(IsLoaded), typeof(bool), typeof(BirthdaysViewModel),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyIsLoadedProperty = ReadOnlyIsLoadedPropertyKey.DependencyProperty;

        public bool IsLoaded
        {
            get { return (bool)GetValue(ReadOnlyIsLoadedProperty); }
            private set { SetValue(ReadOnlyIsLoadedPropertyKey, value); }
        }

        #endregion
        #region ServiceCultureInfo

        public static readonly DependencyProperty ServiceCultureInfoProperty = DependencyProperty.Register(nameof(ServiceCultureInfo), typeof(CultureInfo),
            typeof(BirthdaysViewModel), new PropertyMetadata(System.Threading.Thread.CurrentThread.CurrentUICulture, (s, e) => {
                var model = s as BirthdaysViewModel;
                model?.Load(e.NewValue as Staffing.StaffingViewModel);
            }));

        public CultureInfo ServiceCultureInfo
        {
            get { return (CultureInfo)GetValue(ServiceCultureInfoProperty); }
            set { SetValue(ServiceCultureInfoProperty, value); }
        }
        #endregion
        #region SelectedLevel

        public static readonly DependencyProperty SelectedLevelProperty = DependencyProperty.Register(nameof(SelectedLevel), typeof(LevelViewModel),
            typeof(BirthdaysViewModel), new PropertyMetadata(null, (s, e) => { }));

        public LevelViewModel SelectedLevel
        {
            get { return (LevelViewModel)GetValue(SelectedLevelProperty); }
            set { SetValue(SelectedLevelProperty, value); }
        }
        #endregion
        #region Commands

        private void RaiseCommands()
        {
            levelUpCommand?.RaiseCanExecuteChanged();
            levelDownCommand?.RaiseCanExecuteChanged();
        }

        private void LevelUp()
        {
            if (SelectedLevel == null)
            {
                SelectedLevel = Levels.LastOrDefault();
            }
            else
            {
                var index = levels.IndexOf(SelectedLevel);
                if (index < Levels.Count - 1)
                    SelectedLevel = levels[index + 1];
            }
            RaiseCommands();
        }

        private void LevelDown()
        {
            if (SelectedLevel == null)
            {
                SelectedLevel = Levels.FirstOrDefault();
            }
            else
            {
                var index = levels.IndexOf(SelectedLevel);
                if (index > 0)
                    SelectedLevel = levels[index - 1];
            }
            RaiseCommands();
        }

        private DelegateCommand levelUpCommand = null;
        public ICommand LevelUpCommand
        {
            get
            {
                return levelUpCommand ??
                    (levelUpCommand = new DelegateCommand(
                        (o) =>
                        {
                            LevelUp();
                            levelUpCommand.RaiseCanExecuteChanged();
                        },
                        (o) => SelectedLevel != Levels.LastOrDefault()));
            }
        }

        private DelegateCommand levelDownCommand = null;
        public ICommand LevelDownCommand
        {
            get
            {
                return levelDownCommand
                    ?? (levelDownCommand = new DelegateCommand(
                        (o) =>
                        {
                            LevelDown();
                            levelUpCommand.RaiseCanExecuteChanged();
                        },
                        (o) => SelectedLevel != Levels.FirstOrDefault()));
            }
        }

        #endregion

        private ObservableCollection<EmployeeViewModel> today = new ObservableCollection<EmployeeViewModel>();
        public IReadOnlyNotifyCollection<EmployeeViewModel> Today => (IReadOnlyNotifyCollection<EmployeeViewModel>)today;

        private ObservableCollection<LevelViewModel> levels = new ObservableCollection<LevelViewModel>();
        public IReadOnlyNotifyCollection<LevelViewModel> Levels => (IReadOnlyNotifyCollection<LevelViewModel>)levels;

        //private float levelsHorizontalPosition = 50;
        //public float LevelsHorizontalPosition
        //{
        //    get { return levelsHorizontalPosition; }
        //    set
        //    {
        //        if (levelsHorizontalPosition == value)
        //            return;
        //        levelsHorizontalPosition = value;
        //        RaisePropertyChanged(() => LevelsHorizontalPosition);
        //    }
        //}

        public BirthdaysViewModel()
        {
            if (this.IsDesignMode())
                LoadTest();
        }

        private void LoadTest()
        {
            var today = Enumerable.Range(1, 5)
                .Select(i => new EmployeeViewModel()
                {
                    Employee = new StaffingService.Employee() { Birthday = DateTime.Now, Name = $"Тест {i}", Surname = "Тестов", Stuffing = new StaffingService.Staffing() { Appoint = "Тестовая должность" } },
                    Age = 30,
                    IsBirthdayGone = false,
                    Department = new StaffingService.Department() { Name = "Тестовый департамент" }
                });
            foreach (var i in today)
                this.today.Add(i);

            var dtNow = DateTime.Now;
            var now = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day);
            var start = now - TimeSpan.FromDays(Period.TotalDays / 2d);
            var end = start + Period;

            #region Level 1

            var level1 = new LevelViewModel() { Name = Properties.Resources.VMBIRTHDAYS_BYWEEK };
            var weekStart = start - TimeSpan.FromDays((int)start.DayOfWeek + 1);
            weekStart = new DateTime(weekStart.Year, weekStart.Month, weekStart.Day);
            var rnd = new Random();

            var tmpEmployees = Enumerable.Range(0, rnd.Next(10,100))
                    .Select(i => new StaffingService.Employee()
                    {
                        Birthday = new DateTime(rnd.Next(1960,2000), rnd.Next(1,12), rnd.Next(1,28)),
                        Name = $"Тест {i}",
                        Surname = "Тест",
                        Id = i,
                        Patronymic = "Тестович",
                        Stuffing = new StaffingService.Staffing() { Id = i, Appoint = "Тестовая должность" }
                    }).ToArray();

            var getWeekNameByDate = new Func<DateTime, string>((dt) => $"{dt.Year}/{dt.DayOfYear / 7}");
            var getWeekNameByDates = new Func<DateTime, DateTime, string>((dt0, dt1) =>
            {
                var nameStart = getWeekNameByDate(dt0);
                var nameEnd = getWeekNameByDate(dt1);
                return (nameStart == nameEnd) ? nameStart : $"{nameStart} - {nameEnd}";
            });

            while (weekStart < end)
            {
                var weekEnd = weekStart + TimeSpan.FromDays(7);
                var tp = new TimePartViewModel()
                {
                    Start = weekStart,
                    End = weekEnd,
                    IsCurrent = weekStart <= now && now <= weekEnd,
                    Name = getWeekNameByDates(weekStart, weekEnd)
                };

                var employees = tmpEmployees
                    .Where(e => e.Birthday.HasValue)
                    .Select(e => new { Employee = e, YearBirthday = GetEmployeeDateBirthFor(tp.Start, tp.End, e.Birthday.Value) })
                    .Where(e => e.YearBirthday != null)
                    .Select(e => new EmployeeViewModel()
                    {
                        Employee = e.Employee,
                        Department = new StaffingService.Department() { Name = "Тестовый департамент" },
                        Age = e.YearBirthday.Value.Year - e.Employee.Birthday.Value.Year,
                        IsBirthdayGone = e.YearBirthday.Value < now,
                    });

                foreach (var e in employees)
                    tp.Employee.Add(e);

                level1.Parts.Add(tp);
                weekStart = weekEnd;
            }

            for (int i = level1.Parts.Count - 1; i >= 1; i--)
                if (level1.Parts[i].Employee.Count == 0 && level1.Parts[i - 1].Employee.Count == 0)
                {
                    level1.Parts[i - 1].End = level1.Parts[i].End;
                    level1.Parts[i - 1].IsCurrent |= level1.Parts[i].IsCurrent;
                    level1.Parts[i - 1].Name = getWeekNameByDates(level1.Parts[i - 1].Start, level1.Parts[i - 1].End);
                    level1.Parts.RemoveAt(i);
                }

            #endregion

            levels.Add(level1);
            SelectedLevel = Levels.LastOrDefault();

            IsLoaded = true;
        }

        private void Clear()
        {
            today.Clear();
            levels.Clear();
            SelectedLevel = null;
        }

        private void Load(Staffing.StaffingViewModel model)
        {
            IsLoaded = false;

            Notifications?.Add(new ServiceWorkers.NotificationItem(
                header: Properties.Resources.ERROR,
                message: Properties.Resources.UNLOADED,
                isError: false,
                iconUrl: ServiceWorkers.AbstractBaseWorker.IconUrlByType(GetType(), ServiceCultureInfo)
                ));

            try
            {
                Clear();

                if (model == null)
                    return;

                var dtNow = DateTime.Now;
                var now = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day);
                var start = now - TimeSpan.FromDays(Period.TotalDays / 2d);
                var end = start + Period;
                var departments = model.Departments.AsEnumerable().Traverse(i => i.Childs).Select(i => i.Data);

                #region Today

                var employeesToday = model.Employees
                    .Where(e => e.Birthday.HasValue)
                    .Select(e => new { Employee = e, YearBirthday = GetEmployeeDateBirthFor(now, now, e.Birthday.Value) })
                    .Where(e => e.YearBirthday != null)
                    .Select(e => new EmployeeViewModel()
                    {
                        Employee = e.Employee,
                        Age = e.YearBirthday.Value.Year - e.Employee.Birthday.Value.Year,
                        IsBirthdayGone = e.YearBirthday.Value < now
                    });
                foreach(var employee in employeesToday)
                    today.Add(employee);

                #endregion
                #region Level 1

                var level1 = new LevelViewModel() { Name = Properties.Resources.VMBIRTHDAYS_BYWEEK };
                var weekStart = start - TimeSpan.FromDays((int)start.DayOfWeek + 1);
                weekStart = new DateTime(weekStart.Year, weekStart.Month, weekStart.Day);

                var getWeekNameByDate = new Func<DateTime, string>((dt) => $"{dt.Year}/{dt.DayOfYear / 7}");
                var getWeekNameByDates = new Func<DateTime, DateTime, string>((dt0, dt1) =>
                {
                    var nameStart = getWeekNameByDate(dt0);
                    var nameEnd = getWeekNameByDate(dt1);
                    return (nameStart == nameEnd) ? nameStart : $"{nameStart} - {nameEnd}";
                });

                while (weekStart < end)
                {
                    var weekEnd = weekStart + TimeSpan.FromDays(7);
                    var tp = new TimePartViewModel()
                    {
                        Start = weekStart,
                        End = weekEnd,
                        IsCurrent = weekStart <= now && now <= weekEnd,
                        Name = getWeekNameByDates(weekStart, weekEnd)
                    };

                    var employees = model.Employees
                        .Where(e => e.Birthday.HasValue)
                        .Select(e => new { Employee = e, YearBirthday = GetEmployeeDateBirthFor(tp.Start, tp.End, e.Birthday.Value) })
                        .LeftOuterJoin(departments, e => e.Employee.Stuffing?.DepartmentId, d => d.Id, (e, Department) => new { e.Employee, e.YearBirthday, Department })
                        .Where(e => e.YearBirthday != null)
                        .Select(e => new EmployeeViewModel()
                        {
                            Employee = e.Employee,
                            Department = e.Department,
                            Age = e.YearBirthday.Value.Year - e.Employee.Birthday.Value.Year,
                            IsBirthdayGone = e.YearBirthday.Value < now,
                        });

                    foreach (var e in employees)
                        tp.Employee.Add(e);

                    level1.Parts.Add(tp);
                    weekStart = weekEnd;
                }

                for(int i = level1.Parts.Count - 1; i >= 1; i--)
                    if (level1.Parts[i].Employee.Count == 0 && level1.Parts[i - 1].Employee.Count == 0)
                    {
                        level1.Parts[i - 1].End = level1.Parts[i].End;
                        level1.Parts[i - 1].IsCurrent |= level1.Parts[i].IsCurrent;
                        level1.Parts[i - 1].Name = getWeekNameByDates(level1.Parts[i - 1].Start, level1.Parts[i - 1].End);
                        level1.Parts.RemoveAt(i);
                    }

                #endregion

                levels.Add(level1);
                SelectedLevel = Levels.LastOrDefault();
                IsLoaded = true;

                Notifications?.Add(new ServiceWorkers.NotificationItem(
                    header: Properties.Resources.ERROR,
                    message: Properties.Resources.LOADED,
                    isError: false,
                    iconUrl: ServiceWorkers.AbstractBaseWorker.IconUrlByType(GetType(), ServiceCultureInfo)
                    ));
            }
            catch (Exception ex)
            {
                Notifications?.Add(new ServiceWorkers.NotificationItem(
                    header: Properties.Resources.ERROR,
                    message: ServiceWorkers.AbstractBaseWorker.GetExceptionText(ex),
                    isError: true,
                    iconUrl: ServiceWorkers.AbstractBaseWorker.IconUrlByType(GetType(), ServiceCultureInfo)
                    ));
            }
            finally
            {
                RaiseCommands();
            }
        }

        private static DateTime? GetEmployeeDateBirthFor(DateTime periodDateFrom, DateTime periodDateTo, DateTime realBirthday)
        {
            if (periodDateFrom.Year == periodDateTo.Year)
            {
                if (periodDateFrom.DayOfYear <= realBirthday.DayOfYear && realBirthday.DayOfYear <= periodDateTo.DayOfYear)
                    return new DateTime(periodDateFrom.Year, realBirthday.Month, realBirthday.Day);
            }
            else
            {
                var dateTo2 = new DateTime(periodDateTo.Year, 1, 1);
                if (periodDateFrom.DayOfYear <= realBirthday.DayOfYear && realBirthday.DayOfYear < dateTo2.DayOfYear)
                {
                    return new DateTime(periodDateFrom.Year, realBirthday.Month, realBirthday.Day);
                }
                if (dateTo2.DayOfYear <= realBirthday.DayOfYear && realBirthday.DayOfYear < periodDateTo.DayOfYear)
                {
                    return new DateTime(dateTo2.Year, realBirthday.Month, realBirthday.Day);
                }
            }
            return null;
        }
    }
}
