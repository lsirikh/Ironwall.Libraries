using Caliburn.Micro;
using Ironwall.Libraries.Tcp.Common.Models;

namespace Ironwall.Libraries.Tcp.Client.UI.Infos
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/7/2023 10:14:27 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class ClientStatusViewModel : Screen
    {

        #region - Ctors -
        public ClientStatusViewModel()
        {
            Model = new TcpClientStatusModel();
        }

        public ClientStatusViewModel(ITcpClientStatusModel model)
        {
            Model = model;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public bool IsConnected
        {
            get { return Model.IsConnected; }
            set
            {
                Model.IsConnected = value;
                NotifyOfPropertyChange(nameof(IsConnected));
                ConnectionChanged?.Invoke(value);
            }
        }

        public string Status
        {
            get { return Model.Status; }
            set
            {
                Model.Status = value;
                NotifyOfPropertyChange(nameof(Status));
                StatusChanged?.Invoke(value);
            }
        }

        public ITcpClientStatusModel Model { get; private set; }
        #endregion
        #region - Attributes -

        public delegate void ConnectionChange(bool isConnected);
        public event ConnectionChange ConnectionChanged;

        public delegate void StatusChange(string status);
        public event StatusChange StatusChanged;
        #endregion
    }
}
