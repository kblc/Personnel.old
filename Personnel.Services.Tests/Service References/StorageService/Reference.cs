﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Personnel.Services.Tests.StorageService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Result", Namespace="http://schemas.datacontract.org/2004/07/Personnel.Services.Model")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Personnel.Services.Tests.StorageService.BaseExecutionResultsOfFile3XPrIsxh))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Personnel.Services.Tests.StorageService.FileResults))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Personnel.Services.Tests.StorageService.BaseExecutionResultOfFile3XPrIsxh))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Personnel.Services.Tests.StorageService.FileResult))]
    public partial class Result : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Error {
            get {
                return this.ErrorField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorField, value) != true)) {
                    this.ErrorField = value;
                    this.RaisePropertyChanged("Error");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="BaseExecutionResultsOfFile3XPrIsxh", Namespace="http://schemas.datacontract.org/2004/07/Personnel.Services.Model")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Personnel.Services.Tests.StorageService.FileResults))]
    public partial class BaseExecutionResultsOfFile3XPrIsxh : Personnel.Services.Tests.StorageService.Result {
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Personnel.Services.Tests.StorageService.File[] ValuesField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Personnel.Services.Tests.StorageService.File[] Values {
            get {
                return this.ValuesField;
            }
            set {
                if ((object.ReferenceEquals(this.ValuesField, value) != true)) {
                    this.ValuesField = value;
                    this.RaisePropertyChanged("Values");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FileResults", Namespace="http://schemas.datacontract.org/2004/07/Personnel.Services.Service.File")]
    [System.SerializableAttribute()]
    public partial class FileResults : Personnel.Services.Tests.StorageService.BaseExecutionResultsOfFile3XPrIsxh {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="BaseExecutionResultOfFile3XPrIsxh", Namespace="http://schemas.datacontract.org/2004/07/Personnel.Services.Model")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Personnel.Services.Tests.StorageService.FileResult))]
    public partial class BaseExecutionResultOfFile3XPrIsxh : Personnel.Services.Tests.StorageService.Result {
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private Personnel.Services.Tests.StorageService.File ValueField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Personnel.Services.Tests.StorageService.File Value {
            get {
                return this.ValueField;
            }
            set {
                if ((object.ReferenceEquals(this.ValueField, value) != true)) {
                    this.ValueField = value;
                    this.RaisePropertyChanged("Value");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FileResult", Namespace="http://schemas.datacontract.org/2004/07/Personnel.Services.Service.File")]
    [System.SerializableAttribute()]
    public partial class FileResult : Personnel.Services.Tests.StorageService.BaseExecutionResultOfFile3XPrIsxh {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="File", Namespace="http://schemas.datacontract.org/2004/07/Personnel.Services.Model")]
    [System.SerializableAttribute()]
    public partial class File : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<System.DateTime> DateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EncodingField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<System.Guid> IdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MimeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PreviewField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PreviewSmallField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Nullable<long> SizeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.DateTime> Date {
            get {
                return this.DateField;
            }
            set {
                if ((this.DateField.Equals(value) != true)) {
                    this.DateField = value;
                    this.RaisePropertyChanged("Date");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Encoding {
            get {
                return this.EncodingField;
            }
            set {
                if ((object.ReferenceEquals(this.EncodingField, value) != true)) {
                    this.EncodingField = value;
                    this.RaisePropertyChanged("Encoding");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<System.Guid> Id {
            get {
                return this.IdField;
            }
            set {
                if ((this.IdField.Equals(value) != true)) {
                    this.IdField = value;
                    this.RaisePropertyChanged("Id");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Mime {
            get {
                return this.MimeField;
            }
            set {
                if ((object.ReferenceEquals(this.MimeField, value) != true)) {
                    this.MimeField = value;
                    this.RaisePropertyChanged("Mime");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name {
            get {
                return this.NameField;
            }
            set {
                if ((object.ReferenceEquals(this.NameField, value) != true)) {
                    this.NameField = value;
                    this.RaisePropertyChanged("Name");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Preview {
            get {
                return this.PreviewField;
            }
            set {
                if ((object.ReferenceEquals(this.PreviewField, value) != true)) {
                    this.PreviewField = value;
                    this.RaisePropertyChanged("Preview");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PreviewSmall {
            get {
                return this.PreviewSmallField;
            }
            set {
                if ((object.ReferenceEquals(this.PreviewSmallField, value) != true)) {
                    this.PreviewSmallField = value;
                    this.RaisePropertyChanged("PreviewSmall");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Nullable<long> Size {
            get {
                return this.SizeField;
            }
            set {
                if ((this.SizeField.Equals(value) != true)) {
                    this.SizeField = value;
                    this.RaisePropertyChanged("Size");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="StorageService.IFileService")]
    public interface IFileService {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IBaseService/ChangeLanguage")]
        void ChangeLanguage(string codename);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IBaseService/ChangeLanguage")]
        System.Threading.Tasks.Task ChangeLanguageAsync(string codename);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/Get", ReplyAction="http://tempuri.org/IFileService/GetResponse")]
        Personnel.Services.Tests.StorageService.FileResult Get(System.Guid identifier);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/Get", ReplyAction="http://tempuri.org/IFileService/GetResponse")]
        System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResult> GetAsync(System.Guid identifier);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IFileService/Remove")]
        void Remove(System.Guid identifier);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IFileService/Remove")]
        System.Threading.Tasks.Task RemoveAsync(System.Guid identifier);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/GetRange", ReplyAction="http://tempuri.org/IFileService/GetRangeResponse")]
        Personnel.Services.Tests.StorageService.FileResults GetRange(System.Guid[] identifiers);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/GetRange", ReplyAction="http://tempuri.org/IFileService/GetRangeResponse")]
        System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResults> GetRangeAsync(System.Guid[] identifiers);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/GetSourceByName", ReplyAction="http://tempuri.org/IFileService/GetSourceByNameResponse")]
        System.IO.Stream GetSourceByName(string fileName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/GetSourceByName", ReplyAction="http://tempuri.org/IFileService/GetSourceByNameResponse")]
        System.Threading.Tasks.Task<System.IO.Stream> GetSourceByNameAsync(string fileName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/GetSource", ReplyAction="http://tempuri.org/IFileService/GetSourceResponse")]
        System.IO.Stream GetSource(System.Guid identifier);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/GetSource", ReplyAction="http://tempuri.org/IFileService/GetSourceResponse")]
        System.Threading.Tasks.Task<System.IO.Stream> GetSourceAsync(System.Guid identifier);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/Put", ReplyAction="http://tempuri.org/IFileService/PutResponse")]
        Personnel.Services.Tests.StorageService.FileResult Put(System.IO.Stream content);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/Put", ReplyAction="http://tempuri.org/IFileService/PutResponse")]
        System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResult> PutAsync(System.IO.Stream content);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/Update", ReplyAction="http://tempuri.org/IFileService/UpdateResponse")]
        Personnel.Services.Tests.StorageService.FileResult Update(Personnel.Services.Tests.StorageService.File item);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileService/Update", ReplyAction="http://tempuri.org/IFileService/UpdateResponse")]
        System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResult> UpdateAsync(Personnel.Services.Tests.StorageService.File item);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IFileServiceChannel : Personnel.Services.Tests.StorageService.IFileService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FileServiceClient : System.ServiceModel.ClientBase<Personnel.Services.Tests.StorageService.IFileService>, Personnel.Services.Tests.StorageService.IFileService {
        
        public FileServiceClient() {
        }
        
        public FileServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public FileServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FileServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FileServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void ChangeLanguage(string codename) {
            base.Channel.ChangeLanguage(codename);
        }
        
        public System.Threading.Tasks.Task ChangeLanguageAsync(string codename) {
            return base.Channel.ChangeLanguageAsync(codename);
        }
        
        public Personnel.Services.Tests.StorageService.FileResult Get(System.Guid identifier) {
            return base.Channel.Get(identifier);
        }
        
        public System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResult> GetAsync(System.Guid identifier) {
            return base.Channel.GetAsync(identifier);
        }
        
        public void Remove(System.Guid identifier) {
            base.Channel.Remove(identifier);
        }
        
        public System.Threading.Tasks.Task RemoveAsync(System.Guid identifier) {
            return base.Channel.RemoveAsync(identifier);
        }
        
        public Personnel.Services.Tests.StorageService.FileResults GetRange(System.Guid[] identifiers) {
            return base.Channel.GetRange(identifiers);
        }
        
        public System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResults> GetRangeAsync(System.Guid[] identifiers) {
            return base.Channel.GetRangeAsync(identifiers);
        }
        
        public System.IO.Stream GetSourceByName(string fileName) {
            return base.Channel.GetSourceByName(fileName);
        }
        
        public System.Threading.Tasks.Task<System.IO.Stream> GetSourceByNameAsync(string fileName) {
            return base.Channel.GetSourceByNameAsync(fileName);
        }
        
        public System.IO.Stream GetSource(System.Guid identifier) {
            return base.Channel.GetSource(identifier);
        }
        
        public System.Threading.Tasks.Task<System.IO.Stream> GetSourceAsync(System.Guid identifier) {
            return base.Channel.GetSourceAsync(identifier);
        }
        
        public Personnel.Services.Tests.StorageService.FileResult Put(System.IO.Stream content) {
            return base.Channel.Put(content);
        }
        
        public System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResult> PutAsync(System.IO.Stream content) {
            return base.Channel.PutAsync(content);
        }
        
        public Personnel.Services.Tests.StorageService.FileResult Update(Personnel.Services.Tests.StorageService.File item) {
            return base.Channel.Update(item);
        }
        
        public System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResult> UpdateAsync(Personnel.Services.Tests.StorageService.File item) {
            return base.Channel.UpdateAsync(item);
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="StorageService.IFileServiceREST")]
    public interface IFileServiceREST {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IBaseService/ChangeLanguage")]
        void ChangeLanguage(string codename);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IBaseService/ChangeLanguage")]
        System.Threading.Tasks.Task ChangeLanguageAsync(string codename);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServiceREST/RESTGet", ReplyAction="http://tempuri.org/IFileServiceREST/RESTGetResponse")]
        Personnel.Services.Tests.StorageService.FileResult RESTGet(string identifier);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServiceREST/RESTGet", ReplyAction="http://tempuri.org/IFileServiceREST/RESTGetResponse")]
        System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResult> RESTGetAsync(string identifier);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IFileServiceREST/RESTRemove")]
        void RESTRemove(string identifier);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IFileServiceREST/RESTRemove")]
        System.Threading.Tasks.Task RESTRemoveAsync(string identifier);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServiceREST/RESTGetRange", ReplyAction="http://tempuri.org/IFileServiceREST/RESTGetRangeResponse")]
        Personnel.Services.Tests.StorageService.FileResults RESTGetRange(string[] identifiers);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServiceREST/RESTGetRange", ReplyAction="http://tempuri.org/IFileServiceREST/RESTGetRangeResponse")]
        System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResults> RESTGetRangeAsync(string[] identifiers);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServiceREST/RESTGetSource", ReplyAction="http://tempuri.org/IFileServiceREST/RESTGetSourceResponse")]
        System.IO.Stream RESTGetSource(string fileIdOrName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServiceREST/RESTGetSource", ReplyAction="http://tempuri.org/IFileServiceREST/RESTGetSourceResponse")]
        System.Threading.Tasks.Task<System.IO.Stream> RESTGetSourceAsync(string fileIdOrName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServiceREST/RESTPut", ReplyAction="http://tempuri.org/IFileServiceREST/RESTPutResponse")]
        Personnel.Services.Tests.StorageService.FileResult RESTPut(System.IO.Stream content);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServiceREST/RESTPut", ReplyAction="http://tempuri.org/IFileServiceREST/RESTPutResponse")]
        System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResult> RESTPutAsync(System.IO.Stream content);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServiceREST/RESTUpdate", ReplyAction="http://tempuri.org/IFileServiceREST/RESTUpdateResponse")]
        Personnel.Services.Tests.StorageService.FileResult RESTUpdate(Personnel.Services.Tests.StorageService.File item);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFileServiceREST/RESTUpdate", ReplyAction="http://tempuri.org/IFileServiceREST/RESTUpdateResponse")]
        System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResult> RESTUpdateAsync(Personnel.Services.Tests.StorageService.File item);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IFileServiceRESTChannel : Personnel.Services.Tests.StorageService.IFileServiceREST, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FileServiceRESTClient : System.ServiceModel.ClientBase<Personnel.Services.Tests.StorageService.IFileServiceREST>, Personnel.Services.Tests.StorageService.IFileServiceREST {
        
        public FileServiceRESTClient() {
        }
        
        public FileServiceRESTClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public FileServiceRESTClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FileServiceRESTClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FileServiceRESTClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void ChangeLanguage(string codename) {
            base.Channel.ChangeLanguage(codename);
        }
        
        public System.Threading.Tasks.Task ChangeLanguageAsync(string codename) {
            return base.Channel.ChangeLanguageAsync(codename);
        }
        
        public Personnel.Services.Tests.StorageService.FileResult RESTGet(string identifier) {
            return base.Channel.RESTGet(identifier);
        }
        
        public System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResult> RESTGetAsync(string identifier) {
            return base.Channel.RESTGetAsync(identifier);
        }
        
        public void RESTRemove(string identifier) {
            base.Channel.RESTRemove(identifier);
        }
        
        public System.Threading.Tasks.Task RESTRemoveAsync(string identifier) {
            return base.Channel.RESTRemoveAsync(identifier);
        }
        
        public Personnel.Services.Tests.StorageService.FileResults RESTGetRange(string[] identifiers) {
            return base.Channel.RESTGetRange(identifiers);
        }
        
        public System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResults> RESTGetRangeAsync(string[] identifiers) {
            return base.Channel.RESTGetRangeAsync(identifiers);
        }
        
        public System.IO.Stream RESTGetSource(string fileIdOrName) {
            return base.Channel.RESTGetSource(fileIdOrName);
        }
        
        public System.Threading.Tasks.Task<System.IO.Stream> RESTGetSourceAsync(string fileIdOrName) {
            return base.Channel.RESTGetSourceAsync(fileIdOrName);
        }
        
        public Personnel.Services.Tests.StorageService.FileResult RESTPut(System.IO.Stream content) {
            return base.Channel.RESTPut(content);
        }
        
        public System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResult> RESTPutAsync(System.IO.Stream content) {
            return base.Channel.RESTPutAsync(content);
        }
        
        public Personnel.Services.Tests.StorageService.FileResult RESTUpdate(Personnel.Services.Tests.StorageService.File item) {
            return base.Channel.RESTUpdate(item);
        }
        
        public System.Threading.Tasks.Task<Personnel.Services.Tests.StorageService.FileResult> RESTUpdateAsync(Personnel.Services.Tests.StorageService.File item) {
            return base.Channel.RESTUpdateAsync(item);
        }
    }
}
