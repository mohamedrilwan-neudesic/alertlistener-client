
using Microsoft.AspNetCore.SignalR.Client;

using System;
using System.Threading.Tasks;

namespace SIgnalRNew
{
    class Program
    {

        async static Task Main(string[] args)
        {

            Console.WriteLine("Connect to Hub");

            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44350/alert")
                .WithAutomaticReconnect()
                .Build();

            try
            {
                await connection.StartAsync();
                Console.WriteLine("Connection started");
                Console.WriteLine("Enter User Name");
                var user = Console.ReadLine();
                Console.WriteLine("Enter Message");
                var message = Console.ReadLine();
                await connection.SendAsync("BroadcastToUser", user, message, "nrm");

                Console.WriteLine("Message Sent");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();

        }
    }
}
