using Microsoft.WindowsAzure.Storage.Table;

namespace Recast.WebApp.Models.Entities
{
    public class Feed : TableEntity
    {
        public Feed() {}
        //pk is username, rk is feedName
        public Feed(string userName, string feedName) : base(userName.ToLower(), feedName.ToLower()) {}
    }
}