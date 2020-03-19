using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BaseExplorer.Core
{
    public class Base64Viewer : IViewer
    {
        Encoding DefaultEncoding = Encoding.UTF8;
        const string EncodeKey = "_b6.";
        const string EncodeDirKey = "_b6__";

        const char invalidChar = '/';
        const char replaceChar = '$';

        public bool CheckIsEncodedName(string filename)
        {
            return CheckIsEncodedName(filename, false) || CheckIsEncodedName(filename, true);
        }

        public bool CheckIsEncodedName(string filename, bool isdir)
        {
            if (isdir)
                return filename.StartsWith(EncodeDirKey);
            else
                return filename.StartsWith(EncodeKey);
        }

        public string GetDecodedName(string encodedName, bool isdir)
        {
            if (isdir)
                return baseDec(encodedName.Substring(EncodeDirKey.Length));
            else
                return baseDec(encodedName.Substring(EncodeKey.Length));
        }

        public string GetEncodedName(string decodedName, bool isdir)
        {
            if (isdir)
                return EncodeDirKey + baseEnc(decodedName);
            else
                return EncodeKey + baseEnc(decodedName);
        }

        public void DecodeFile(string parent, string name)
        {
            var newpath = Path.Combine(parent, GetDecodedName(name, false));
            File.Move(Path.Combine(parent, name) , newpath);
        }

        public void DecodeFile(string filepath)
        {
            var filename = Path.GetFileName(filepath);
            var parentdir = Path.GetDirectoryName(filepath);

            DecodeFile(parentdir, filename);
        }

        public void EncodeFile(string parent, string name)
        {
            var newpath = Path.Combine(parent, GetEncodedName(name, false));
            File.Move(Path.Combine(parent, name), newpath);
        }

        public void EncodeFile(string filepath)
        {
            var filename = Path.GetFileName(filepath);
            var parentdir = Path.GetDirectoryName(filepath);

            EncodeFile(parentdir, filename);
        }

        public void DecodeDir(string parent, string name)
        {
            var dirpath = Path.Combine(parent, name);
            var newpath = Path.Combine(parent, GetDecodedName(name, true));

            Directory.Move(dirpath, newpath);
        }

        public void DecodeDir(string dirpath)
        {
            var d = DivideDir(dirpath);
            DecodeDir(d.Item1, d.Item2);
        }

        public void EncodeDir(string parent, string name)
        {
            var dirpath = Path.Combine(parent, name);
            var newpath = Path.Combine(parent, GetEncodedName(name, true));

            Directory.Move(dirpath, newpath);
        }

        public void EncodeDir(string dirpath)
        {
            var d = DivideDir(dirpath);
            EncodeDir(d.Item1, d.Item2);
        }

        string baseEnc(string input)
        {
            return Convert.ToBase64String(DefaultEncoding.GetBytes(input)).Replace(invalidChar, replaceChar);
        }

        string baseDec(string input)
        {
            return DefaultEncoding.GetString(Convert.FromBase64String(input.Replace(replaceChar, invalidChar)));
        }

        Tuple<string, string> DivideDir(string dirpath)
        {
            var dirIndex = dirpath.LastIndexOf(Path.DirectorySeparatorChar);

            var parent = dirpath.Substring(0, dirIndex);
            var name = dirpath.Substring(dirIndex + 1);

            return new Tuple<string, string>(parent, name);
        }
    }
}
