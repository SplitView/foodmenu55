using FoodMenuWeb.Models;
using Microsoft.Azure;
using Microsoft.Azure.CosmosDB.Table;
using Microsoft.Azure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodMenuWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() => View();
        public ActionResult TakeOrder(int itemId, string userId)
        {
            var cS = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cS);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("orderTable");
            table.CreateIfNotExists();
            var order = new Order(itemId, userId);
            TableOperation retrieveOperation = TableOperation.Retrieve<Order>(userId.ToLower(), DateTime.Now.Date.ToString("MM:dd:yy").ToLower());
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (retrievedResult.Result == null)
            {
                TableOperation insertOperation = TableOperation.Insert(order);

                table.Execute(insertOperation);
                ViewBag.saveMessage = "Thanks, you have sucessfully ordered a item.";
            }
            else
            {
                ViewBag.saveMessage = "Sorry, you have already ordered an item today";
            }
            return View();
        }
    }
}