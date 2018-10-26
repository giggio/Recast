using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Recast.WebApp.Infra;
using Recast.WebApp.Models.Entities;

namespace Recast.WebApp.Models
{
    public class Posts
    {
        private readonly CloudStorageAccount account;
        private readonly CloudTableClient client;
        private readonly CloudTable table;

        public Posts(IConfiguration configuration)
        {
            account = AzureTableExtensions.GetStorageAccount(configuration);
            client = account.CreateCloudTableClient();
            table = client.GetTableReference<Post>();
        }

        public async Task<IEnumerable<Post>> GetForFeed(Feed feed)
        {
            var query = new TableQuery<Post>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Post.CreateKey(feed.PartitionKey, feed.RowKey)));
            var posts = new List<Post>();
            TableContinuationToken token = null;
            do
            {
                var seg = await table.ExecuteQuerySegmentedAsync(query, token);
                token = seg.ContinuationToken;
                posts.AddRange(seg);
            } while (token != null);
            return posts;
        }

        public bool Insert(Post post)
        {
            table.Insert(post);
            return true;
        }

        public async Task Delete(string userName, string feedName, string title)
        {
            var post = await table.Retrieve<Post>(Post.CreateKey(userName, feedName), HttpUtility.UrlEncode(title));
            if (post != null)
                await table.ExecuteAsync(TableOperation.Delete(post));
        }

        public async Task<Post> Get(string userName, string feedName, string title)
        {
            var post = await table.Retrieve<Post>(Post.CreateKey(userName, feedName), HttpUtility.UrlEncode(title));
            return post;
        }

        public Task Update(Post post)
        {
            return table.Merge(post);
        }

    }
}