using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;

namespace BaseExplorer.Core
{
    class ImagePreviewer : IPreviewer
    {
        public string Name => "Image";

        public string[] SupportExtensions => new string[]
            {
                ".jpeg",
                ".jpg",
                ".png",
                ".bmp",
                ".ico"
            };

        public bool CanDir => false;

        const int MaxPixel = 100;

        public PreviewData GetPreview(string realPath, string displayPath)
        {
            BitmapImage img = new BitmapImage();
            using (var fs = new FileStream(realPath, FileMode.Open))
            {
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = fs;
                img.DecodePixelWidth = MaxPixel;
                img.EndInit();
                img.Freeze();
            }

            return new PreviewData(img.Format.ToString(), img, realPath, displayPath);
        }
    }
}
