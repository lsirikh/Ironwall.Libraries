using Ironwall.Middleware.Message.Framework;
using Ironwall.Redis.Message.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace Ironwall.Libraries.WebApi.Utils
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/21/2023 2:36:12 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public static class PacketHelper
    {

        #region - Ctors -
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        static public string ObjString(String objectIP)
        {
            /* 
             * type : public
             * name : ObjString method
             * arg : PktJPIDS packet
             * return : string json
             * content : class DBManager would send json(string) data to Django API DB server 
             *           to string type data.
             */

            Dictionary<string, string> get_object = new Dictionary<string, string> { { "ip", objectIP } };
            string json = JsonConvert.SerializeObject(get_object, Formatting.Indented);
            return json;
        }

        //Packet Message Overloading
        //BrokerMessage Converting Process
        static public string JtoSConverter(BrkAction packetData)
        {
            /* 
             * type : static public
             * name : JtoSConverter method
             * arg : BrkAction redisData
             * return : string json
             * content : BrkAction class redisData would be converted 
             *           to string type data.
             */

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(packetData, settings);

            return json;
        }

        static public string JtoSConverter(BrkWindyMode packetData)
        {
            /* 
             * type : static public
             * name : JtoSConverter method
             * arg : BrkAction redisData
             * return : string json
             * content : BrkAction class redisData would be converted 
             *           to string type data.
             */

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(packetData, settings);

            return json;
        }

        static public string JtoSConverter(BrkConnection packetData)
        {
            /* 
             * type : static public
             * name : JtoSConverter method
             * arg : BrkConnection redisData
             * return : string json
             * content : BrkConnection class redisData would be converted 
             *           to string type data.
             */

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(packetData, settings);

            return json;
        }

        static public string JtoSConverter(BrkDectection packetData)
        {
            /* 
             * type : static public
             * name : JtoSConverter method
             * arg : BrkDectection redisData
             * return : string json
             * content : BrkDectection class redisData would be converted 
             *           to string type data.
             */

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(packetData, settings);

            return json;
        }

        static public string JtoSConverter(BrkMalfunction packetData)
        {
            /* 
             * type : static public
             * name : JtoSConverter method
             * arg : BrkMalfunction redisData
             * return : string json
             * content : BrkMalfunction class redisData would be converted 
             *           to string type data.
             */

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(packetData, settings);

            return json;
        }


        //Packet Message Overloading
        static public string JtoSConverter(PktJPIDS packetData)
        {
            /* 
             * type : static public
             * name : JtoSConverter method
             * arg : PktJPIDS packet
             * return : string json
             * content : PktJPIDS class redisData would be converted 
             *           to string type data.
             */

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(packetData, settings);

            return json;
        }

        //Packet Message Overloading
        //BrokerMessage Converting Process for BrkConnection(연결보고)
        static public string JtoSConverter(List<BrkConnection> dataList)
        {
            /* 
             * type : static public
             * name : JtoSConverter method
             * arg : List<BrkConnection> redisData
             * return : string json
             * content : List<BrkConnection> class redisData would be converted 
             *           to string type data.
             */

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(dataList, settings);

            return json;
        }

        //Packet Message Overloading
        //BrokerMessage Converting Process for BrkDectection(탐지보고)
        static public string JtoSConverter(List<BrkDectection> dataList)
        {
            /* 
             * type : static public
             * name : JtoSConverter method
             * arg : List<BrkDectection> redisData
             * return : string json
             * content : List<BrkDectection> class redisData would be converted 
             *           to string type data.
             */

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(dataList, settings);

            return json;
        }

        //Packet Message Overloading
        //BrokerMessage Converting Process for BrkMalfunction(고장보고)
        static public string JtoSConverter(List<BrkMalfunction> dataList)
        {
            /* 
             * type : static public
             * name : JtoSConverter method
             * arg : List<BrkMalfunction> redisData
             * return : string json
             * content : List<BrkMalfunction> class redisData would be converted 
             *           to string type data.
             */

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(dataList, settings);

            return json;
        }

        //Packet Message Overloading
        //BrokerMessage Converting Process for BrkContactOut(접점 신호 요청)
        static public string JtoSConverter(List<BrkContactOut> dataList)
        {
            /* 
             * type : static public
             * name : JtoSConverter method
             * arg : List<BrkContactOut> redisData
             * return : string json
             * content : List<BrkContactOut> class redisData would be converted 
             *           to string type data.
             */

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(dataList, settings);

            return json;
        }

        //BrokerMessage Converting Process for BrkObjRecognition(객체인식)
        static public string JtoSConverter(List<BrkObjRecognition> dataList)
        {
            /*
             * type : static public
             * name : JtoSConverter method
             * arg : List<BrkObjRecognition> redisData
             * return : string json
             * content : List<BrkObjRecognition> class redisData would be converted 
             *           to string type data.
             */

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(dataList, settings);

            return json;
        }

        //Packet Message Overloading
        //BrokerMessage Converting Process for BrkObjDetection(객체전용 탐지메시지)
        static public string JtoSConverter(BrkObjDetection data)
        {
            /* 
             * type : static public
             * name : JtoSConverter method
             * arg : BrkObjDetection Object Edge-node Data
             * return : string json
             * content : 
             */

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include
            };

            var json = JsonConvert.SerializeObject(data, settings);

            return json;
        }

        // 연결보고 패킷 변환 메소드
        static public string PktToBrkCon(PktConnection packet)
        {
            /* 
             * type : static public
             * name: PktToBrkCon method
             * arg : PktConnection packet
             * return : string
             * content : Convert PktConnection format data to PktConnection format
             * nameChannel : "Channel1"
             * note : Single Json data make Json data of List<PktConnection> type.
             */

            var data = new BrkConnection
            {
                /*
                 * IdCommand = idCommand,
                 * IdGroup = packet.IdGroup,
                 * IdController = packet.IdController,
                 * IdSensor = packet.IdSensor,
                 * TypeMessage = packet.TypeMessage,
                 * Sequence = packet.Sequence,
                 * TypeDevice = packet.TypeDevice,
                 */

                IdCommand = packet.TypeMessage,
                IdGroup = packet.IdGroup,
                IdController = packet.IdController,
                IdSensor = packet.IdSensor,
                TypeMessage = packet.TypeMessage,
                Sequence = packet.Sequence,
                TypeDevice = packet.TypeDevice,
            };
            List<BrkConnection> rDataList = new List<BrkConnection>();
            rDataList.Add(data);

            var str = JtoSConverter(rDataList);
            return str;
        }

        //고장보고 패킷 변환 메소드
        static public string PktToBrkMal(PktMalfunction packet)
        {
            /* 
             * type : static public
             * name: PktToBrkMal method
             * arg : PktMalfunction packet
             * return : string
             * content : Convert PktMalfunction format data to BktMalfunction format
             * nameChannel : "Channel1"
             * note : Single Json data make Json data of List<BktMalfunction> type.
             */

            var data = new BrkMalfunction
            {
                IdCommand = packet.TypeMessage,
                IdGroup = packet.IdGroup,
                IdController = packet.IdController,
                IdSensor = packet.IdSensor,
                TypeMessage = packet.TypeMessage,
                Sequence = packet.Sequence,
                TypeDevice = packet.TypeDevice,
                Reason = packet.Reason,
                CutFirstStart = packet.CutFirstStart,
                CutFirstEnd = packet.CutFirstEnd,
                CutSecondStart = packet.CutSecondStart,
                CutSecondEnd = packet.CutSecondEnd,
            };
            List<BrkMalfunction> rDataList = new List<BrkMalfunction>();
            rDataList.Add(data);

            var str = JtoSConverter(rDataList);
            return str;
        }


        //탐지보고 패킷 변환 메소드
        //가능 사용 안됨
        static public string PktToBktD(PktDetection packet)
        {

            /* 
             * type : static public
             * name: PktToBktD method
             * arg : PktJPIDS packet
             * return : void
             * content : Convert PktDetection format data to BktDetection format
             * nameChannel : "Channel1"
             * note : Single Json data make Json data of List<BktDetection> type.
             */

            //서인천발전소 : 객체인식 서버 전달(카메라쪽 정보?)
            //부산발전소   : 3D 관제 데이터 전달(센서데이터 정보)


            var data = new BrkDectection
            {
                IdCommand = packet.TypeMessage,
                IdGroup = packet.IdGroup,
                IdController = packet.IdController,
                IdSensor = packet.IdSensor,
                TypeMessage = packet.TypeMessage,
                TypeDevice = packet.TypeDevice,
                DetectionResult = packet.DetectionResult
            };
            List<BrkDectection> rDataList = new List<BrkDectection>();
            rDataList.Add(data);

            var str = JtoSConverter(rDataList);
            return str;
        }


        //탐지보고 패킷 변환 메소드
        //현재 카메라 정보 수집 후 송신 구조
        static public string PktToBktD(PktDetection packet, List<Camera> camList)
        {
            /* 
             * type : static public
             * name: PktToBktD method
             * arg : PktJPIDS packet
             * return : void
             * content : Convert PktDetection format data to BktDetection format
             * nameChannel : "Channel1"
             * note : Single Json data make Json data of List<BktDetection> type.
             */

            var data = new BrkDectection
            {
                IdCommand = packet.TypeMessage,
                IdGroup = packet.IdGroup,
                IdController = packet.IdController,
                IdSensor = packet.IdSensor,
                TypeMessage = packet.TypeMessage,
                TypeDevice = packet.TypeDevice,
                DetectionResult = packet.DetectionResult,
                Sequence = packet.Sequence,
            };
            List<BrkDectection> rDataList = new List<BrkDectection>();
            rDataList.Add(data);

            var str = JtoSConverter(rDataList);
            return str;
        }

        //객체용 탐지 패킷 변환 메소드
        //객체용 카메라 정보
        static public string PktToBktObjD(PktDetection packet, List<ObjCamera> camList)
        {
            /* 
             * type : static public
             * name: PktToBktObjD method
             * arg : PktJPIDS packet
             * return : void
             * content : Convert PktDetection format data to BrkObjDetection format
             * note : Single Json data make Json data of List<BrkObjDetection> type.
             */

            var data = new BrkObjDetection
            {
                IdGroup = packet.IdGroup,
                IdController = packet.IdController,
                IdSensor = packet.IdSensor,
                TypeMessage = packet.TypeMessage,
                Sequence = packet.Sequence,
                TypeDevice = packet.TypeDevice,
                DetectionSignal = packet.DetectionSignal,
                DetectionResult = packet.DetectionResult,
                ListCamera = camList
            };

            var str = JtoSConverter(data);
            return str;
        }


        //접점신호 요청 패킷 변환 메소드
        static public string PktToBktCOUT(PktContactOut packet)
        {
            /* 
             * type : static public
             * name: PktToBktCOUT method
             * arg : PktContactOut packet
             * return : string
             * content : Convert PktContactOut format data to BktDetection format
             * nameChannel : "Channel1"
             * note : Single Json data make Json data of List<BrkContactOut> type.
             */

            //서인천발전소 : 접점 방송 출력용
            var data = new BrkContactOut
            {
                IdCommand = packet.TypeMessage,
                IdGroup = packet.IdGroup,
                IdController = packet.IdController,
                IdSensor = packet.IdSensor,
                TypeMessage = packet.TypeMessage,
                Sequence = packet.Sequence,
                ReadWrite = packet.ReadWrite,
                SerialNumber = packet.Serial,
                ContactOutNumber = packet.ContactOutNumber,
                //ContactOutNumber = packet.IdSensor !=0 ? 1 : 2,
                ContactOutSignal = packet.ContactOutSignal
            };
            List<BrkContactOut> rDataList = new List<BrkContactOut>();
            rDataList.Add(data);

            var str = JtoSConverter(rDataList);
            return str;
        }

        // 객체 인식의 경우 활용하는 메소드
        static public List<BrkObjRecognition> StrToBktObjRecog(String json)
        {
            /* 
             * type : static public
             * name : Publish method
             * arg : String json
             * return : string
             * content : Convert PktDetection format data to BrkObjRecognition format
             * nameChannel : "Channel1"
             * note : Single Json data make Json data of List<BrkObjRecognition> type.
             */

            //서인천발전소 : 객체인식 서버 전달(카메라쪽 정보?)
            //부산발전소   : 3D 관제 데이터 전달(센서데이터 정보)

            try
            {
                var jsonList = JsonConvert.DeserializeObject<List<BrkObjRecognition>>(json);

                if (jsonList != null)
                {
                    jsonList.ForEach((item) =>
                    {
                        DateTime dt = new DateTime();

                        dt = DateTime.Now;
                        //2020-10-27 07:17:14

                        item.Time = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    });
                }

                //var str = JtoSConverter(jsonList);
                return jsonList;
            }
            catch
            {
                return null;
            }
        }

        // 객체에서 탐지로 패킷 전환
        static public List<BrkDectection> BrkObjToBrkDetect(List<BrkObjRecognition> brkObjRecog)
        {
            /* 
             * type : static public
             * name : BrkObjToBrkDetect
             * arg : List<BrkObjRecognition>
             * return : List<BrkDectection>
             * content : Convert List<BrkObjRecognition> format data to List<BrkDectection> format
             * nameChannel : "Channel1"
             * note : Single Json data make Json data of List<BrkDectection> type.
             */
            try
            {
                List<BrkDectection> brkDetectList = new List<BrkDectection>();

                if (brkObjRecog != null)
                {
                    brkObjRecog.ForEach((item) =>
                    {
                        BrkDectection brkDetection = new BrkDectection();

                        brkDetection.IdCommand = 90;
                        brkDetection.IdGroup = item.IdGroup;
                        brkDetection.IdController = item.IdController;
                        brkDetection.IdSensor = item.IdSensor;
                        brkDetection.TypeMessage = item.TypeMessage;
                        brkDetection.Sequence = item.Sequence;
                        brkDetection.TypeDevice = item.TypeDevice;
                        brkDetection.DetectionResult = item.DetectionResult;
                        brkDetection.Sequence = item.Sequence;

                        brkDetectList.Add(brkDetection);
                    });
                }
                return brkDetectList;
            }
            catch
            {
                return null;
            }

        }

        // 객체에서 탐지로 패킷 전환
        static public List<BrkObjDetection> BrkObjRecogToBrkObjDetect(List<BrkObjRecognition> brkObjRecog)
        {
            /* 
             * type : static public
             * name : BrkObjRecogToBrkObjDetect
             * arg : List<BrkObjRecognition>
             * return : List<BrkObjDetection>
             * content : Convert List<BrkObjRecognition> format data to List<BrkObjDetection> format
             */
            try
            {
                List<BrkObjDetection> brkObjDetectList = new List<BrkObjDetection>();

                if (brkObjRecog != null)
                {
                    brkObjRecog.ForEach((item) =>
                    {
                        BrkObjDetection brkObjDetection = new BrkObjDetection();

                        brkObjDetection.IdGroup = item.IdGroup;
                        brkObjDetection.IdController = item.IdController;
                        brkObjDetection.IdSensor = item.IdSensor;
                        brkObjDetection.TypeMessage = item.TypeMessage;
                        brkObjDetection.Sequence = item.Sequence;
                        brkObjDetection.TypeDevice = item.TypeDevice;
                        brkObjDetection.DetectionSignal = item.DetectionSignal;
                        brkObjDetection.DetectionResult = item.DetectionResult;

                        brkObjDetection.ListCamera = new List<ObjCamera>();

                        brkObjDetectList.Add(brkObjDetection);
                    });
                }
                return brkObjDetectList;
            }
            catch
            {
                return null;
            }

        }



        static public PktContactOut EntryToPktCOut(Entry entry, int mType, int selectNum, int selectSig)
        {

            /* 
             * type : static public
             * name : Publish method
             * arg : Entry entry, int selectNum, int selectSig
             * return : PktContactOut
             * content : Convert Entry instance data to PktContactOut format data
             * note : this method is used for converting entry data 
             *        into PktContactOut Packet to send MSG to M/W 
             */

            var packet = new PktContactOut();
            packet.IdGroup = 0x00;
            packet.IdController = Convert.ToByte(entry.IdController);//제어기
            packet.IdDestination = Convert.ToByte(entry.IdSensor);//탐지 센서 아이디
            packet.IdSensor = 0x00;//접점 센서의 아이디
            packet.TypeMessage = Convert.ToByte(mType);
            packet.Sequence = 0x00;
            packet.LengthData = Convert.ToByte(33);
            packet.SerialNumber = new byte[16];
            packet.ReadWrite = 0x01;
            packet.ContactOutNumber = Convert.ToByte(selectNum);
            packet.ContactOutSignal = Convert.ToByte(selectSig);

            return packet;
        }

        static public PktWindyMode BrkToPktWind(BrkWindyMode brkWindyMod)
        {
            /* 
             * type : static public
             * name : Publish method
             * arg : BrkWindyMode brkWindyMod
             * return : PktWindyMode
             * content : Convert BrkWindyMode json data to PktWindyMode data
             * note : this method is used for converting BrkWindyMode data
             *          into PktWindyMode data and sending this packet to Middleware to control
             *          WindyMode option
             */
            PktWindyMode packet = new PktWindyMode();
            packet.IdGroup = 0x00;
            packet.IdController = 0x00;
            packet.IdDestination = 0x00;
            packet.IdSensor = 0x00;
            packet.TypeMessage = Convert.ToByte(brkWindyMod.IdCommand);
            packet.Sequence = 0x00;
            packet.LengthData = 0x01;
            packet.ModeWindy = Convert.ToByte(brkWindyMod.ModeWindy);

            return packet;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        #endregion
    }
}
