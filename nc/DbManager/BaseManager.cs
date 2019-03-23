using System;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace nc.DbManager
{
    public class BaseManager
    {
        static MongoClient client;
        static IMongoDatabase db;
        public BaseManager(IConfiguration config)
        {
            if (client == null)
            {
                client = new MongoClient(config.GetConnectionString("Db"));
                client.Settings.ReadPreference = ReadPreference.PrimaryPreferred;
                client.Settings.WriteConcern = WriteConcern.WMajority;
                client.Settings.MaxConnectionPoolSize = 4000;
                client.Settings.MinConnectionPoolSize = 50;
            }
        }
       
        public static IMongoDatabase GetDataBase(string dbName)
        {
            if (db == null)
            {
                db = client.GetDatabase(dbName);
            }
            return db;
        }

    }
}
