using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketServer.Models
{
    class MainModel
    {
        public WSServer wsServer { get; }

        public MainModel()
        {
            wsServer = new WSServer();
        }

        public async Task Start()
        {
            await wsServer.StartAsync();
            await wsServer.StartReceiveAsync();
        }

        public void End()
        {
            wsServer.Stop();
        }
    }
}
