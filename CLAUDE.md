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
- **세이브/로드**: Easy Save 3 (`Assets/Plugins/`) — `.es3` 파일 기반 저장
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
    ├── Sirenix/                # Odin Inspector (수정 금지)
    └── Easy Save 3/            # Easy Save 3 (수정 금지)
```

### 스크립트 폴더 구조 (`02_Scripts/`)

어셈블리 분리를 염두에 두고 도메인 레이어별로 구성합니다.
각 레이어 폴더는 추후 독립적인 `.asmdef` 파일로 분리할 수 있습니다.

```
02_Scripts/
├── Core/
│   ├── BootStrap/
│   │   └── MIBootStrap.cs
│   ├── Managers/
│   │   └── MIGameManager.cs
│   ├── Pool/
│   │   ├── MIObjectPool.cs
│   │   └── MIPoolManager.cs            # FPoolConfig 구조체 포함
│   ├── StateMachine/
│   │   ├── MIBaseStateMachineBehaviour.cs
│   │   └── Tile/
│   │       └── MITileStateMahcine.cs
│   ├── MIContracts.cs
│   ├── MIGameRoot.cs
│   ├── MISceneContext.cs
│   └── MISingleton.cs
├── Data/
│   ├── Config/
│   │   ├── MIBackgroundConfig.cs
│   │   ├── MILevelData.cs
│   │   ├── MIMineralConfig.cs
│   │   ├── MIPickaxeConfig.cs
│   │   ├── MIStageConfig.cs
│   │   ├── MIStatusConfig.cs
│   │   ├── MITileConfig.cs
│   │   └── MIWallConfig.cs
│   ├── UIRes/
│   │   ├── MIItemIconDataTable.cs
│   │   └── MIUINumberResources.cs
│   ├── Cloud/                          # 예정
│   ├── Save/                           # 예정
│   └── Table/                          # 예정
├── Domain/
│   ├── Inventory/
│   │   ├── EItemType.cs              # MIItemTypeConverter 포함
│   │   ├── MIItemDropEvent.cs
│   │   └── MIUserInventory.cs
│   ├── Pickaxe/
│   │   ├── EPickaxePart.cs
│   │   └── FPickaxeStats.cs
│   ├── Stage/
│   │   ├── IMITileAlgorithm.cs
│   │   ├── MIBackgroundPainter.cs
│   │   ├── MIChunkBuffer.cs
│   │   ├── MIDepthTracker.cs
│   │   ├── MIFloodFillAlgorithm.cs
│   │   ├── MILevelResolver.cs
│   │   ├── MISeedPlacer.cs
│   │   ├── MITileSpawner.cs
│   │   ├── MIWallPainter.cs
│   │   └── MIWallSpawner.cs
│   ├── Status/
│   │   ├── FLevelEntry.cs
│   │   ├── FStatusSnapshot.cs
│   │   ├── IMIStatusListener.cs
│   │   └── MIStatusManager.cs
│   ├── Tile/
│   │   ├── EBreakResult.cs
│   │   ├── EMineralDensity.cs
│   │   ├── EMineralType.cs
│   │   ├── ETileType.cs
│   │   ├── ETreasureType.cs
│   │   ├── FBackgroundVariant.cs
│   │   ├── FChunkData.cs
│   │   ├── FMineralAffinity.cs
│   │   ├── FMineralDensityRange.cs
│   │   ├── FMineralDropEntry.cs
│   │   ├── FMineralWeight.cs
│   │   ├── FTileData.cs
│   │   ├── FTileDropEntry.cs
│   │   ├── FTileWeight.cs
│   │   ├── FTreasurePlacement.cs
│   │   ├── FTreasureWeight.cs
│   │   └── IMIBreakable.cs
│   ├── TouchBreaker/
│   │   └── MITouchObjectSpawner.cs
│   └── User/
│       └── MIUserState.cs
├── Editor/
│   └── HotKey/
│       └── MIHierarchyToggleActive.cs
├── Infrastructure/
│   └── Input/
│       ├── IMIInputListener.cs
│       ├── MIInputActions.cs
│       ├── MIInputActions.inputactions
│       └── MIInputHandler.cs
├── Presentation/
│   ├── UI/
│   │   ├── Common/
│   │   │   ├── MIButton.cs
│   │   │   └── MINumberShaker.cs
│   │   ├── HUD/
│   │   │   ├── Items/
│   │   │   │   ├── MIDropItemViewer.cs
│   │   │   │   └── MIItemDropNotifyUI.cs
│   │   │   ├── Status/
│   │   │   │   └── MIStatusHUD.cs
│   │   │   └── MICanvasHUD.cs
│   │   ├── Interface/
│   │   │   └── IMIItemViewer.cs
│   │   ├── Popup/
│   │   │   ├── Inventory/
│   │   │   │   ├── IMIInventoryViewer.cs
│   │   │   │   ├── MIInventoryItemViewer.cs
│   │   │   │   └── MIPopupInventory.cs
│   │   │   ├── MICanvasPopup.cs
│   │   │   └── MIPopupBase.cs
│   │   ├── IMIUIContext.cs
│   │   └── MIUIRoot.cs
│   └── World/
│       ├── Camera/
│       │   ├── IMICameraFollower.cs
│       │   └── MICameraFollower.cs
│       ├── Pickaxe/
│       │   ├── MIPickaxeController.cs
│       │   └── MIPickaxePartCollider.cs
│       ├── Stage/
│       │   └── MIStageOrchestrator.cs
│       ├── Tile/
│       │   ├── Animation/
│       │   │   └── MIAnimationEvent.cs
│       │   ├── MITileModel.cs
│       │   └── MITileView.cs
│       ├── TouchBreaker/
│       │   ├── MITouchBreaker.cs
│       │   └── MITouchObjectViewer.cs
│       └── VFX/
│           └── MIFxAutoFade.cs
└── Utility/
    ├── MIAppLifeTime.cs
    ├── MIIntRange.cs
    └── MILog.cs
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
모든 클래스 파일에는 반드시 네임스페이스를 선언합니다.

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
| **비공개 인스턴스 변수** | `_` + camelCase | `private float _fallSpeed;` |
| **비공개 static 변수** | `s_` + camelCase | `private static readonly int s_bSelected;` |
| **상수** | SCREAMING_SNAKE_CASE | `const int MAX_BREAK_COUNT = 5;` |

