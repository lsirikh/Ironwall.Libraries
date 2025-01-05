using Caliburn.Micro;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Onvif.Models;
using Mictlanix.DotNet.Onvif;
using Mictlanix.DotNet.Onvif.Common;
using Mictlanix.DotNet.Onvif.Device;
using Mictlanix.DotNet.Onvif.Imaging;
using Mictlanix.DotNet.Onvif.Media;
using Mictlanix.DotNet.Onvif.Ptz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnvifControl.Libraries.Onvif.Services
{
    public class OnvifControlService : IOnvifControlService
    {
        

        public OnvifControlService()
        {
            _log = IoC.Get<ILogService>();
        }

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


        public Task<bool> DeviceReady(string host, string username, string password, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    Device = await OnvifClientFactory.CreateDeviceClientAsync(host, username, password);
                    if (token.IsCancellationRequested) return false;

                    Caps = await Device.GetCapabilitiesAsync(new CapabilityCategory[] { CapabilityCategory.All });
                    if (token.IsCancellationRequested) return false;

                    //WriteLine("Capabilities");
                    //WriteLine("\tDevice: " + Caps?.Capabilities?.Device?.XAddr);
                    //WriteLine("\tEvents: " + Caps?.Capabilities?.Events?.XAddr);
                    //WriteLine("\tImaging: " + Caps?.Capabilities?.Imaging?.XAddr);
                    //WriteLine("\tMedia: " + Caps?.Capabilities?.Media?.XAddr);
                    //WriteLine("\tPTZ: " + Caps?.Capabilities?.PTZ?.XAddr);

                    if(Caps?.Capabilities?.Media != null)
                        Media = await OnvifClientFactory.CreateMediaClientAsync(host, username, password);
                    if (token.IsCancellationRequested) return false;
                    
                    if(Caps?.Capabilities?.Imaging != null)
                        Imaging = await OnvifClientFactory.CreateImagingClientAsync(host, username, password);
                    if (token.IsCancellationRequested) return false;
                    
                    if(Caps?.Capabilities?.PTZ != null)
                        Ptz = await OnvifClientFactory.CreatePTZClientAsync(host, username, password);
                    if (token.IsCancellationRequested) return false;

                    Absolute_move = false;
                    Relative_move = false;
                    Continuous_move = false;
                    Focus = false;
                }
                catch (TaskCanceledException)
                {
                    WriteLine($"Task({nameof(DeviceReady)}) was Finished by Cancellation");
                    return false;
                }
                catch (Exception ex)
                {
                    WriteLine("Raised Exception in DeviceReady : " + ex.ToString());
                    return false;
                }
                return true;
            }, token);
        }

        public Task CreateProfile(CancellationToken token = default)
        {
            if (!CheckMedia())
                return Task.CompletedTask;

            return Task.Run(async () =>
            {
                Profile_token = null;

                try
                {
                    var profiles = await Media.GetProfilesAsync();

                    WriteLine("Profiles count :" + profiles.Profiles.Length);

                    foreach (var profile in profiles?.Profiles)
                    {
                        WriteLine($"Profile: {profile?.token}");

                        if (token.IsCancellationRequested) return;

                        if (Profile_token == null)
                        {
                            Profile_token = profile?.token;
                            Absolute_move = !string.IsNullOrWhiteSpace(profile?.PTZConfiguration?.DefaultAbsolutePantTiltPositionSpace);
                            Relative_move = !string.IsNullOrWhiteSpace(profile?.PTZConfiguration?.DefaultRelativePanTiltTranslationSpace);
                            Continuous_move = !string.IsNullOrWhiteSpace(profile?.PTZConfiguration?.DefaultContinuousPanTiltVelocitySpace);
                        }

                        if (profile?.PTZConfiguration == null)
                            continue;

                        WriteLine($"\tTranslation Support");
                        WriteLine($"\t\tAbsolute Translation: {!string.IsNullOrWhiteSpace(profile?.PTZConfiguration?.DefaultAbsolutePantTiltPositionSpace)}");
                        WriteLine($"\t\tRelative Translation: {!string.IsNullOrWhiteSpace(profile?.PTZConfiguration?.DefaultRelativePanTiltTranslationSpace)}");
                        WriteLine($"\t\tContinuous Translation: {!string.IsNullOrWhiteSpace(profile?.PTZConfiguration?.DefaultContinuousPanTiltVelocitySpace)}");

                        if (!string.IsNullOrWhiteSpace(profile?.PTZConfiguration?.DefaultRelativePanTiltTranslationSpace))
                        {
                            var pan = profile?.PTZConfiguration?.PanTiltLimits?.Range?.XRange;
                            var tilt = profile?.PTZConfiguration?.PanTiltLimits?.Range?.YRange;
                            var zoom = profile?.PTZConfiguration?.ZoomLimits?.Range?.XRange;

                            WriteLine($"\tPan Limits: [{pan?.Min}, {pan?.Max}] Tilt Limits: [{tilt?.Min}, {tilt?.Max}] Zoom Limits: [{zoom?.Min}, {zoom?.Max}]");
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
            });
        }


        public Task MovePtz(int moveType, CancellationToken token = default)
        {
            if (!(CheckPtz() || CheckProfileToken()))
                return Task.CompletedTask;

            return Task.Run(async () =>
            {
                try
                {
                    if (Continuous_move)
                    {
                        //WriteLine($"PTZ Move Upward");
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
                            case 2://Down
                                {
                                    WriteLine($"PTZ Move Downward");
                                    panTilt = new Vector2D { x = 0, y = -1 };
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
                                    WriteLine($"PTZ Zoom In");
                                    panTilt = new Vector2D { x = 0, y = 0 };
                                    zoom = new Vector1D { x = 1 };
                                }
                                break;
                            case 6://ZoomOut
                                {
                                    WriteLine($"PTZ Zoom Out");
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

        public Task<Mictlanix.DotNet.Onvif.Ptz.GetPresetsResponse> GetPresets(CancellationToken token = default)
        {
            if (!(CheckPtz() || CheckProfileToken()))
                return null;

            return Task<Mictlanix.DotNet.Onvif.Ptz.GetPresetsResponse>.Run(async () =>
            {
                Mictlanix.DotNet.Onvif.Ptz.GetPresetsResponse presets = null;
                try
                {
                    WriteLine($"Get Ptz Presets...");
                    presets = await Ptz.GetPresetsAsync(Profile_token);
                    WriteLine("Presets count: " + presets?.Preset?.Length);

                    //foreach (var preset in presets?.Preset)
                    //{
                    //    var pan = preset?.PTZPosition?.PanTilt?.x;
                    //    var tilt = preset?.PTZPosition?.PanTilt?.y;
                    //    var zoom = preset?.PTZPosition?.Zoom?.x;

                    //    WriteLine($"Preset: {preset?.token} Name: {preset?.Name} Pan: {pan} Tilt: {tilt} Zoom: {zoom}");

                    //    if (token.IsCancellationRequested)
                    //        break;
                    //}

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

            }, token);
        }

        public Task<bool> GotoPreset(PTZPreset preset, CancellationToken token = default)
        {
            if (!(CheckPtz() || CheckProfileToken()))
                return Task<bool>.FromResult(false);

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
                    

                    return true;
                }
                catch (TaskCanceledException)
                {
                    WriteLine($"Task({nameof(GotoPreset)}) was Finished by Cancellation");
                    return true;
                }
                catch (Exception ex)
                {
                    WriteLine($"Raised Exception during {nameof(GotoPreset)} : " + ex.ToString());
                    return false;
                }

            }, token);
        }

        public Task<bool> StopPreset(bool stopPanTile = true, bool stopZoom = true)
        {
            if (!(CheckPtz() || CheckProfileToken()))
                return Task<bool>.FromResult(false);

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
            if (!(CheckPtz() || CheckProfileToken()))
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

        public async Task<string> IsPanTiltMoving()
        {
            var ptzStatus = await GetPtzStatus();
            return ptzStatus.MoveStatus.PanTilt.ToString();
        }

        public async Task<string> IsZoomMoving()
        {
            var ptzStatus = await GetPtzStatus();
            return ptzStatus.MoveStatus.Zoom.ToString();
        }

        public bool CheckPtz()
        {
            return Ptz != null ? true : false;
        }

        public bool CheckProfileToken()
        {
            return Profile_token != null ? true : false;
        }

        public bool CheckDevice()
        {
            return Device != null ? true : false;
        }

        public bool CheckMedia()
        {
            return Media != null ? true : false;
        }

        public bool CheckImaging()
        {
            return Imaging != null ? true : false;
        }

        public bool CheckCaps()
        {
            return Caps != null ? true : false;
        }

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

                    await ptz.RelativeMoveAsync(profile_token, new PTZVector
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

        public Task<IDeviceInfoModel> GetDeviceInfo(CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var req = new GetDeviceInformationRequest();
                    var res = await Device.GetDeviceInformationAsync(req);

                    IDeviceInfoModel deviceInfo = new DeviceInfoModel();
                    deviceInfo.FirmwareVersion = res?.FirmwareVersion;
                    deviceInfo.HardwareId = res?.HardwareId;
                    deviceInfo.Manufacturer = res?.Manufacturer;
                    deviceInfo.DeviceModel = res?.Model;
                    deviceInfo.SerialNumber = res?.SerialNumber;

                    return deviceInfo;
                }
                catch (Exception ex)
                {
                    WriteLine("Raised Exception in GetDeviceInfo : " + ex.ToString());
                    return null;
                }
            }
            , token);
        }
        public Task<IHostInfoModel> GetHostInfo(CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var res = await Device.GetHostnameAsync();
                    IHostInfoModel hostInfo = new HostInfoModel();
                    hostInfo.Extension = res?.Extension;
                    hostInfo.FromDHCP = res.FromDHCP;
                    hostInfo.Name = res?.Name;

                    return hostInfo;
                }
                catch (Exception ex)
                {
                    WriteLine("Raised Exception in GetHostInfo : " + ex.ToString());
                    return null;
                }
            }
            , token);
        }

        public Task<Dictionary<string, string>> GetProfileTokens(CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var res = await Media.GetProfilesAsync();
                    var list = new Dictionary<string, string>();

                    foreach (var profile in res.Profiles)
                    {
                        list.Add(profile.Name, profile.token);
                    }

                    return list;
                }
                catch (Exception ex)
                {
                    WriteLine("Raised Exception in GetProfileTokens : " + ex.ToString());
                    return null;
                }
            }
            , token);
        }

        public Task<IMediaUrlModel> GetRtspUri(string tokenProfile, CancellationToken token = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var setUpStream = new StreamSetup()
                    {
                        Stream = StreamType.RTPUnicast,
                        Transport = new Transport() { Protocol = TransportProtocol.RTSP }
                    };

                    var res = await Media?.GetStreamUriAsync(setUpStream, tokenProfile);

                    IMediaUrlModel mediaUri = new MediaUrlModel();
                    mediaUri.Any = res?.Any;
                    mediaUri.InvalidAfterReboot = res.InvalidAfterReboot;
                    mediaUri.InvalidAfterConnect = res.InvalidAfterConnect;
                    mediaUri.Timeout = res?.Timeout;
                    mediaUri.Uri = res?.Uri;

                    return mediaUri;
                }
                catch (Exception ex)
                {
                    WriteLine("Raised Exception in GetRtspUri : " + ex.ToString());
                    return null;
                }
            }
            , token);
        }

        private void WriteLine(string data)
        {
            _log.Info(data);
            SendLog?.Invoke(data);
        }

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

        public delegate void LogEvent(string data);

        public event LogEvent SendLog;

        private ILogService _log;
    }
}
