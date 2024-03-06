using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Messages
{
    public interface ICommonMessageModel
        : IMessageModel
    {
        public string Title { get; set; }
        public string Explain { get; set; }
        public IMessageModel MessageModel  { get; set;}

    }
}
