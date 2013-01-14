using System;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace Recast.WebApp.Models.Entities
{
    //feed key is pk, title is rowkey
    public class Post : TableEntity
    {
        private const string splitToken = @"_fjhadkjdfhk2313aj_";
        //public Post() {}
        //public Post(string partitionKey, string rowKey) : base(partitionKey, rowKey) {}

        public string SongLink { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Subtitle { get; set; }
        public string Summary { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public DateTime PublishDate { get; set; }
        public long Duration { get; set; }

        public string GetUserName()
        {
            return PartitionKey.Split(new[] {splitToken}, StringSplitOptions.None)[0];
        }
        public string GetFeedName()
        {
            return PartitionKey.Split(new[] {splitToken}, StringSplitOptions.None)[1];
        }
        public static string CreateKey(string userName, string feedName)
        {
            return string.Concat(HttpUtility.UrlEncode(userName), splitToken, HttpUtility.UrlEncode(feedName));
        }
    }
}