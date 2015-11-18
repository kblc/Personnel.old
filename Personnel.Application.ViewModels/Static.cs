using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace Personnel.Application.ViewModels
{
    public static class Automapper
    {
        private static bool isInitialized = false;

        internal static void Init()
        {
            //Init automapper

            if (isInitialized)
                return;

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

            isInitialized = true;
        }
    }
}
