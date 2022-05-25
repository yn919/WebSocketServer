using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketServer.Models;

namespace WebSocketServer.ViewModels
{
    public class MainViewModel : IDisposable
    {
        private MainModel innerModel;

        public WSServerViewModel wsServerViewModel { get; set; }

        public MainViewModel()
        {
            innerModel = new MainModel();

            wsServerViewModel = new WSServerViewModel(innerModel.wsServer);
        }

        public async void Start()
        {
            await innerModel.Start();
        }

        public async void End()
        {
            await innerModel.End();
        }
        public void Dispose()
        {
            wsServerViewModel.Dispose();
        }
    }
}
