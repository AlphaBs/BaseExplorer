using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseExplorer.Core
{
    interface IViewer
    {
        string GetEncodedName(string decodedName, bool isdir);
        string GetDecodedName(string encodedName, bool isdir);

        bool CheckIsEncodedName(string filename);
        bool CheckIsEncodedName(string filename, bool isdir);

        void EncodeFile(string parent, string name);
        void EncodeFile(string filepath);
        void DecodeFile(string parent, string name);
        void DecodeFile(string filepath);

        void EncodeDir(string parent, string name);
        void EncodeDir(string dirpath);
        void DecodeDir(string parent, string name);
        void DecodeDir(string dirpath);
    }
}
