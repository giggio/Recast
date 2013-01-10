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
    }
}