using Microsoft.Azure.CosmosDB.Table;
using System;

namespace FoodMenuWeb.Models
{
    public class Users : TableEntity
    {
        public string UserName { get; set; }
        public string Channel { get; set; }
        public string UserId { get; set; }

        public Users()
        {

        }
        public Users(string id, string name, string channelId)
        {
            this.PartitionKey = channelId.ToLower();
            this.RowKey = id.ToLower();
            this.UserName = name;
            this.UserId = id;
            this.Channel = channelId;
        }
    }
}
