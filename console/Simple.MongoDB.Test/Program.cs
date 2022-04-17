using MongoDB.Driver;
using Simple.Core.Localization;
using Simple.MongoDB;
using Simple.MongoDB.Test.Model;

var client = new MongoClient(AppsettingConfig.GetConnectionString("MongoConnection"));
IMongoDatabase db = client.GetDatabase("test");

var collection = db.GetCollection<User>();

db.Insert(new User() { ID = 1 });
long count = db.Count<User>(c => c.ID == 1);
User user = db.FirstOrDefault<User>(c => c.ID == 1);
var query = db.Query<User>(c => c.ID == 1);
bool exists = db.Any<User>(c => c.ID == 1);

db.Delete<User>(c => c.ID == 1);
count = query.Count();

db.Update(new User { ID = 1 }, c => new { c.ID }, c => c.ID == 1);


