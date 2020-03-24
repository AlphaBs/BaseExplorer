using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BaseExplorer.Core
{
    public class PreviewData
    {
        public PreviewData(string name, BitmapImage img, string path, string dpath)
        {
            this.Name = name;
            this.PreviewImage = img;
            this.Path = path;
            this.DisplayPath = dpath;
        }

        public string Name { get; private set; }
        public BitmapImage PreviewImage { get; private set; }
        public string Path { get;private set; }
        public string DisplayPath { get; private set; }
    }
}
