using System.Reflection;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Recast.WebApp.Infra
{
    public static class AzureTableExtensions
    {
        public static CloudStorageAccount GetStorageAccount()
        {
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting(
                    Assembly.GetExecutingAssembly().GetName().Name + ".StorageAccount"));
            return storageAccount;
        }
        public static TableResult Insert<T>(this CloudTable table, T obj) where T : ITableEntity
        {
            return table.Execute(TableOperation.Insert(obj));
        }
        public static TableResult Delete<T>(this CloudTable table, T obj) where T : ITableEntity
        {
            return table.Execute(TableOperation.Delete(obj));
        }
        public static TableResult Merge<T>(this CloudTable table, T obj) where T : ITableEntity
        {
            return table.Execute(TableOperation.Merge(obj));
        }
        public static T Retrieve<T>(this CloudTable table, string partitionKey, string rowKey) where T : ITableEntity
        {
            return (T)table.Execute(TableOperation.Retrieve<T>(partitionKey, rowKey)).Result;
        }
        public static CloudTable GetTableReference<T>(this CloudTableClient cloudTableClient)
        {
            return cloudTableClient.GetTableReference(typeof (T).Name);
        }
    }
}