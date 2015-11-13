using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Personnel.Services.Tests
{
    [TestClass]
    public class StuffingTests
    {
        [TestMethod]
        public void Stuffing_GetAppoints()
        {
            using (var ssc = new StaffingService.StaffingServiceClient())
            { 
                var allApp = ssc.AppointsGet();
                if (allApp.Error != null)
                    throw new Exception(allApp.Error);

                foreach(var a in allApp.Values)
                {
                    Console.WriteLine($"[{a.Id}] {a.Name}");
                }

                Assert.AreNotEqual(0, allApp.Values.Count);
            }
        }

        [TestMethod]
        public void Stuffing_GetEmployees()
        {
            using (var ssc = new StaffingService.StaffingServiceClient())
            {
                var getRes = ssc.EmployeesGet();
                if (getRes.Error != null)
                    throw new Exception(getRes.Error);

                foreach (var a in getRes.Values)
                {
                    Console.WriteLine($"[{a.Id}] {a.Surname} {a.Name} {a.Patronymic} (email: {a.Email})");
                }

                Assert.AreNotEqual(0, getRes.Values.Count);
            }
        }

        [TestMethod]
        public void Stuffing_GetRights()
        {
            using (var ssc = new StaffingService.StaffingServiceClient())
            {
                var allRgh = ssc.RightsGet();
                if (allRgh.Error != null)
                    throw new Exception(allRgh.Error);

                foreach (var a in allRgh.Values)
                {
                    Console.WriteLine($"[{a.Id}] [{a.SystemName}] {a.Name}");
                }

                Assert.AreNotEqual(0, allRgh.Values.Count);
            }
        }

        [TestMethod]
        public void Stuffing_GetDepartments()
        {
            using (var ssc = new StaffingService.StaffingServiceClient())
            {
                var allDep = ssc.DepartmentsGet();
                if (allDep.Error != null)
                    throw new Exception(allDep.Error);

                foreach (var a in allDep.Values)
                {
                    Console.WriteLine($"[{a.Id}] [parent:{a.ParentId}] {a.Name}");
                }

                Assert.AreNotEqual(0, allDep.Values.Count);
            }
        }

        [TestMethod]
        public void Stuffing_AddRemoveDepartment()
        {
            using (var ssc = new StaffingService.StaffingServiceClient())
            {
                var depRes = ssc.DepartmentInsert(new StaffingService.Department() { Name = "testDep", ParentId = 1 });
                if (depRes.Error != null)
                    throw new Exception(depRes.Error);

                var allDep = ssc.DepartmentGet(depRes.Value.Id);
                if (allDep.Error != null)
                    throw new Exception(allDep.Error);

                var delRes = ssc.DepartmentRemove(depRes.Value.Id);
                if (delRes.Error != null)
                    throw new Exception(delRes.Error);

                var allDep2 = ssc.DepartmentGet(depRes.Value.Id);
                Assert.AreNotEqual(null, allDep2.Error, "Department must not exists");
            }
        }

        [TestMethod]
        public void Stuffing_AddRemoveAppoint()
        {
            using (var ssc = new StaffingService.StaffingServiceClient())
            {
                var insRes = ssc.AppointInsert(new StaffingService.Appoint() { Name = "TEST" });
                if (insRes.Error != null)
                    throw new Exception(insRes.Error);

                var getRes = ssc.AppointGet(insRes.Value.Id);
                if (getRes.Error != null)
                    throw new Exception(getRes.Error);

                var delRes = ssc.AppointRemove(insRes.Value.Id);
                if (delRes.Error != null)
                    throw new Exception(delRes.Error);

                var getRes2 = ssc.AppointGet(insRes.Value.Id);
                Assert.AreNotEqual(null, getRes2.Error, "Appoint must not exists");
            }
        }

        [TestMethod]
        public void Stuffing_AddRemoveEmployee()
        {
            using (var ssc = new StaffingService.StaffingServiceClient())
            {
                var loginRght = ssc.RightsGet().Values.FirstOrDefault(r => string.Compare("login", r.SystemName, true) == 0);

                var i = new StaffingService.Employee() { Name = "TEST", Surname = "TEST" };
                i.Rights = new System.Collections.Generic.List<StaffingService.EmployeeRight>();
                i.Logins = new System.Collections.Generic.List<StaffingService.EmployeeLogin>();
                i.Rights.Add(new StaffingService.EmployeeRight() { RightId = loginRght.Id });
                i.Logins.Add(new StaffingService.EmployeeLogin() { Login = "TST\\TST" });

                var insRes = ssc.EmployeeInsert(i);
                if (insRes.Error != null)
                    throw new Exception(insRes.Error);

                insRes.Value.Name += insRes.Value.Name;
                insRes.Value.Surname += insRes.Value.Surname;
                insRes.Value.Patronymic = "TEST";
                insRes.Value.Logins.Add(new StaffingService.EmployeeLogin() { Login = "TST\\TST2" });

                var getUpd = ssc.EmployeeUpdate(insRes.Value);
                if (getUpd.Error != null)
                    throw new Exception(getUpd.Error);

                var delRes = ssc.EmployeeRemove(insRes.Value.Id);
                if (delRes.Error != null)
                    throw new Exception(delRes.Error);

                var getRes2 = ssc.EmployeeGet(insRes.Value.Id);
                Assert.AreNotEqual(null, getRes2.Error, "Employee must not exists");
            }
        }
    }
}
