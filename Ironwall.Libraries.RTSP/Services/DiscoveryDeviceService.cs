using Caliburn.Micro;
using Iodo.Onvif;
using Ironwall.Libraries.RTSP.Models;
using Ironwall.Libraries.RTSP.Models.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.RTSP.Services
{
    public class DiscoveryDeviceService
    {

        #region - Ctors -
        public DiscoveryDeviceService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.SubscribeOnPublishedThread(this);
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async Task DiscoveryDevice()
        {
            Duration = 5;
            MaxDevice = 100;
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            var discovery = new Discovery(Duration, MaxDevice);

            ///ManualResetEvent는 하나의 쓰레드만 통과시키고 닫는 AutoResetEvent와 달리, 
            ///한번 열리면 대기중이던 모든 쓰레드를 실행하게 하고 코드에서 수동으로 Reset()을 호출하여 
            ///문을 닫고 이후 도착한 쓰레드들을 다시 대기토록 한다.
            ///https://www.csharpstudy.com/Threads/manualresetevent.aspx
            ///

            discovery.FindCompletedEventHandler += (sender, e) =>
            {
                _eventAggregator.PublishOnUIThreadAsync(new DiscoveryTaskFinishedMessageModel());
            };


            discovery.FindProgressChangedEventHandler += (sender, e) =>
            {
                var ip = e.EndpointDiscoveryMetadata?.ListenUris[0]?.Host;
                int port = (int)(e.EndpointDiscoveryMetadata?.ListenUris[0]?.Port);
                var profiles = "";
                var types = "";
                var mac = "";
                var deviceModel = "";
                var company = "";
                var location = "";

                foreach (var item in e.EndpointDiscoveryMetadata?.Scopes)
                {
                    try
                    {
                        if (item.IsAbsoluteUri)
                        {
                            var str = item.AbsoluteUri.Split('/');
                            if (!(str.Length > 0)) continue;

                            Debug.WriteLine(str[str.Length - 1]);

                            if (item.AbsoluteUri.ToLower().Contains("profile"))
                            {
                                profiles += str[str.Length - 1];
                                //profiles += string.IsNullOrEmpty(profiles) ? item?.Segments[segmentIndex] : $", {item?.Segments[segmentIndex]}";
                            }
                            else if (item.AbsoluteUri.ToLower().Contains("type"))
                            {
                                types += str[str.Length - 1];
                                //types += string.IsNullOrEmpty(types) ? item?.Segments[segmentIndex] : $", {item?.Segments[segmentIndex]}";
                            }
                            else if (item.AbsoluteUri.ToLower().Contains("mac"))
                            {
                                mac += str[str.Length - 1];
                                //mac += item?.Segments[segmentIndex];
                            }
                            else if (item.AbsoluteUri.ToLower().Contains("hardware"))
                            {
                                deviceModel += str[str.Length - 1];
                                //deviceModel += item?.Segments[segmentIndex];
                            }
                            else if (item.AbsoluteUri.ToLower().Contains("name"))
                            {
                                company += str[str.Length - 1];
                            }
                            else if (item.AbsoluteUri.ToLower().Contains("location"))
                            {
                                location += str[str.Length - 1];
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                var name = e.EndpointDiscoveryMetadata?.ContractTypeNames[0]?.Name;


                try
                {
                    if (!(DiscoveryDeviceList?.Where(t => t.IpAddress == ip).Count() > 0))
                    {
                        DiscoveryDeviceList?.Add(new DiscoveryDeviceModel(ip, port, null, null, profiles, types, mac, deviceModel, company, location));
                        Debug.WriteLine($"{ip}, {port}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised Exception during add discovered devices : {ex.Message}");
                }

            };
            await discovery.DiscoveryAsync(tokenSource.Token);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -

        public int Duration { get; set; }
        public int MaxDevice { get; set; }

        public List<DiscoveryDeviceModel> DiscoveryDeviceList { get; set; }
        #endregion
        #region - Attributes -
        private IEventAggregator _eventAggregator;
        #endregion
    }
}
