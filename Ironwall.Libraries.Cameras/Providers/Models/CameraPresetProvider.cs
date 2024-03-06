using Ironwall.Framework.DataProviders;
using Ironwall.Libraries.Cameras.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Cameras.Providers.Models
{
    public class CameraPresetProvider
        : CameraBaseProvider
    {
        public CameraPresetProvider()
        {
            ClassName = nameof(CameraPresetProvider);
        }
    }
}
