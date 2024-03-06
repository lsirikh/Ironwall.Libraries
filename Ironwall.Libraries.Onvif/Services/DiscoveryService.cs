using Caliburn.Micro;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Cameras.Models;
using Ironwall.Libraries.Cameras.Models.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Ironwall.Libraries.Onvif.Services
{
    public class DiscoveryService
    {
        
        #region - Ctors -
        public DiscoveryService(IEventAggregator eventAggregator
                                , ILogService log)
        {
            _eventAggregator = eventAggregator;
            _log = log;
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void SetDiscovery(int duration, int maxDevices)
        {
            Duration = duration;
            MaxDevice = maxDevices;
        }

        public async Task DiscoveryDevice()
        {

            SetDiscovery(5, 100);
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            FindCompletedEventHandler += (sender, e) =>
            {
                _eventAggregator.PublishOnUIThreadAsync(new DiscoveryTaskFinishedMessageModel());
            };


            FindProgressChangedEventHandler += (sender, e) =>
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

                            _log.Error(str[str.Length - 1]);

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
                        _log.Info($"{ip}, {port}");
                    }
                }
                catch (Exception ex)
                {
                    _log.Error($"Raised Exception during add discovered devices : {ex.Message}");
                }

            };
            await DiscoveryAsync(tokenSource.Token);
        }

        public Task DiscoveryAsync(CancellationToken token = default)
        {
            ///DiscoveryClient
            ///사용 가능한 서비스를 검색할 수 있습니다.
            DiscoveryClient client = null;
            try
            {
                #region ServicePoint, ServicePointManager
                ///ServicePoint
                ///HTTP 연결에 대해 연결 관리를 제공합니다.

                ///System.Net.ServicePointManager
                ///ServicePoint 개체의 컬렉션을 관리합니다.

                ///Expect100Continue	
                ///100 - Continue 동작을 사용할지 여부를 결정하는 Boolean 값을 가져오거나 설정합니다.
                /*
                    이 속성 설정 된 경우 true, 클라이언트를 사용 하는 요청을 POST 100 수신 하고자 하는 메서드-클라이언트에 게시 될 데이터를 전송 해야 하는 서버에서 응답을 계속 합니다. 이 메커니즘은 요청 헤더에 따라 서버에서 요청을 거부 하는 경우 네트워크를 통해 많은 양의 데이터를 전송 하지 않도록 클라이언트를 허용 합니다.

                    예를 들어 속성이 Expect100Continue = false라고 가정해봅시다. 요청이 서버로 전송되면 데이터가 포함됩니다. 요청 헤더를 읽은 후 서버에서 인증이 필요하고 401 응답을 보내는 경우 클라이언트는 적절한 인증 헤더를 사용하여 데이터를 다시 전송해야 합니다.

                    속성이 Expect100Continue true면 요청 헤더가 서버로 전송됩니다. 서버가 요청을 거부하지 않은 경우 데이터를 전송할 수 있다는 100-Continue 응답을 보냅니다. 앞의 예제와 같이 서버에서 인증이 필요한 경우 401 응답을 보내고 클라이언트가 데이터를 불필요하게 전송하지 않았습니다.

                    이 속성의 값을 변경해도 기존 연결에는 영향을 주지 않습니다. 변경 후 생성된 새 연결만 영향을 받습니다.

                    100-Continue 예상 동작은 IETF RFC 2616 섹션 10.1.1에 완전히 설명되어 있습니다.
                 */
                #endregion

                System.Net.ServicePointManager.Expect100Continue = false;

                #region DiscoveryClient
                ///DiscoveryClient(DiscoveryEndpoint)	
                ///지정된 검색 엔드포인트 구성을 사용하여 DiscoveryClient 클래스의 새 인스턴스를 만듭니다.

                /*
                    UdpDiscoveryEndpoint(DiscoveryVersion.WSDiscoveryApril2005)
                    UDP 멀티캐스트 바인딩을 통한 검색 작업에 대해 미리 구성된 표준 엔드포인트입니다. 
                    이 엔드포인트는 DiscoveryEndpoint에서 상속되며 이와 유사하게 고정된 계약을 가지고 있고
                    두 가지 WS-Discovery 프로토콜 버전을 지원합니다. 
                    또한 WS-Discovery 사양(WS-Discovery April 2005 또는 WS-Discovery V1.1)에 지정된 
                    고정된 UDP 바인딩 및 기본 주소가 있습니다.
                 */
                #endregion
                client = new DiscoveryClient(new UdpDiscoveryEndpoint(DiscoveryVersion.WSDiscoveryApril2005));
                //client = new DiscoveryClient(new UdpDiscoveryEndpoint(DiscoveryVersion.WSDiscovery11));
                //client = new DiscoveryClient(new UdpDiscoveryEndpoint(DiscoveryVersion.WSDiscoveryCD1));

                ///FindCriteria()	
                ///FindCriteria의 새 인스턴스를 만듭니다.
                ///서비스를 검색할 때 사용할 조건을 나타냅니다.
                FindCriteria findCriteria = new FindCriteria
                {
                    ///Duration	
                    ///찾기 작업의 제한 시간을 지정하는 TimeSpan을 가져오거나 설정합니다.
                    Duration = TimeSpan.FromSeconds(Duration),
                    ///MaxResults	
                    ///찾기 작업에서 필요한 최대 응답 수를 가져오거나 설정합니다.
                    MaxResults = MaxDevice,

                };

                ///ContractTypeNames	
                ///검색할 계약 형식 이름의 컬렉션을 가져옵니다.
                findCriteria.ContractTypeNames.Add(
                    ///XmlQualifiedName(String, String)	
                    ///정규화된 XML 이름을 나타냅니다.
                    ///
                    ///지정된 이름과 네임스페이스를 사용하여 
                    ///XmlQualifiedName 클래스의 새 인스턴스를 초기화합니다.
                    new XmlQualifiedName(
                        "NetworkVideoTransmitter",
                        @"http://www.onvif.org/ver10/network/wsdl"));

                ///FindProgressChanged	
                ///클라이언트가 특정 서비스로부터 응답을 받을 때마다 발생합니다.
                client.FindProgressChanged += (sender, e) =>
                {
                    ///FindProgressChangedEventHandler 호출
                    FindProgressChangedEventHandler?.Invoke(sender, e);
                };

                ///FindCompleted	
                ///전체 찾기 작업이 완료되면 발생합니다.
                client.FindCompleted += (sender, e) =>
                {
                    ///FindCompletedEventHandler 호출
                    FindCompletedEventHandler?.Invoke(sender, e);

                    ///Close()
                    ///검색 클라이언트를 닫습니다.
                    client.Close();
                };

                ///FindTaskAsync(FindCriteria, CancellationToken)	
                ///지정된 기준 및 취소 토큰 개체를 사용하여 비동기 찾기 작업을 시작합니다.
                return Task.Run(() => client.FindAsync(findCriteria), token);
            }
            catch (Exception ex)
            {
                _log.Error($"{(new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name} : {ex.Message}");
                throw ex;
            }
        }

        public static string GetName()
        {
            return Name;
        }

        private static string Name => nameof(DiscoveryService);
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
        private ILogService _log;

        public event EventHandler<FindProgressChangedEventArgs> FindProgressChangedEventHandler;
        public event EventHandler<FindCompletedEventArgs> FindCompletedEventHandler;
        #endregion
    }
}
