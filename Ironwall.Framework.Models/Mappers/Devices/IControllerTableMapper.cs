﻿namespace Ironwall.Framework.Models.Mappers
{
    public interface IControllerTableMapper : IDeviceMapperBase
    {
        string IpAddress { get; set; }
        int Port { get; set; }
    }
}