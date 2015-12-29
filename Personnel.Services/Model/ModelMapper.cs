using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Personnel.Repository.Model;

namespace Personnel.Services.Model
{
    public class StringToLongConverter : ValueResolver<string, long>
    {
        protected override long ResolveCore(string source)
        {
            long res;
            if (!long.TryParse(source, out res))
                res = 0;
            return res;
        }
    }

    public class LongToStringConverter : ValueResolver<long, string>
    {
        protected override string ResolveCore(long source)
        {
            if (source == 0)
                return null;
            else
                return source.ToString();
        }
    }

    public class StringToNullableLongConverter : ValueResolver<string, long?>
    {
        protected override long? ResolveCore(string source)
        {
            long res;
            if (!long.TryParse(source, out res))
                return null;
            return res;
        }
    }

    public class NullableLongToStringConverter : ValueResolver<long?, string>
    {
        protected override string ResolveCore(long? source)
        {
            return (!source.HasValue || source.Value == 0) ? null : source.Value.ToString();
        }
    }

    public class AddUrlPrefixConverter : ValueResolver<string, string>
    {
        private static Configuration config = new Configuration();

        public static string AddUrlPrefix(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;
            try
            {
                var fileName = source;
                if (config.FileServiceUrlPrefix != null)
                    fileName = config.FileServiceUrlPrefix.AbsoluteUri + fileName;
                return fileName;
            }
            catch { return null; }
        }

        protected override string ResolveCore(string source)
        {
            return AddUrlPrefix(System.IO.Path.GetFileName(source));
        }
    }

    public static class ModelMapper
    {
        private static bool isInited = false;

