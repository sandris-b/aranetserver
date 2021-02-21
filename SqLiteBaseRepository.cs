using System;
using System.Data.SQLite;

namespace AranetServer
{
    public class SqLiteBaseRepository : IDisposable
    {
        protected SQLiteConnection connection = null;

        public SQLiteConnection db
        {
            get
            {
                if (connection == null)
                {
                    connection = new SQLiteConnection($"{Startup.DbPath};Version=3;DateTimeKind=Utc;UTF8Encoding=True;Pooling=True;Max Pool Size=100;");
                    connection.Open();
                }

                return connection;
            }
        }

        public void Dispose()
        {
            if (connection == null)
                return;

            connection?.Close();
            connection?.Dispose();
            connection = null;
        }
    }
}
