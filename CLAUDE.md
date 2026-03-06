# CLAUDE.md

이 파일은 Claude Code(claude.ai/code)가 본 저장소에서 작업할 때 참조하는 프로젝트 가이드입니다.
모든 응답과 코드 작성은 **한국어**로 진행합니다.

---

## 📌 프로젝트 개요

| 항목 | 내용 |
|------|------|
| 게임 이름 | **Mine Mine Mine** |
| 게임 장르 | 2D 도트아트 스타일 — 중력 기반 타일 파괴 게임 |
| 핵심 메카닉 | 곡괭이 오브젝트가 중력의 영향을 받아 낙하하며 바닥 타일을 파괴. 터치 입력으로 직접 타일 파괴 가능 |
| 플레이 방식 | 싱글 플레이어 |
| 플랫폼 | 모바일 (iOS / Android) + PC 크로스 플랫폼 |
| 서버 | 클라우드 서버 지원 예정 |
| 엔진 | Unity 6 (6000.3.10f1) |
| 개발 환경 | Visual Studio (Windows), JetBrains Rider (MacBook Air) |

---

## 🛠️ 기술 스택

- **렌더 파이프라인**: URP 2D (`com.unity.render-pipelines.universal` 17.3.0) — `Assets/Settings/` 에 설정
- **입력**: New Input System (`com.unity.inputsystem` 1.18.0) — `Assets/InputSystem_Actions.inputactions`
- **타일맵**: Tilemap + Extras (`com.unity.2d.tilemap`, `com.unity.2d.tilemap.extras`)
- **스프라이트**: 2D Animation / Aseprite / PSD Importer
- **에디터 확장**: Odin Inspector (`Assets/Plugins/Sirenix/`) — `[ShowInInspector]`, `[Button]` 등 활용
- **테스트**: Unity Test Framework 1.6.0 — PlayMode / EditMode 테스트
- **언어**: C# (.NET 기반)

---

## 🎨 아트 스타일 가이드

- **도트아트 (Pixel Art)** 기반
- 픽셀 블러 방지: `Filter Mode = Point (no filter)`, `Compression = None`
- `Pixels Per Unit` 통일 (권장: 16 또는 32)
- 카메라 설정: 정수 배율(Integer Scaling) 사용 권장

---

## 📁 프로젝트 디렉토리 구조

> `Assets` 바로 하위 폴더는 **Plugins**와 **Editor**를 제외하고 모두 **접두 번호**를 붙입니다.

### 현재 실제 구조 (초기 개발 단계)

```
Assets/
├── 01_Scenes/                  # 씬 파일 (SampleScene.unity)
├── 02_Scripts/                 # C# 스크립트 (현재 비어있음)
├── 03_Resources/               # 런타임 로드 리소스
│   ├── 01_Sprites/
│   │   ├── bg/                 # 배경 스프라이트
│   │   ├── block/              # 블록 스프라이트 (soil, stone00~03, wood)
│   │   ├── mineral/            # 광물 스프라이트 (copper, gold, silver, steel)
│   │   └── pickaxe/            # 곡괭이 스프라이트 (pickaxe_00~05, 6단계)
│   ├── 02_Prefabs/             # GameObject 프리팹
│   ├── 03_Datas/               # ScriptableObject 데이터 에셋
│   ├── 04_Materials/           # URP 머티리얼
│   ├── 05_FX/                  # 파티클/비주얼 이펙트
│   └── 06_SFX/                 # 오디오 클립
├── Settings/                   # URP 렌더러 설정 에셋
├── Editor/                     # 에디터 전용 스크립트 (번호 없음)
└── Plugins/                    # 서드파티 플러그인 (번호 없음)
    └── Sirenix/                # Odin Inspector (수정 금지)
```

### 스크립트 폴더 구조 (`02_Scripts/`)

어셈블리 분리를 염두에 두고 도메인 레이어별로 구성합니다.
각 레이어 폴더는 추후 독립적인 `.asmdef` 파일로 분리할 수 있습니다.

```
02_Scripts/
├── Core/                       # 게임 부트스트랩, 씬 관리, 이벤트 버스 등 핵심 인프라
│   └── MI.Core.asmdef          # (예정)
├── Domain/                     # 게임 규칙, 엔티티, 인터페이스 — 순수 로직 (Unity 의존 없음)
│   ├── Pickaxe/
│   ├── Tile/
│   └── MI.Domain.asmdef        # (예정)
├── Presentation/               # MonoBehaviour, UI, 입력 처리 — 화면에 보이는 것
│   ├── Pickaxe/
│   ├── Tile/
│   ├── UI/
│   ├── Input/
│   └── MI.Presentation.asmdef  # (예정)
└── Data/                       # ScriptableObject, 저장/로드, 외부 API 연동
    ├── SaveLoad/
    ├── Config/
    └── MI.Data.asmdef          # (예정)
```

