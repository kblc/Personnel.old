using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Personnel.Repository.Additional;

namespace Personnel.Repository.Model
{
    public partial class RepositoryContext
    {
        /// <summary>
        /// Хранилище фотографий
        /// </summary>
        public DbSet<Picture> Pictures { get; set; }
    }

    /// <summary>
    /// Тип фотографии
    /// </summary>
    public enum PictureType
    {
        [ResourceDescription("MODEL_PICTURETYPE_None")]
        None = 0,
        [ResourceDescription("MODEL_PICTURETYPE_Avatar32")]
        Avatar32,
        [ResourceDescription("MODEL_PICTURETYPE_Avatar64")]
        Avatar64,
        [ResourceDescription("MODEL_PICTURETYPE_Avatar128")]
        Avatar128,
        [ResourceDescription("MODEL_PICTURETYPE_Avatar256")]
        Avatar256,
    }

    /// <summary>
    /// Фотография
    /// </summary>
    [Table("picture")]
    public partial class Picture : HistoryAbstractBase<Guid, Picture>
    {
        #region File
        /// <summary>
        /// Идентификатор сотрудника, которому принадлежат логины
        /// </summary>
        [Key, ForeignKey("File"), Column("file_uid"), Required]
        public Guid FileId { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные
        /// </summary>
        public virtual File File { get; set; }
        #endregion
        /// <summary>
        /// Picture type name
        /// </summary>
        [Column("picture_type"), Required, MaxLength(20)]
        [Obsolete("Use PictureType property instead")]
        public string PictureSystemTypeName { get; set; }

        /// <summary>
        /// Picture type name
        /// </summary>
        [NotMapped]
        public string PictureTypeName => PictureType.GetResourceDescription();

        /// <summary>
        /// Picture type
        /// </summary>
        [NotMapped]
        public PictureType PictureType
        {
#pragma warning disable 618
            get { return Extensions.GetEnumValueByName<PictureType>(PictureSystemTypeName); }
            set { PictureSystemTypeName = value.ToString().ToUpper(); }
#pragma warning restore 618
        }

        /// <summary>
        /// Picture width
        /// </summary>
        [Column("width"), Required]
        public int Width { get; set; }

        /// <summary>
        /// Picture height
        /// </summary>
        [Column("height"), Required]
        public int Height { get; set; }
    }
}
