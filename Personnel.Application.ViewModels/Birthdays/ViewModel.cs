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
using Personnel.Application.ViewModels.AdditionalModels;

namespace Personnel.Application.ViewModels.Birthdays
{
    public class ViewModel : AbstractBaseViewModel
    {
        private TimeSpan period = TimeSpan.FromDays(365.25 * 3);
        public TimeSpan Period
        {
            get { return period; }
            set
            {
                if (period == value)
                    return;
                period = value;
                RaisePropertyChanged(() => Period);
            }
        }

        public ObservableCollection<EmployeeViewModel> Today { get; } = new MTObservableCollection<EmployeeViewModel>();

        public ObservableCollection<LevelViewModel> Levels { get; } = new MTObservableCollection<LevelViewModel>();

        private float levelsHorizontalPosition = 50;
        public float LevelsHorizontalPosition
        {
            get { return levelsHorizontalPosition; }
            set
            {
                if (levelsHorizontalPosition == value)
                    return;
                levelsHorizontalPosition = value;
                RaisePropertyChanged(() => LevelsHorizontalPosition);
            }
        }

        protected override void Init()
        {
            if (this.IsDesignMode())
            {
                LoadTest();
                return;
            }

            Static.Staffing.IsLoadedChanged += (s, isStuffingLoaded) =>
            {
                if (isStuffingLoaded)
                    Load();
                else
                    IsLoaded = false;
            };
            if (Static.Staffing.IsLoaded)
                Load();
        }

        private void LoadTest()
        {
            var today = Enumerable.Range(1, 5)
                .Select(i => new EmployeeViewModel()
                {
                    Employee = new StaffingService.Employee() { Birthday = DateTime.Now, Name = $"Тест {i}", Surname = "Тестов" },
                    Age = 30,
                    IsBirthdayGone = false,
                    Appoint = new StaffingService.Appoint() { Name = "Тестовая должность" },
                    Department = new StaffingService.Department() { Name = "Тестовый департамент" }
                });
            foreach (var i in today)
                Today.Add(i);

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
                        Stuffing = new StaffingService.Staffing() { Id = i }
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
                        Appoint = new StaffingService.Appoint() { Name = "Тестовая должность" },
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

            Levels.Add(level1);
            SelectedLevel = Levels.LastOrDefault();

            IsLoaded = true;
        }

        private void Load()
        {
            IsLoaded = false;
            try
            {
                Levels.Clear();
                SelectedLevel = null;

                var dtNow = DateTime.Now;
                var now = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day);
                var start = now - TimeSpan.FromDays(Period.TotalDays / 2d);
                var end = start + Period;

                #region Today

                var employeesToday = Static.Staffing.Employee
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
                    Today.Add(employee);

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

                    var employees = Static.Staffing.Employee
                        .Where(e => e.Birthday.HasValue)
                        .Select(e => new { Employee = e, YearBirthday = GetEmployeeDateBirthFor(tp.Start, tp.End, e.Birthday.Value) })
                        .LeftOuterJoin(Static.Staffing.Department, e => e.Employee.Stuffing?.DepartmentId, d => d.Id, (e, Department) => new { e.Employee, e.YearBirthday, Department })
                        .LeftOuterJoin(Static.Staffing.Appoint, e => e.Employee.Stuffing?.AppointId, d => d.Id, (e, Appoint) => new { e.Employee, e.YearBirthday, e.Department, Appoint })
                        .Where(e => e.YearBirthday != null)
                        .Select(e => new EmployeeViewModel()
                        {
                            Employee = e.Employee,
                            Department = e.Department,
                            Appoint = e.Appoint,
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

                Levels.Add(level1);
                SelectedLevel = Levels.LastOrDefault();
                IsLoaded = true;
            }
            catch(Exception ex)
            {
                Static.Notifications.AddExceptionNotification(ex);
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

        private LevelViewModel selectedLevel = null;
        public LevelViewModel SelectedLevel
        {
            get { return selectedLevel; }
            set
            {
                if (selectedLevel == value)
                    return;
                selectedLevel = value;
                RaisePropertyChanged(() => SelectedLevel);
            }
        }

        private void LevelUp()
        {
            if (SelectedLevel == null)
            {
                SelectedLevel = Levels.LastOrDefault();
            } else
            {
                var index = Levels.IndexOf(SelectedLevel);
                if (index < Levels.Count - 1)
                    SelectedLevel = Levels[index + 1];
            }
        }

        private void LevelDown()
        {
            if (SelectedLevel == null)
            {
                SelectedLevel = Levels.FirstOrDefault();
            }
            else
            {
                var index = Levels.IndexOf(SelectedLevel);
                if (index > 0)
                    SelectedLevel = Levels[index - 1];
            }
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
    }
}
