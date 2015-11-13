using Helpers.WPF;
using Personnel.Application.ViewModels.Additional;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Helpers.Linq;

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

        public ObservableCollection<LevelViewModel> Levels { get; } = new ObservableCollection<LevelViewModel>();

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
        private void Load()
        {
            IsLoaded = false;
            try
            {
                Levels.Clear();
                SelectedLevel = null;

                var now = DateTime.Now;
                var start = now - TimeSpan.FromDays(Period.TotalDays / 2d);
                var end = start + Period;

                #region Level 1

                var getWeekNameByDate = new Func<DateTime, string>((dt) => $"{Properties.Resources.VMBIRTHDAYS_WEEK} {dt.DayOfYear / 7} - {dt.Year}");
                var getWeekNameByDates = new Func<DateTime, DateTime, string>((dt0, dt1) => 
                {
                    var nameStart = getWeekNameByDate(dt0);
                    var nameEnd = getWeekNameByDate(dt1);
                    return (nameStart == nameEnd) ? nameStart : $"{nameStart} ~ {nameEnd}";
                });

                var getEmployeeDateBirth = new Func<DateTime, DateTime, DateTime, DateTime?>((dateFrom, dateTo, birthDay) =>
                {
                    if (dateFrom.Year == dateTo.Year)
                    {
                        if (dateFrom.DayOfYear <= birthDay.DayOfYear && birthDay.DayOfYear <= dateTo.DayOfYear)
                            return new DateTime(dateFrom.Year, birthDay.Month, birthDay.Day);
                    } else
                    {
                        var dateTo2 = new DateTime(dateTo.Year, 1, 1);
                        if (dateFrom.DayOfYear <= birthDay.DayOfYear && birthDay.DayOfYear < dateTo2.DayOfYear)
                        {
                            return new DateTime(dateFrom.Year, birthDay.Month, birthDay.Day);
                        }
                        if (dateTo2.DayOfYear <= birthDay.DayOfYear && birthDay.DayOfYear < dateTo.DayOfYear)
                        {
                            return new DateTime(dateTo2.Year, birthDay.Month, birthDay.Day);
                        }
                    }
                    return null;
                });
                

                var level1 = new LevelViewModel() { Name = Properties.Resources.VMBIRTHDAYS_BYWEEK };
                var weekStart = start - TimeSpan.FromDays((int)start.DayOfWeek) - TimeSpan.FromTicks(start.Ticks);
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
                        .Select(e => new { Employee = e, YearBirthday = getEmployeeDateBirth(tp.Start, tp.End, e.Birthday.Value) })
                        .Where(e => e.YearBirthday != null)
                        .Select(e => new EmployeeViewModel()
                        {
                            Employee = e.Employee,
                            Age = e.YearBirthday.Value.Year - e.Employee.Birthday.Value.Year,
                            IsBirthdayGone = e.YearBirthday.Value < now
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

        private LevelViewModel selectedLevel = null;
        public LevelViewModel SelectedLevel
        {
            get { return selectedLevel; }
            set
            {
                if (selectedLevel == value)
                    return;
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
