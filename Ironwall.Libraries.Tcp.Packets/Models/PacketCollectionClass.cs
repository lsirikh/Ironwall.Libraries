using Ironwall.Libraries.Tcp.Packets.Utils;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Ironwall.Libraries.Tcp.Packets.Models
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/18/2023 10:15:55 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class PacketCollectionClass<T> : ICollection<T> where T : PacketClass
    {
        #region - Ctors -
        public PacketCollectionClass()
        {
            PacketList = new List<T>();
            _syncLock = new object();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void Add(T item)
        {
            try
            {
                if (IsBlank)
                {
                    Id = item.Id;
                    TotalSequence = item.TotalSequence;
                    Type = (EnumPacketType)item.DataType;
                    FileName = item.FileName;
                    FileExtension = item.FileExtension;

                    IsBlank = false;
                }

                if (PacketList.Where(entity => entity.CurrentSequence == item.CurrentSequence).Count() > 0)
                    return;

                PacketList.Add(item);
                MessageSize += item.BodyLength;

                if (Count == item.TotalSequence + 1)
                    IsFullList = true;
            }
            catch (System.Exception)
            {
                //Exception Message
            }
        }

        public bool Remove(T item)
        {
            try
            {
                var result = false;
                lock (_syncLock)
                    result = PacketList.Remove(item);
                return result;
            }
            catch (System.Exception)
            {
                //Exception Message
                throw;
            }

        }

        public void Clear()
        {
            try
            {
                lock (_syncLock)
                    PacketList.Clear();
            }
            catch (System.Exception)
            {
                //Exception Message
                throw;
            }
        }

        public bool Contains(T item)
        {
            try
            {
                if (PacketList.Contains(item))
                    return true;
                else
                    return false;
            }
            catch (System.Exception)
            {
                //Exception Message
                throw;
            }

        }

        public void CopyTo(T[] array, int arrayIndex)
        {

        }

        public IEnumerator<T> GetEnumerator()
        {
            return PacketList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return PacketList.GetEnumerator();
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public int Id { get; set; } = -1;
        public bool IsBlank { get; set; } = true;
        public bool IsFullList { get; set; } = false;
        public int TotalSequence { get; set; }
        public long MessageSize { get; set; }
        public EnumPacketType Type { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public List<T> PacketList { get; set; }

        /* Intertface implementation */
        public int Count => PacketList.Count;
        public bool IsReadOnly => true;

        #endregion
        #region - Attributes -
        private object _syncLock;
        #endregion
    }
}
