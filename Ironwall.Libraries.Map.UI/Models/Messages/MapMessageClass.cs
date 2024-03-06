using Ironwall.Framework.Models.Maps;
using Ironwall.Framework.Models.Maps.Symbols;
using Ironwall.Framework.Models.Maps.Symbols.Points;
using Ironwall.Framework.Models.Messages;
using Ironwall.Libraries.Map.UI.ViewModels.Symbols;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace Ironwall.Libraries.Map.UI.Models.Messages
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/28/2023 1:04:07 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class EventMapChangeMessage
    {
        public EventMapChangeMessage(int mapNumber)
        {

            MapNumber = mapNumber;  

        }
        public int MapNumber { get; set; }
    }

    public class EditorSetupFromBaseMessage
    {
        public EditorSetupFromBaseMessage(bool isOnEditable)
        {
            IsOnEditable = isOnEditable;
        }

        public bool IsOnEditable { get; set; }
    }

    public class EditorSetupFromLocalMessage
    {
        public EditorSetupFromLocalMessage(bool isOnEditable)
        {
            IsOnEditable = isOnEditable;
        }

        public bool IsOnEditable { get; set; }
    }

    public class EditorSymbolLoadMessage
    {
    }

    public class EditorSymbolSaveMessage
    {
        public EditorSymbolSaveMessage(List<PointClass> points, List<SymbolModel> symbols, List<ShapeSymbolModel> shapes, List<ObjectShapeModel> objects)
        {
            Points = points;
            Symbols = symbols;
            Shapes = shapes;
            Objects = objects;
        }

        public List<PointClass> Points { get; private set; }
        public List<SymbolModel> Symbols { get; private set; }
        public List<ShapeSymbolModel> Shapes { get; private set; }
        public List<ObjectShapeModel> Objects { get; private set; }
    }

    public class EditorMapSaveMessage
    {
        public EditorMapSaveMessage(List<MapModel> maps)
        {
            Maps = maps;
        }

        public List<MapModel> Maps { get; private set; }
    }

    /// <summary>
    /// Shape Edit 이벤트 메시지
    /// </summary>
    public class EditShapeMessage
    {
        public EditShapeMessage(bool isEditable, ISymbolViewModel viewModel)
        {
            IsEditable = isEditable;
            ViewModel = viewModel;
        }

        /// <summary>
        /// Shape Edit Option
        /// True/False
        /// </summary>
        public bool IsEditable { get; }
        /// <summary>
        /// 편집 할 인스턴스
        /// </summary>
        public ISymbolViewModel ViewModel { get; }
    }

    /// <summary>
    /// Shape 삭제 이벤트 메시지
    /// </summary>
    public class DeleteShapeMessage
    {
        public DeleteShapeMessage(ISymbolModel model)
        {
            symbolModel = model;
        }

        /// <summary>
        /// 삭제 할 인스턴스
        /// </summary>
        public ISymbolModel symbolModel { get; }
    }

    /// <summary>
    /// Shape 복제 이벤트 메시지
    /// </summary>
    public class CopyShapeMessage
    {
        public CopyShapeMessage(ISymbolModel model)
        {
            symbolModel = model;
        }

        /// <summary>
        /// 복제 할 인스턴스
        /// </summary>
        public ISymbolModel symbolModel { get; }
    }

    /// <summary>
    /// Camera Streaming  Popup을 위한 메시지
    /// </summary>
    public class RequestCameraStreaming
    {
        public RequestCameraStreaming(string nameDevice)
        {
            NameDevice = nameDevice;
        }
        /// <summary>
        /// Camera 이름이 Camera Streaming 대상의 요소
        /// </summary>
        public string NameDevice { get; }
    }

    public class RunMapDbSaveMessage : IMessageModel
    {
        public RunMapDbSaveMessage() { }
    }

    public class RunMapDeleteMessage : IMessageModel
    {
        public RunMapDeleteMessage() { }
    }

    public class RunMapSymbolClearMessage : IMessageModel
    {
        public RunMapSymbolClearMessage() { }
    }

    public class RunSymbolDbSaveMessage : IMessageModel
    {
        public RunSymbolDbSaveMessage() {}
    }

    public class RunSymbolDbLoadMessage : IMessageModel
    {
        public RunSymbolDbLoadMessage(){}
    }

    
}
