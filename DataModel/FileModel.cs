using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

namespace DataModels
{
    public enum FileType
    {
        Folder,
        File,
        Zip,
        Exe,
        Music,
        Video,
        Xml,
        Picture,
        Dll,
        Config,
        FixedRoot,
        NetworkRoot,
        RemovableRoot,
        DiscRoot,
        SysRoot,
        Computer
    }

    public class Category
    {
        FileType value;

        public Category(FileType type)
        {
            value = type;
        }

        public FileType Value
        {
            get { return value; }
        }

        public override bool Equals(object obj)
        {
            if (obj is Category)
                return value.Equals((obj as Category).value);
            return base.Equals(obj);
        }

        public override string ToString()
        {
            switch (value)
            {
                case FileType.Folder:
                    return "Folder";
                case FileType.File:
                    return "File";
                case FileType.Zip:
                    return "Zip";
                case FileType.Config:
                    return "Config";
                case FileType.Dll:
                    return "Dll";
                case FileType.Exe:
                    return "Exe";
                case FileType.Music:
                    return "Music";
                case FileType.Picture:
                    return "Picture";
                case FileType.Video:
                    return "Video";
                case FileType.Xml:
                    return "Xml";
                case FileType.FixedRoot:
                    return "FixedRoot";
                case FileType.SysRoot:
                    return "SysRoot";
                case FileType.NetworkRoot:
                    return "NetworkRoot";
                case FileType.DiscRoot:
                    return "DiscRoot";
                case FileType.RemovableRoot:
                    return "RemovableRoot";
                case FileType.Computer:
                    return "Computer";
                default:
                    return "File";
            }
        }
     }

