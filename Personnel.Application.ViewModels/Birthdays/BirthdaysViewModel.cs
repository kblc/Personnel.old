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
using System.Threading;

namespace Personnel.Application.ViewModels.Birthdays
{
    public class BirthdaysViewModel : DependencyObject
    {
        #region PeriodForDay

        public static readonly DependencyProperty PeriodForDayProperty = DependencyProperty.Register(nameof(PeriodForDay), typeof(TimeSpan),
            typeof(BirthdaysViewModel), new PropertyMetadata(TimeSpan.FromDays(4), (s, e) => { }));

        public TimeSpan PeriodForDay
        {
            get { return (TimeSpan)GetValue(PeriodForDayProperty); }
            set { SetValue(PeriodForDayProperty, value); }
        }
        #endregion
        #region PeriodForWeek

        public static readonly DependencyProperty PeriodForWeekProperty = DependencyProperty.Register(nameof(PeriodForWeek), typeof(TimeSpan),
            typeof(BirthdaysViewModel), new PropertyMetadata(TimeSpan.FromDays(7 * 3), (s, e) => { }));

        public TimeSpan PeriodForWeek
        {
            get { return (TimeSpan)GetValue(PeriodForWeekProperty); }
            set { SetValue(PeriodForWeekProperty, value); }
        }
        #endregion
        #region PeriodForMonth

        public static readonly DependencyProperty PeriodForMonthProperty = DependencyProperty.Register(nameof(PeriodForMonth), typeof(TimeSpan),
            typeof(BirthdaysViewModel), new PropertyMetadata(TimeSpan.FromDays(365.25 / 12 * 3), (s, e) => { }));

        public TimeSpan PeriodForMonth
        {
            get { return (TimeSpan)GetValue(PeriodForMonthProperty); }
            set { SetValue(PeriodForMonthProperty, value); }
        }
        #endregion
        #region PeriodForYear

        public static readonly DependencyProperty PeriodForYearProperty = DependencyProperty.Register(nameof(PeriodForYear), typeof(TimeSpan),
            typeof(BirthdaysViewModel), new PropertyMetadata(TimeSpan.FromDays(365.25 * 2), (s, e) => { }));

        public TimeSpan PeriodForYear
        {
            get { return (TimeSpan)GetValue(PeriodForYearProperty); }
            set { SetValue(PeriodForYearProperty, value); }
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
                var staffingNew = e.NewValue as Staffing.StaffingViewModel;
                var staffingOld = e.OldValue as Staffing.StaffingViewModel;
                if (model != null)
                {
                    if (staffingOld != null)
                    {
                        staffingOld.OnIsLoadedChanged -= model.RaiseStaffingModelLoaded;
                        staffingOld.Employees.CollectionChanged -= model.RaiseStaffingEmployeeChanged;
                    }
                    model.Load(staffingNew);
                    if (staffingNew != null)
                    { 
                        staffingNew.OnIsLoadedChanged += model.RaiseStaffingModelLoaded;
                        staffingNew.Employees.CollectionChanged += model.RaiseStaffingEmployeeChanged;
                    }
                }
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

        private NotifyCollection<EmployeeViewModel> today = new NotifyCollection<EmployeeViewModel>();
        public IReadOnlyNotifyCollection<EmployeeViewModel> Today => today;

        private NotifyCollection<LevelViewModel> levels = new NotifyCollection<LevelViewModel>();
        public IReadOnlyNotifyCollection<LevelViewModel> Levels => levels;

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

        private void RaiseStaffingModelLoaded(object sender, bool isLoaded)
        {
            RunUnderDispatcher(new Action(() => Load(Staffing)));
        }

        private void RaiseStaffingEmployeeChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RunUnderDispatcher(new Action(() => Load(Staffing)));
        }

