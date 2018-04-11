using Microsoft.Azure.CosmosDB.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodMenuWeb.Models
{
    public class Order : TableEntity
    {
        public string UserId { get; set; }
        public int ItemId { get; set; }
        public DateTime Date { get; set; }
        public Order()
        {

        }
        public Order(int itemId, string userId)
        {
            this.PartitionKey = userId.ToLower();
            this.RowKey = DateTime.Now.Date.ToString("MM:dd:yy").ToLower();
            this.ItemId = itemId;
            this.UserId = userId;
            this.Date = DateTime.Now.Date;
        }

    }
    public class OrderVM
    {
        public string Item { get; set; }
        public DateTime Date { get; set; }
    }
}
