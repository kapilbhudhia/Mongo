using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
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
        public string Id {get; set; }
        public double LowerFaceWidth {get; set; }
        public double UpperFaceWidth {get; set; }

        public double FaceWidthProp => UpperFaceWidth + LowerFaceWidth;
            // get { 
            // return LowerFaceWidth + UpperFaceWidth;
            //  }
            //  private set {
            //      this.FaceWidthProp = value;
            //  }
            // }
        public MacroGeometry(double lfw, double ufw)
        {
            this.Id = Guid.NewGuid().ToString();
            this.LowerFaceWidth = lfw;
            this.UpperFaceWidth = ufw;

        }
    }
    class Program
    {
        static void Main(string[] args)
        {           
            var dbContext = new MongoDbContext("mongodb://localhost:27017", "tryout");
            
            // the RegisterClassMap has to be called here, if we try to 
            // call this after getting the collection the it leads to 
            // an exception
            
            BsonClassMap.RegisterClassMap<MacroGeometry>(cm => {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
                cm.MapProperty(c => c.FaceWidthProp);
            });


            var mgCollection = 
                dbContext.Database.GetCollection<MacroGeometry>("MacroGeometry");
            
            // insert a MacroGeometry object
            var ex = new MacroGeometry (4.5, 6.5);
            mgCollection.InsertOne(ex);
            
            var addFieldDoc = new BsonDocument {
                {
                    "$addFields", new BsonDocument ("FaceWidth", 8)
                }
            };
            
            // var faceWidths =
            // mgCollection.Find(x => x.FaceWidthProp > 4).ToList();
            var faceWidths = 
                mgCollection
                .Aggregate().AppendStage<BsonDocument>(addFieldDoc)
            //     .AppendStage(new BsonDocument {
            //         {
            //             "$match", new BsonDocument{
            //                 { "$lte", new BsonDocument()}
            //             }
            //         }
            //     })
                //.Match(bd => )
                // .Project(mg => new
                // {
                    
                //     FaceWidth = mg..LowerFaceWidth + mg.UpperFaceWidth,
                //     LowerFaceWidth = mg.LowerFaceWidth,
                //     UpperFaceWidth = mg.UpperFaceWidth
                // })
            //     .Match(bd => bd..FaceWidth > 4)
            //     .Project(at => new 
            //     {
            //         LowerFaceWidth = at.LowerFaceWidth,
            //         UpperFaceWidth = at.UpperFaceWidth
            //     })
            //     .As<MacroGeometry>()
                  .ToList();
            
            // foreach(var fw in faceWidths)
            // {
            //     Console.WriteLine("FaceWidth = {0}", fw.FaceWidthProp);
            // }

            // foreach (var item in faceWidths)
            // {
            //     var nameList = item.Names;//.ToList();
            //     foreach (string name in nameList)
            //     {
            //         BsonValue bsonValue = item.GetValue(name);
            //         Console.WriteLine(string.Format("{0}: {1}", name, bsonValue));
            //     }
            // }
        }
    }
}
