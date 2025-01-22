using System;

namespace Ironwall.Libraries.Dotnet.Ollama.Ui.Helpers;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 1/14/2025 2:47:47 PM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
public static class FormatHelper
{
    // Helper method to format the file size
    public static string FormatFileSize(long sizeInBytes)
    {
        const double bytesPerGB = 1_000_000_000; // 1GB in bytes (decimal-based calculation)
        if (sizeInBytes >= bytesPerGB)
        {
            // Convert to GB and format with commas and two decimal places
            return $"{(sizeInBytes / bytesPerGB):N2} GB";
        }
        else
        {
            // If less than 1 GB, display the size in bytes with commas
            return $"{sizeInBytes:N0} bytes";
        }
    }

    // Helper method to format the file size
    public static string FormatParamSize(long sizeInParam)
    {
        const double sizePerBillion = 1_000_000_000; // 1GB in bytes (decimal-based calculation)
        if (sizeInParam >= sizePerBillion)
        {
            // Convert to GB and format with commas and two decimal places
            return $"{(sizeInParam / sizePerBillion):N2}B";
        }
        else
        {
            // If less than 1 GB, display the size in bytes with commas
            return $"{sizeInParam:N0}";
        }
    }
}