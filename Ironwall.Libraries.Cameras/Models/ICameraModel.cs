using Ironwall.Libraries.Cameras.Models;
using System;

namespace Ironwall.Libraries.Cameras
{
    /// <summary>
    /// ICameraModel - Camera 데이터를 갖고 있는 모델,
    /// ICloneable 인터페이스를 Implementation 하기 때문에
    /// 복제(클래스에서 기존 인스턴스와 같은 값을 갖는 새 인스턴스를 만듦)를 지원
    /// </summary>
    public interface ICameraModel 
        : ICameraBaseModel
    {
        #region - Properties -
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }
        public string HostName { get; set; }
        public string DeviceModel { get; set; }
        public string FirmwareVersion { get; set; }
        public string SerialNumber { get; set; }
        public string HardwareId { get; set; }
        public string Manufacturer { get; set; }
        public string Uri { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public int Profile { get; set; }
        #endregion
    }
}
