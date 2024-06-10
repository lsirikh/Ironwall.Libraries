using Gst;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows;
using System;
using System.Windows.Controls;
using Wpf.GstD3DStream.UI.Utils;
using D3D11Scene;
using System.Threading;
using Wpf.GstD3DStream.UI.Views;

namespace Wpf.GstD3DStream.UI.Controls
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 1/19/2024 4:04:45 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class D3DStreamView : Control, IDisposable
    {
        private D3DImageEx _d3DImageEx;
        private D3D11TestScene _D3D11Scene;
        private Playback _playback;
        private Image d3dScene;
        private const bool _enableOverlay = true;
        private const bool _enableSoftwareFallback = true;
        private readonly object _disposeLock = new object();
        private bool _disposed;

        static D3DStreamView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(D3DStreamView), new FrameworkPropertyMetadata(typeof(D3DStreamView)));
        }
        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public static readonly DependencyProperty UriProperty =
            DependencyProperty.Register("Uri", typeof(string), typeof(D3DStreamView), new PropertyMetadata(""));


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(D3DStreamView), new PropertyMetadata(""));


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            d3dScene = GetTemplateChild("PART_d3dScene") as Image;

            // Loaded 이벤트에 대한 핸들러 추가
            Loaded += StreamView_Loaded;

            // Unloaded 이벤트에 대한 핸들러 추가
            Unloaded += StreamView_Unloaded;
        }

        private void StreamView_Loaded(object sender, RoutedEventArgs e)
        {
            // Loaded 이벤트가 발생했을 때 실행할 로직
            try
            {
                _d3DImageEx = new D3DImageEx();
                _D3D11Scene = new D3D11TestScene(1920, 1080);

                d3dScene.Source = _d3DImageEx;

                /* Set the backbuffer, which is a ID3D11Texture2D pointer */
                var renderTarget = _D3D11Scene.GetRenderTarget();
                var backBuffer = _d3DImageEx.CreateBackBuffer(D3DResourceTypeEx.ID3D11Texture2D, renderTarget);

                _d3DImageEx?.Lock();
                _d3DImageEx?.SetBackBuffer(D3DResourceType.IDirect3DSurface9, backBuffer, _enableSoftwareFallback);
                _d3DImageEx?.Unlock();

                _playback = new Playback(IntPtr.Zero, _enableOverlay, uri: Uri);
                _playback.OnDrawSignalReceived += VideoSink_OnBeginDraw;

                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        private void StreamView_Unloaded(object sender, RoutedEventArgs e)
        {
            // Unloaded 이벤트가 발생했을 때 실행할 로직
            Dispose();
        }


        private void VideoSink_OnBeginDraw(Element sink, GLib.SignalArgs args)
        {
            var sharedHandle = _D3D11Scene.GetSharedHandle();
            _ = sink.Emit("draw", sharedHandle, (UInt32)2, (UInt64)0, (UInt64)0);
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            InvalidateD3DImage();
        }

        private void InvalidateD3DImage()
        {
            _d3DImageEx?.Lock();

            _d3DImageEx?.AddDirtyRect(new Int32Rect()
            {
                X = 0,
                Y = 0,
                Height = _d3DImageEx.PixelHeight,
                Width = _d3DImageEx.PixelWidth
            });

            _d3DImageEx?.Unlock();
        }

        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (!_disposed)
                {
                    try
                    {
                        // CompositionTarget.Rendering 이벤트 해제
                        CompositionTarget.Rendering -= CompositionTarget_Rendering;

                        _playback?.Cleanup();
                        _D3D11Scene?.Dispose();
                        _D3D11Scene = null;
                        _playback = null;
                        _d3DImageEx = null;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                    finally
                    {
                        _disposed = true;
                    }
                }
            }
        }
    }
}
