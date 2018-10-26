using Microsoft.Extensions.Configuration;
using Recast.WebApp.Infra;
using System.Threading.Tasks;

namespace Recast.WebApp
{
    public class AzureStorage
    {
        public static async Task CreateAllTables(IConfiguration configuration)
        {
            var account = AzureTableExtensions.GetStorageAccount(configuration);
            var client = account.CreateCloudTableClient();
            var tableNames = new[] {"Feed", "Post"};
            foreach (var tableName in tableNames)
            {
                await client.GetTableReference(tableName).CreateIfNotExistsAsync();
            }

        }
    }
}