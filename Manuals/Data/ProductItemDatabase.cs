using Manuals.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;

namespace Manuals.Data
{
    public class ProductItemDatabase
    {
        static SQLiteAsyncConnection Database;
        
        private void OnDatabaseChanged()
        {
            //MessagingCenter.Send<>
        }

        public static readonly AsyncLazy<ProductItemDatabase> Instance = new AsyncLazy<ProductItemDatabase>(async () =>
        {
            var instance = new ProductItemDatabase();
            CreateTableResult result = await Database.CreateTableAsync<ProductItem>();
            return instance;
        });

        public ProductItemDatabase()
        {
            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public Task<List<ProductItem>> GetItemsAsync()
        {
            return Database.Table<ProductItem>().ToListAsync();
        }

        public Task<List<ProductItem>> GetItemsNotDoneAsync()
        {
            return Database.QueryAsync<ProductItem>("SELECT * FROM [ProductItem] WHERE [Done] = 0");
        }

        public Task<ProductItem> GetItemAsync(int id)
        {
            return Database.Table<ProductItem>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(ProductItem item)
        {
            if (item.ID != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(ProductItem item)
        {
            return Database.DeleteAsync(item);
        }
    }
}
