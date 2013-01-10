using Microsoft.WindowsAzure.Storage.Table;

namespace Recast.WebApp.Models.Entities
{
    public class Podcast : TableEntity
    {
        public Podcast() {}

        public Podcast(string partitionKey, string rowKey) : base(partitionKey, rowKey) {}
    }
}