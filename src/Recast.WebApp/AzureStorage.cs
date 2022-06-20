using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Recast.WebApp.Infra;

namespace Recast.WebApp
{
    public class AzureStorage
    {
        public static async Task CreateAllTablesAsync(IConfiguration configuration)
        {
            var account = AzureTableExtensions.GetStorageAccount(configuration);
            var tablesCreationTimeoutInSeconds = TimeSpan.FromSeconds(configuration.GetValue<uint?>("AzureStorage:TablesCreationTimeoutInSeconds") ?? 60);
            var client = account.CreateCloudTableClient();
            var tableNames = new[] { "Feed", "Post" };
            foreach (var tableName in tableNames)
                await client.GetTableReference(tableName).CreateIfNotExistsAsync(new TableRequestOptions { MaximumExecutionTime = tablesCreationTimeoutInSeconds }, new OperationContext { });
        }
    }
}
