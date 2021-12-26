using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Manuals.Core.Models
{
    public class ProductItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } = 0;
        public string Name { get; set; }
        //public string Notes { get; set; }
        //public string ManualPath { get; set; }
        public string ProductImageName { get; set; }

        [TextBlob(nameof(TagsBlobbed))]
        public List<string> Tags { get; set; } = new List<string>();
        public string TagsBlobbed { get; set; }
    }
}
