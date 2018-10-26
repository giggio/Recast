using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Recast.WebApp.Infra
{
    public static class AzureTableExtensions
    {
        public static CloudStorageAccount GetStorageAccount(IConfiguration configuration)
        {
            var storageAccount = CloudStorageAccount.Parse(configuration.GetConnectionString("StorageAccount"));
            return storageAccount;
        }
        public static Task<TableResult> Insert<T>(this CloudTable table, T obj) where T : ITableEntity
        {
            return table.ExecuteAsync(TableOperation.Insert(obj));
        }
        public static Task<TableResult> Delete<T>(this CloudTable table, T obj) where T : ITableEntity
        {
            return table.ExecuteAsync(TableOperation.Delete(obj));
        }
        public static Task<TableResult> Merge<T>(this CloudTable table, T obj) where T : ITableEntity
        {
            return table.ExecuteAsync(TableOperation.Merge(obj));
        }
        public static async Task<T> Retrieve<T>(this CloudTable table, string partitionKey, string rowKey) where T : ITableEntity
        {
            var tableResult = await table.ExecuteAsync(TableOperation.Retrieve<T>(partitionKey, rowKey));
            return (T)tableResult.Result;
        }
        public static CloudTable GetTableReference<T>(this CloudTableClient cloudTableClient)
        {
            return cloudTableClient.GetTableReference(typeof(T).Name);
        }
    }
}