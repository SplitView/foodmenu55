using FoodMenu.Models;
using Microsoft.Azure;
using Microsoft.Azure.CosmosDB.Table;
using Microsoft.Azure.Storage;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodMenu.Jobs
{
    public class SendMenuJob
    {
        public async Task SendMenuAsync()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("usertable");

            TableQuery<Users> query = new TableQuery<Users>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "slack"));
            string serviceUrl = "https://slack.botframework.com/";
            var users = table.ExecuteQuery(query);
            var connector = new ConnectorClient(new Uri(serviceUrl));
            // Print the fields for each customer.
            foreach (Users entity in users)
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<Users>(entity.PartitionKey, entity.RowKey);

                // Execute the retrieve operation.
                TableResult retrievedResult = table.Execute(retrieveOperation);
                var user = (Users)retrievedResult.Result;
                var userAccount = new ChannelAccount(name: user.UserName, id: user.UserId);

                var botAccount = new ChannelAccount("BA4D86SVD:TA3BN4CCS", "foodmenu55");
                var conversationId = await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount);

                IMessageActivity message = Activity.CreateMessageActivity();
                message.From = botAccount;
                message.Recipient = userAccount;
                message.Conversation = new ConversationAccount(id: conversationId.Id);

                //message.Text = $"Hello {user.UserName}!";
                //message.Locale = "en-Us";
                List<CardAction> cardBtns = new List<CardAction>();
                cardBtns.Add(new CardAction
                {
                    Value = $"http://foodmenuweb55.azurewebsites.net/home/takeorder?itemId=1&userId=" + userAccount.Id,
                    Type = ActionTypes.OpenUrl,
                    Title = "Chowmin"
                });
                cardBtns.Add(new CardAction
                {
                    Value = $"http://foodmenuweb55.azurewebsites.net/home/takeorder?itemId=2&userId=" + userAccount.Id,
                    Type = ActionTypes.OpenUrl,
                    Title = "Pizza"
                });
                cardBtns.Add(new CardAction
                {
                    Value = $"http://foodmenuweb55.azurewebsites.net/home/takeorder?itemId=3&userId="+userAccount.Id,
                    Type = ActionTypes.OpenUrl,
                    Title = "Mo-Mo"
                });

                HeroCard plCard = new HeroCard()
                {
                    Title = "Lunch Menu",
                    Buttons = cardBtns,
                    Text = $"Please select a dish u like"

                };
                Attachment plAttachment = plCard.ToAttachment();
                message.Attachments.Add(plAttachment);
                await connector.Conversations.SendToConversationAsync((Activity)message);


            }

        }
    }

}
