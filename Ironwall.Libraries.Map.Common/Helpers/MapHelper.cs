using System.IO;
using System;
using System.Diagnostics;

namespace Ironwall.Libraries.Map.Common.Helpers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 5/26/2023 2:45:11 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public static class MapHelper
    {

        public static bool IsMapFileExist(string url)
        {
            string mapFile = CreateFullUrl(url);

            string folder = url.Split('\\')[0];
            if (File.Exists(mapFile))
            {
                Debug.WriteLine($"{url} exists in the {folder} directory.");
                return true;
            }
            else
            {
                Debug.WriteLine($"{url} does not exist in the {folder} directory.");
                return false;
            }
        }

        public static string CreateFullUrl(string url)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            return Path.Combine(currentDirectory, url);
        }
    }
}
