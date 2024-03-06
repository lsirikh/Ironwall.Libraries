using Caliburn.Micro;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Framework.Services;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Ironwall.Libraries.Map.UI.ViewModels.Symbols
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/21/2023 11:39:05 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class ShapeSymbolViewModel : SymbolViewModel, IShapeSymbolViewModel
    {

        #region - Ctors -
        public ShapeSymbolViewModel()
        {
            ShapeFill = "#00FFFFFF";
            ShapeStroke = "#FF0000FF";
            ShapeStrokeThick = 2.0d;
            _model = new ShapeSymbolModel();
        }
        public ShapeSymbolViewModel(IShapeSymbolModel model) : base(model)
        {
            //ShapeFill = model.ShapeFill;
            //ShapeStroke = model.ShapeStroke;
            //ShapeStrokeThick = model.ShapeStrokeThick;
            //Points = model.Points;
        }

        
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        public override void OnLoaded(object sender, SizeChangedEventArgs e)
        {
            base.OnLoaded(sender, e);
            Refresh();
        }
        public override void Dispose()
        {
            _model = new ShapeSymbolModel();
            GC.Collect();
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -

        public string ShapeStroke
        {
            get { return (_model as IShapeSymbolModel).ShapeStroke; }
            set
            {
                (_model as IShapeSymbolModel).ShapeStroke = value;
                NotifyOfPropertyChange(() => ShapeStroke);
            }
        }

        public double ShapeStrokeThick
        {
            get { return (_model as IShapeSymbolModel).ShapeStrokeThick; }
            set
            {
                (_model as IShapeSymbolModel).ShapeStrokeThick = value;
                NotifyOfPropertyChange(() => ShapeStrokeThick);
            }
        }

        public string ShapeFill
        {
            get{ return (_model as IShapeSymbolModel).ShapeFill; }
            set
            {
                (_model as IShapeSymbolModel).ShapeFill = value;
                NotifyOfPropertyChange(() => ShapeFill);
            }
        }

        public PointCollection Points
        {
            get { return points; }
            set
            {
                points = value;
                NotifyOfPropertyChange(() => Points);
            }
        }
        #endregion
        #region - Attributes -
        PointCollection points;

        protected bool isClosed;
        #endregion
    }
}
