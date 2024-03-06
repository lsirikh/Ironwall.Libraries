using Ironwall.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels
{
    public abstract class EventViewModel
        : NotifierPropertyChanged, IEventViewModel
    {
        #region - Ctors -
        public EventViewModel(IEventModel eventModel)
        {
            EventModel = eventModel;
        }
        #endregion
        #region - Implementations for IEventViewModel -   
        public Task ExecuteAsync(CancellationToken tokenSourceEvent = default, CancellationTokenSource tokenSourceOuter = null)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region - Properties for IEventViewModel -
        public int Id
        {
            get => EventModel.Id;
            set
            {
                EventModel.Id = value;
                OnPropertyChanged();
            }
        }

        public DateTime DateTime
        {
            get => EventModel.DateTime;
            set
            {
                EventModel.DateTime = value;
                OnPropertyChanged();
            }
        }

        public IEventModel EventModel { get; set; }
        public CancellationTokenSource CancellationTokenSourceEvent { get; set; }
        public string TagFault { get; set; }

        CancellationTokenSource IEventViewModel.CancellationTokenSourceEvent { get; set; }
        #endregion
        #region - Attriibtes -
        #endregion
    }
}
