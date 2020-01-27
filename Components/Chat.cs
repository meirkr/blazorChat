using blazorChat.Models;
using Blazor.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace blazorChat.Components
{
    public partial class Chat
    {
        private Message _message = new Message();


        private string _myName = "_myName";
        private string _myMsg = "_myMsg";
        List<(string user, string msg)> _messages = new List<(string user , string msg)>();
        IEnumerable<(string user, string msg)> ReversedMessages => Enumerable.Reverse(_messages);
        HubConnection hub;

        [Inject]
        private HubConnectionBuilder _hubConnectionBuilder { get; set; }

        protected override async Task OnInitializedAsync()
        {
            hub = _hubConnectionBuilder // the injected one from above.
            .WithUrl("http://localhost:5000/chat",
                opt =>
                {
                    opt.LogLevel = SignalRLogLevel.Trace; // Client log level
                    opt.Transport = HttpTransportType.WebSockets; // Which transport you want to use for this connection
                })
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