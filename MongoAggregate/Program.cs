using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoAggregate
{
    public class MongoDbContext
    {
        /// <summary>
        /// Database Name
        /// </summary>
        public string DatabaseName { get; }

        /// <summary>
        /// Actvive MongoDB Connection Client
        /// </summary>
        public IMongoClient Client { get; }

        /// <summary>
        /// Database Name
        /// </summary>
        public IMongoDatabase Database { get; }

        /// <summary>
        /// Creates a <see cref="MongoDbContext"/> by given connection string
        /// </summary>
        public MongoDbContext(string connectionString, string databaseName)
        {
            DatabaseName = databaseName;
            MongoUrl mongoUri = MongoUrl.Create(connectionString);
            Client = new MongoClient(mongoUri);

            Database = Client.GetDatabase(databaseName);
        }
    }
    class MacroGeometry
    {
        //public string Id {get; set; }
        public double LowerFaceWidth {get; set; }
        public double UpperFaceWidth {get; set; }

        public MacroGeometry(double lfw, double ufw)
        {
            //this.Id = Guid.NewGuid().ToString();
            this.LowerFaceWidth = lfw;
            this.UpperFaceWidth = ufw;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new MongoDbContext("mongodb://localhost:27017", "tryout");
            var mgCollection = 
                dbContext.Database.GetCollection<MacroGeometry>("MacroGeometry");
            var matchingGeometries = 
                mgCollection.
                Aggregate().
                Match(mg => mg.UpperFaceWidth > 4).
                As<MacroGeometry>().
                ToList();
            
            foreach(var mg in matchingGeometries)
            {
                Console.WriteLine("UpperFaceWidth = {0}", mg.UpperFaceWidth);
            }
        }
    }
}
