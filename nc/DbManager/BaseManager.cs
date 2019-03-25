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
        static string constr = "mongodb://root:CareDaily20191123@192.168.100.251:27017,192.168.100.252:27017,192.168.100.250:27017/?authSource=admin&readPreference=primaryPreferred&replicaSet=rs1";
       
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
