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
using System.Collections.ObjectModel;
using System.Collections;
using Personnel.Application.ViewModels.AdditionalModels;
using System.Collections.Specialized;

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
            this.employee.PropertyChanged += (_, e) => 
            {
                if (e.PropertyName == nameof(employee.Photos))
                {
                    RaisePropertyChanged("[]");
                }
            };
        }

        public StaffingService.Picture this[int width, int height]
        {
            get
            {
                var photo = 
                    employee.Photos?
                        .Select(p => p.Picture)
                        .OrderBy(p => Math.Abs(p.Width - width) + Math.Abs(p.Height - height))
                        .ThenByDescending(p => p.File.Date)
                        .FirstOrDefault();
                return photo;
            }
        }

        public StaffingService.Picture this[StaffingService.PictureType type]
        {
            get
            {
                var photo =
                    employee.Photos?
                        .Select(p => p.Picture)
                        .Where(p => p.PictureType == type)
                        .OrderByDescending(p => p.File.Date)
                        .FirstOrDefault()
                        ?? this[int.MaxValue, int.MaxValue];
                return photo;
            }
        }

        public StaffingService.Picture this[string typeName]
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

    public class RightView : NotifyPropertyChangedBase
    {

        private Right right = null;
        public Right Right { get { return right; } set { if (value == right) return; right = value; RaisePropertyChanged(); } }

        private bool isChecked = false;
        public bool IsChecked { get { return isChecked; } set { if (value == isChecked) return; isChecked = value; RaisePropertyChanged(); } }
    }

    public class EmployeeRightsCollectionView : List<RightView>
    {
        public EmployeeRightsCollectionView(Employee emp, IEnumerable<Right> rights)
        {
            var items = rights
                .LeftOuterJoin(emp.Rights, r => r.Id, e => e.RightId, (Right, EmpRight) => new { Right, EmpRight })
                .Select(i => new RightView() { Right = i.Right, IsChecked = i.EmpRight != null })
                .ToList();

            items.ForEach(i =>
            {
                i.PropertyChanged += (s, e) => 
                {
                    if (e.PropertyName == nameof(RightView.IsChecked)) 
                    {
                        var rw = (RightView)s;
                        if (rw.IsChecked)
                        {
                            emp.Rights = emp.Rights.Union(new EmployeeRight[] { new EmployeeRight() { EmployeeId = emp.Id, RightId = rw.Right.Id } }).ToArray();
                        } else
                        {
                            emp.Rights = emp.Rights.Where(r => r.RightId != rw.Right.Id).ToArray();
                        }
                    }
                };
            });

            this.AddRange(items);
        }
    }

    public class EmployeeViewModel : NotifyPropertyChangedBase
    {
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

        private bool isEmpty = false;
        public bool IsEmpty
        {
            get { return isEmpty; }
            set { if (isEmpty == value) return; isEmpty = value; RaisePropertyChanged(); RaiseAllComamnds(); }
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

        private string newLoginToAdd = string.Empty;
        public string NewLoginToAdd
        {
            get { return newLoginToAdd; }
            set { if (newLoginToAdd == value) return; newLoginToAdd = value; RaisePropertyChanged(); RaiseAllComamnds(); }
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
                Photos = employee == null ? null : new EmployeePhotoCollection(this.Employee);

                //if (employee != null)
                //    employee.PropertyChanged += Employee_PropertyChanged;

                RaisePropertyChanged();
            }
        }

        private EmployeeRightsCollectionView rightView = null;
        public EmployeeRightsCollectionView RightView { get { return rightView; } private set { if (rightView == value) return; rightView = value; RaisePropertyChanged(); } }

        //private void Employee_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    //
        //}

        private void StartEdit()
        {
            var emp = new Employee();
            emp.CopyObjectFrom(Employee);
            RightView = new EmployeeRightsCollectionView(emp, Owner.Rights);
            EmployeeForEdit = emp;
        }

        private void StopEdit()
        {
            EmployeeForEdit = null;
            rightView = null;
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
        public EmployeePhotoCollection Photos
        {
            get { return photos ?? (photos = new EmployeePhotoCollection(this.Employee)); }
            set { photos = value; RaisePropertyChanged(); }
        }

        private void RaiseAllComamnds()
        {
            deleteCommand?.RaiseCanExecuteChanged();
            saveCommand?.RaiseCanExecuteChanged();
            editCommand?.RaiseCanExecuteChanged();
            cancelCommand?.RaiseCanExecuteChanged();
            addLoginCommand?.RaiseCanExecuteChanged();
            deleteLoginCommand?.RaiseCanExecuteChanged();
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
            RaiseOnEditCommandExecuted();
            IsEditMode = true;
        }, o => (owner?.CanManageEmployes ?? false) && !IsDeleted && !IsBusy && !IsEditMode)); } }

        private DelegateCommand cancelCommand = null;
        public ICommand CancelCommand { get { return cancelCommand ?? (cancelCommand = new DelegateCommand(o => { IsEditMode = false; }, o => (owner?.CanManageEmployes ?? false) && !IsDeleted && !IsBusy && IsEditMode)); } }

        private DelegateCommand changePhotoCommand = null;
        public ICommand ChangePhotoCommand { get { return changePhotoCommand ?? (changePhotoCommand = new DelegateCommand(o => 
        {
            ChangePhoto();
        }, o => (owner?.CanManageEmployes ?? false) && !IsDeleted && !IsBusy)); } }

        private DelegateCommand addLoginCommand = null;
        public ICommand AddLoginCommand
        {
            get
            {
                return addLoginCommand ?? (addLoginCommand = new DelegateCommand((o) => 
                {
                    var login = NewLoginToAdd?.ToString();
                    if (string.IsNullOrWhiteSpace(login))
                    {
                        Error = "Enter login to add";
                    }
                    else
                    {
                        if (EmployeeForEdit.Logins.Any(el => string.Compare(el.Login, login,true) == 0 ))
                        {
                            Error = "Same login already exists";
                        } else
                        {
                            var newEmployeeLogin = new EmployeeLogin() { Login = login.ToUpperInvariant(), EmployeeId = EmployeeForEdit.Id };
                            EmployeeForEdit.Logins = EmployeeForEdit.Logins.Union(new EmployeeLogin[] { newEmployeeLogin }).ToArray();
                            NewLoginToAdd = string.Empty;
                        }
                    }
                }, (o) => Owner.CanManageEmployeeLogins && IsEditMode && NewLoginToAdd.Length > 0));
            }
        }

        private DelegateCommand deleteLoginCommand = null;
        public ICommand DeleteLoginCommand
        {
            get
            {
                return deleteLoginCommand ?? (deleteLoginCommand = new DelegateCommand((employeeLogin) =>
                {
                    if (employeeLogin == null)
                        Error = "Select login to delete first";
                    else
                        EmployeeForEdit.Logins = EmployeeForEdit.Logins.Except(new EmployeeLogin[] { (EmployeeLogin)employeeLogin }).ToArray();
                }, (o) => Owner.CanManageEmployeeLogins && IsEditMode));
            }
        }

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
                var uploadFilesTask = changeLangTask.ContinueWith((t) =>
                {
                    if (t.Exception != null)
                        throw t.Exception;

                    ssc.ChangeLanguage(CultureInfo.CurrentCulture.TwoLetterISOLanguageName);

                    //After language change upload files
                    var items = filePaths
                        .Select(fp => new { FilePath = fp, Stream = new System.IO.FileStream(fp, System.IO.FileMode.Open) })
                        .ToArray();

                    var tasks = items.Select(i => 
                    
                        fsc.PutAsync(i.Stream)
                            .ContinueWith(t2 =>
                            {
                                try
                                {
                                    if (t2.Exception != null)
                                        throw t2.Exception;

                                    if (!string.IsNullOrWhiteSpace(t2.Result.Error))
                                        throw new Exception(t2.Result.Error);

                                    var file = t2.Result.Value;
                                    file.Name = System.IO.Path.GetFileName(i.FilePath);

                                    var fileUpdateResult = fsc.Update(file);
                                    if (!string.IsNullOrWhiteSpace(fileUpdateResult.Error))
                                        throw new Exception(fileUpdateResult.Error);

                                    file = fileUpdateResult.Value;

                                    var fileToPicturesResult = fsc.FileToPictures(file.Id);
                                    if (fileToPicturesResult.Error != null)
                                        throw new Exception(fileToPicturesResult.Error);

                                    var photos = fileToPicturesResult
                                        .Values
                                        .Select(p => new EmployeePhoto()
                                        {
                                            EmployeeId = emp.Id,
                                            FileId = p.FileId,
                                            Picture = AutoMapper.Mapper.Map<StaffingService.Picture>(p),
                                        }).ToArray();

                                    var addPhotosResult = ssc.EmployeePhotosAdd(emp.Id, photos);
                                    if (addPhotosResult.Error != null)
                                        throw new Exception(addPhotosResult.Error);

                                    return addPhotosResult.Values;
                                }
                                finally
                                {
                                    try { i.Stream.Dispose(); } catch { }
                                }
                            }, CancellationToken.None, TaskContinuationOptions.AttachedToParent, TaskScheduler.Current)
                    ).ToArray();

                    Task.WaitAll(tasks);

                    return tasks
                        .Where(tsk => tsk.Exception == null)
                        .SelectMany(tsk => tsk.Result)
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
                        emp.Photos = emp.Photos
                            .FullOuterJoin(t.Result, i => i.FileId, i => i.FileId, (p1, p2) => p1 ?? p2)
                            .ToArray();
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
                var canMangeLogins = Owner.CanManageEmployeeLogins;
                var canMangeRights = Owner.CanManageEmployeeRights;

                var task = Task.Factory.StartNew(() => {
                    var sc = new StaffingServiceClient();
                    try
                    { 
                        var updateRes = Employee.Id == 0
                            ? sc.EmployeeInsert(employeeToSave)
                            : sc.EmployeeUpdate(employeeToSave);

                        if (!string.IsNullOrWhiteSpace(updateRes.Error))
                        {
                            throw new Exception(updateRes.Error);
                        }
                        else
                        {
                            //Error = null;
                            this.Employee.CopyObjectFrom(updateRes.Value);
                            //IsEmpty = false;

                            if (canMangeLogins)
                            {
                                var updateLoginsRes = sc.EmployeeLoginsUpdate(employeeToSave.Id, employeeToSave.Logins.Select(r => r.Login).ToArray());
                                if (updateLoginsRes.Error != null)
                                {
                                    throw new Exception(updateLoginsRes.Error);
                                }
                                else
                                {
                                    this.Employee.Logins = updateLoginsRes.Values;
                                }
                            }

                            if (canMangeRights)
                            {
                                var updateRightsRes = sc.EmployeeRightsUpdate(employeeToSave.Id, employeeToSave.Rights.Select(r => r.RightId).ToArray());
                                if (updateRightsRes.Error != null)
                                {
                                    throw new Exception(updateRightsRes.Error);
                                } else
                                {
                                    this.Employee.Rights = updateRightsRes.Values;
                                }
                            }

                            //if (string.IsNullOrWhiteSpace(Error))
                            //    IsEditMode = false;
                        }
                    }
                    finally
                    {
                        try { sc.Close(); } catch { }
                        //IsBusy = false;
                    }
                });

                await task;

                IsEmpty = Employee.Id == 0;

                if (task.Exception != null)
                    throw task.Exception;

                Error = null;
                IsBusy = false;
                IsEditMode = false;
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
