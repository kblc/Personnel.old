using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Personnel.Repository.Additional;
using System.ComponentModel;
using Helpers.Linq;

namespace Personnel.Repository.Model
{
    /// <summary>
    /// Тип права пользователя
    /// </summary>
    public enum RightType
    {
        [ResourceDescription("MODEL_RIGHTTYPE_Login")]
        Login,
        [ResourceDescription("MODEL_RIGHTTYPE_ManageEmployeesRights")]
        ManageEmployeesRights,
        [ResourceDescription("MODEL_RIGHTTYPE_ManageEmployeesLogins")]
        ManageEmployeesLogins,
        [ResourceDescription("MODEL_RIGHTTYPE_ViewStaffing")]
        ViewStaffing,
        [ResourceDescription("MODEL_RIGHTTYPE_ManageStaffing")]
        ManageStaffing,
        [ResourceDescription("MODEL_RIGHTTYPE_ManageDepartments")]
        ManageDepartments,
        [ResourceDescription("MODEL_RIGHTTYPE_ManageAppoints")]
        ManageAppoints,
        [ResourceDescription("MODEL_RIGHTTYPE_ViewEmployes")]
        ViewEmployes,
        [ResourceDescription("MODEL_RIGHTTYPE_ManageEmployes")]
        ManageEmployes,
        [ResourceDescription("MODEL_RIGHTTYPE_ViewChanges")]
        ViewChanges,
        [ResourceDescription("MODEL_RIGHTTYPE_UploadFile")]
        UploadFile,
        [ResourceDescription("MODEL_RIGHTTYPE_ManageFile")]
        ManageFile,
    }

    public partial class RepositoryContext
    {
        /// <summary>
        /// Хранилище прав
        /// </summary>
        public DbSet<Right> Rights { get; set; }
    }

    /// <summary>
    /// Право
    /// </summary>
    [Table("right")]
    public partial class Right : HistoryAbstractBase<long, Right>, IDefaultRepositoryInitialization
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("right_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RightId { get; set; }

        /// <summary>
        /// Системное имя права, по которому можно подгрузить из ресурсов полное имя
        /// </summary>
        [Column("system_name"), Index("UIX_RIGHT_SYSTEMNAME", IsUnique = true)]
        [Required(ErrorMessageResourceName = "MODEL_RIGHT_SystemNameRequred")]
        [MaxLength(100, ErrorMessageResourceName = "MODEL_RIGHT_SystemNameMaxLength")]
        [Obsolete("User Type property instead")]
        public string SystemName { get; set; }

        /// <summary>
        /// Тип права из существующих
        /// </summary>
        [NotMapped]
        public RightType Type
        {
#pragma warning disable 618
            get { return Extensions.GetEnumValueByName<RightType>(SystemName); }
            set { SystemName = value.ToString().ToUpper(); }
#pragma warning restore 618
        }

        /// <summary>
        /// Название права
        /// </summary>
        [NotMapped]
        public string Name => Type.GetResourceDescription();

        #region Initialization

        void IDefaultRepositoryInitialization.InitializeDefault(RepositoryContext context)
        {
            var fullRights = typeof(RightType)
                .GetEnumValues()
                .Cast<RightType>()
                .Select(t => new Right() { Type = t });

#pragma warning disable 618
            var allRights = fullRights
                    .FullOuterJoin(context.Rights, ct => ct.SystemName, c => c.SystemName, (def, existed) => new { Default = def, Existed = existed })
                    .ToArray();

            context.Rights.AddRange(allRights.Where(i => i.Existed == null).Select(i => i.Default));
            context.Rights.RemoveRange(allRights.Where(i => i.Default == null).Select(i => i.Existed));
#pragma warning restore 618
        }

        #endregion
        #region ToString()

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }

        #endregion
    }
}
