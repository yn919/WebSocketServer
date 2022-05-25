using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketServer.Models
{
    public class WSServer : NotifyPropertyChanged
    {
        private HttpListener hl;
        private HttpListenerContext hlc;
        private HttpListenerWebSocketContext wsc;
        private WebSocket ws;
        private int bufferSize = 1024;

        private int _port = 51999;
        public int port
        {
            get { return _port; }
            set
            {
                _port = value;
                RaisePropertyChange();
            }
        }

        private bool _isConnected = false;
        public bool isConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                RaisePropertyChange();
            }
        }

        private string _sendMessage = string.Empty;
        public string sendMessage
        {
            get { return _sendMessage; }
            set
            {
                _sendMessage = value;
                RaisePropertyChange();
            }
        }

        private string _receiveMessage = string.Empty;
        public string receiveMessage
        {
            get { return _receiveMessage; }
            set
            {
                _receiveMessage = value;
                RaisePropertyChange();
            }
        }

        public async Task StartAsync()
        {
            try
            {
                hl = new HttpListener();
                hl.Prefixes.Add($"http://localhost:{port}/ws/");
                hl.Start();

                hlc = await hl.GetContextAsync();
                wsc = await hlc.AcceptWebSocketAsync(null);
                ws = wsc.WebSocket;

                isConnected = true;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        public async Task StopAsync()
        {
            if (ws != null && hl != null && isConnected == true)
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "close async", CancellationToken.None);
                await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "close output async", CancellationToken.None);

                ws.Dispose();
                ws = null;

                wsc = null;
                hlc = null;

                hl.Stop();
                hl.Close();
                hl = null;

                isConnected = false;
            }
        }

        public async Task Restart()
        {
            await StopAsync();
            await StartAsync();
            await StartReceiveAsync();
        }

        public async Task SendAsync()
        {
            if (isConnected == false) return;

            byte[] sendBuffer = Encoding.UTF8.GetBytes(sendMessage);
            ArraySegment<byte> segment = new ArraySegment<byte>(sendBuffer);

            await ws.SendAsync(segment, WebSocketMessageType.Text, false, CancellationToken.None);
        }

        public async Task StartReceiveAsync()
        {
            if (isConnected == false) return;

            byte[] buffer = new byte[bufferSize];

            while (true)
            {
                ArraySegment<byte> segment = new ArraySegment<byte>(buffer);

                WebSocketReceiveResult result = await ws.ReceiveAsync(segment, CancellationToken.None);

                int count = result.Count;
                while (!result.EndOfMessage)
                {
                    if (count >= buffer.Length)
                    {
                        await StopAsync();
                        return;
                    }
                    segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                    result = await ws.ReceiveAsync(segment, CancellationToken.None);

                    count += result.Count;
                }

                receiveMessage = Encoding.UTF8.GetString(buffer, 0, count);
            }
        }
    }
}
