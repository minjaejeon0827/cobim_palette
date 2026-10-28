# CobimPalette

## 프로젝트 소개

`CobimPalette`는 국토교통부 COBIM 연구과제에서 개발한 **AutoCAD 2025 애드인 프로그램**이다. (.NET 8, `net8.0-windows`, WPF + WinForms)

AutoCAD 화면 안에 WebView2 기반의 "COBIM 탐색기" Palette(도킹 창)를 띄운다. 
이 Palette는 COBIM/HubBox 웹 탐색기(`http://explorer.cobim.kr:8100/...`)를 보여 주고, 
웹 페이지에서 C# 메서드를 직접 호출할 수 있게 해 준다.
그래서 웹 화면에서 COBIM Rest API를 통해 성과품(CDE) 작업(다운로드, 체크아웃, 체크인, 작업취소, 파일열기)을 실행할 수 있다.

단독(Stand Alone)으로는 실행할 수 없고, `acad.exe`에 로드되어야만 동작한다.

> 참고: git 저장소의 루트는 상위 폴더(`D:\minjae\cobim_palette`)이고, 이 솔루션은 그 아래 `CobimPalette/` 폴더에 있다.

## 빌드 / 실행 방법

```powershell
dotnet build CobimPalette.sln  # 또는 Visual Studio에서 빌드
```