    public class FileModel
    {
        public string Extension { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Accessed { get; set; }
        public string FullPath { get; set; }
        public Category Category { get; set; }
        public bool IsFolder {get; private set;}

        public string FullName { get; private set; }


        public string DirectoryName { get { return Path.GetDirectoryName(FullName); } }

        public string Size { get; private set; }

        public IList<FileModel> SubFolders { get; set; }

        public FileModel()
        {
            Name = "Computer";
            FullName = "\\";
            FullPath = Encode("\\");
            Category = new Category(FileType.Computer);
            SubFolders = GetRootDirectories();
            IsFolder = true;
        }

        public FileModel(FileInfo fi)
        {
            Name = fi.Name;
            Modified = fi.LastWriteTime;
            Created = fi.CreationTime;
            Accessed = fi.LastAccessTime;
            Extension = fi.Extension.ToLower();
            Location = fi.DirectoryName;
            FullName = fi.FullName;
            FullPath = Encode(fi.FullName);
            IsFolder = false;
            if (fi.Length > 1048576)
                Size = string.Format("{0:F2} MB", (float)fi.Length / 1048576);
            else
                Size = string.Format("{0:F2} KB", (float)fi.Length / 1024);
            switch (Extension)
            {
                case ".exe":
                    Category = new Category(FileType.Exe);
                    break;
                case ".config":
                    Category = new Category(FileType.Config);
                    break;
                case ".dll":
                    Category = new Category(FileType.Dll);
                    break;
                case ".zip":
                    Category = new Category(FileType.Zip);
                    break;
                case ".xml":
                    Category = new Category(FileType.Xml);
                    break;
                case ".mp3":
                    Category = new Category(FileType.Music);
                    break;
                case ".wmv":
                    Category = new Category(FileType.Video);
                    break;
                case ".bmp":
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                case ".cur":
                case ".jp2":
                case ".ami":
                case ".ico":
                    Category = new Category(FileType.Picture);
                    break;
                default:
                    Category = new Category(FileType.File);
                    break;
            }
         }

        public FileModel(DirectoryInfo di)
        {
            Name = di.Name;
            FullName = di.FullName;
            FullPath = Encode(di.FullName);
            Location = di.Parent != null ? di.Parent.FullName : "";
            Modified = di.LastWriteTime;
            Created = di.CreationTime;
            Accessed = di.LastAccessTime;
            Category = new Category(FileType.Folder);
            IsFolder = true;
        }

        public string CategoryText
        {
            get { return Category.ToString(); }
        }

        public static string Encode(string filepath)
        {
            return filepath.Replace("\\", "/");
        }

        public static string Decode(string filepath)
        {
            return filepath.Replace("/", "\\");
        }

        public static IList<FileModel> GetRootDirectories()
        {
            List<FileModel> result = new List<FileModel>();
            DriveInfo[] drives = DriveInfo.GetDrives();
            string winPath = Environment.GetEnvironmentVariable("windir");
            string winRoot = Path.GetPathRoot(winPath);
            foreach (DriveInfo di in drives)
            {
                if (!di.IsReady)
                    continue;
                if (di.RootDirectory == null)
                    continue;
                if (di.RootDirectory.FullName == winRoot)
                {
                    result.Add(new FileModel(di.RootDirectory) { Category = new Category(FileType.SysRoot), Accessed = null, Created = null, Modified = null });
                    continue;
                }
                switch (di.DriveType)
                {
                    case DriveType.CDRom:
                        result.Add(new FileModel(di.RootDirectory) { Category = new Category(FileType.DiscRoot), Accessed = null, Created = null, Modified = null });
                        break;
                    case DriveType.Fixed:
                        result.Add(new FileModel(di.RootDirectory) { Category = new Category(FileType.FixedRoot), Accessed = null, Created = null, Modified = null });
                        break;
                    case DriveType.Network:
                        result.Add(new FileModel(di.RootDirectory) { Category = new Category(FileType.NetworkRoot), Accessed = null, Created = null, Modified = null });
                        break;
                    case DriveType.Removable:
                        result.Add(new FileModel(di.RootDirectory) { Category = new Category(FileType.RemovableRoot), Accessed = null, Created = null, Modified = null });
                        break;
                    default:
                        result.Add(new FileModel(di.RootDirectory) {Accessed = null, Created = null, Modified = null});
                        break;
                }
                
            }
            return result;
        }

        public static IList<FileModel> GetFiles(string path)
        {
            List<FileModel> result = new List<FileModel>();
            if (string.IsNullOrEmpty(path))
            {
                return GetRootDirectories();
            }
            else
                path = Decode(path);
            try
            {
                string[] dirs = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
                foreach (string dir in dirs)
                {
                    DirectoryInfo di = new DirectoryInfo(dir);
                    result.Add(new FileModel(di));
                }

                string[] files = Directory.GetFiles(path, "*.*",
                                          SearchOption.TopDirectoryOnly);

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    result.Add(new FileModel(fi));
                }
                result.Sort((a, b) =>
                {
                    var name1 = a.Name;
                    var name2 = b.Name;
                    if (a.Category.Value == FileType.Folder)
                        name1 = " " + name1;
                    if (b.Category.Value == FileType.Folder)
                        name2 = " " + name2;
                    return name1.CompareTo(name2);
                });
                return result;
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static IList<FileModel> GeFolders(string path)
        {
            List<FileModel> result = new List<FileModel>();
            if (string.IsNullOrEmpty(path))
            {
                return GetRootDirectories();
            }
            else
                path = Decode(path);
            try
            {
                string[] dirs = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);
                foreach (string dir in dirs)
                {
                    DirectoryInfo di = new DirectoryInfo(dir);
                    result.Add(new FileModel(di));
                }
                          
                return result;
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static IList<FileModel> Search(string folderPath, string search)
        {
            string path = Decode(folderPath);
            List<FileModel> result = new List<FileModel>();
            GetFiles(path, search, result);
            return result;

        }

        public static void GetFiles(string path, string search, List<FileModel> result)
        {
            try
            {
                string[] files = Directory.GetFiles(path, search, SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    try
                    {
                        FileInfo fi = new FileInfo(file);
                        result.Add(new FileModel(fi));
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception)
            {
            }

            try
            {
                string[] dirs = Directory.GetDirectories(path, search, SearchOption.TopDirectoryOnly);
                foreach (string dir in dirs)
                {
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(dir);
                        result.Add(new FileModel(di));
                    }
                    catch
                    {
                    }
                }

                foreach (string dir in Directory.GetDirectories(path))
                    GetFiles(dir, search, result);
            }
            catch (Exception)
            {
            }
        }
    }
}