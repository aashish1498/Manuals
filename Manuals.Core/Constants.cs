using System;
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

        public static string ProductImagesFolder
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(basePath, "ProductImages");
            }
        }
    }
}
