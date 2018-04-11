using System;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Azure.CosmosDB.Table;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using FoodMenu.Models;

namespace FoodMenu.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            await context.PostAsync(activity.Text);
            await context.PostAsync("Welcome to lunch Bot. Please press 1 to register and 2 to deactivate if you are already registered.");
            context.Wait(RegisterUser);
        }
        private async Task RegisterUser(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var cS = CloudConfigurationManager.GetSetting("StorageConnectionString");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(cS);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("usertable");
            table.CreateIfNotExists();
            var user = new Users(activity.From.Id, activity.From.Name, activity.ChannelId);
            TableOperation retrieveOperation = TableOperation.Retrieve<Users>(activity.ChannelId.ToLower(), activity.From.Id.ToLower());
            TableResult retrievedResult = table.Execute(retrieveOperation);
            if (activity.Text == "1")
            {
                try
                {
                    if (retrievedResult.Result == null)
                    {
                        TableOperation insertOperation = TableOperation.Insert(user);

                        table.Execute(insertOperation);

                        await context.PostAsync($"Hello, Nice to meet you. I have registered you to our lunch menu program, you will receive notification at 1 PM daily on working days to choose from menu. Please press 2 to unregister from the application.");

                    }
                    else
                    {
                        await context.PostAsync("you have alreaddy been registered to launch bot" + activity.Recipient.Id + activity.Recipient.Name);
                    }

                }
                catch (Exception ex)
                {
                    await context.PostAsync(ex.Message);

                }
                context.Wait(MessageReceivedAsync);
            }
            else if (activity.Text == "2")
            {
                try
                {
                    if (retrievedResult.Result != null)
                    {
                        Users deleteEntity = (Users)retrievedResult.Result;
                        TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                        // Execute the operation.
                        table.Execute(deleteOperation);

                        await context.PostAsync($"You have sucessfully unregistered from lunch bot.");

                    }
                    else
                    {
                        await context.PostAsync("you have not been registered to launch bot. Please register first.");
                    }

                }
                catch (Exception ex)
                {
                    await context.PostAsync(ex.Message);

                }
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                context.Wait(MessageReceivedAsync);
            }


        }
    }
}