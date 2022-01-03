using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;

namespace Manuals.Models
{
    public class ProductItem
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; } = "";
        public string ProductImageName { get; set; } = "";

        [TextBlob(nameof(TagsBlobbed))]
        public List<string> Tags { get; set; } = new List<string>();
        public string TagsBlobbed { get; set; }

        [TextBlob(nameof(ManualNamesBlobbed))]
        public List<string> ManualNames { get; set; } = new List<string>();
        public string ManualNamesBlobbed { get; set; }
    }
}
