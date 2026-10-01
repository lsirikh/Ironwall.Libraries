# Ironwall Libraries — .NET Framework

.NET Framework 기반 관제 프로그램의 공통 기능을 모은 저장소입니다. 계정·장치·이벤트·지도·영상·통신 코드를 분리해 WPF 애플리케이션에서 조합할 수 있도록 구성합니다.

## 주요 구성

- `Ironwall.Framework*`: 모델, ViewModel과 공통 기반
- `Ironwall.Libraries.Account*`, `Devices`, `Events`: 관제 데이터와 서비스
- `Ironwall.Libraries.Map.*`, `Device.UI`, `Event.UI`: 지도·장치·이벤트 화면
- `LibVlcRtsp.UI`, `OpenCvRtsp.UI`, `RTSP`, `Dotnet.Streaming.UI`: 영상 표시 관련 모듈
- `Redis`, `Tcp.*`, `Api.*`: 통신과 연동
- `CSource`: 네이티브 그래픽 연동 코드

## 개발 환경

주요 C# 프로젝트는 Windows / .NET Framework 4.8 / WPF를 사용합니다. Visual Studio의 해당 개발 도구와 각 프로젝트의 NuGet·네이티브 의존성이 필요합니다.

모듈별 프로젝트 참조와 외부 DLL 경로를 확인한 뒤 빌드해야 합니다. 모든 모듈을 단일 실행 프로그램으로 바로 실행하는 구조는 아닙니다. .NET 8 계열의 관련 코드는 [Ironwall.Dotnet.Libraries](https://github.com/lsirikh/Ironwall.Dotnet.Libraries)에 있습니다.

Sensorway / Ironwall 프로젝트 및 포함된 외부 소스의 출처와 라이선스 표기를 유지합니다.
