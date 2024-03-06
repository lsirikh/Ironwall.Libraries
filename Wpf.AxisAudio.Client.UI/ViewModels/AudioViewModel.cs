using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Utils;
using System.Collections.ObjectModel;
using Wpf.AxisAudio.Common;
using Wpf.AxisAudio.Common.Models;

namespace Wpf.AxisAudio.Client.UI.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/18/2023 2:43:16 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioViewModel : AudioBaseViewModel<IAudioModel>
    {

        #region - Ctors -
        public AudioViewModel(IAudioModel model) : base(model)
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
            Groups = new ObservableCollection<AudioGroupBaseModel>();
            foreach (var item in _model.Groups)
            {
                Groups.Add(item);
            }

            Groups.CollectionChanged += Groups_CollectionChanged;
        }


        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private void Groups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    // New items added
                    foreach (AudioGroupModel newItem in e.NewItems)
                    {
                        _model.Groups.Add(newItem as AudioGroupModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (AudioGroupModel newItem in e.OldItems)
                    {
                        _model.Groups.Remove(newItem as AudioGroupModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (AudioGroupModel oldItem in e.OldItems)
                    {
                        _model.Groups.Remove(oldItem as AudioGroupModel);
                    }
                    foreach (AudioGroupModel newItem in e.NewItems)
                    {
                        _model.Groups.Add(newItem as AudioGroupModel);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    Groups.Clear();
                    foreach (AudioGroupModel item in _model.Groups)
                    {
                        Groups.Add(item);
                    }
                    break;
            }
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public ObservableCollection<AudioGroupBaseModel> Groups
        {
            get { return _groups; }
            set 
            { 
                _groups = value;
                NotifyOfPropertyChange(() => Groups);
            }
        }


        public string DeviceName
        {
            get { return _model.DeviceName; }
            set 
            {
                _model.DeviceName = value;
                NotifyOfPropertyChange(() => DeviceName);
            }
        }

        public string UserName
        {
            get { return _model.UserName; }
            set 
            {
                _model.UserName = value;
                NotifyOfPropertyChange(() => UserName);
            }
        }

        public string Password
        {
            get { return _model.Password; }
            set 
            {
                _model.Password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        public string IpAddress
        {
            get { return _model.IpAddress; }
            set
            {
                _model.IpAddress = value; 
                NotifyOfPropertyChange(() => IpAddress);
            }
        }

        public int Port
        {
            get { return _model.Port; }
            set 
            {
                _model.Port = value; 
                NotifyOfPropertyChange(() => Port);
            }
        }

        public int Mode
        {
            get { return _model.Mode; }
            set
            {
                _model.Mode = value;
                NotifyOfPropertyChange(() => Mode);
            }
        }

        public MediaClipConfigModel MediaClip
        {
            get { return _model.MediaClip; }
            set
            {
                _model.MediaClip = value;
                NotifyOfPropertyChange(() => MediaClip);
            }
        }
        #endregion
        #region - Attributes -
        private ObservableCollection<AudioGroupBaseModel> _groups;
        #endregion
    }
}
