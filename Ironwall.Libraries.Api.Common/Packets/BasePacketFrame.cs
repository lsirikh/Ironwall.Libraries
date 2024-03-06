using System;

namespace Ironwall.Libraries.Api.Common.Packets
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BasePacketFrame : Attribute
    {
        public BasePacketFrame(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public int Entry { get; set; }
        public int Size { get; set; }
        public string Type { get; set; }
    }
}
