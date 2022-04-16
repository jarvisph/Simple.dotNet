using MongoDB.Driver;
using Simple.MongoDB.Test.Model;

var connectionString = "mongodb://admin:a123456@192.168.0.21:27017";
var client = new MongoClient(connectionString);
string database = "test";
IMongoDatabase db = client.GetDatabase(database);
db.CreateCollection("user");

IMongoCollection<User> collection = db.GetCollection<User>("user");
collection.InsertOne(new User() { ID = 1 });

long count = collection.CountDocuments(c => c.ID == 1);
User user = collection.Find(c => c.ID == 1).FirstOrDefault();
user = collection.AsQueryable().Where(c => c.ID == 1).FirstOrDefault();
