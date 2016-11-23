using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using bankBot;
using System.Collections.Generic;

namespace Weather_Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                StateClient stateClient = activity.GetStateClient();
                BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

                currencyObject.RootObject rootObject;
                

                var msg = activity.Text;
                string endOutput = "Hello and welcome to Contoso bot service. Please state a command.. such as help or clear";

                HttpClient client = new HttpClient();

                if (userData.GetProperty<bool>("SentGreeting")) //2nd visit
                {
                    if (msg.ToLower() == "help")
                    {
                        endOutput = "With the use of this service you can find out the currency rate of your desired value e.g. just type USD or NZD";

                    }
                    else if (msg.ToLower() == "branches")
                    {
                        endOutput = "With the use of this service you can find out the currency rate of your desired value e.g. just type USD or NZD";

                    }
                    else if (msg.ToLower().Equals("msa"))
                    {
                        Activity replyToConversation = activity.CreateReply("MSA information");
                        replyToConversation.Recipient = activity.From;
                        replyToConversation.Type = "message";
                        replyToConversation.Attachments = new List<Attachment>();

                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: "https://cdn2.iconfinder.com/data/icons/ios-7-style-metro-ui-icons/512/MetroUI_iCloud.png"));

                        List<CardAction> cardButtons = new List<CardAction>();
                        CardAction plButton = new CardAction()
                        {
                            Value = "http://msa.ms",
                            Type = "openUrl",
                            Title = "MSA Website"
                        };
                        cardButtons.Add(plButton);

                        ThumbnailCard plCard = new ThumbnailCard()
                        {
                            Title = "Visit MSA",
                            Subtitle = "The MSA Website is here",
                            Images = cardImages,
                            Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        await connector.Conversations.SendToConversationAsync(replyToConversation);

                        return Request.CreateResponse(HttpStatusCode.OK);

                    }
                    else if (msg.ToLower() == "clear")
                    {
                        endOutput = "User data cleared!";
                        await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);
                    }
                    else if (msg.ToLower() == "usd")
                    {
                        string x = await client.GetStringAsync(new Uri("http://api.fixer.io/latest?base=" + activity.Text));
                        rootObject = JsonConvert.DeserializeObject<currencyObject.RootObject>(x);
                        endOutput = x;
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