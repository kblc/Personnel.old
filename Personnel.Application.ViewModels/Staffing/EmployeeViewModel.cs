using Helpers;
using Helpers.WPF;
using Microsoft.Win32;
using Personnel.Application.ViewModels.Additional;
using Personnel.Application.ViewModels.StaffingService;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Helpers.Linq;
using System.Threading;
using System.Globalization;

namespace Personnel.Application.ViewModels.Staffing
{
    public class EmployeePhotoCollection : NotifyPropertyChangedBase
    {
        private StaffingService.Employee employee = null;

        public EmployeePhotoCollection(StaffingService.Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));
            this.employee = employee;
        }

        StaffingService.Picture this[int width, int height]
        {
            get
            {
                var photo = 
                    employee.Photos
                        .Select(p => p.Picture)
                        .OrderBy(p => Math.Abs(p.Width - width) + Math.Abs(p.Height - height))
                        .ThenByDescending(p => p.File.Date)
                        .FirstOrDefault();
                return photo;
            }
        }

        StaffingService.Picture this[StaffingService.PictureType type]
        {
            get
            {
                var photo =
                    employee.Photos
                        .Select(p => p.Picture)
                        .Where(p => p.PictureType == type)
                        .OrderByDescending(p => p.File.Date)
                        .FirstOrDefault()
                        ?? this[int.MaxValue, int.MaxValue];
                return photo;
            }
        }

        StaffingService.Picture this[string typeName]
        {
            get
            {
                var type = typeof(StaffingService.PictureType)
                    .GetEnumValues()
                    .Cast<StaffingService.PictureType>()
                    .Where(t => string.Compare(t.ToString(), typeName, true) == 0)
                    .DefaultIfEmpty(StaffingService.PictureType.None)
                    .FirstOrDefault();
                return this[type];
            }
        }
    }

    public class EmployeeViewModel : NotifyPropertyChangedBase
    {
        private bool isEmpty = false;
        public bool IsEmpty
        {
            get { return isEmpty; }
            set { if (isEmpty == value) return; isEmpty = value; RaisePropertyChanged(); RaiseAllComamnds(); }
        }

        private DataOwner owner;
        public DataOwner Owner
        {
            get { return owner; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(Owner));

                if (owner != null)
                    owner.PropertyChanged -= Owner_PropertyChanged;

                owner = value;

                if (owner != null)
                    owner.PropertyChanged += Owner_PropertyChanged;

                RaisePropertyChanged();
                RaiseAllComamnds();
            }
        }

        private bool isDeleted = false;
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { if (isDeleted == value) return; isDeleted = value; RaisePropertyChanged(); RaiseAllComamnds(); }
        }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            private set { if (isBusy == value) return; isBusy = value; RaisePropertyChanged(); RaiseAllComamnds(); }
        }

        private bool isEditMode = false;
        public bool IsEditMode
        {
            get { return isEditMode; }
            private set
            {
                if (isEditMode == value)
                    return;
                isEditMode = value;

                if (isEditMode)
                    StartEdit();
                else
                    StopEdit();

                RaisePropertyChanged();
                RaiseAllComamnds();
            }
        }

        public bool HasError { get { return !string.IsNullOrWhiteSpace(Error); } }

        private string error = string.Empty;
        public string Error
        {
            get { return error; }
            internal set { if (error == value) return; error = value; RaisePropertyChanged(); RaisePropertyChanged(() => HasError); RaiseAllComamnds(); }
        }

        private void Owner_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaiseAllComamnds();
        }

        public EmployeeViewModel(DataOwner owner)
        {
            this.Owner = owner;
        }
        public EmployeeViewModel() { }

        private StaffingService.Employee employee = null;
        public StaffingService.Employee Employee
        {
            get { return employee; }
            set
            {
                if (employee == value)
                    return;

                //if (employee != null)
                //    employee.PropertyChanged -= Employee_PropertyChanged;

                employee = value;

                //if (employee != null)
                //    employee.PropertyChanged += Employee_PropertyChanged;

                RaisePropertyChanged();
            }
        }

        //private void Employee_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    //
        //}

        private void StartEdit()
        {
            var emp = new Employee();
            emp.CopyObjectFrom(Employee);
            EmployeeForEdit = emp;
        }

        private void StopEdit()
        {
            EmployeeForEdit = null;
        }

        private StaffingService.Employee employeeForEdit = null;
        public StaffingService.Employee EmployeeForEdit
        {
            get { return employeeForEdit; }
            private set
            {
                if (employeeForEdit == value)
                    return;
                employeeForEdit = value;
                RaisePropertyChanged();
            }
        }

        private StaffingService.Department department = null;
        public StaffingService.Department Department
        {
            get { return department; }
            set
            {
                if (department == value)
                    return;
                department = value;
                RaisePropertyChanged(() => Department);
            }
        }

        private EmployeePhotoCollection photos = null;
        public EmployeePhotoCollection Photos { get { return photos ?? (photos = new EmployeePhotoCollection(this.Employee)); } }

        private void RaiseAllComamnds()
        {
            deleteCommand?.RaiseCanExecuteChanged();
            saveCommand?.RaiseCanExecuteChanged();
            editCommand?.RaiseCanExecuteChanged();
            cancelCommand?.RaiseCanExecuteChanged();
        }

        private string GetExceptionText(string whereCatched, Exception ex)
        {
            return ex.GetExceptionText($"{GetType().Name}.{whereCatched}()"
#if !DEBUG
                , clearText: true, includeData: false, includeStackTrace: false
#endif

                );
        }

        private DelegateCommand deleteCommand = null;
        public ICommand DeleteCommand { get { return deleteCommand ?? (deleteCommand = new DelegateCommand(o=> DeleteAsync(), o => (owner?.CanManageEmployes ?? false) && !IsDeleted && !IsBusy && !IsEmpty)); } }

        private DelegateCommand saveCommand = null;
        public ICommand SaveCommand { get { return saveCommand ?? (saveCommand = new DelegateCommand(o => 
        {
            SaveAsync(EmployeeForEdit);
        }, o => (owner?.CanManageEmployes ?? false) && !IsDeleted && !IsBusy && IsEditMode)); } }

        private DelegateCommand editCommand = null;
        public ICommand EditCommand { get { return editCommand ?? (editCommand = new DelegateCommand(o => 
        {
            IsEditMode = true;
            RaiseOnEditCommandExecuted();
        }, o => (owner?.CanManageEmployes ?? false) && !IsDeleted && !IsBusy && !IsEditMode)); } }

        private DelegateCommand cancelCommand = null;
        public ICommand CancelCommand { get { return cancelCommand ?? (cancelCommand = new DelegateCommand(o => { IsEditMode = false; }, o => (owner?.CanManageEmployes ?? false) && !IsDeleted && !IsBusy && IsEditMode)); } }

        private DelegateCommand changePhotoCommand = null;
        public ICommand ChangePhotoCommand { get { return changePhotoCommand ?? (changePhotoCommand = new DelegateCommand(o => 
        {
            ChangePhoto();
        }, o => (owner?.CanManageEmployes ?? false) && !IsDeleted && !IsBusy)); } }

        //private DelegateCommand emptyCommand = null;
        //public ICommand EmptyCommand { get { return emptyCommand ?? (emptyCommand = new DelegateCommand(o => 
        //{
        //    IsEditMode = true;
        //    RaiseOnEmptyCommandExecuted();
        //}, o => IsEmpty)); } }

        private void ChangePhoto()
        {
            var filters = new List<string>();
            var codecs = ImageCodecInfo.GetImageEncoders();

            var allExtensions = codecs.Concat(i => i.FilenameExtension,";");
            filters.Add($"All image files|{allExtensions}");
            filters.AddRange(
                codecs.Select(c => 
                {
                    string codecName = c.CodecName.Replace("Built-in", string.Empty).Replace("Codec","Files").Trim();
                    string extensionView = c.FilenameExtension.Replace(";", ",").ToLowerInvariant();
                    return $"{codecName} ({extensionView})|{c.FilenameExtension}";
                }).ToArray());
            filters.Add($"All files (*.*)|*.*");
            var ofDialog = new OpenFileDialog() {
                CheckFileExists = true,
                Filter = filters.Concat(i => i,"|"),
                Multiselect = false,
            };

            if (ofDialog.ShowDialog() == true)
                UploadPhotosAsync(ofDialog.FileNames, EmployeeForEdit);
        }

        private async void UploadPhotosAsync(string[] filePaths, Employee emp)
        {
            if (filePaths == null)
                throw new ArgumentNullException(nameof(filePaths));

            IsBusy = true;
            try
            {
                var fsc = new StorageService.FileServiceClient();
                var ssc = new StaffingService.StaffingServiceClient();

                var changeLangTask = fsc.ChangeLanguageAsync(CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                var uploadFilesTask = changeLangTask.ContinueWith<EmployeePhotoResults[]>((t) =>
                {
                    if (t.Exception != null)
                        throw t.Exception;
                    //After language change upload files

                    var items = filePaths
                        .Select(fp => new { FilePath = fp, Stream = new System.IO.FileStream(fp, System.IO.FileMode.Open) })
                        .ToArray();

                    var tasks = items.Select(i =>
                        fsc.PutAsync(i.Stream)
                            .ContinueWith<StorageService.PictureResults>(t2 =>
                            {
                                try
                                {
                                    if (t2.Exception != null)
                                        throw t2.Exception;

                                    if (!string.IsNullOrWhiteSpace(t2.Result.Error))
                                        throw new Exception(t2.Result.Error);

                                    var fileId = t2.Result.Value.Id;
                                    return fsc.FileToPictures(fileId);
                                }
                                finally
                                {
                                    try { i.Stream.Dispose(); } catch { }
                                }
                            }, CancellationToken.None, TaskContinuationOptions.AttachedToParent, TaskScheduler.Current)
                            .ContinueWith<EmployeePhotoResults[]>(t2 => 
                            {
                                if (t2.Exception != null)
                                    throw t2.Exception;

                                if (t2.Result.Error != null)
                                    throw new Exception(t2.Result.Error);

                                var results = t2.Result.Values
                                    .Select(p => ssc.EmployeePhotosAdd(emp.Id, new EmployeePhoto()
                                    {
                                        EmployeeId = emp.Id,
                                        FileId = p.FileId
                                    })).ToArray();

                                return results;
                            }
                            , CancellationToken.None, TaskContinuationOptions.AttachedToParent, TaskScheduler.Current)
                    ).ToArray();

                    Task.WaitAll(tasks);

                    return tasks
                        .Where(tsk => tsk.Exception == null)
                        .Select(tsk => tsk.Result)
                        .ToArray();
                }, System.Threading.CancellationToken.None,
                   TaskContinuationOptions.AttachedToParent,
                   TaskScheduler.Default);



                await uploadFilesTask.ContinueWith(t =>
                {
                    try
                    {
                        if (t.Exception != null)
                            throw t.Exception;
                        
                        //Uploaded pictures
                        var addPictures = t.Result
                            .Where(p => string.IsNullOrWhiteSpace(p.Error))
                            .SelectMany(p => p.Values);

                        

                        //emp.Photos = emp.Photos
                        //    .Union(addPictures
                        //        .Select(p => new EmployeePhoto() {  })
                        //        )
                        //        .ToArray();
                    }
                    catch (Exception ex)
                    {
                        Error = GetExceptionText(nameof(UploadPhotosAsync), ex);
                    }
                    finally
                    {
                        try { fsc.Close(); } catch { }
                        try { ssc.Close(); } catch { }
                        IsBusy = false;
                    }
                },
                    System.Threading.CancellationToken.None,
                    TaskContinuationOptions.AttachedToParent,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch(Exception ex)
            {
                IsBusy = false;
                Error = GetExceptionText(nameof(UploadPhotosAsync), ex);
            }
        }

        private async void DeleteAsync()
        {
            if (Employee.Id == 0)
            {
                IsDeleted = true;
                return;
            }

            IsBusy = true;
            try
            {
                var sc = new StaffingServiceClient();
                var waittask = sc.EmployeeRemoveAsync(Employee.Id);
                await waittask.ContinueWith(t =>
                {
                    try
                    {
                        if (t.Exception != null)
                        {
                            Error = GetExceptionText(nameof(DeleteAsync), t.Exception);
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(t.Result.Error))
                            {
                                Error = t.Result.Error;
                            }
                            else
                            {
                                Error = null;
                                EmployeeForEdit = null;
                                IsDeleted = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = GetExceptionText(nameof(DeleteAsync), ex);
                    }
                    finally
                    {
                        try { sc.Close(); } catch { }
                        IsBusy = false;
                    }
                },
                    System.Threading.CancellationToken.None,
                    TaskContinuationOptions.AttachedToParent,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Error = GetExceptionText(nameof(DeleteAsync), ex);
            }

        }

        private async void SaveAsync(StaffingService.Employee employeeToSave)
        {
            IsBusy = true;
            try
            {
                var sc = new StaffingServiceClient();
                var waittask = Employee.Id == 0
                    ? sc.EmployeeInsertAsync(employeeToSave)
                    : sc.EmployeeUpdateAsync(employeeToSave);
                await waittask.ContinueWith(t =>
                {
                    try
                    {
                        if (t.Exception != null)
                        {
                            Error = GetExceptionText(nameof(SaveAsync), t.Exception);
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(t.Result.Error))
                            {
                                Error = t.Result.Error;
                            }
                            else
                            {
                                Error = null;
                                this.Employee.CopyObjectFrom(t.Result.Value);
                                IsEditMode = false;
                                IsEmpty = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = GetExceptionText(nameof(SaveAsync), ex);
                    }
                    finally
                    {
                        try { sc.Close(); } catch { }
                        IsBusy = false;
                    }
                },
                    System.Threading.CancellationToken.None,
                    TaskContinuationOptions.AttachedToParent,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                IsBusy = false;
                Error = GetExceptionText(nameof(SaveAsync), ex);
            }
        }

        //private void RaiseOnEmptyCommandExecuted()
        //{
        //    var e = OnEmptyCommandExecuted;
        //    if (e != null)
        //        e(this, new EventArgs());
        //}
        private void RaiseOnEditCommandExecuted() => OnEditCommandExecuted?.Invoke(this, new EventArgs());

        //public event EventHandler OnEmptyCommandExecuted;
        public event EventHandler OnEditCommandExecuted;
    }
}
