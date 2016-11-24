using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using bankBot;
using System.Collections.Generic;
using bankBot.dataModel;
using System.Linq;

namespace Weather_Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

                CurrencyObject.RatesObj ratesObj;
                

                var msg = activity.Text;
                string endOutput = "Hello and welcome to Contoso bot service. Please state a command.. such as help or clear";

                HttpClient client = new HttpClient();

                if (userData.GetProperty<bool>("SentGreeting")) //2nd visit
                {
                    if (msg.ToLower() == "help")
                    {
                        endOutput = "Here is a list of the commands we provide.. \n\nclear: removes all current user data \n\nbranches: displays all branches \n\nbranches [branch name]: will provide details about that branch \n\ncurrency rate [currency type]: displays the rate \n\ncreate branch [branch name] contoso location [street number] [street name] weekdayhours [hours] sat [hours] sun [hours]";

                    }
                    else if (msg.ToLower().Contains("clear"))
                    {
                        endOutput = "User data cleared!";
                        await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                    }

                    else if (msg.ToLower().Contains("logout"))
                    {
                        userData.SetProperty<bool>("Admin", false);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                        endOutput = "You have successfully logged out! \nsee you later.";
                    }

                    else if (msg.ToLower().Substring(0, 5) == "login") //login username password
                    {
                        userData.SetProperty<bool>("Admin", false);
                        await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                        List<Accounts> accounts = await AzureManager.AzureManagerInstance.GetAccounts();
                        string temp = msg.Substring(6);
                        string[] userDetail = temp.Split(' ');

                        var y = accounts.First(ac => ac.userPassword.ToLower() == userDetail[1].ToLower());

                    }

                    else if (msg.ToLower() == "branches")
                    {
                        List<Branch> branches = await AzureManager.AzureManagerInstance.GetBranches();
                        endOutput = "";
                        foreach (Branch b in branches)
                        {

                            endOutput += "---------------------------------------------" + "\nBranch: " + b.name +  "\n\nLocation: " + b.location + "\n" + "\nWeekday Hours: " +  b.weekDayHours + "\n\n" + "Sat Hours: " + b.satHours + "\n\nSun Hours: " + b.sunHours + "\n\n";
                        }

                    }
                    else if (msg.ToLower().Substring(0, 13) == "currency rate")
                    {
                        string temp = msg.Substring(14);
                        //string[] userDetails = temp.Split(' ');
                        string x = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base=" + temp.ToUpper()));
                        ratesObj = JsonConvert.DeserializeObject<CurrencyObject.RatesObj>(x);

                        string NZD = "NZD: " + ratesObj.rates.NZD;
                        string AUD = "AUD: " + ratesObj.rates.AUD;
                        string EUR = "EUR: " + ratesObj.rates.EUR;
                        string JPY = "JPY: " + ratesObj.rates.JPY;
                        string GBP = "GBP: " + ratesObj.rates.GBP;
                        string CAD = "CAD: " + ratesObj.rates.CAD;
                        string CNY = "CNY: " + ratesObj.rates.CNY;
                        string USD = "USD: " + ratesObj.rates.USD;
                        string Base = ratesObj.@base;

                        if (Base == "USD")
                        {
                            string mes = "$1.00 USD equals..";
                            endOutput = mes + "\n\n" + NZD + "\n\n" + AUD + "\n\n" + EUR + "\n\n" + JPY + "\n\n" + GBP  + "\n\n" + CAD + "\n\n"  + CNY;
                        }
                        else
                        {
                            string mes = "The currency rate for " + Base + " are..";
                            endOutput = mes + "\n\n" + NZD + "\n\n" + AUD + "\n\n" + EUR + "\n\n" + JPY + "\n\n" + GBP + "\n\n" + CAD + "\n\n" + CNY + "\n\n" + USD;
                        }
                        
                    }
                    else if (msg.ToLower().Substring(0, 6) == "branch")
                    {
                        List<Branch> branches = await AzureManager.AzureManagerInstance.GetBranches();                        
                        string temp = msg.Substring(7);

                        var y = branches.First(br => br.name.ToLower() == temp.ToLower());

                        Activity replyToConversation = activity.CreateReply();
                        replyToConversation.Recipient = activity.From;
                        replyToConversation.Type = "message";
                        replyToConversation.Attachments = new List<Attachment>();

                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: "https://cdn1.iconfinder.com/data/icons/ecommerce-free/96/Savings-512.png"));

                        List<CardAction> cardButtons = new List<CardAction>();
                        CardAction plButton = new CardAction()
                        {
                            Value = "http://www.google.com",
                            Type = "openUrl",
                            Title = "Contoso Homepage"
                        };
                        cardButtons.Add(plButton);

                        ThumbnailCard plCard = new ThumbnailCard()
                        {
                            Title = y.name,
                            Subtitle = y.location,
                            Text = "Weekday:-" + y.weekDayHours + "\n\nSat:-" + y.satHours + "\n\nSun:-" + y.sunHours,
                            Images = cardImages,
                            Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        await connector.Conversations.SendToConversationAsync(replyToConversation);

                        return Request.CreateResponse(HttpStatusCode.OK);
  

                    }
                    else if (msg.ToLower().Substring(0, 13) == "create branch")// e.g. pakuranga contoso location 123 Fake street weekdays 8:30am-5:00pm saturday 10:00am-2:00pm sunday closed 
                    {
                        string temp = msg.Substring(13);
                        string[] userDetails = temp.Split(' ');
                        endOutput = "Successfully created new branch!\n" + "\n" + userDetails[1] + " " + userDetails[2] + "\n" + "\nLocation: "+ userDetails[4] + " " + userDetails[5] + " " + userDetails[6] + "\n" + "\nWeekday hours: " + userDetails[8] + "\n" + "\nSat hours: " + userDetails[10] + "\n" + "\nSun hours: " + userDetails[12];

                        Branch branch = new Branch()
                        {
                            name = userDetails[1] + " " + userDetails[2],
                            location = userDetails[4] + " " + userDetails[5] + " " + userDetails[6],
                            weekDayHours = userDetails[8],
                            satHours = userDetails[10],
                            sunHours = userDetails[12] 
                        };

                        await AzureManager.AzureManagerInstance.AddBranches(branch);
                    }
                    else if (msg.ToLower().Substring(0, 13) == "delete branch") 
                    {
                        string temp = msg.Substring(13);
                        string[] userDetails = temp.Split(' ');

                        List<Branch> branches = await AzureManager.AzureManagerInstance.GetBranches();
                        var y = branches.First(br => br.name.ToLower() == temp.ToLower());


                        //await AzureManager.AzureManagerInstance.DeleteBranches(y.ID);
                    }


                    else if (msg.ToLower().Substring(0, 14) == "create account")
                    {
                        string temp = msg.Substring(15);
                        string[] userDetails = temp.Split(' ');
                        endOutput = "username: " + userDetails[0] + " " + "password: " + userDetails[1];

                        Accounts account = new Accounts()
                        {
                            userName = userDetails[0],
                            userPassword = userDetails[1]
                        };

                        await AzureManager.AzureManagerInstance.AddAccounts(account);
                    }

                }
                else
                {
                    userData.SetProperty<bool>("SentGreeting", true);
                    await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
                    endOutput = "Hello and welcome to Contoso bot service. Please state a command.. such as help or clear";

                }
                Activity infoReply = activity.CreateReply(endOutput);
                await connector.Conversations.ReplyToActivityAsync(infoReply);
            }
            else
            {
                HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;



        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}