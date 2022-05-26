using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WebSocketServer.Models;

namespace WebSocketServer.ViewModels
{
    public class WSServerViewModel : IDisposable
    {
        private WSServer innerModel;

        public ReactiveProperty<int> port { get; }
        public ReactiveProperty<bool> isConnected { get; }
        public ReadOnlyReactiveProperty<string> connectedState { get; }
        public ReadOnlyReactiveProperty<SolidColorBrush> connectedStateColor { get; }
        public ReactiveProperty<string> sendMessage { get; }
        public ReactiveProperty<string> receiveMessage { get; }
        public ReactiveCommand restartCommand { get; }
        public ReactiveCommand sendCommand { get; }
        public WSServerViewModel(WSServer innerModel)
        {
            this.innerModel = innerModel;

            port = innerModel.ToReactivePropertyAsSynchronized(x => x.port);
            isConnected = innerModel.ToReactivePropertyAsSynchronized(x => x.isConnected);
            connectedState = isConnected.Select(x => x == true ? "connected" : "disconnected").ToReadOnlyReactiveProperty();
            connectedStateColor = isConnected.Select(x => x == true ? Brushes.LightGreen : Brushes.Red).ToReadOnlyReactiveProperty();
            sendMessage = innerModel.ToReactivePropertyAsSynchronized(x => x.sendMessage);
            receiveMessage = innerModel.ToReactivePropertyAsSynchronized(x => x.receiveMessage);

            restartCommand = new ReactiveCommand();
            restartCommand.Subscribe(async () => await innerModel.Restart());

            sendCommand = new ReactiveCommand();
            sendCommand.Subscribe(async () =>
            {
                await innerModel.SendAsync();
            });
        }

        public void Dispose()
        {
            port.Dispose();
            isConnected.Dispose();
            connectedState.Dispose();
            connectedStateColor.Dispose();
            sendMessage.Dispose();
            receiveMessage.Dispose();

            restartCommand.Dispose();
            sendCommand.Dispose();
        }
    }
}
