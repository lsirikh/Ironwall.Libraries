namespace Ironwall.Framework.Helpers
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 8/3/2023 4:01:42 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    static class GetIdHelper
    {

        static int GetControllerId(string id)
        {
            if(id == null) return 0;

            var pId = id?.Split('c');
            return int.Parse(pId[1]);
        }

        static int GetSensorId(string id)
        {
            if (id == null) return 0;

            var pId = id?.Split('s');
            return int.Parse(pId[1]);
        }

        static int GetCameraId(string id)
        {
            if (id == null) return 0;

            var pId = id?.Split('v');
            return int.Parse(pId[1]);
        }

        static int GetOptionId(string id)
        {
            if (id == null) return 0;

            var pId = id?.Split('o');
            return int.Parse(pId[1]);
        }

        static int GetMappingId(string id)
        {
            if (id == null) return 0;

            var pId = id?.Split('m');
            return int.Parse(pId[1]);
        }
    }
}
