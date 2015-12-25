using FileStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using Personnel.Services.Additional;
using System.IO;
using System.Drawing;

namespace Personnel.Services.Service.File.Additional
{
    public static class FileResizer
    {
        private static ImageSizeAttribute GetSize(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (Attribute.GetCustomAttribute(field, typeof(ImageSizeAttribute)) as ImageSizeAttribute);
            return attribute;
        }

        public static IEnumerable<Model.Picture> RersizeTo(Repository.Model.File fromFile,
            Repository.Logic.Repository repository,
            IFileStorage storage,
            Model.PictureType[] toType)
        {
            if (fromFile == null)
                throw new ArgumentNullException(nameof(fromFile));
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            toType = toType
                ?? typeof(Model.PictureType)
                .GetEnumValues()
                .Cast<Model.PictureType>()
                .ToArray();

            var toSizes = toType
                .Select(i => new { PictureType = i, Size = GetSize(i) })
                .Where(i => i.Size != null)
                .ToArray();

            if (toSizes.Any())
                using (var fileStream = storage.FileGet(fromFile.FileId))
                using (var originalImage = Bitmap.FromStream(fileStream))
                {
                    var pictures = toSizes.Select(t =>
                    {
                        var newWidth = (t.Size.Width == 0) ? originalImage.Width : t.Size.Width;
                        var newHeight = (t.Size.Height == 0) ? originalImage.Height : t.Size.Height;

                        using (var resizedbitmap = ResizeBitmap(originalImage, newWidth, newHeight))
                        using (var newPictureStream = new MemoryStream())
                        {
                            resizedbitmap.Save(newPictureStream, originalImage.RawFormat);
                            newPictureStream.Seek(0, SeekOrigin.Begin);

                            var dbFile = repository.FilePut(storage, newPictureStream, t.PictureType.ToString() + fromFile.FileName);
                            var picture = repository.New<Repository.Model.Picture>();
                            picture.File = dbFile;
                            picture.FileId = dbFile.FileId;
                            picture.Height = newWidth;
                            picture.Width = newHeight;
                            picture.PictureType = (Repository.Model.PictureType)(int)t.PictureType;
                            repository.Add(picture);
                            return AutoMapper.Mapper.Map<Model.Picture>(picture);
                        }
                    }).ToArray();
                    return pictures;
                }
            return Enumerable.Empty<Model.Picture>();
        }

        private static Bitmap ResizeBitmap(Image image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (var gfx = Graphics.FromImage(resizedImage))
            {
                gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gfx.DrawImage(image,
                    new Rectangle(0, 0, width, height),
                    new Rectangle(0, 0, image.Width, image.Height),
                    GraphicsUnit.Pixel);
            }
            return resizedImage;
        }
    }
}
