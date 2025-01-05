using Autofac;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.LibVlcRtsp.UI.Factories;
using Ironwall.Libraries.LibVlcRtsp.UI.ViewModels;
using LibVLCSharp.Shared;
using System;

namespace Ironwall.Libraries.LibVlcRtsp.UI.Modules
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 9/5/2023 8:25:14 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class MediaPlayerModule : Module
    {
        

        #region - Ctors -
        public MediaPlayerModule()
        {
            
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        //protected override void Load(ContainerBuilder builder)
        //{
        //    try
        //    {
        //        builder.Register(ctx =>
        //        {
        //            Core.Initialize();
        //            var libVlc = new LibVLC();
        //            var mediaPlayer = new VlcMediaPlayer(libVlc);
        //            return mediaPlayer;
        //        })
        //        .As<VlcMediaPlayer>();
        //        builder.RegisterType<VlcComponentViewModel>().InstancePerDependency();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        protected override void Load(ContainerBuilder builder)
        {
            try
            {
                builder.RegisterType<VlcMediaPlayerFactory>().AsSelf().SingleInstance();
                builder.RegisterType<VlcComponentViewModel>().InstancePerDependency();
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        #endregion
    }
}
