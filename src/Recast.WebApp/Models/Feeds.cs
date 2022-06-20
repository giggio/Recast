using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Recast.WebApp.Infra;
using Recast.WebApp.Models.Entities;
using System.Net;

namespace Recast.WebApp.Models;

public class Feeds
{
    private readonly IConfiguration configuration;

    public Feeds(IConfiguration configuration) => this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    public bool Insert(Feed feed)
    {
        var account = AzureTableExtensions.GetStorageAccount(configuration);
        var client = account.CreateCloudTableClient();
        var table = client.GetTableReference("Feed");

        try
        {
            table.Insert(feed);
        }
        catch (StorageException ex)
        {
            if (ex.InnerException is WebException exception && ((HttpWebResponse)exception.Response).StatusCode == HttpStatusCode.Conflict)
                return false;
            throw;
        }
        return true;
    }

    public async Task<Feed> GetAsync(string userName, string feedName)
    {
        userName = userName.ToLower();
        feedName = feedName.ToLower();
        var account = AzureTableExtensions.GetStorageAccount(configuration);
        var client = account.CreateCloudTableClient();
        var feedsTable = client.GetTableReference("Feed");
        var tableResult = await feedsTable.ExecuteAsync(TableOperation.Retrieve<Feed>(userName, feedName));
        return (Feed)tableResult.Result;
    }
}
