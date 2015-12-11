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
                        var widthRatio = (double)Math.Max(t.Size.Height, t.Size.Width) / (double)originalImage.Width;
                        var heightRatio = (double)Math.Max(t.Size.Height, t.Size.Width) / (double)originalImage.Height;

                        var newWidth = (int)(t.Size.Width * widthRatio);
                        var newHeight = (int)(t.Size.Height * heightRatio);

                        var bitmap = new Bitmap(newWidth, newHeight);
                        using (var newPictureStream = new MemoryStream())
                        {
                            bitmap.Save(newPictureStream, originalImage.RawFormat);
                            newPictureStream.Seek(0, SeekOrigin.Begin);

                            var dbFile = repository.FilePut(storage, newPictureStream, t.PictureType.ToString() + fromFile.FileName);
                            var picture = repository.New<Repository.Model.Picture>();
                            picture.File = dbFile;
                            picture.Height = newHeight;
                            picture.Width = newWidth;
                            picture.PictureType = (Repository.Model.PictureType)(int)t.PictureType;

                            return AutoMapper.Mapper.Map<Model.Picture>(picture);
                        }
                    });
                    return pictures;
                }
            return Enumerable.Empty<Model.Picture>();
        }

        private static Bitmap ResizeBitmap(Bitmap image, int width, int height)
        {
            Bitmap resizedImage = new Bitmap(width, height);
            using (var gfx = Graphics.FromImage(resizedImage))
                gfx.DrawImage(image, 
                    new Rectangle(0, 0, width, height), 
                    new Rectangle(0, 0, image.Width, image.Height),
                    GraphicsUnit.Pixel);
            return resizedImage;
        }
    }
}
