using MongoDB.Bson;
using MongoDB.Driver;
using Simple.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Simple.MongoDB
{
    public static class MongoExtension
    {
        public static IMongoCollection<TDocument> GetCollection<TDocument>(this IMongoDatabase db)
        {
            MongoDocumentAttribute attribute = GetMongoDocumentAttribute<TDocument>();
            return db.GetCollection<TDocument>(attribute.Collection);
        }
        public static bool Insert<TDocument>(this IMongoDatabase db, TDocument document)
        {
            IMongoCollection<TDocument> collection = db.GetCollection<TDocument>();
            collection.InsertOne(document);
            return true;
        }
        public static bool Insert<TDocument>(this IMongoDatabase db, IEnumerable<TDocument> document)
        {
            IMongoCollection<TDocument> collection = db.GetCollection<TDocument>();
            collection.InsertMany(document);
            return true;
        }

        public static bool Delete<TDocument>(this IMongoDatabase db, Expression<Func<TDocument, bool>> expression)
        {
            IMongoCollection<TDocument> collection = db.GetCollection<TDocument>();
            DeleteResult result = collection.DeleteMany(expression);
            return result.DeletedCount > 0;
        }
        public static bool Update<TDocument>(this IMongoDatabase db, TDocument document, Expression<Func<TDocument, object>> fields, Expression<Func<TDocument, bool>> expression)
        {
            IMongoCollection<TDocument> collection = db.GetCollection<TDocument>();
            BsonDocument bson = new BsonDocument();
            Dictionary<string, object> value = new Dictionary<string, object>();
            foreach (var item in fields.GetPropertys())
            {
                value.Add(item.Name, item.GetValue(document));
            }
            bson.Add(value);
            UpdateResult result = collection.UpdateMany(expression, bson);
            return result.ModifiedCount > 0;
        }
        public static bool Update<TDocument, TValue>(this IMongoDatabase db, TValue value, Expression<Func<TDocument, TValue>> field, Expression<Func<TDocument, bool>> expression)
        {
            IMongoCollection<TDocument> collection = db.GetCollection<TDocument>();
            string fieldName = field.GetPropertyInfo().Name;
            BsonDocument bson = new BsonDocument();
            bson.Add(fieldName, value.ToString());
            UpdateResult result = collection.UpdateMany(expression, bson);
            return result.ModifiedCount > 0;
        }
        public static long Count<TDocument>(this IMongoDatabase db, Expression<Func<TDocument, bool>> expression)
        {
            IMongoCollection<TDocument> collection = db.GetCollection<TDocument>();
            return collection.CountDocuments(expression);
        }

        public static bool Any<TDocument>(this IMongoDatabase db, Expression<Func<TDocument, bool>> expression)
        {
            return db.Count<TDocument>(expression) > 0;
        }

        public static TDocument FirstOrDefault<TDocument>(this IMongoDatabase db, Expression<Func<TDocument, bool>> expression)
        {
            IMongoCollection<TDocument> collection = db.GetCollection<TDocument>();
            return collection.Find(expression).FirstOrDefault();
        }
        public static IQueryable<TDocument> Query<TDocument>(this IMongoDatabase db)
        {
            IMongoCollection<TDocument> collection = db.GetCollection<TDocument>();
            return collection.AsQueryable();
        }
        public static IQueryable<TDocument> Query<TDocument>(this IMongoDatabase db, Expression<Func<TDocument, bool>> expression)
        {
            IMongoCollection<TDocument> collection = db.GetCollection<TDocument>();
            return collection.AsQueryable().Where(expression);
        }
        /// <summary>
        /// 获取mongo文档特性
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        /// <exception cref="MongoException"></exception>
        private static MongoDocumentAttribute GetMongoDocumentAttribute<TDocument>()
        {
            MongoDocumentAttribute attribute = typeof(TDocument).GetAttribute<MongoDocumentAttribute>();
            if (attribute == null) throw new MongoException(nameof(MongoDocumentAttribute));
            return attribute;
        }
    }
}