---

## 🖼️ 스프라이트 네이밍 규칙

`{타입}_{변형}.png` 패턴을 따릅니다.

| 종류 | 패턴 | 설명 |
|------|------|------|
| 블록 | `soil_00~02`, `stone00_00~04`, `wood_00~02` 등 | 숫자 접미사 = 파괴 단계 (00 = 온전함, 최대 = 파괴 직전) |
| 광물 | `copper_00~02`, `gold_00~01`, `silver_00~02`, `steel_00~02` | |
| 곡괭이 | `pickaxe_00~05` | 6단계 티어 |

---

## 📐 코딩 컨벤션

### 네임스페이스

게임 이름 **Mine Mine Mine**에서 따온 `MI`를 사용하며, 폴더 경로를 그대로 따라갑니다.

```csharp
namespace MI.Core { }
namespace MI.Domain.Pickaxe { }
namespace MI.Domain.Tile { }
namespace MI.Presentation.Pickaxe { }
namespace MI.Presentation.Input { }
namespace MI.Data.Config { }
```

### 타입 네이밍 규칙

| 타입 | 규칙 | 예시 |
|------|------|------|
| **클래스** | `MI` + PascalCase | `MIPickaxeController`, `MITileBreaker` |
| **인터페이스** | `IMI` + PascalCase | `IMIBreakable`, `IMIDamageable` |
| **구조체** | `F` + PascalCase | `FTileData`, `FPickaxeStats` |
| **열거형** | `E` + PascalCase | `ETileType`, `EBreakResult` |
| **메서드** | PascalCase | `HandleBreak()`, `ApplyGravity()` |
| **공개 프로퍼티** | PascalCase | `public int Health { get; private set; }` |
| **비공개 변수** | `_` + camelCase | `private float _fallSpeed;` |
| **상수** | SCREAMING_SNAKE_CASE | `const int MAX_BREAK_COUNT = 5;` |

> `MI` 접두사는 프로젝트에서 정의하는 모든 외부 노출 타입(클래스, 인터페이스, 구조체, Enum)에 붙입니다.

### 코딩 기본 원칙

- 한 파일 = 한 타입
- `[SerializeField]`로 인스펙터 노출 (`public` 최소화)
- ScriptableObject(`FXxxConfig` 형태)로 데이터/로직 분리 — `Assets/03_Resources/03_Datas/` 에 저장
- Odin Inspector 속성(`[ShowInInspector]`, `[Button]`, `[FoldoutGroup]` 등)으로 에디터 편의성 향상
- 코루틴보다 `async/await` 우선 고려
- 코드 주석은 **한국어**로 작성

---

## 🌐 크로스 플랫폼 주의사항

- 입력: `Input.GetMouseButton` 대신 **New Input System** 사용 (`Touchscreen` 디바이스 이벤트 활용)
- UI: Canvas Scaler — Scale With Screen Size
- 저장 경로: `Application.persistentDataPath` 사용
- iOS/Android 빌드: **IL2CPP** 백엔드 + **ARM64** 타겟 필수

---

## 🚀 빌드 및 실행

모든 빌드/실행/테스트는 **Unity Editor**에서 진행합니다 (CLI 빌드 없음).

- **에디터 실행**: Unity Hub에서 `6000.3.10f1` 버전으로 프로젝트 열기
- **테스트 실행**: Window → General → Test Runner
- **빌드**: File → Build Settings

| 플랫폼 | Build Target | 비고 |
|--------|-------------|------|
| PC (Windows) | StandaloneWindows64 | |
| PC (macOS) | StandaloneOSX | Apple Silicon 지원 |
| Android | Android | IL2CPP, ARM64 필수 |
| iOS | iOS | Xcode 필요, MacBook에서 빌드 |

`Library/`, `Temp/`, `Logs/`, `obj/`, `Build/` 는 `.gitignore` 에 포함되어 있으며 Unity가 자동 재생성합니다.

---

## ☁️ 클라우드 서버 연동 (예정)

현재 로컬 개발 단계이며, 추후 아래 기능 연동 예정:
- 클라우드 세이브 (Unity Cloud Save 또는 커스텀 API)
- 플레이어 인증 (Unity Authentication)
- 리더보드 / 통계 (Unity Leaderboards)
- 원격 설정 (Unity Remote Config)
