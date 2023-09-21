using System.Data;

namespace InfoSafe.Read.Data
{
    public class ReadDbContext
    {
        public readonly IDbConnection db;

        public ReadDbContext(IDbConnection dbConnection)
        {
            db = dbConnection;
        }
    }
}