        public static void ModelMapperInit()
        {
            if (isInited)
                return;

            #region Department

            AutoMapper.Mapper.CreateMap<Repository.Model.Department, Model.Department>()
                .ForMember(dst => dst.Id, e => e.MapFrom(a => a.DepartmentId))
                .ForMember(dst => dst.ParentId, e => e.MapFrom(a => a.ParentDepartmentId));
            AutoMapper.Mapper.CreateMap<Model.Department, Repository.Model.Department>()
                .ForMember(dst => dst.DepartmentId, e => e.MapFrom(a => a.Id))
                .ForMember(dst => dst.ParentDepartmentId, e => e.MapFrom(a => a.ParentId));

            #endregion
            #region Right

            AutoMapper.Mapper.CreateMap<Repository.Model.Right, Model.Right>()
                .ForMember(dst => dst.Id, e => e.MapFrom(a => a.RightId));
            AutoMapper.Mapper.CreateMap<Model.Right, Repository.Model.Right>()
                .ForMember(dst => dst.RightId, e => e.MapFrom(a => a.Id));

            #endregion
            #region EmployeePhoto

            AutoMapper.Mapper.CreateMap<Repository.Model.EmployeePhoto, Model.EmployeePhoto>();
            AutoMapper.Mapper.CreateMap<Model.EmployeePhoto, Repository.Model.EmployeePhoto>();

            #endregion
            #region EmployeeRight

            AutoMapper.Mapper.CreateMap<Repository.Model.EmployeeRight, Model.EmployeeRight>()
                .ForMember(dst => dst.Id, e => e.MapFrom(src => src.EmployeeRightId));

            AutoMapper.Mapper.CreateMap<Model.EmployeeRight, Repository.Model.EmployeeRight>()
                .ForMember(dst => dst.EmployeeRightId, e => e.MapFrom(src => src.Id));

            #endregion
            #region EmployeeLogin

            AutoMapper.Mapper.CreateMap<Repository.Model.EmployeeLogin, Model.EmployeeLogin>()
                .ForMember(dst => dst.Id, e => e.MapFrom(src => src.EmployeeLoginId))
                .ForMember(dst => dst.Login, e => e.MapFrom(src => src.DomainLogin));
            AutoMapper.Mapper.CreateMap<Model.EmployeeLogin, Repository.Model.EmployeeLogin>()
                .ForMember(dst => dst.EmployeeLoginId, e => e.MapFrom(src => src.Id))
                .ForMember(dst => dst.DomainLogin, e => e.MapFrom(src => src.Login));

            #endregion
            #region Staffing

            AutoMapper.Mapper.CreateMap<Repository.Model.Staffing, Model.Staffing>()
                .ForMember(dst => dst.Id, e => e.MapFrom(a => a.StaffingId));
            AutoMapper.Mapper.CreateMap<Model.Staffing, Repository.Model.Staffing>()
                .ForMember(dst => dst.StaffingId, e => e.MapFrom(a => a.Id));

            #endregion
            #region Picture

            AutoMapper.Mapper.CreateMap<Repository.Model.Picture, Model.Picture>()
                .ForMember(dst => dst.Description, e => e.MapFrom(a => a.PictureTypeName));

            AutoMapper.Mapper.CreateMap<Model.Picture, Repository.Model.Picture>()
                .ForMember(dst => dst.File, e => e.Ignore());

            #endregion
            #region File

            AutoMapper.Mapper.CreateMap<Repository.Model.File, Model.File>()
                .ForMember(dst => dst.Id, e => e.MapFrom(a => a.FileId))
                .ForMember(dst => dst.Name, e => e.MapFrom(a => a.FileName))
                .ForMember(dst => dst.Size, e => e.MapFrom(a => a.FileSize))
                .ForMember(dst => dst.Mime, e => e.MapFrom(a => a.MimeType))
                .AfterMap((src, dst) =>
                {
                    dst.Link = AddUrlPrefixConverter.AddUrlPrefix(src.FileId.ToString());
                    var mimeInfo = FileStorage.MimeStorage.GetPreviewImagesForMimeType(dst.Mime);
                    if (mimeInfo != null)
                    {
                        dst.Preview = AddUrlPrefixConverter.AddUrlPrefix(mimeInfo.Big);
                        dst.PreviewSmall = AddUrlPrefixConverter.AddUrlPrefix(mimeInfo.Small);
                    }
                });

            AutoMapper.Mapper.CreateMap<Model.File, Repository.Model.File>()
                .ForMember(dst => dst.FileId, a => a.MapFrom(src => src.Id))
                .ForMember(dst => dst.FileName, e => e.MapFrom(a => a.Name))
                .ForMember(dst => dst.FileSize, e => e.MapFrom(a => a.Size))
                .ForMember(dst => dst.MimeType, e => e.MapFrom(a => a.Mime));

            #endregion
            #region Employee

            AutoMapper.Mapper.CreateMap<Repository.Model.Employee, Model.Employee>()
                .ForMember(dst => dst.Id, e => e.MapFrom(a => a.EmployeeId));

            AutoMapper.Mapper.CreateMap<Model.Employee, Repository.Model.Employee>()
                .ForMember(dst => dst.EmployeeId, e => e.MapFrom(a => a.Id));

            #endregion

            #region VacationLevel

            AutoMapper.Mapper.CreateMap<Repository.Model.VacationLevel, Model.VacationLevel>()
                .ForMember(dst => dst.Id, e => e.MapFrom(a => a.VacationLevelId));

            AutoMapper.Mapper.CreateMap<Model.VacationLevel, Repository.Model.VacationLevel>()
                .ForMember(dst => dst.VacationLevelId, e => e.MapFrom(a => a.Id));

            #endregion
            #region Vacation

            AutoMapper.Mapper.CreateMap<Repository.Model.Vacation, Model.Vacation>()
                .ForMember(dst => dst.Id, e => e.MapFrom(a => a.VacationId));

            AutoMapper.Mapper.CreateMap<Model.Vacation, Repository.Model.Vacation>()
                .ForMember(dst => dst.VacationId, e => e.MapFrom(a => a.Id));

            #endregion

            isInited = true;
        }
    }
}
