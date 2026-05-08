using MongoDB.Bson;
using MongoDB.Driver;
using Simple.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        public static long Delete<TDocument>(this IMongoDatabase db, Expression<Func<TDocument, bool>> expression)
        {
            IMongoCollection<TDocument> collection = db.GetCollection<TDocument>();
            DeleteResult result = collection.DeleteMany(expression);
            return result.DeletedCount;
        }
        public static bool Update<TDocument>(this IMongoDatabase db, TDocument document, Expression<Func<TDocument, object>> fields, Expression<Func<TDocument, bool>> where)
        {
            IMongoCollection<TDocument> collection = db.GetCollection<TDocument>();

            // 1. 构建更新定义（使用 Builders）
            var updateBuilder = Builders<TDocument>.Update;
            var updateDefinitions = new List<UpdateDefinition<TDocument>>();

            // 2. 获取要更新的字段信息
            var memberInfos = fields.GetPropertys(); // 假设这个方法正确返回 MemberInfo 列表

            foreach (var memberInfo in memberInfos)
            {
                // 获取字段名
                string fieldName = memberInfo.GetFieldName<ColumnAttribute>();

                // 获取字段对应的值
                var value = memberInfo.GetValue(document);

                // 添加更新定义
                updateDefinitions.Add(updateBuilder.Set(fieldName, value));
            }

            // 3. 组合所有更新操作
            var combinedUpdate = updateBuilder.Combine(updateDefinitions);

            // 4. 执行更新（修正：使用 where 表达式而不是未定义的 expression）
            UpdateResult result = collection.UpdateMany(where, combinedUpdate);

            // 5. 返回是否更新成功
            return result.ModifiedCount > 0;
        }
        public static bool Update<TDocument>(this IMongoDatabase db, TDocument document, FilterDefinition<TDocument> filter, ReplaceOptions? options = null)
        {
            IMongoCollection<TDocument> collection = db.GetCollection<TDocument>();
            var result = collection.ReplaceOne(filter, document, options);
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
