using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Personnel.Repository.Additional;
using System.ComponentModel;
using Helpers.Linq;
using System.Data.Entity;

namespace Personnel.Repository.Model
{
    /// <summary>
    /// Уровень отпуска
    /// </summary>
    public enum VacationLevelKind
    {
        /// <summary>
        /// Плнируемый отпуск
        /// </summary>
        [ResourceDescription("MODEL_VACATIONLEVELKIND_Plan")]
        Plan,
        /// <summary>
        /// Перенос отпуска
        /// </summary>
        [ResourceDescription("MODEL_VACATIONLEVELKIND_Transfer")]
        Transfer,
        /// <summary>
        /// Фактический отпуск
        /// </summary>
        [ResourceDescription("MODEL_VACATIONLEVELKIND_Fact")]
        Fact,
    }

    public partial class RepositoryContext
    {
        /// <summary>
        /// Хранилище уровней отпусков
        /// </summary>
        public DbSet<VacationLevel> VacationLevels { get; set; }
    }

    /// <summary>
    /// Уровень отпусков
    /// </summary>
    [Table("vacation_level")]
    public class VacationLevel : HistoryAbstractBase<long, VacationLevel>, IDefaultRepositoryInitialization
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("vacation_level_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VacationLevelId { get; set; }

        /// <summary>
        /// Системное имя уровня отпуска, по которому можно подгрузить из ресурсов полное имя
        /// </summary>
        [Column("system_name"), Index("UIX_VOCATION_LEVEL_TYPE_SYSTEMNAME", IsUnique = true)]
        [Required(ErrorMessageResourceName = "MODEL_VOCATION_LEVEL_TYPE_SystemNameRequred")]
        [MaxLength(100, ErrorMessageResourceName = "MODEL_VOCATION_LEVEL_TYPE_SystemNameMaxLength")]
        [Obsolete("Use Type property instead")]
        public string SystemName { get; set; }

        /// <summary>
        /// Тип уровня отпуска
        /// </summary>
        [NotMapped]
        public VacationLevelKind Type
        {
#pragma warning disable 618
            get { return Extensions.GetEnumValueByName<VacationLevelKind>(SystemName); }
            set { SystemName = value.ToString().ToUpper(); }
#pragma warning restore 618
        }

        /// <summary>
        /// Название типа отпуска
        /// </summary>
        [NotMapped]
        public string Name => Type.GetResourceDescription();

        #region Initialization

        void IDefaultRepositoryInitialization.InitializeDefault(RepositoryContext context)
        {
            var fullVacationTypes = typeof(VacationLevelKind)
                .GetEnumValues()
                .Cast<VacationLevelKind>()
                .Select(t => new VacationLevel() { Type = t });

#pragma warning disable 618
            var allVacationTypes = fullVacationTypes
                    .FullOuterJoin(context.VacationLevels, ct => ct.SystemName, c => c.SystemName, (def, existed) => new { Default = def, Existed = existed })
                    .ToArray();

            context.VacationLevels.AddRange(allVacationTypes.Where(i => i.Existed == null).Select(i => i.Default));
            context.VacationLevels.RemoveRange(allVacationTypes.Where(i => i.Default == null).Select(i => i.Existed));
#pragma warning restore 618
        }

        #endregion
    }
}
