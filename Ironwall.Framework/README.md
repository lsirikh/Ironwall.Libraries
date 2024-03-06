# Ironwall Framework

### Current Version : v2.0.0

### Goal
> Ironwall 프레임워크는 센서웨이사의 다양한 WPF 프로젝트에서 활요하는 기본 프레임워크입니다.
> * MVVM 프레임워크 라이브러리로 Caliburn.micro
> * IoC 프레임워크로 Autofac
> * 디자인 프레임워크로 Material Design, MahApp
> 등 다양한 라이브러리를 혼합하여, 효율적이고, 높은 재사용성과 Easy to use를 지향하고 있다. 

### Site : Common

### Update Date: 2022/07/19
* Version : v1.0.0

* 기능
    1) ViewModel Abstract 정의
    2) Service Abstract 정의
    3) 각종 Helper 및 Base 개념의 Parent 클래스 제공

### Update Date: 2023/01/06
* Version : v2.0.0

* 기능  
    1) DataProviders 기능 추가
        - BaseCommonProvider를 통한 Model - ViewModel Property Update 이벤트 구현(Collection Type)
        - InstanceFactory 구현 - 사실 타입에 맞게 팩토리 구현이 더 적절하다 판단하여, 점차 Deprecated 될 요소로 보임

    2) Helpers 기능 추가
        - EnumHelper로 EnumCmdType 구분 로직 구현
        - EnumLanguageHelper는 즉흥적 결정이라 Setting.Setting을 통해서 CultureInfo의 LanguageName에 맞게 변형되는 변수 타입으로 데이터를 사용하는 로직이 더 효율적이고 유지보수측면에서 유리할 것으로 보임
        - IdCodeGenerator는 Record로 등록될 다양한 데이터의 Id를 생성하는 기능을 수행함
        - ModelTypeHelper는 PIDS 시스템에 활용되는 이벤트 형식과 디바이스 형식을 쉽게 찾아주는 기능을 수행함
    
    3) Models 기능 추가
        - 이번 Server-Client 기능 분화로 새로 제작을 하면서 모델이 대폭 추가되었다.특히 계정과 Communication(TCP 통신)을 위한 다양한 모델과 그 모델의 데이터를 바로 저장할 수 있도록 돕는 Mapper모델 등 다양한 기능이 추가 되었다.

    4) 이번에 추가된 내용의 로직은 Ironwall_monitoring_server.drawio를 통해 자세히 확인 할 수 있다.

    5) ViewModels 기능 추가
        - Account 및 Device, Event 등 ViewModel형식을 취해 PropertyChange를 반영하여, GUI 연동을 유연하게 할 수 있도록 구현하였음.  

    6) ClassDiagram 정리를 통한 전체적인 연결 상태 확인
        - ClassDiagram 데이터가 깨져서 이미지화가 불가능한 상태로 됨
        - VS2022에서는 ClassDiagram Generate가 되지 않음
  
   