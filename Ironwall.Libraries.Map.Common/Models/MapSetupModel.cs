using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Map.Common.Models
{
    public class MapSetupModel
    {
        public string TableCameras => Properties.Settings.Default.TableCameras;
        public string TableMaps => Properties.Settings.Default.TableMaps;
        public string TableControllers => Properties.Settings.Default.TableControllers;
        public string TableGeometries => Properties.Settings.Default.TableGeometies;
        public string TablePoints => Properties.Settings.Default.TablePoints;
        public string TableFences => Properties.Settings.Default.TableFences;
        public string TableMultiSensors => Properties.Settings.Default.TableMultiSensors;
        public string TableSymbols => Properties.Settings.Default.TableSymbols;
        public string TableObjects => Properties.Settings.Default.TableObjects;
        public string TableObjectInfo => Properties.Settings.Default.TableSymbolInfo;
    }
}
