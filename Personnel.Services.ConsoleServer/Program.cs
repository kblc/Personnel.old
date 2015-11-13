using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.ConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostName = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).HostName;

            var hostStaffing = new ServiceHost(typeof(Service.Staffing.StaffingService), new Uri($"http://{hostName}:8733/staffing/"));
            hostStaffing.Open();

            var hostHistory = new ServiceHost(typeof(Service.History.HistoryService), new Uri($"http://{hostName}:8733/history/"));
            hostHistory.Open();

            var hostStorage = new ServiceHost(typeof(Service.File.FileService), new Uri($"http://{hostName}:8733/storage/"));
            hostStorage.Open();

            Console.ReadLine();
        }
    }
}
