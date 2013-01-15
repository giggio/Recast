using System.Collections.Generic;
using System.Web;
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

        public Posts()
        {
            account = AzureTableExtensions.GetStorageAccount();
            client = account.CreateCloudTableClient();
            table = client.GetTableReference<Post>();
        }

        public IEnumerable<Post> GetForFeed(Feed feed)
        {
            var query = new TableQuery<Post>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, Post.CreateKey(feed.PartitionKey, feed.RowKey)));
            var posts = table.ExecuteQuery(query);
            return posts;
        }

        public bool Insert(Post post)
        {
            table.Insert(post);
            return true;
        }

        public void Delete(string userName, string feedName, string title)
        {
            var post = table.Retrieve<Post>(Post.CreateKey(userName, feedName), HttpUtility.UrlEncode(title));
            if (post != null)
                table.Delete(post);
        }

        public Post Get(string userName, string feedName, string title)
        {
            var post = table.Retrieve<Post>(Post.CreateKey(userName, feedName), HttpUtility.UrlEncode(title));
            return post;
        }

        public void Update(Post post)
        {
            table.Merge(post);
        }

    }
}