> `MI` 접두사는 프로젝트에서 정의하는 모든 외부 노출 타입(클래스, 인터페이스, 구조체, Enum)에 붙입니다.

### 변수 네이밍 상세

```csharp
// 인스턴스 필드 — _camelCase
private float _fallSpeed;
[SerializeField] private GameObject _goPopup;

// static 필드 — s_camelCase
private static readonly Dictionary<int, int> s_pow;
private static readonly List<IMIInputListener> s_listeners;

// Animator 파라미터 해시 캐시 — static readonly + s_ 접두사
private static readonly int s_bSelected = Animator.StringToHash("bSelected");
private static readonly int s_tShake = Animator.StringToHash("tShake");

// 상수 — SCREAMING_SNAKE_CASE
private const string ROOT_SCENE_NAME = "BootStrap";
private const int TILE_OFFSET = 100;
```

### 코딩 기본 원칙

- 한 파일 = 한 타입
- `[SerializeField]`로 인스펙터 노출 (`public` 최소화)
- ScriptableObject(`FXxxConfig` 형태)로 데이터/로직 분리 — `Assets/03_Resources/03_Datas/` 에 저장
- Odin Inspector 속성(`[ShowInInspector]`, `[Button]`, `[FoldoutGroup]` 등)으로 에디터 편의성 향상
- 코루틴보다 `async/await` 우선 고려
- 코드 주석은 **한국어**로 작성
- access modifier(`private`, `public`, `protected`)는 항상 명시적으로 작성

### 주석 스타일

- **클래스 / 인터페이스 / 메서드**: `/// <summary>` XML doc 주석 사용
- **필드 / 변수 인라인 설명**: `//` 한 줄 주석 사용
- XML doc 주석의 `<param>`, `<returns>` 태그도 필요 시 사용
- 주석 내용은 **한국어**로 작성

```csharp
/// <summary>
/// 타일을 생성한다.
/// </summary>
/// <param name="config">타일 설정 데이터</param>
public void SpawnTile(MITileConfig config) { }

// ❌ 클래스/메서드에 // 단독 주석 사용 지양
// 타일을 생성한다.
public void SpawnTile() { }
```

```csharp
// 비활성 오브젝트를 보관할 부모 Transform
[SerializeField] private Transform _parent;

// 풀 소진 시 자동 확장 여부
private readonly bool _autoExpand;
```

### 중괄호 및 포맷팅

- **Allman 스타일** (여는 중괄호를 새 줄에 배치)
- 단일행 가드 절에서는 중괄호 생략 허용 (`if (...) return;`)
- 메서드 사이에 빈 줄 1개

```csharp
// ✅ Allman 스타일
if (condition)
{
    DoSomething();
}

// ✅ 단일행 가드 절 — 중괄호 생략 허용
if (obj == null) return;
if (index < 0 || index >= length) continue;
```

### Expression Body (`=>`)

1줄로 표현 가능한 단순 메서드, 속성에서는 `=>`를 적극 사용합니다.

```csharp
// ✅ 단순 속성
public int GridWidth => _gridWidth;
public bool IsDestroyed => CurrentDurability <= 0;

// ✅ 단순 메서드
public int GetAmount(EItemType type) => _items.TryGetValue(type, out var v) ? v : 0;
private void OnEnable() => _inputHandler.RegisterListener(this);

// ❌ 여러 줄이 필요한 메서드에서는 사용하지 않음
```

### SerializeField 패턴

