using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace Personnel.Application.ViewModels
{
    public static class Static
    {
        static Static()
        {
            //Init automapper
            System.Reflection.Assembly.GetCallingAssembly().GetTypes()
                .Where(t => t.Namespace != null &&
                    ( t.Namespace.StartsWith(typeof(HistoryService.HistoryServiceClient).Namespace)
                    || t.Namespace.StartsWith(typeof(StaffingService.StaffingServiceClient).Namespace)
                    || t.Namespace.StartsWith(typeof(StorageService.FileServiceClient).Namespace)))
                .GroupBy(t => t.Name)
                .Select(g => new { Name = g.Key, GroupedTypes = g.ToList() })
                .ToList()
                .ForEach(i =>
                {
                    i.GroupedTypes
                        .ForEach(sourceType =>
                        {
                            i.GroupedTypes.Except(new[] { sourceType })
                                .ToList()
                                .ForEach(destType =>
                                {
                                    AutoMapper.Mapper.CreateMap(sourceType, destType);
                                });
                        });
                });
        }

        private static ViewModels.Notifications.ViewModel notifications = null;
        public static ViewModels.Notifications.ViewModel Notifications { get { return notifications ?? (notifications = new ViewModels.Notifications.ViewModel()); } }

        private static ViewModels.History.ViewModel history = null;
        public static ViewModels.History.ViewModel History
        {
            get
            {
                return history ?? (history = new ViewModels.History.ViewModel());
            }
        }

        private static ViewModels.Staffing.ViewModel staffing = null;
        public static ViewModels.Staffing.ViewModel Staffing { get { return staffing ?? (staffing = new ViewModels.Staffing.ViewModel()); } }

        private static ViewModels.Staffing.DepartmentsEditViewModel departments = null;
        public static ViewModels.Staffing.DepartmentsEditViewModel Departments { get { return departments ?? (departments = new ViewModels.Staffing.DepartmentsEditViewModel()); } }

        private static ViewModels.Birthdays.ViewModel birthdays = null;
        public static ViewModels.Birthdays.ViewModel Birthdays { get { return birthdays ?? (birthdays = new ViewModels.Birthdays.ViewModel()); } }
    }
}
