using Helpers.Linq;
using Personnel.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Service.Staffing
{
    public partial class StaffingService
    {
        /// <summary>
        /// Get photos for employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        public Model.EmployeePhotoExecutionResults EmployeePhotosGet(long employeeId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var emp = EmployeeGet(employeeId);
                    if (emp.Exception != null)
                        throw emp.Exception;

                    return new EmployeePhotoExecutionResults(emp.Value.Photos);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeePhotoExecutionResults(ex);
                }
        }

        /// <summary>
        /// Add photos to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photos">Employee photo</param>
        public Model.EmployeePhotoExecutionResults EmployeePhotosAdd(long employeeId, Model.EmployeePhoto photo)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var emp = EmployeeGet(employeeId);
                    if (emp.Exception != null)
                        throw emp.Exception;

                    using (var rep = GetNewRepository(logSession))
                    {
                        var dbPhoto = AutoMapper.Mapper.Map<Repository.Model.EmployeePhoto>(photo);
                        rep.AddOrUpdate(dbPhoto, false);
                    }

                    return EmployeePhotosGet(employeeId);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(photo), photo);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeePhotoExecutionResults(ex);
                }
        }

        /// <summary>
        /// Remove photo to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photoIdentifier">Employee photo identifier</param>
        public Model.EmployeePhotoExecutionResults EmployeePhotosRemove(long employeeId, Guid photoIdentifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var emp = EmployeeGet(employeeId);
                    if (emp.Exception != null)
                        throw emp.Exception;

                    using (var rep = GetNewRepository(logSession))
                    {
                        var dbPhoto = rep.Get<Repository.Model.EmployeePhoto>(e => e.FileId == photoIdentifier).SingleOrDefault();
                        if (dbPhoto == null)
                            throw new Exception(string.Format(Properties.Resources.FILESERVICE_FileNotFound, photoIdentifier));
                        rep.Remove(dbPhoto);
                    }

                    return EmployeePhotosGet(employeeId);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(photoIdentifier), photoIdentifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeePhotoExecutionResults(ex);
                }
        }

        /// <summary>
        /// Get photos for employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        public Model.EmployeePhotoExecutionResults RESTEmployeePhotosGet(string employeeId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeePhotosGet(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeePhotoExecutionResults(ex);
                }
        }

        /// <summary>
        /// Add photos to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photos">Employee photo</param>
        public Model.EmployeePhotoExecutionResults RESTEmployeePhotosAdd(string employeeId, Model.EmployeePhoto photo)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeePhotosAdd(id, photo);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(photo), photo);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeePhotoExecutionResults(ex);
                }
        }

        /// <summary>
        /// Remove photo to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photoIdentifier">Employee photo identifier</param>
        public Model.EmployeePhotoExecutionResults RESTEmployeePhotosRemove(string employeeId, string photoIdentifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    var guid = GuidFromString(photoIdentifier);
                    return EmployeePhotosRemove(id, guid);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(photoIdentifier), photoIdentifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeePhotoExecutionResults(ex);
                }
        }
    }
}
