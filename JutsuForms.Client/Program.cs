using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using System.Linq;

namespace JutsuForms.Client
{
    class Program
    {
        static HubConnection HubConnection;

        static async Task Main(string[] args)
        {
            var senderIdString = args[0];// args.ElementAtOrDefault(0);

            if (senderIdString is not null)
            {
                long senderId = Convert.ToInt64(senderIdString);

                HubConnection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:5000/update", options =>
                    {
                        options.Headers.Add("userId", senderIdString);
                    })
                    .ConfigureLogging(logging =>
                    {
                        // Log to the Console
                        logging.AddConsole();

                        // This will set ALL logging to Debug level
                        logging.SetMinimumLevel(LogLevel.Error);
                    })
                    .Build();

                HubConnection.On<string>("Send", message => Console.WriteLine($"B: {message}"));

                await HubConnection.StartAsync();

                bool isExit = false;

                while (!isExit)
                {
                    //Console.Write("U: ");
                    var message = Console.ReadLine();

                    var update = new Update()
                    {
                        Message = new Message()
                        {
                            Text = message,
                            From = new User()
                            {
                                Id = senderId
                            },
                            Chat = new Chat()
                            {
                                Id = senderId
                            }
                        }
                    };

                    if (message != "exit")
                    {
                        await  HubConnection.SendAsync("GetUpdate", update);
                    }
                    else
                    {
                        isExit = true;
                    }

                    //Console.ReadLine();
                }
            }
        }
    }
}