- **AutoCAD 2025 설치 필수**: `C:\Program Files\Autodesk\AutoCAD 2025\` 경로에 설치되어 있어야 한다. `CobimPalette.csproj`가 `accoremgd`, `acdbmgd`, `acmgd`, `AcCui`, `AcWindows`, `AdWindows` dll을 이 경로로 고정(`HintPath`)해서 참조한다. 
(`Private=False`라서 출력 폴더로 복사되지 않습니다.)
- **빌드 후 복사(PostBuild) 주의**: 모든 프로젝트가 빌드가 끝나면 결과 dll을 아래의 **고정된 사용자 경로**로 복사한다.
  `C:\Users\bhjeon\AppData\Roaming\Autodesk\ApplicationPlugins\CobimPalette.bundle\`
  다른 PC나 다른 사용자 계정에서는 이 복사가 실패해서 빌드 전체가 실패한다. 하여 이 경로를 내 환경에 맞게 고치거나, 해당 폴더를 미리 만들어 두어야 한다.
- **디버깅**: `Properties/launchSettings.json`의 `AutoCAD2025` 실행 프로필을 사용하면 `acad.exe`가 실행된다. AutoCAD는 위 bundle 폴더에서 애드인을 불러온다. (또는 AutoCAD에서 `NETLOAD` 명령으로 dll을 직접 로드해도 된다.)
- 테스트 프로젝트와 린트(lint) 설정은 없다.

## 솔루션 구성 (참조 순서)

| 프로젝트 | 참조 | 역할 |
| --- | --- | --- |
| `CobimNet` | – | MVVM 기본 클래스. `BindableBase`(`Changed`, `SetAndNotify`, static `StaticChanged`), `PropertyChangedDisableSection` |
| `CobimUtil` | – | 공통 헬퍼, 로거 (Rest.Api에서 사용) |
| `CobimPalette.Data.Models` | CobimNet | 기능별 요청/응답 데이터 모델 (`CobimBase/{Login,HubBox,WebView2Explorer,AutoCAD}`) |
| `CobimPalette.Rest.Api` | Data.Models, CobimUtil | Rest API 통신 |
| `CobimPalette` | 위 프로젝트 전부 | 애드인 dll 본체 |

- **데이터 모델**은 기능마다 `T<이름>View<TData>` / `<이름>View` 클래스 한 쌍으로 되어 있다. 
값은 **수동 의존성 주입(DI)** 방식으로 채운다. 즉, 뷰모델이 Rest API를 호출하기 전에 `InputParamData.SetData(new RequestHeader(...), new ContainerPack(...), new FilePack(...))`를 호출해서 입력값을 넣어 준다.
- **Rest API 계층**은 실제 HTTP 호출을 하는 static 클래스 `*RestServer`(`WebView2ExplorerRestServer`, `HubBoxRestServer`)와, API 주소·상수를 모아 둔 `*Helper` 클래스로 나뉜다. 서버 주소는 `HubBoxHelper`에 있다.

## 애드인 구조 (CobimPalette 프로젝트)

### 진입점

- `App.cs`: `IExtensionApplication` 구현 클래스이다. (`[assembly: ExtensionApplication]`) Revit 애드인의 `Application.cs`와 비슷한 역할이다.
- `Command.cs`: AutoCAD 명령어(`[CommandMethod]`)를 모아 둔 클래스이다.
현재 실제로 쓰는 명령어는 `CmdWebExplorer`이고, 나머지는 주석 처리되어 있다.

### 시작 흐름 (`App.Initialize`)

1. `AppDomain.AssemblyResolve` 이벤트에 `LoadFromCobimPalette` 연결
   → `AnimatedImage.Wpf` 같은 NuGet dll을 애드인 dll과 같은 폴더에서 찾도록 한다. AutoCAD 2025부터 .NET Core 기반으로 바뀌면서 dll을 `acad.exe` 폴더에서 찾기 때문에 꼭 필요하다.
2. Serilog 로그 설정
3. `ConfigureAppSetting()` 실행 (HttpClient, Window, Palette 객체 생성 및 Palette 등록)
4. 리본 메뉴 생성 (`Common/RibbonBase/Ribbon.CreateRibbonControl`). 리본이 아직 준비되지 않았으면 `ComponentManager.ItemInitialized` 이벤트에서 나중에 생성한다.
5. `DocumentCreated` / `DocumentActivated` / `DocumentBecameCurrent` 이벤트 연결 → 도면이 바뀔 때마다 WebView2 Palette를 다시 띄운다.

### 전역 설정

`Settings/AppSetting.Default`는 처음 사용할 때 생성되는(lazy) 싱글톤이며, 하위 설정들을 모아 둔다.
`HttpClientBase`(HttpClient 싱글톤), `LoginBase`, `DirectoryBase`, `PaletteBase`, `WindowBase`, `ImagineBuilderBase`
→ 모든 설정은 `AppSetting.Default.<X>Base` 형태로 접근한다.

### Palette

`PaletteSetting`과 `Common/Extensions/PaletteExtensions`(`PaletteSetManager`, `CustomPaletteSet`)가 화면 뷰를 싱글톤으로 만들고,
**제목 + GUID**로 등록하고 화면에 띄운다. 제목, GUID, 뷰 유형 이름 같은 상수는 `Common/CobimBase/CobimHelper.cs`에 있다.

**Palette 새로 추가하려면:**
1. `CobimHelper`에 상수(제목, GUID, 뷰 유형) 추가
2. `PaletteSetting`에 뷰 필드 추가 및 `CreatePalette` / `GetPalette` / `RegisterPalette`에 case 추가
3. `App.ConfigureAppSetting`에서 `RegisterPalette` 호출 추가

HubBox Palette는 코드가 있지만 등록 부분이 주석 처리되어 있다.

### 리본 버튼 → 명령어

리본 버튼은 `Commands/RibbonCommand`를 사용하며, 클릭하면 `SendStringToExecute(button.CommandParameter)`를 호출한다.
그래서 `RibbonHelper`에 있는 파라미터 문자열이 `Command.cs`의 `[CommandMethod]` 이름과 **정확히 같아야** 한다.

### MVVM 폴더 구조

- `Views/`와 `ViewModels/`는 같은 폴더 구조(`Pages/Palettes/...`, `Part/...`, `Windows/...`)를 가진다.
- 뷰는 이름 끝에 `V`, 뷰모델은 `VM`을 붙인다.
- 기본 클래스는 `Services/`에 있다. (`CobimScreen` → `PageBase` / `PageBase<TData>`, `PartBase`)

### WebView2 ↔ C# 연동 (핵심 기능)

`WebView2ExplorerV.xaml.cs`에서 다음 순서로 초기화한다.

1. `CoreWebView2Environment` 생성 (사용자 데이터 폴더: `%LOCALAPPDATA%\TestWV`)
2. `HubBoxHelper.requestLoginURL`(로그인 페이지)로 이동
3. `DataContext = new WebView2ExplorerVM()` 설정
4. `AddHostObjectToScript("boundAsync", vm)`로 뷰모델을 웹 페이지에 등록

이후 웹 페이지의 마우스 우클릭 메뉴에서 뷰모델의 public 비동기 메서드(`DownLoadAsync`, `CheckOutAsync`, `CheckInAsync`, `CancelWorkAsync`, `OpenFileAsync`)를 호출한다.

**이 메서드들을 만들거나 고칠 때 지켜야 할 규칙:**
- 뷰모델에 `[ComVisible(true)]`, `[ClassInterface(AutoDual)]` 특성을 꼭 유지하고, **제네릭이 아닌** `PageBase`를 상속해야 한다. `PageBase<WebView2ExplorerView>`를 상속하면 웹 페이지와 연결이 끊어진다.
- 메서드 파라미터는 `object`가 아니라 `string`(JSON 문자열)이어야 한다. 받은 문자열은 `System.Text.Json`으로 역직렬화한다.
- 반환 타입은 `Task<string>`이다. 이 메서드 안에서 화면(UI)을 다룰 때는 `Dispatcher.Invoke`를 사용한다.

## 코딩 규칙

- **언어/인코딩**: 주석과 사용자에게 보여 주는 문구는 한국어로 작성한다. 대부분의 파일은 UTF-8(BOM 포함)이다. 
`App.cs`와 `CobimNet/BindableBase.cs`는 한국어 주석이 이미 깨져 있다(U+FFFD 문자 포함).
저장소 전체의 인코딩을 한꺼번에 고치려 하지 말고, 새로 수정하는 내용은 UTF-8로 저장한다.
- **예외 처리 패턴**: 모든 메서드가 아래 형태를 따릅니다.
  ```csharp
  var currentMethod = MethodBase.GetCurrentMethod();

  try { ... }

  catch (Exception ex)
  {
      Log.Error(Logger.GetMethodPath(currentMethod) + Logger.errorMessage + ex.Message);
      ...
  }
  ```
  - 최상위 처리부(이벤트, 명령어, 웹 페이지에서 호출되는 메서드)는 `MessageBox.Show(ex.Message, CobimHelper.ErrorTitle)`로 오류를 보여 준다.
  - 내부/설정 메서드는 `throw;`로 호출한 쪽에 예외를 넘긴다.
- **코드 정리 방식**: 멤버마다 `#region <이름>` / `#endregion <이름>`으로 묶고, 파일 끝에는 빈 `#region Sample`을 둔다.
사용하지 않는 기능은 지우지 않고 주석으로 남겨 두며, 날짜가 들어간 `// TODO : ... (yyyy.MM.dd minjae)` 메모를 함께 적는다.
- **로그 파일**: Serilog 파일 로그가 `Documents\COBIM\CobimPalette\Logs\CobimPalette_.log`에 저장된다. (최대 30개 파일 보관)

### 참고
- [COBIM BIM 기반 인프라 발주-설계 프로세스 디지털 협업 체계 개발](https://scienceon.kisti.re.kr/srch/selectPORSrchReport.do?cn=TRKO202500010351)