using blazorChat.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Logging;

namespace BlazorChat.Components
{
    public partial class Chat
    {
        private Message _message = new Message();

        private string _myName = "user?";
        private string _myMsg = "";
        public bool WaitFoUserName => string.IsNullOrEmpty(_myName);
        List<(string user, string msg)> _messages = new List<(string user , string msg)>();
        IEnumerable<(string user, string msg)> ReversedMessages => Enumerable.Reverse(_messages);
        HubConnection hub;

        [Inject]
        private HubConnectionBuilder _hubConnectionBuilder { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var baseUri = NavigationManager.BaseUri;
            var hubUri = $"{baseUri}chat";
            Console.WriteLine($"baseUri: {baseUri}, hubUri:{hubUri}");

            hub = _hubConnectionBuilder // the injected one from above.
                .WithAutomaticReconnect()
                //.WithUrl("http://localhost:5000/chat")
                .WithUrl(hubUri)
                .ConfigureLogging(e => e.SetMinimumLevel(LogLevel.Trace))
                .Build(); // Build the HubConnection



            hub.On<string, string>("sendToAll", (user, msg) =>
            {
                _messages.Add((user, msg));

                base.StateHasChanged();
                return Task.CompletedTask;

            });

            try
            {
                await hub.StartAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine("-------- Exception\n" + e);

                _messages.Add(("errr", ""));

                //            base.StateHasChanged();

            }
            await base.OnInitializedAsync();
        }

        private void SendMsg()
        {
            hub.InvokeAsync("sendToAll", _myName, _myMsg);
        }        
    }
}