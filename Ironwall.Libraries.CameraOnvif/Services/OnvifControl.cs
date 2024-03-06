using Mictlanix.DotNet.Onvif.Common;
using Mictlanix.DotNet.Onvif.Device;
using Mictlanix.DotNet.Onvif.Imaging;
using Mictlanix.DotNet.Onvif.Media;
using Mictlanix.DotNet.Onvif.Ptz;
using Mictlanix.DotNet.Onvif;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Ironwall.Libraries.CameraOnvif.Services
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/16/2023 9:56:21 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class OnvifControl
    {

        #region - Ctors -
        public OnvifControl()
        {
            InitClass();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public void InitClass()
        {
            Device = null;
            Media = null;
            Ptz = null;
            Imaging = null;
            Caps = null;

            Absolute_move = false;
            Relative_move = false;
            Continuous_move = false;
            Focus = false;

            Profile_token = null;
        }

        public async Task DeviceReady(string host, string username, string password, CancellationToken token = default)
        {
            try
            {
                try
                {
                    Device = await OnvifClientFactory.CreateDeviceClientAsync(host, username, password);
                }
                catch { }
                await Task.Delay(10, token); if (token.IsCancellationRequested) throw new TaskCanceledException();

                try
                {
                    Media = await OnvifClientFactory.CreateMediaClientAsync(host, username, password);
                }
                catch { }
                await Task.Delay(10, token); if (token.IsCancellationRequested) throw new TaskCanceledException();

                try
                {
                    Ptz = await OnvifClientFactory.CreatePTZClientAsync(host, username, password);
                }
                catch { }
                await Task.Delay(10, token); if (token.IsCancellationRequested) throw new TaskCanceledException();

                try
                {
                    Imaging = await OnvifClientFactory.CreateImagingClientAsync(host, username, password);
                }
                catch { }
                await Task.Delay(10, token); if (token.IsCancellationRequested) throw new TaskCanceledException(); ;

                //try
                //{
                //    Caps = await Device.GetCapabilitiesAsync(new CapabilityCategory[] { CapabilityCategory.All });
                //}
                //catch { }

                Absolute_move = false;
                Relative_move = false;
                Continuous_move = false;
                Focus = false;

                //WriteLine("Capabilities");
                //WriteLine("\tDevice: " + Caps.Capabilities.Device.XAddr);
                //WriteLine("\tEvents: " + Caps.Capabilities.Events.XAddr);
                //WriteLine("\tImaging: " + Caps.Capabilities.Imaging.XAddr);
                //WriteLine("\tMedia: " + Caps.Capabilities.Media.XAddr);
                //WriteLine("\tPTZ: " + Caps.Capabilities.PTZ?.XAddr);
            }
            catch (TaskCanceledException)
            {
                WriteLine($"Task({nameof(DeviceReady)}) was Finished by Cancellation");
            }
            catch (Exception ex)
            {
                WriteLine("Raised Exception in DeviceReady : " + ex.ToString());
            }
        }

        public async Task CreateProfile(CancellationToken token = default)
        {
            try
            {
                if (!CheckMedia) return;

                Profile_token = null;

                var profiles = await Media.GetProfilesAsync();

                WriteLine("Profiles count :" + profiles.Profiles.Length);

                foreach (var profile in profiles?.Profiles)
                {
                    if (token.IsCancellationRequested) throw new TaskCanceledException();

                    WriteLine($"Profile: {profile.token}");

                    if (Profile_token == null)
                    {
                        Profile_token = profile.token;
                        Absolute_move = !string.IsNullOrWhiteSpace(profile?.PTZConfiguration?.DefaultAbsolutePantTiltPositionSpace);
                        Relative_move = !string.IsNullOrWhiteSpace(profile?.PTZConfiguration?.DefaultRelativePanTiltTranslationSpace);
                        Continuous_move = !string.IsNullOrWhiteSpace(profile?.PTZConfiguration?.DefaultContinuousPanTiltVelocitySpace);
                    }

                    WriteLine($"\tTranslation Support");
                    WriteLine($"\t\tAbsolute Translation: {!string.IsNullOrWhiteSpace(profile?.PTZConfiguration?.DefaultAbsolutePantTiltPositionSpace)}");
                    WriteLine($"\t\tRelative Translation: {!string.IsNullOrWhiteSpace(profile?.PTZConfiguration?.DefaultRelativePanTiltTranslationSpace)}");
                    WriteLine($"\t\tContinuous Translation: {!string.IsNullOrWhiteSpace(profile?.PTZConfiguration?.DefaultContinuousPanTiltVelocitySpace)}");

                    if (!string.IsNullOrWhiteSpace(profile?.PTZConfiguration?.DefaultRelativePanTiltTranslationSpace))
                    {
                        var pan = profile?.PTZConfiguration?.PanTiltLimits.Range.XRange;
                        var tilt = profile?.PTZConfiguration?.PanTiltLimits.Range.YRange;
                        var zoom = profile?.PTZConfiguration?.ZoomLimits.Range.XRange;

                        WriteLine($"\tPan Limits: [{pan.Min}, {pan.Max}] Tilt Limits: [{tilt.Min}, {tilt.Max}] Zoom Limits: [{zoom.Min}, {zoom.Max}]");
                    }
                }

            }
            catch (TaskCanceledException)
            {
                WriteLine($"Task({nameof(CreateProfile)}) was Finished by Cancellation");
            }
            catch (Exception ex)
            {
                WriteLine("Raised Exception in media.GetProfilesAsync : " + ex.ToString());
            }
        }

        public async Task CreateVideoSource(CancellationToken token = default)
        {

            try
            {
                if (!CheckMedia) return;


                var video_sources = await Media.GetVideoSourcesAsync();
                VSource_token = null;


                // Part Video Sources
                foreach (var source in video_sources.VideoSources)
                {

                    if (token.IsCancellationRequested) throw new TaskCanceledException();

                    WriteLine($"Video Source: {source.token}");

                    try
                    {
                        if (VSource_token == null)
                        {
                            VSource_token = source?.token;
                            Focus = source?.Imaging?.Focus != null;
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLine("Raised Exception in Video Sources , source.Imaging.Focus : " + ex.ToString());
                    }

                    try
                    {
                        WriteLine($"\tImage Settings");
                        if (Imaging == null) return;

                        Imaging_opts = await Imaging?.GetOptionsAsync(source.token);

                        WriteLine($"\t\tBrightness: {source?.Imaging?.Brightness} [{Imaging_opts?.Brightness?.Min}, {Imaging_opts?.Brightness?.Max}]");
                        WriteLine($"\t\tColor Saturation: {source?.Imaging?.ColorSaturation} [{Imaging_opts?.ColorSaturation?.Min}, {Imaging_opts?.ColorSaturation?.Max}]");
                        WriteLine($"\t\tContrast: {source?.Imaging?.Contrast} [{Imaging_opts?.Contrast?.Min}, {Imaging_opts?.Contrast?.Max}]");
                        WriteLine($"\t\tSharpness: {source?.Imaging?.Sharpness} [{Imaging_opts?.Sharpness?.Min}, {Imaging_opts?.Sharpness?.Max}]");

                        Focus_opts = await Imaging?.GetMoveOptionsAsync(VSource_token);
                    }
                    catch
                    {
                    }

                    WriteLine($"\tFramerate: {source?.Framerate}");
                    WriteLine($"\tResolution: {source?.Resolution?.Width}x{source?.Resolution?.Height}");
                    WriteLine($"\tFocus Settings");

                    if (source?.Imaging?.Focus == null)
                    {
                        WriteLine($"\t\tNone");
                    }
                    else
                    {
                        WriteLine($"\t\tMode: {source?.Imaging?.Focus?.AutoFocusMode}");
                        WriteLine($"\t\tNear Limit: {source?.Imaging?.Focus?.NearLimit}");
                        WriteLine($"\t\tFar Limit: {source?.Imaging?.Focus?.FarLimit}");
                        WriteLine($"\t\tDefault Speed: {source?.Imaging?.Focus?.DefaultSpeed}");
                    }

                    WriteLine($"\tExposure Settings");

                    if (source?.Imaging?.Exposure == null)
                    {
                        WriteLine($"\t\tNone");
                    }
                    else
                    {
                        WriteLine($"\t\tMode: {source?.Imaging?.Exposure?.Mode}");
                        WriteLine($"\t\tMin Iris: {source?.Imaging?.Exposure?.MinIris}");
                        WriteLine($"\t\tMax Iris: {source?.Imaging?.Exposure?.MaxIris}");
                    }
                }

            }
            catch (TaskCanceledException)
            {
                WriteLine($"Task({nameof(CreateVideoSource)}) was Finished by Cancellation");
            }
            catch (Exception ex)
            {
                WriteLine("Raised Exception in CreateVideoSource : " + ex.ToString());
            }
        }

        public Task FocusInOut(bool direction, CancellationToken token = default)
        {
            if (!(CheckFocus_opts
                || CheckImaging
                || CheckVSource_token))
                return Task.CompletedTask;

            return Task.Run(async () =>
            {
                try
                {
                    if (Focus_opts.Continuous != null)
                    {
                        WriteLine($"\tMoving Focus Continuous...");

                        ContinuousFocus continuous = null;
                        if (direction)
                            continuous = new ContinuousFocus { Speed = 0.5f };
                        else
                            continuous = new ContinuousFocus { Speed = -0.5f };

                        await Imaging.MoveAsync(VSource_token, new FocusMove
                        {
                            Continuous = continuous
                        });

                        await Task.Delay(-1, token);
                    }
                }
                catch (TaskCanceledException)
                {
                    await Imaging.StopAsync(VSource_token);
                    WriteLine($"Task({nameof(FocusInOut)}) was Finished by Cancellation");
                }
                catch (Exception ex)
                {
                    WriteLine("Raised Exception in FocusInOut : " + ex.ToString());
                }

            }, token);
        }

        public Task FocusNormal(CancellationToken token = default)
        {
            if (!(CheckFocus_opts
                || CheckImaging
                || CheckVSource_token))
                return Task.CompletedTask;

            return Task.Run(async () =>
            {
                try
                {
                    if (Focus_opts.Continuous != null)
                    {
                        WriteLine($"\tMoving Focus Continuous...");

                        AbsoluteFocus absolute = null;

                        absolute = new AbsoluteFocus
                        {
                            Speed = 1f,
                            Position = 0f,
                            SpeedSpecified = false
                        };
                        await Imaging.MoveAsync(VSource_token, new FocusMove
                        {
                            Absolute = absolute
                        });

                        await Task.Delay(3000, token);
                    }
                }
                catch (TaskCanceledException)
                {
                    await Imaging.StopAsync(VSource_token);
                    WriteLine($"Task({nameof(FocusInOut)}) was Finished by Cancellation");
                }
                catch (Exception ex)
                {
                    WriteLine("Raised Exception in FocusInOut : " + ex.ToString());
                }

            }, token);
        }


        public Task MovePtz(int moveType, CancellationToken token = default)
        {
            if (!(CheckPtz || CheckProfileToken))
                return Task.CompletedTask;

            return Task.Run(async () =>
            {
                try
                {
                    if (Continuous_move)
                    {

                        Vector2D panTilt = null;
                        Vector1D zoom = null;

                        switch (moveType)
                        {
                            case 1://Up
                                {
                                    WriteLine($"PTZ Move Upward");
                                    panTilt = new Vector2D { x = 0, y = 1 };
                                    zoom = new Vector1D { x = 0 };
                                }
                                break;
                            case 11://UpLeft
                                {
                                    WriteLine($"PTZ Move Upward and Left");
                                    panTilt = new Vector2D { x = -1, y = 1 };
                                    zoom = new Vector1D { x = 0 };
                                }
                                break;
                            case 12://UpRight
                                {
                                    WriteLine($"PTZ Move Upward and Right");
                                    panTilt = new Vector2D { x = 1, y = 1 };
                                    zoom = new Vector1D { x = 0 };
                                }
                                break;
                            case 2://Down
                                {
                                    WriteLine($"PTZ Move Downward");
                                    panTilt = new Vector2D { x = 0, y = -1 };
                                    zoom = new Vector1D { x = 0 };
                                }
                                break;
                            case 21://DownLeft
                                {
                                    WriteLine($"PTZ Move Downward and Left");
                                    panTilt = new Vector2D { x = -1, y = -1 };
                                    zoom = new Vector1D { x = 0 };
                                }
                                break;
                            case 22://DownRight
                                {
                                    WriteLine($"PTZ Move Downward and Right");
                                    panTilt = new Vector2D { x = 1, y = -1 };
                                    zoom = new Vector1D { x = 0 };
                                }
                                break;
                            case 3://Left
                                {
                                    WriteLine($"PTZ Move Left");
                                    panTilt = new Vector2D { x = -1, y = 0 };
                                    zoom = new Vector1D { x = 0 };
                                }
                                break;
                            case 4://Right
                                {
                                    WriteLine($"PTZ Move Right");
                                    panTilt = new Vector2D { x = 1, y = 0 };
                                    zoom = new Vector1D { x = 0 };
                                }
                                break;
                            case 5://ZoomIn
                                {
                                    WriteLine($"PTZ Move ZoomIn");
                                    panTilt = new Vector2D { x = 0, y = 0 };
                                    zoom = new Vector1D { x = 1 };
                                }
                                break;
                            case 6://ZoomOut
                                {
                                    WriteLine($"PTZ Move ZoomOut");
                                    panTilt = new Vector2D { x = 0, y = 0 };
                                    zoom = new Vector1D { x = -1 };
                                }
                                break;
                            default:
                                break;
                        }

                        await Ptz.ContinuousMoveAsync(Profile_token, new PTZSpeed
                        {
                            PanTilt = panTilt,
                            Zoom = zoom
                        }, null);

                        await Task.Delay(-1, token);
                    }
                }
                catch (TaskCanceledException)
                {

                    await Ptz.StopAsync(Profile_token, true, true);

                    var ptz_status = await Ptz.GetStatusAsync(Profile_token);

                    WriteLine($"Position: [{ptz_status?.Position?.PanTilt?.x}, {ptz_status?.Position?.PanTilt?.y}, {ptz_status?.Position?.Zoom?.x}]");
                    WriteLine($"Pan/Tilt Status: {ptz_status?.MoveStatus?.PanTilt} Zoom Status: {ptz_status?.MoveStatus?.Zoom}");
                }
                catch (Exception ex)
                {
                    WriteLine($"Raised Exception during {nameof(MovePtz)} : " + ex.ToString());
                }

            }, token);
        }

        public Task SetAutoFocus(bool isAuto, CancellationToken token = default)
        {
            if (!(CheckImaging
                || CheckVSource_token))
                return Task.CompletedTask;

            return Task.Run(async () =>
            {
                try
                {
                    var image_settings = await Imaging.GetImagingSettingsAsync(VSource_token);

                    if (isAuto)
                    {

                    }
                    else
                    {

                    }

                    if (image_settings?.Focus == null)
                    {
                        image_settings.Focus = new FocusConfiguration20
                        {
                            AutoFocusMode = AutoFocusMode.AUTO
                        };
                    }
                    else
                    {
                        image_settings.Focus.AutoFocusMode = AutoFocusMode.AUTO;
                    }
                    await Imaging.SetImagingSettingsAsync(VSource_token, image_settings, false);

                }
                catch (TaskCanceledException)
                {
                    WriteLine($"Task({nameof(SetAutoFocus)}) was Finished by Cancellation");
                }
                catch (Exception ex)
                {
                    WriteLine($"Raised Exception during {nameof(MovePtz)} : " + ex.ToString());
                }
            }, token);
        }

        public async Task<Mictlanix.DotNet.Onvif.Ptz.GetPresetsResponse> GetPresets(CancellationToken token = default)
        {
            if (!(CheckPtz || CheckProfileToken)) return null;

            Mictlanix.DotNet.Onvif.Ptz.GetPresetsResponse presets = null;
            try
            {
                WriteLine($"Get Ptz Presets...");
                presets = await Ptz.GetPresetsAsync(Profile_token);
                WriteLine("Presets count: " + presets?.Preset?.Length);

                _ = Task.Run(() =>
                {
                    foreach (var preset in presets?.Preset)
                    {
                        var pan = preset?.PTZPosition?.PanTilt?.x;
                        var tilt = preset?.PTZPosition?.PanTilt?.y;
                        var zoom = preset?.PTZPosition?.Zoom?.x;

                        WriteLine($"Preset: {preset?.token} Name: {preset?.Name} Pan: {pan} Tilt: {tilt} Zoom: {zoom}");

                        if (token.IsCancellationRequested) break;
                    }
                });

                return presets;
            }
            catch (TaskCanceledException)
            {
                WriteLine($"Task({nameof(GetPresets)}) was Finished by Cancellation");
                return presets;
            }
            catch (Exception ex)
            {
                WriteLine($"Raised Exception during {nameof(GetPresets)} : " + ex.ToString());
                return null;
            }

        }

        public Task GotoPreset(PTZPreset preset, CancellationToken token = default)
        {
            if (!(CheckPtz || CheckProfileToken))
                return Task.CompletedTask;

            return Task.Run(async () =>
            {
                try
                {
                    var pan = preset?.PTZPosition?.PanTilt?.x;
                    var tilt = preset?.PTZPosition?.PanTilt?.y;
                    var zoom = preset?.PTZPosition?.Zoom?.x;

                    WriteLine($"Move Ptz presets...");
                    WriteLine($"Preset: {preset?.token} Name: {preset?.Name} Pan: {pan} Tilt: {tilt} Zoom: {zoom}");
                    await Ptz?.GotoPresetAsync(Profile_token, preset.token, null);
                    await Task.Delay(2000, token);
                }
                catch (TaskCanceledException)
                {
                    WriteLine($"Task({nameof(GotoPreset)}) was Finished by Cancellation");
                }
                catch (Exception ex)
                {
                    WriteLine($"Raised Exception during {nameof(GotoPreset)} : " + ex.ToString());
                }
            }, token);
        }

        public Task<bool> StopPreset(bool stopPanTile = true, bool stopZoom = true)
        {
            if (!(CheckPtz || CheckProfileToken))
                return Task.FromResult(false);

            return Task.Run(async () =>
            {
                try
                {
                    await Ptz?.StopAsync(Profile_token, stopPanTile, stopZoom);
                    return true;
                }
                catch (TaskCanceledException)
                {
                    WriteLine($"Task({nameof(StopPreset)}) was Finished by Cancellation");
                    return true;
                }
                catch (Exception ex)
                {
                    WriteLine($"Raised Exception during {nameof(StopPreset)} : " + ex.ToString());
                    return false;
                }

            });
        }

        public Task<PTZStatus> GetPtzStatus()
        {
            if (!(CheckPtz || CheckProfileToken))
                return null;

            return Task.Run(async () =>
            {
                try
                {
                    return await Ptz.GetStatusAsync(Profile_token);
                }
                catch (Exception ex)
                {
                    WriteLine($"Raised Exception during {nameof(GetPtzStatus)} : " + ex.ToString());
                    return null;
                }
            });
        }

        public bool CheckPtz => Ptz != null ? true : false;

        public bool CheckProfileToken => Profile_token != null ? true : false;

        public bool CheckDevice => Device != null ? true : false;

        public bool CheckMedia => Media != null ? true : false;

        public bool CheckImaging => Imaging != null ? true : false;

        public bool CheckCaps => Caps != null ? true : false;

        public bool CheckFocus_opts => Focus_opts != null ? true : false;

        public bool CheckImaging_opts => Imaging_opts != null ? true : false;

        public bool CheckVSource_token => VSource_token != null ? true : false;

        public async Task MainAsync(string host, string username, string password, CancellationToken token = default)
        {
            try
            {
                var device = await OnvifClientFactory.CreateDeviceClientAsync(host, username, password);
                var media = await OnvifClientFactory.CreateMediaClientAsync(host, username, password);
                var ptz = await OnvifClientFactory.CreatePTZClientAsync(host, username, password);
                var imaging = await OnvifClientFactory.CreateImagingClientAsync(host, username, password);
                var caps = await device.GetCapabilitiesAsync(new CapabilityCategory[] { CapabilityCategory.All });
                bool absolute_move = false;
                bool relative_move = false;
                bool continuous_move = false;
                bool focus = false;


                WriteLine("Capabilities");
                WriteLine("\tDevice: " + caps.Capabilities.Device.XAddr);
                WriteLine("\tEvents: " + caps.Capabilities.Events.XAddr);
                WriteLine("\tImaging: " + caps.Capabilities.Imaging.XAddr);
                WriteLine("\tMedia: " + caps.Capabilities.Media.XAddr);
                WriteLine("\tPTZ: " + caps.Capabilities.PTZ.XAddr);

                if (token.IsCancellationRequested)
                    return;


                string profile_token = null;

                try
                {
                    var profiles = await media.GetProfilesAsync();

                    WriteLine("Profiles count :" + profiles.Profiles.Length);

                    foreach (var profile in profiles?.Profiles)
                    {
                        WriteLine($"Profile: {profile.token}");

                        if (profile_token == null)
                        {
                            profile_token = profile.token;
                            absolute_move = !string.IsNullOrWhiteSpace(profile.PTZConfiguration.DefaultAbsolutePantTiltPositionSpace);
                            relative_move = !string.IsNullOrWhiteSpace(profile.PTZConfiguration.DefaultRelativePanTiltTranslationSpace);
                            continuous_move = !string.IsNullOrWhiteSpace(profile.PTZConfiguration.DefaultContinuousPanTiltVelocitySpace);
                        }

                        WriteLine($"\tTranslation Support");
                        WriteLine($"\t\tAbsolute Translation: {!string.IsNullOrWhiteSpace(profile.PTZConfiguration.DefaultAbsolutePantTiltPositionSpace)}");
                        WriteLine($"\t\tRelative Translation: {!string.IsNullOrWhiteSpace(profile.PTZConfiguration.DefaultRelativePanTiltTranslationSpace)}");
                        WriteLine($"\t\tContinuous Translation: {!string.IsNullOrWhiteSpace(profile.PTZConfiguration.DefaultContinuousPanTiltVelocitySpace)}");

                        if (!string.IsNullOrWhiteSpace(profile.PTZConfiguration.DefaultRelativePanTiltTranslationSpace))
                        {
                            var pan = profile.PTZConfiguration.PanTiltLimits.Range.XRange;
                            var tilt = profile.PTZConfiguration.PanTiltLimits.Range.YRange;
                            var zoom = profile.PTZConfiguration.ZoomLimits.Range.XRange;

                            WriteLine($"\tPan Limits: [{pan.Min}, {pan.Max}] Tilt Limits: [{tilt.Min}, {tilt.Max}] Zoom Limits: [{zoom.Min}, {zoom.Max}]");
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLine("Raised Exception in media.GetProfilesAsync : " + ex.ToString());
                }


                if (token.IsCancellationRequested)
                    return;

                var configs = await ptz.GetConfigurationsAsync();

                foreach (var config in configs.PTZConfiguration)
                {
                    WriteLine($"PTZ Configuration: {config.token}");
                }

                if (token.IsCancellationRequested)
                    return;

                var metaDataConfig = await media.GetMetadataConfigurationAsync(profile_token);
                var video_sourcesConfig = await media.GetVideoSourceConfigurationsAsync();

                foreach (var config in video_sourcesConfig.Configurations)
                {

                    var video_sourcesConfigOption = await media.GetVideoSourceConfigurationOptionsAsync(config.token, profile_token);

                }



                var video_sources = await media.GetVideoSourcesAsync();
                string vsource_token = null;

                // Part Video Sources
                foreach (var source in video_sources.VideoSources)
                {
                    WriteLine($"Video Source: {source.token}");

                    try
                    {
                        if (vsource_token == null)
                        {
                            vsource_token = source?.token;
                            focus = source?.Imaging?.Focus != null;
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLine("Raised Exception in Video Sources , source.Imaging.Focus : " + ex.ToString());
                    }

                    WriteLine($"\tVideo Mode");
                    await media.GetVideoSourceModesAsync(vsource_token);

                    WriteLine($"\tFramerate: {source?.Framerate}");
                    WriteLine($"\tResolution: {source?.Resolution?.Width}x{source?.Resolution?.Height}");

                    WriteLine($"\tFocus Settings");

                    if (source?.Imaging?.Focus == null)
                    {
                        WriteLine($"\t\tNone");
                    }
                    else
                    {
                        WriteLine($"\t\tMode: {source?.Imaging?.Focus?.AutoFocusMode}");
                        WriteLine($"\t\tNear Limit: {source?.Imaging?.Focus?.NearLimit}");
                        WriteLine($"\t\tFar Limit: {source?.Imaging?.Focus?.FarLimit}");
                        WriteLine($"\t\tDefault Speed: {source?.Imaging?.Focus?.DefaultSpeed}");
                    }

                    WriteLine($"\tExposure Settings");

                    if (source?.Imaging?.Exposure == null)
                    {
                        WriteLine($"\t\tNone");
                    }
                    else
                    {
                        WriteLine($"\t\tMode: {source?.Imaging?.Exposure?.Mode}");
                        WriteLine($"\t\tMin Iris: {source?.Imaging?.Exposure?.MinIris}");
                        WriteLine($"\t\tMax Iris: {source?.Imaging?.Exposure?.MaxIris}");
                    }

                    WriteLine($"\tImage Settings");

                    var imaging_opts = await imaging.GetOptionsAsync(source.token);

                    WriteLine($"\t\tBrightness: {source?.Imaging?.Brightness} [{imaging_opts?.Brightness?.Min}, {imaging_opts?.Brightness?.Max}]");
                    WriteLine($"\t\tColor Saturation: {source?.Imaging?.ColorSaturation} [{imaging_opts?.ColorSaturation?.Min}, {imaging_opts?.ColorSaturation?.Max}]");
                    WriteLine($"\t\tContrast: {source?.Imaging?.Contrast} [{imaging_opts?.Contrast?.Min}, {imaging_opts?.Contrast?.Max}]");
                    WriteLine($"\t\tSharpness: {source?.Imaging?.Sharpness} [{imaging_opts?.Sharpness?.Min}, {imaging_opts?.Sharpness?.Max}]");
                }

                if (token.IsCancellationRequested)
                    return;

                if (focus)
                {
                    WriteLine($"Focus");

                    var image_status = await imaging.GetStatusAsync(vsource_token);

                    WriteLine($"\tStatus");

                    WriteLine($"\t\tPosition: {image_status?.FocusStatus20?.Position}");
                    WriteLine($"\t\tStatus: {image_status?.FocusStatus20?.MoveStatus}");
                    WriteLine($"\t\tError: {image_status?.FocusStatus20?.Error}");

                    WriteLine($"\tSetting Focus Mode: Manual");

                    var image_settings = await imaging.GetImagingSettingsAsync(vsource_token);

                    if (image_settings?.Focus == null)
                    {
                        image_settings.Focus = new FocusConfiguration20
                        {
                            AutoFocusMode = AutoFocusMode.MANUAL
                        };

                        await imaging.SetImagingSettingsAsync(vsource_token, image_settings, false);
                    }
                    else if (image_settings.Focus.AutoFocusMode != AutoFocusMode.MANUAL)
                    {
                        image_settings.Focus.AutoFocusMode = AutoFocusMode.MANUAL;
                        await imaging.SetImagingSettingsAsync(vsource_token, image_settings, false);
                    }
                }

                if (token.IsCancellationRequested)
                    return;

                var focus_opts = await imaging.GetMoveOptionsAsync(vsource_token);

                if (focus_opts.Absolute != null)
                {
                    WriteLine($"\tMoving Focus Absolute");

                    await imaging.MoveAsync(vsource_token, new FocusMove
                    {
                        Absolute = new AbsoluteFocus
                        {
                            Position = 0f,
                            //Speed = 1f,
                            //SpeedSpecified = true
                        }
                    });

                    await Task.Delay(500);
                }

                if (token.IsCancellationRequested)
                    return;

                if (focus_opts.Relative != null)
                {
                    WriteLine($"\tMoving Focus Relative");

                    await imaging.MoveAsync(vsource_token, new FocusMove
                    {
                        Relative = new RelativeFocus
                        {
                            Distance = 0.01f,
                            //Speed = 1f,
                            //SpeedSpecified = true
                        }
                    });

                    await Task.Delay(500);
                }

                if (token.IsCancellationRequested)
                    return;

                if (focus_opts.Continuous != null)
                {
                    WriteLine($"\tMoving Focus Continuous...");

                    await imaging.MoveAsync(vsource_token, new FocusMove
                    {
                        Continuous = new ContinuousFocus
                        {
                            Speed = 1f
                        }
                    });

                    await Task.Delay(500);

                    await imaging.StopAsync(vsource_token);
                }

                if (token.IsCancellationRequested)
                    return;

                var ptz_status = await ptz.GetStatusAsync(profile_token);

                WriteLine($"Position: [{ptz_status?.Position?.PanTilt?.x}, {ptz_status?.Position?.PanTilt?.y}, {ptz_status?.Position?.Zoom?.x}]");
                WriteLine($"Pan/Tilt Status: {ptz_status?.MoveStatus?.PanTilt} Zoom Status: {ptz_status?.MoveStatus?.Zoom}");

                if (absolute_move)
                {
                    WriteLine($"Absolute Move...");

                    await ptz.AbsoluteMoveAsync(profile_token, new PTZVector
                    {
                        PanTilt = new Vector2D
                        {
                            x = 0.5f,
                            y = 0
                        },
                        Zoom = new Vector1D
                        {
                            x = 0f
                        }
                    }, new PTZSpeed
                    {
                        PanTilt = new Vector2D
                        {
                            x = 1f,
                            y = 1f
                        },
                        Zoom = new Vector1D
                        {
                            x = 0f
                        }
                    });

                    await Task.Delay(3000, token);

                    if (token.IsCancellationRequested)
                        return;

                    ptz_status = await ptz.GetStatusAsync(profile_token);

                    WriteLine($"Position: [{ptz_status?.Position?.PanTilt?.x}, {ptz_status?.Position?.PanTilt?.y}, {ptz_status?.Position?.Zoom?.x}]");
                    WriteLine($"Pan/Tilt Status: {ptz_status?.MoveStatus?.PanTilt} Zoom Status: {ptz_status?.MoveStatus?.Zoom}");
                }

                if (relative_move)
                {
                    WriteLine($"Relative Move...");

                    await ptz.RelativeMoveAsync(profile_token,
                    new PTZVector
                    {
                        PanTilt = new Vector2D
                        {
                            x = 0.1f,
                            y = 0.1f
                        },
                        Zoom = new Vector1D
                        {
                            x = 0.1f
                        }
                    }, new PTZSpeed
                    {
                        PanTilt = new Vector2D
                        {
                            x = 0.1f,
                            y = 0.1f
                        },
                        Zoom = new Vector1D
                        {
                            x = 0.1f
                        }
                    });

                    await Task.Delay(3000, token);

                    if (token.IsCancellationRequested)
                        return;

                    ptz_status = await ptz.GetStatusAsync(profile_token);

                    WriteLine($"Position: [{ptz_status?.Position?.PanTilt?.x}, {ptz_status?.Position?.PanTilt?.y}, {ptz_status?.Position?.Zoom?.x}]");
                    WriteLine($"Pan/Tilt Status: {ptz_status?.MoveStatus?.PanTilt} Zoom Status: {ptz_status?.MoveStatus?.Zoom}");
                }

                if (continuous_move)
                {
                    WriteLine($"Continuous Move...");

                    await ptz.ContinuousMoveAsync(profile_token, new PTZSpeed
                    {
                        PanTilt = new Vector2D
                        {
                            x = 0,
                            y = -1
                        },
                        Zoom = new Vector1D
                        {
                            x = 0
                        }
                    }, null);

                    await Task.Delay(1500, token);

                    if (token.IsCancellationRequested)
                        return;

                    await ptz.StopAsync(profile_token, true, true);

                    ptz_status = await ptz.GetStatusAsync(profile_token);

                    WriteLine($"Position: [{ptz_status?.Position?.PanTilt?.x}, {ptz_status?.Position?.PanTilt?.y}, {ptz_status?.Position?.Zoom?.x}]");
                    WriteLine($"Pan/Tilt Status: {ptz_status?.MoveStatus?.PanTilt} Zoom Status: {ptz_status?.MoveStatus?.Zoom}");
                }

                if (token.IsCancellationRequested)
                    return;

                var presets = await ptz.GetPresetsAsync(profile_token);

                WriteLine("Presets count: " + presets?.Preset?.Length);

                foreach (var preset in presets?.Preset)
                {
                    var pan = preset?.PTZPosition?.PanTilt?.x;
                    var tilt = preset?.PTZPosition?.PanTilt?.y;
                    var zoom = preset?.PTZPosition?.Zoom?.x;

                    WriteLine($"Preset: {preset?.token} Name: {preset?.Name} Pan: {pan} Tilt: {tilt} Zoom: {zoom}");

                    await ptz?.GotoPresetAsync(profile_token, preset.token, null);
                    await Task.Delay(1500, token);

                    if (token.IsCancellationRequested)
                        return;
                    //await ptz.RemovePresetAsync (profile_token, preset.token);
                }

                if (presets.Preset.Length == 0)
                {
                    var new_preset = await ptz.SetPresetAsync(new SetPresetRequest
                    {
                        ProfileToken = profile_token,
                        PresetName = "P1"
                    });

                    WriteLine($"New Preset: {new_preset.PresetToken}");
                }
            }
            catch (Exception ex)
            {
                WriteLine("Raised Exception in MainAsync : " + ex.ToString());
            }

        }

        private void WriteLine(string data)
        {
            //Debug.WriteLine(data);
            SendLog?.Invoke(data);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public DeviceClient Device { get; private set; }
        public MediaClient Media { get; private set; }
        public PTZClient Ptz { get; private set; }
        public ImagingClient Imaging { get; private set; }
        public GetCapabilitiesResponse Caps { get; private set; }
        public bool Absolute_move { get; private set; }
        public bool Relative_move { get; private set; }
        public bool Continuous_move { get; private set; }
        public bool Focus { get; private set; }
        public string Profile_token { get; private set; }
        public string VSource_token { get; private set; }
        public ImagingOptions20 Imaging_opts { get; private set; }
        public MoveOptions20 Focus_opts { get; private set; }
        public ImagingSettings20 Image_settings { get; private set; }

        public delegate void LogEvent(string data);
        public event LogEvent SendLog;
        #endregion
        #region - Attributes -
        #endregion
    }
}
