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
                Stop();
            }
        }

        public void Stop()
        {
            if (ws != null && hl != null && isConnected == true)
            {
                ws.Dispose();
                ws = null;

                wsc = null;
                hlc = null;

                hl.Close();
                hl = null;

                isConnected = false;
            }
        }

        public async Task Restart()
        {
            Stop();
            await StartAsync();
            await StartReceiveAsync();
        }

        public async Task SendAsync()
        {
            if (isConnected == false) return;

            byte[] sendBuffer = new byte[bufferSize];
            byte[] stringBuffer = Encoding.UTF8.GetBytes(sendMessage);

            if (bufferSize < stringBuffer.Length) return;

            Array.Copy(stringBuffer, 0, sendBuffer, 0, stringBuffer.Length);

            ArraySegment<byte> segment = new ArraySegment<byte>(sendBuffer);

            await ws.SendAsync(segment, WebSocketMessageType.Text, false, CancellationToken.None);
        }

        public async Task StartReceiveAsync()
        {
            if (isConnected == false) return;

            byte[] buffer = new byte[bufferSize];

            while (true)
            {
                try
                {
                    ArraySegment<byte> segment = new ArraySegment<byte>(buffer);

                    WebSocketReceiveResult result = await ws.ReceiveAsync(segment, CancellationToken.None);

                    receiveMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    Stop();
                    break;
                }
            }
        }
    }
}
