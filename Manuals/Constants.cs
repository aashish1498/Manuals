using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Manuals
{
    public static class Constants
    {
        public const string DatabaseFilename = "ManualsSQLite.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }

        public static string GetProductItemsFolder()
        {
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(basePath, "ProductItems");
        }

        public static string GetLocalFolder(FileType fileType, int id)
        {
            var folder = Path.Combine(GetProductItemsFolder(), id.ToString());
            return Path.Combine(folder, Enum.GetName(typeof(FileType), fileType));
            //return fileType == FileType.Manual ? Path.Combine(folder, ID.ToString()) : folder;
        }

        public static void CreateFolders(int id)
        {
            foreach(FileType fileType in Enum.GetValues(typeof(FileType)))
            {
                Directory.CreateDirectory(GetLocalFolder(fileType, id));
            }
        }
        public static void AddRange<T>(this ObservableCollection<T> coll, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                coll.Add(item);
            }
        }
        public static string Truncate(this string value, int maxLength, string truncationSuffix = "…")
        {
            return value?.Length > maxLength
                ? value.Substring(0, maxLength) + truncationSuffix
                : value;
        }

        public enum FileType
        {
            ProductImage, Manual
        }
    }
}
