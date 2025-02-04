﻿using Ironwall.Framework.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Components
{
    public abstract class EventVisitor
    {

        protected EventVisitor()
        {
            _class = this.GetType();
        }

        public abstract void Visit(PreIntrusionEventViewModel viewModel, ActionEventModel actionModel);
        public abstract void Visit(PreFaultEventViewModel viewModel, ActionEventModel actionModel);
        

        protected Type _class;
    }
}