`[SerializeField]`는 필드와 같은 줄에 작성합니다.
Odin Inspector 속성(`[Title]`, `[Required]`, `[InfoBox]` 등)은 별도 줄에 배치합니다.

```csharp
// ✅ 기본
[SerializeField] private int _gridWidth = 8;
[SerializeField] private MITileConfig _tileConfig;

// ✅ Odin 속성 조합
[Title("스테이지 설정")]
[Required]
[SerializeField] private MIStageConfig _stageConfig;

// ✅ 복합 속성
[SerializeField, ReadOnly] private FTileData _data;
```

### Property 패턴

| 용도 | 패턴 | 예시 |
|------|------|------|
| Config/SO 읽기 전용 노출 | `[SerializeField]` + `=>` getter | `public int GridWidth => _gridWidth;` |
| 런타임 상태 | auto-property | `public int CurrentRow { get; private set; }` |
| 외부 쓰기 허용 (드물게) | auto-property | `public int CurrentRow { get; set; }` |

### 클래스 멤버 구역 분류

클래스 내부의 멤버를 기능별로 구분할 때 주석 구분선 대신 `#region`을 사용합니다.
`#endregion`에도 반드시 같은 이름을 명시합니다.
짧은 클래스(enum, struct, 50줄 이하)에서는 `#region`을 생략해도 됩니다.

```csharp
// ❌ 지양
// -- Public API --------------------------------------------

// ✅ 권장
#region Public API

// ... 멤버 ...

#endregion Public API
```

### 클래스 멤버 배치 순서

```
1. [SerializeField] 필드  (#region Inspector / #region Fields)
2. private 런타임 필드
3. 프로퍼티
4. Unity Lifecycle      (#region Unity Lifecycle / #region Unity Events)
   → Awake → Start → OnEnable → OnDisable → Update → FixedUpdate → OnDestroy
5. Public API           (#region Public API)
6. Private 헬퍼         (#region Helper / #region Internal)
7. #if UNITY_EDITOR     (에디터 전용 디버그)
```

### using 지시문 순서

```csharp
// 1. System 계열
using System;
using System.Collections.Generic;

// 2. 프로젝트 내부 (MI.*)
using MI.Core;
using MI.Domain.Tile;

// 3. 서드파티 (Odin 등)
using Sirenix.OdinInspector;

// 4. Unity 계열
using UnityEngine;
using UnityEngine.Tilemaps;

// ※ using alias는 namespace 블록 내부에 배치
namespace MI.Presentation.World.Stage
{
    using Camera = UnityEngine.Camera;
    // ...
}
```

### 이벤트/콜백 패턴

| 용도 | 패턴 |
|------|------|
| UI/단순 알림 | `event Action<T>` |
| 핵심 시스템 리스너 | `Register/Unregister` + `List<IListener>` (역방향 순회) |
| 인스펙터 바인딩 | `UnityEvent<T>` |

이벤트 호출 시에는 null-conditional `?.Invoke()`를 사용합니다.

```csharp
OnInventoryUpdated?.Invoke(_items);
```

### 문자열 사용

- 문자열 보간 `$""` 사용 (concatenation `+` 지양)
- Animator 파라미터는 반드시 `Animator.StringToHash()`로 캐시하여 사용

```csharp
// ✅
MILog.Log($"[MIObjectPool<{typeof(T).Name}>] 풀 최대 크기({_maxSize}) 초과.");
private static readonly int s_tShake = Animator.StringToHash("tShake");

// ❌
_animator.SetTrigger("tShake");  // 문자열 직접 사용 금지
```

### Null 체크

- 일반 null 체크: `== null` / `!= null` 직접 비교
- 이벤트/콜백 호출: `?.Invoke()` null-conditional
- nullable struct: `.HasValue` 사용
- 컴포넌트 탐색: `TryGetComponent` 패턴 권장

```csharp
if (_controller == null) return;                     // 일반 null 체크
OnEnterState?.Invoke(stateInfo);                      // 이벤트 호출
if (_data.MineralDrop.HasValue) { }                   // nullable struct
collision.gameObject.TryGetComponent(out IMIBreakable breakable);  // 컴포넌트
```

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

## 💾 세이브/로드 시스템

- 세이브/로드 시스템은 **Easy Save 3** 애셋을 기반으로 사용한다
- Easy Save 3 플러그인 위치: `Assets/Plugins/Easy Save 3/` (Odin Inspector와 동일 레벨)
- 세이브 파일 확장자: `.sav`
- 세이브 파일 경로: `Application.persistentDataPath` 사용
- 공식 문서: https://docs.moodkie.com/product/easy-save-3/

---

## ☁️ 클라우드 서버 연동 (예정)

현재 로컬 개발 단계이며, 추후 아래 기능 연동 예정:
- 클라우드 세이브 (Unity Cloud Save 또는 커스텀 API)
- 플레이어 인증 (Unity Authentication)
- 리더보드 / 통계 (Unity Leaderboards)
- 원격 설정 (Unity Remote Config)
