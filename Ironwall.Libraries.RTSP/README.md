# Ironwall RTSP with Onvif

## Site : Common

### Update Date: 2022/07/19
* Version : v1.0.0

* 사용 Nuget Package RTSPSharp
* 기능

    1) Wpf RTSP video View 제공
        
        ㄱ. 일부 기능 구현 시, Code behind 구조로 구현
            (behavior로 추후 업데이트 예정)

    2) Onvif 기능 일부 이용가능 (PTZ 컨트롤, Discovery 등...)

        ㄱ. Onvif.Solution Library를 이용

        ㄴ. Onvif 관련 항목을 Service로 이용가능

        ㄷ. service 제공 기능 - ptz control, authentication, preset, discovery 등

    3) Onvif.Solution Dependancy
