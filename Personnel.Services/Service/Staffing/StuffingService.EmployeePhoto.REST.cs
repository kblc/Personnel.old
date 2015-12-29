using Helpers.Linq;
using Personnel.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Service.Staffing
{
    public partial class StaffingService : IStaffingServiceREST
    {
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
        /// Add photo to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photos">Employee photo</param>
        public Model.EmployeePhotoExecutionResult RESTEmployeePhotoAdd(string employeeId, Model.EmployeePhoto photo)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeePhotoAdd(id, photo);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(photo), photo);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeePhotoExecutionResult(ex);
                }
        }

        /// <summary>
        /// Add photos to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photos">Employee photo</param>
        public Model.EmployeePhotoExecutionResults RESTEmployeePhotosAdd(string employeeId, Model.EmployeePhoto[] photos)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    return EmployeePhotosAdd(id, photos);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(photos), photos.Concat(p => p.ToString(), ", "));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeePhotoExecutionResults(ex);
                }
        }

        /// <summary>
        /// Remove photo to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photoIdentifiers">Employee photo identifier</param>
        public Model.EmployeePhotoExecutionResult RESTEmployeePhotoRemove(string employeeId, string photoIdentifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    var guid = GuidFromString(photoIdentifier);
                    return EmployeePhotoRemove(id, guid);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(photoIdentifier), photoIdentifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeePhotoExecutionResult(ex);
                }
        }

        /// <summary>
        /// Remove photo to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photoIdentifiers">Employee photo identifier</param>
        public Model.EmployeePhotoExecutionResults RESTEmployeePhotosRemove(string employeeId, string[] photoIdentifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = LongFromString(employeeId);
                    var guids = photoIdentifiers.Select(s => GuidFromString(s)).ToArray();
                    return EmployeePhotosRemove(id, guids);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(photoIdentifiers), photoIdentifiers.Concat(s => s, ", "));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeePhotoExecutionResults(ex);
                }
        }
    }
}
