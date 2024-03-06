using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Base.DataProviders
{
    public interface ICollector<T> : IEnumerable<T>
    {
        public void Add(T item);
        public void Remove(T item);
        public int Count { get; }
        public void Clear();
        public void MoveItem(int oldIndex, int newIndex);
    }
}
