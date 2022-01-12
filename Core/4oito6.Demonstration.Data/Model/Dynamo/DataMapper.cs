using _4oito6.Demonstration.Data.Model.Dynamo.Base;
using Amazon.DynamoDBv2.DocumentModel;
using Linq2DynamoDb.DataContext.Utils;
using System;
using System.Diagnostics.CodeAnalysis;

namespace _4oito6.Demonstration.Data.Model.Dynamo
{
    [ExcludeFromCodeCoverage]
    public static class DataMapper
    {
        public static Document ToDocument<T>(this T document) where T : DynamoEntityDto
        {
            var doc = new Document();
            var properties = document.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.Name == nameof(document.TableName) || property.Name == nameof(document.Namespace))
                {
                    continue;
                }

                var value = property.GetValue(document, null);
                var type = property.PropertyType;
                doc[property.Name] = value?.ToDynamoDbEntry(type);
            }

            return doc;
        }

        private static T ToDataObject<T>(this Document document) where T : DynamoEntityDto
        {
            if (document is null)
            {
                return default;
            }

            var entity = (T)Activator.CreateInstance(typeof(T));
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if (!document.ContainsKey(property.Name))
                {
                    continue;
                }

                var value = document[property.Name];

                if (property.PropertyType.Equals(typeof(DateTime)))
                {
                    property.SetValue(entity, value.ToObject(property.PropertyType));
                }

                property.SetValue(entity, value.ToObject(property.PropertyType));
            }

            return entity;
        }

        public static T ToEntity<T>(this Document document) where T : DynamoEntityDto
        {
            return document.ToDataObject<T>();
        }
    }
}