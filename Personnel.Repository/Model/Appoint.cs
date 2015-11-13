using Personnel.Repository.Additional;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Linq;

namespace Personnel.Repository.Model
{
    public partial class RepositoryContext
    {
        /// <summary>
        /// Хранилище должностей
        /// </summary>
        public DbSet<Appoint> Appoints { get; set; }
    }

    /// <summary>
    /// Должность
    /// </summary>
    [Table("appoint")]
    public partial class Appoint : HistoryAbstractBase<long,Appoint>
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("appoint_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AppointId { get; set; }

        /// <summary>
        /// Название должности
        /// </summary>
        [Column("name"), Required(ErrorMessageResourceName = "MODEL_APPOINT_NameRequired")]
        [MaxLength(200, ErrorMessageResourceName = "MODEL_APPOINT_NameMaxLength")]
        public string Name { get; set; }
    }
}
