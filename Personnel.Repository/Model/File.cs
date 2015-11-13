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
        /// Хранилище файлов
        /// </summary>
        public DbSet<File> Files { get; set; }
    }

    /// <summary>
    /// Файл
    /// </summary>
    [Table("file")]
    public partial class File : HistoryAbstractBase<Guid, File>
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("file_uid")]
        public Guid FileId { get; set; }

        /// <summary>
        /// Имя файла
        /// </summary>
        [Column("file_name"), Required]
        public string FileName { get; set; }

        /// <summary>
        /// Имя файла (уникальное)
        /// </summary>
        [Column("unique_file_name"), Required]
        public string UniqueFileName { get; set; }

        /// <summary>
        /// Размер файла
        /// </summary>
        [Column("file_size"), Required]
        public long FileSize { get; set; }

        /// <summary>
        /// Mime-тип файла
        /// </summary>
        [Column("mime"), Required]
        public string MimeType { get; set; }

        /// <summary>
        /// Дата создания записи
        /// </summary>
        [Column("date"), Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// Название кодировки
        /// </summary>
        [Column("encoding_name"), MaxLength(100)]
        [Obsolete("Use Encoding property instead")]
        public string EncodingName { get; set; }

        /// <summary>
        /// Кодировка файла
        /// </summary>
        [NotMapped]
        public Encoding Encoding
        {
#pragma warning disable 618
            get { return Extensions.GetEncodingFromName(EncodingName); }
            set { EncodingName = (value ?? Encoding.Default).WebName; }
#pragma warning restore 618
        }
    }
}
