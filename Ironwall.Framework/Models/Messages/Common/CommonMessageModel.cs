using Ironwall.Framework.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Messages
{
    public abstract class CommonMessageModel
        : NotifierPropertyChanged, ICommonMessageModel
    {
        private string _title;
        private string _content;
        private IMessageModel _messageModel;

        public string Title
        {
            get { return _title; }
            set { 
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public string Explain
        {
            get { return _content; }
            set { 
                _content = value;
                OnPropertyChanged("Content");
            }
        }

        public IMessageModel MessageModel
        {
            get { return _messageModel; }
            set { 
                _messageModel = value;
                OnPropertyChanged("MessageModel");
            }
        }


    }
}
