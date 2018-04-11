using FoodMenuWeb.Models;
using Microsoft.Azure;
using Microsoft.Azure.CosmosDB.Table;
using Microsoft.Azure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FoodMenuWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly string cS;
        private readonly CloudTableClient tableClient;
        public AdminController()
        {
            cS = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cS);
            tableClient = storageAccount.CreateCloudTableClient();
        }
        public ActionResult Index() => View();
        public ActionResult Users()
        {
            List<Users> usersList = new List<Users>();
            CloudTable table = tableClient.GetTableReference("usertable");
            table.CreateIfNotExists();
            TableQuery<Users> query = new TableQuery<Users>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "slack"));

            var users = table.ExecuteQuery(query);
            // Print the fields for each customer.
            foreach (Users entity in users)
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<Users>(entity.PartitionKey, entity.RowKey);

                // Execute the retrieve operation.
                TableResult retrievedResult = table.Execute(retrieveOperation);
                var user = (Users)retrievedResult.Result;
                usersList.Add(user);
            }
            return View(usersList);
        }
        public ActionResult Orders(string userId)
        {
            List<OrderVM> orderList = new List<OrderVM>();
            CloudTable table = tableClient.GetTableReference("orderTable");
            table.CreateIfNotExists();


            CloudTable table1 = tableClient.GetTableReference("usertable");
            table1.CreateIfNotExists();
            TableOperation retrieveUserOperation = TableOperation.Retrieve<Users>("slack", userId.ToLower());

            // Execute the retrieve operation.
            TableResult retrievedUserResult = table1.Execute(retrieveUserOperation);
            if (retrievedUserResult.Result != null)
            {
                ViewBag.UserName = ((Users)retrievedUserResult.Result).UserName;
                TableQuery<Order> query = new TableQuery<Order>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId.ToLower()));

                var orders = table.ExecuteQuery(query);
                // Print the fields for each customer.
                foreach (Order entity in orders)
                {
                    TableOperation retrieveOperation = TableOperation.Retrieve<Order>(entity.PartitionKey, entity.RowKey);

                    // Execute the retrieve operation.
                    TableResult retrievedResult = table.Execute(retrieveOperation);
                    var order = (Order)retrievedResult.Result;
                    var orderVm = new OrderVM { Date = order.Date };
                    if (order.ItemId == 1)
                    {
                        orderVm.Item = "Chowmin";
                    }
                    else if (order.ItemId == 2)
                    {
                        orderVm.Item = "Pizza";
                    }
                    else
                    {
                        orderVm.Item = "Momo";
                    }
                    orderList.Add(orderVm);
                }
            }


            return View(orderList);
        }
        public ActionResult AllOrders()
        {
            List<OrderVM1> orderList = new List<OrderVM1>();
            CloudTable table = tableClient.GetTableReference("orderTable");
            table.CreateIfNotExists();

            CloudTable table1 = tableClient.GetTableReference("usertable");
            table1.CreateIfNotExists();

            TableQuery<Order> query = new TableQuery<Order>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, DateTime.Now.Date.ToString("MM:dd:yy").ToLower()));

            var orders = table.ExecuteQuery(query);
            // Print the fields for each customer.
            foreach (Order entity in orders)
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<Order>(entity.PartitionKey, entity.RowKey);

                // Execute the retrieve operation.
                TableResult retrievedResult = table.Execute(retrieveOperation);
                var order = (Order)retrievedResult.Result;

                TableOperation retrieveUserOperation = TableOperation.Retrieve<Users>("slack", order.UserId.ToLower());

                // Execute the retrieve operation.
                TableResult retrievedUserResult = table1.Execute(retrieveUserOperation);
                var user = ((Users)retrievedUserResult.Result);
                var orderVm = new OrderVM1 { UserName = user.UserName };
                if (order.ItemId == 1)
                {
                    orderVm.Item = "Chowmin";
                }
                else if (order.ItemId == 2)
                {
                    orderVm.Item = "Pizza";
                }
                else
                {
                    orderVm.Item = "Momo";
                }
                orderList.Add(orderVm);

            }


            return View(orderList);
        }
    }
}
