using Recast.WebApp.Infra;

namespace Recast.WebApp
{
    public class AzureStorage
    {
        public static void CreateAllTables()
        {
            var account = AzureTableExtensions.GetStorageAccount();
            var client = account.CreateCloudTableClient();
            var tableNames = new[] {"Feed", "Post"};
            foreach (var tableName in tableNames)
            {
                client.GetTableReference(tableName).CreateIfNotExists();
            }

        }
    }
}