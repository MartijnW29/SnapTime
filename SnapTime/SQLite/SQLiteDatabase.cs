using SnapTime.Classes;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnapTime.SQLite
{
    public class SQLiteDatabase
    {

        SQLiteAsyncConnection Database;

        public SQLiteDatabase()
        {
        }

        async Task Init()
        {
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(SQLiteConstants.DatabasePath, SQLiteConstants.Flags);
            var result = await Database.CreateTableAsync<LoggedInUser>();
        }

        public async Task<LoggedInUser> GetPreviouslyLoggedInUserAsync()
        {
            await Init();
            return await Database.Table<LoggedInUser>().FirstOrDefaultAsync();
        }

        public async Task<int> SaveUserAsync(LoggedInUser user)
        {
            await Init();
            return await Database.InsertAsync(user);
        }

        public async Task<int> DeleteUserAsync(string id)
        {
            await Init(); 
            return await Database.Table<LoggedInUser>()
                .Where(user => user.Id == id)
                .DeleteAsync();
        }

        internal void DeleteUserAsync(object id)
        {
            throw new NotImplementedException();
        }
    }
}
