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
        string Title { get; set; }
        string Explain { get; set; }
        IMessageModel MessageModel  { get; set;}

    }
}
