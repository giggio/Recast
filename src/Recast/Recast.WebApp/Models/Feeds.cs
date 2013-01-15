using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Recast.WebApp.Infra;
using Recast.WebApp.Models.Entities;

namespace Recast.WebApp.Models
{
    public class Feeds
    {
        public bool Insert(Feed feed)
        {
            var account = AzureTableExtensions.GetStorageAccount();
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("Feed");

            try
            {
                table.Insert(feed);
            }
            catch (StorageException ex)
            {
                if (ex.InnerException is WebException && ((HttpWebResponse)((WebException)ex.InnerException).Response).StatusCode == HttpStatusCode.Conflict)
                {
                    return false;
                }
                throw;
            }
            return true;
        }

        public Feed Get(string userName, string feedName)
        {
            userName = userName.ToLower();
            feedName = feedName.ToLower();
            var account = AzureTableExtensions.GetStorageAccount();
            var client = account.CreateCloudTableClient();
            var feedsTable = client.GetTableReference("Feed");
            var feed = (Feed)feedsTable.Execute(TableOperation.Retrieve<Feed>(userName, feedName)).Result;
            return feed;
        }
    }
}