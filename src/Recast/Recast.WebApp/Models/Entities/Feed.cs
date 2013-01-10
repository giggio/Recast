using Microsoft.WindowsAzure.Storage.Table;

namespace Recast.WebApp.Models.Entities
{
    public class Feed : TableEntity
    {
        public Feed() {}
        //pk is username, rk is name
        public Feed(string userName, string name) : base(userName, name) {}
    }
}