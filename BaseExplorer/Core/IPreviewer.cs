using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseExplorer.Core
{
    interface IPreviewer
    {
        string Name { get; }
        string[] SupportExtensions { get; }
        bool CanDir { get; }
        PreviewData GetPreview(string realPath, string displayPath);
    }
}
