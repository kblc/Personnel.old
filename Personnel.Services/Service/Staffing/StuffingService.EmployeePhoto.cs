using Helpers.Linq;
using Personnel.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Service.Staffing
{
    public partial class StaffingService : IStaffingService
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
        public Model.EmployeePhotoExecutionResult EmployeePhotoAdd(long employeeId, Model.EmployeePhoto photo)
        {
            var res = EmployeePhotosAdd(employeeId, new EmployeePhoto[] { photo });
            return new EmployeePhotoExecutionResult()
            {
                Error = res.Error,
                Exception = res.Exception,
                Value = res.Values.FirstOrDefault()
            };
        }

        /// <summary>
        /// Add photos to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photos">Employee photo</param>
        public Model.EmployeePhotoExecutionResults EmployeePhotosAdd(long employeeId, Model.EmployeePhoto[] photos)
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
                        var empphs = photos.Select(p => new Repository.Model.EmployeePhoto()
                        {
                            EmployeeId = employeeId,
                            FileId = p.FileId,
                        });
                        rep.AddRange(empphs);
                    }

                    return EmployeePhotosGet(employeeId);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(photos), photos.Concat(p => p.ToString(),", "));
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
        public Model.EmployeePhotoExecutionResult EmployeePhotoRemove(long employeeId, Guid photoIdentifier)
        {
            var res = EmployeePhotosRemove(employeeId, new Guid[] { photoIdentifier });
            return new EmployeePhotoExecutionResult()
            {
                Error = res.Error,
                Exception = res.Exception,
                Value = res.Values.FirstOrDefault()
            };
        }

        /// <summary>
        /// Remove photo to employee
        /// </summary>
        /// <param name="employeeId">Employee identifier</param>
        /// <param name="photoIdentifier">Employee photo identifier</param>
        public Model.EmployeePhotoExecutionResults EmployeePhotosRemove(long employeeId, Guid[] photoIdentifiers)
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
                        photoIdentifiers.ToList().ForEach(photoIdentifier => {
                            var dbPhoto = rep.Get<Repository.Model.EmployeePhoto>(e => e.FileId == photoIdentifier).SingleOrDefault();
                            if (dbPhoto == null)
                                throw new Exception(string.Format(Properties.Resources.FILESERVICE_FileNotFound, photoIdentifier));
                            rep.Remove(dbPhoto, saveAfterRemove: false);
                        });
                        rep.SaveChanges(true);
                    }

                    return EmployeePhotosGet(employeeId);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(employeeId), employeeId);
                    ex.Data.Add(nameof(photoIdentifiers), photoIdentifiers.Concat(i => i.ToString(),", "));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new EmployeePhotoExecutionResults(ex);
                }
        }
    }
}