        private void RunUnderDispatcher(Delegate a)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, a);
        }

        //public BirthdaysViewModel()
        //{
            //if (this.IsDesignMode())
            //    LoadTest();
        //}

        //private void LoadTest()
        //{
        //    var today = Enumerable.Range(1, 5)
        //        .Select(i => new EmployeeViewModel()
        //        {
        //            Employee = new StaffingService.Employee() { Birthday = DateTime.Now, Name = $"Тест {i}", Surname = "Тестов", Stuffing = new StaffingService.Staffing() { Appoint = "Тестовая должность" } },
        //            Age = 30,
        //            Department = new StaffingService.Department() { Name = "Тестовый департамент" }
        //        });
        //    foreach (var i in today)
        //        this.today.Add(i);

        //    var dtNow = DateTime.Now;
        //    var now = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day);
        //    var start = now - TimeSpan.FromDays(PeriodForWeek.TotalDays / 2d);
        //    var end = start + PeriodForWeek;

        //    #region Level 1

        //    var level1 = new LevelViewModel() { Name = Properties.Resources.VMBIRTHDAYS_BYWEEK };
        //    var weekStart = start - TimeSpan.FromDays((int)start.DayOfWeek + 1);
        //    weekStart = new DateTime(weekStart.Year, weekStart.Month, weekStart.Day);
        //    var rnd = new Random();

        //    var tmpEmployees = Enumerable.Range(0, rnd.Next(10,100))
        //            .Select(i => new StaffingService.Employee()
        //            {
        //                Birthday = new DateTime(rnd.Next(1960,2000), rnd.Next(1,12), rnd.Next(1,28)),
        //                Name = $"Тест {i}",
        //                Surname = "Тест",
        //                Id = i,
        //                Patronymic = "Тестович",
        //                Stuffing = new StaffingService.Staffing() { Id = i, Appoint = "Тестовая должность" }
        //            }).ToArray();

        //    var getWeekNameByDate = new Func<DateTime, string>((dt) => $"{dt.Year}/{dt.DayOfYear / 7}");
        //    var getWeekNameByDates = new Func<DateTime, DateTime, string>((dt0, dt1) =>
        //    {
        //        var nameStart = getWeekNameByDate(dt0);
        //        var nameEnd = getWeekNameByDate(dt1);
        //        return (nameStart == nameEnd) ? nameStart : $"{nameStart} - {nameEnd}";
        //    });

        //    while (weekStart < end)
        //    {
        //        var weekEnd = weekStart + TimeSpan.FromDays(7);
        //        var tp = new TimePartViewModel()
        //        {
        //            Start = weekStart,
        //            End = weekEnd,
        //            IsCurrent = weekStart <= now && now <= weekEnd,
        //            Name = getWeekNameByDates(weekStart, weekEnd)
        //        };

        //        var employees = tmpEmployees
        //            .Where(e => e.Birthday.HasValue)
        //            .Select(e => new { Employee = e, YearBirthday = GetEmployeeDateBirthFor(tp.Start, tp.End, e.Birthday.Value) })
        //            .Where(e => e.YearBirthday != null)
        //            .Select(e => new EmployeeViewModel()
        //            {
        //                Employee = e.Employee,
        //                Department = new StaffingService.Department() { Name = "Тестовый департамент" },
        //                Age = e.YearBirthday.Value.Year - e.Employee.Birthday.Value.Year,
        //                IsBirthdayGone = e.YearBirthday.Value < now,
        //            });

        //        foreach (var e in employees)
        //            tp.Employee.Add(e);

        //        level1.Parts.Add(tp);
        //        weekStart = weekEnd;
        //    }

        //    for (int i = level1.Parts.Count - 1; i >= 1; i--)
        //        if (level1.Parts[i].Employee.Count == 0 && level1.Parts[i - 1].Employee.Count == 0)
        //        {
        //            level1.Parts[i - 1].End = level1.Parts[i].End;
        //            level1.Parts[i - 1].IsCurrent |= level1.Parts[i].IsCurrent;
        //            level1.Parts[i - 1].Name = getWeekNameByDates(level1.Parts[i - 1].Start, level1.Parts[i - 1].End);
        //            level1.Parts.RemoveAt(i);
        //        }

        //    #endregion

        //    levels.Add(level1);
        //    SelectedLevel = Levels.LastOrDefault();

        //    IsLoaded = true;
        //}

        private void Clear()
        {
            today.Clear();
            levels.Clear();
            SelectedLevel = null;
        }

        private static LevelViewModel GetLevel(
            Staffing.StaffingViewModel model, 
            TimeSpan period, 
            string name,
            Func<DateTime, DateTime> changeStartPeriodDate,
            Func<DateTime, string> getTimePartName,
            Func<DateTime, DateTime> getNextPeriodStartDate
            )
        {
            var level = new LevelViewModel() { Name = name, Period = period };

            if (model == null)
                return level;

            var dtNow = DateTime.Now;
            var now = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day);
            var start = now - TimeSpan.FromDays(period.TotalDays / 2d);
            var end = start + period;
            var departments = model.Departments.AsEnumerable().Traverse(i => i.Childs).Select(i => i.Data.Department);

            var periodStart = changeStartPeriodDate(start);
            periodStart = new DateTime(periodStart.Year, periodStart.Month, periodStart.Day);

            var getName = new Func<DateTime, DateTime, string>((dt0, dt1) =>
            {
                var nameStart = getTimePartName(dt0);
                var nameEnd = getTimePartName(dt1);
                return (nameStart == nameEnd) ? nameStart : $"{nameStart} - {nameEnd}";
            });

            while (periodStart < end)
            {
                var periodEnd = getNextPeriodStartDate(periodStart).AddDays(-1); // periodStart + TimeSpan.FromDays(7);
                var tp = new TimePartViewModel()
                {
                    Start = periodStart,
                    End = periodEnd,
                    IsCurrent = periodStart <= now && now <= periodEnd,
                    Name = getName(periodStart, periodEnd)
                };

                var employees = model.Employees
                    .Where(e => e.Employee.Birthday.HasValue)
                    .Select(e => new { EmployeeVM = e, YearBirthday = GetEmployeeDateBirthFor(tp.Start, tp.End, e.Employee.Birthday.Value) })
                    .Where(e => e.YearBirthday.HasValue)
                    .OrderBy(r => r.YearBirthday.Value)
                    .Select(e => new EmployeeViewModel()
                    {
                        Employee = e.EmployeeVM.Employee,
                        Department = e.EmployeeVM.Department,
                        Age = e.YearBirthday.Value.Year - e.EmployeeVM.Employee.Birthday.Value.Year,
                        DayOfBirthday = e.YearBirthday.Value,
                    });

                foreach (var e in employees)
                    tp.Employee.Add(e);

                level.Parts.Add(tp);
                periodStart = periodEnd.AddDays(1);
            }

            for (int i = level.Parts.Count - 1; i >= 1; i--)
                if (level.Parts[i].Employee.Count == 0 && level.Parts[i - 1].Employee.Count == 0)
                {
                    level.Parts[i - 1].End = level.Parts[i].End;
                    level.Parts[i - 1].IsCurrent |= level.Parts[i].IsCurrent;
                    level.Parts[i - 1].Name = getName(level.Parts[i - 1].Start, level.Parts[i - 1].End);
                    level.Parts.RemoveAt(i);
                }

            return level;
        }

        private static LevelViewModel GetDayLevel(Staffing.StaffingViewModel model, TimeSpan period)
        {
            return GetLevel(model, period, Properties.Resources.VMBIRTHDAYS_BYDAY,
                start => start,
                dt => dt.ToShortDateString(),
                dt => dt.AddDays(1));
        }
        private static LevelViewModel GetWeekLevel(Staffing.StaffingViewModel model, TimeSpan period)
        {
            return GetLevel(model, period, Properties.Resources.VMBIRTHDAYS_BYWEEK, 
                start => start - TimeSpan.FromDays((int)start.DayOfWeek + 1),
                dt => $"{dt.Year}/{dt.DayOfYear / 7}",
                dt => dt.AddDays(7));
        }
        private static LevelViewModel GetMonthLevel(Staffing.StaffingViewModel model, TimeSpan period)
        {
            return GetLevel(model, period, Properties.Resources.VMBIRTHDAYS_BYMONTH,
                start => start - TimeSpan.FromDays(start.Day - 1),
                dt => dt.ToString("MMMMMM yyyy"),
                dt => dt.AddMonths(1));
        }

        private static LevelViewModel GetMonth3Level(Staffing.StaffingViewModel model, TimeSpan period)
        {
            return GetLevel(model, period, Properties.Resources.VMBIRTHDAYS_BY3MONTH,
                start => start - TimeSpan.FromDays(start.Day - 1),
                dt => dt.ToString("MMMMMM yyyy"),
                dt => dt.AddMonths(3));
        }

        private static LevelViewModel GetYearLevel(Staffing.StaffingViewModel model, TimeSpan period)
        {
            return GetLevel(model, period, Properties.Resources.VMBIRTHDAYS_BYYEAR,
                start => start - TimeSpan.FromDays(start.DayOfYear - 1),
                dt => dt.ToString("yyyy"),
                dt => dt.AddYears(1));
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
                //var start = now - TimeSpan.FromDays(Period.TotalDays / 2d);
                //var end = start + Period;
                var departments = model.Departments.AsEnumerable().Traverse(i => i.Childs).Select(i => i.Data.Department);

                #region Today

                var employeesToday = model.Employees
                    .Where(e => e.Employee.Birthday.HasValue)
                    .Select(e => new { EmployeeVM = e, YearBirthday = GetEmployeeDateBirthFor(now, now, e.Employee.Birthday.Value) })
                    .Where(e => e.YearBirthday.HasValue)
                    .OrderBy(r => r.YearBirthday.Value)
                    .Select(e => new EmployeeViewModel()
                    {
                        Employee = e.EmployeeVM.Employee,
                        Department = e.EmployeeVM.Department,
                        Age = e.YearBirthday.Value.Year - e.EmployeeVM.Employee.Birthday.Value.Year,
                        DayOfBirthday = e.YearBirthday.Value,
                    });

                #endregion

                levels.Clear();
                var weekLevel = GetWeekLevel(model, PeriodForWeek);
                var monthLevel = GetMonthLevel(model, PeriodForMonth);
                var month3Level = GetMonth3Level(model, PeriodForMonth + PeriodForMonth + PeriodForMonth);
                var yearLevel = GetYearLevel(model, PeriodForYear);
                foreach (var level in new[] { weekLevel, monthLevel, month3Level, yearLevel }.OrderByDescending(p => p.Period))
                    levels.Add(level);

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
