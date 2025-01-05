using Ironwall.Libraries.LibVlcRtsp.UI.Modules;
using LibVLCSharp.Shared;
using System;
using System.Threading.Tasks;

namespace Ironwall.Libraries.LibVlcRtsp.UI.Factories
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 12/26/2024 7:45:48 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VlcMediaPlayerFactory
    {
        public async Task<VlcMediaPlayer> CreateAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    Core.Initialize();
                    var libVlc = new LibVLC();
                    return new VlcMediaPlayer(libVlc);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                
            });
        }
    }

}