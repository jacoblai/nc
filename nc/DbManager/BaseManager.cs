using System;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Linq;

namespace nc.DbManager
{
    public class BaseManager
    {
        static MongoClient client;
        static IMongoDatabase db;
        static string constr = "mongodb://localhost:27017";
       
        public static IMongoDatabase GetDataBase(string dbName)
        {
            if (client == null)
            {
                client = new MongoClient(constr);
            }
            if (db == null)
            {
                db = client.GetDatabase(dbName);
            }
            return db;
        }

    }
}
