using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personnel.Services.Additional
{
    /// <summary>
    /// Image size attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ImageSizeAttribute : Attribute
    {
        public int Width { get; private set; } = 0;
        public int Height { get; private set; } = 0;

        //public bool Fit { get; set; } = false;

        public ImageSizeAttribute() { }
        public ImageSizeAttribute(int width, int height)//, bool fit = false)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));
            Width = width;
            Height = height;
            //Fit = fit;
        }
    }
}
