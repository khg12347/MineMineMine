# Mine Mine Mine

> 광물을 캐고 곡괭이를 성장시켜 최강의 곡괭이를 만드는 모바일 캐주얼 게임
>
> **개발 중인 개인 프로젝트입니다.**

![gameplay](docs/gameplay.gif)

---

## 목차

1. [프로젝트 개요](#프로젝트-개요)
2. [아키텍처](#아키텍처)
3. [핵심 시스템](#핵심-시스템)
   - [제네릭 오브젝트 풀링](#1-제네릭-오브젝트-풀링--micorepool)
   - [무한 하강 스테이지 생성](#2-무한-하강-스테이지-생성--midomainstage)
   - [타일 파괴 시스템](#3-타일-파괴-시스템--midomaintile--mipresentationworld)
   - [입력 추상화](#4-입력-추상화--miinfrastructureinput)
   - [ScriptableObject 기반 데이터 설계](#5-scriptableobject-기반-데이터-설계--midataconfig)
   - [플레이어 상태 관리](#6-플레이어-상태-관리--midomainstatus)
4. [기술 스택](#기술-스택)
5. [개발 현황](#개발-현황)

---

## 프로젝트 개요

| 항목 | 내용 |
|------|------|
| 장르 | 2D 도트아트 · 중력 기반 타일 파괴 |
| 핵심 메카닉 | 곡괭이 오브젝트가 중력을 받아 낙하하며 바닥 타일을 파괴, 터치로 타일 직접 파괴 가능 |
| 플랫폼 | Android / iOS / PC |
| 엔진 | Unity 6 (6000.3.10f1) · URP 2D |
| 언어 | C# |
| 개발 인원 | 1인 |

---

## 아키텍처

### 레이어 구조

```
┌──────────────────────────────────────────────────────────────────┐
│  Presentation  │ MonoBehaviour · UI · Camera · VFX               │
├──────────────────────────────────────────────────────────────────┤
│  Infrastructure│ New Input System 어댑터 · 외부 서비스 연동        │
├──────────────────────────────────────────────────────────────────┤
│  Domain        │ 게임 규칙 · 상태 · 알고리즘                       │
├──────────────────────────────────────────────────────────────────┤
│  Data          │ ScriptableObject · 설정값 · 리소스 참조          │
├──────────────────────────────────────────────────────────────────┤
│  Core          │ Singleton · ObjectPool · StateMachine 베이스    │
└──────────────────────────────────────────────────────────────────┘
```

### 의존 방향

```
Presentation
    │
    ▼
  Domain  ◀──  Infrastructure
    │
    ▼
   Data
    │
    ▼
  Core
```

의존 방향은 **항상 위→아래 단방향**입니다. `Utility`와 `Editor`는 어느 레이어에서도 참조 가능한 횡단 레이어입니다.

**이 구조를 선택한 이유**

이전 프로젝트에서 싱글톤 기반으로 시스템을 구현하다 의존 구조가 꼬이면서, 씬 전환 시 초기화 순서 문제로 큰 버그를 겪었습니다. 데모 제출 후 의존성 주입(DI) 패턴으로 전면 개편하는 과정에서 의존 방향이 단방향으로 정리되면 DI 구조가 자연스럽게 따라온다는 것을 체감했습니다.

이 프로젝트는 아직 초반이라 asmdef 분리나 DI 프레임워크를 본격 도입하지는 않았지만, 나중에 그 시점이 왔을 때 구조적으로 준비된 상태이기 위해 의존 방향을 의식적으로 관리하고 있습니다.

**결과적으로:**
- 플랫폼 입력 변경(터치 → 키보드 등)이 `Infrastructure/Input`만 수정하면 됨
- 새 기능의 추가 위치를 레이어 기준으로 즉시 판단 가능

---

## 핵심 시스템

---

### 1. 제네릭 오브젝트 풀링 — `MI.Core.Pool`

**선제적 설계 결정 — 이전 프로젝트 경험에서 출발**

Pixel Poker Defense 개발 당시 `Instantiate` / `Destroy`만으로 구현했고, 투사체가 많아질수록 프레임 드랍이 발생했습니다. 당시에는 원인을 몰랐지만 이후 오브젝트 풀링 패턴을 학습하면서 GC 압박이 원인이었음을 파악했습니다.

다만 풀링이 항상 정답은 아닙니다. 수 초에 한두 번 생성·파괴되는 수준에서는 GC 스파이크가 발생하지 않고, 오히려 미리 다수를 스폰해두는 것이 메모리만 낭비합니다. 이 프로젝트에서는 **짧은 시간 안에 다수가 반복 생성·파괴될 수 있는 타일·이펙트·터치 이펙트**에 한해 풀링을 적용했습니다. 예를 들어 곡괭이가 연속으로 타일을 깨며 내려가는 경우 수십 개가 한꺼번에 반환·재사용되는데, 이때 GC 스파이크를 방지하는 것이 목적입니다.

**구조**

```
MIObjectPool<T>   — 단일 타입 풀 (핵심 구현)
MIPoolManager     — 전역 싱글톤, 다중 타입 통합 관리
FPoolConfig       — 풀 초기화 파라미터 (InitialSize, GrowSize) 구조체
```

**`MIObjectPool<T>` — 중복 반환 방지**

`Queue<T>.Contains`는 O(n)입니다. `HashSet<T>`를 병행하여 중복 반환 체크를 O(1)로 처리합니다.

```csharp
public void Return(T obj)
{
    if (obj == null) return;
    if (!_inPool.Add(obj)) return;  // 이미 풀에 있으면 Add가 false → 중복 반환 방지
    obj.gameObject.SetActive(false);
    obj.transform.SetParent(_parent);
    _pool.Enqueue(obj);
}
```

| | Queue.Contains | HashSet 병행 |
|---|:---:|:---:|
| containment 체크 | O(n) | O(1) |
| Get / Return | O(1) / O(n) | O(1) / O(1) |

**`MIObjectPool<T>` — 자동 확장**

풀 소진 시 `GrowSize` 단위로 확장합니다. 한 번에 전부 확장하지 않고 단위 확장하여 GC 스파이크를 분산시킵니다.

```csharp
// 풀 소진 시 GrowSize 단위로 점진 확장
else if (_autoExpand && _totalCreated < _maxSize)
{
    for (int i = 0; i < _growSize && _totalCreated < _maxSize; i++)
        Expand();
}
```

**`MIPoolManager` — 타입 통합 관리**

여러 타입의 풀을 단일 매니저로 관리하기 위해 타입 소거(type erasure) 패턴을 사용합니다.

```csharp
// 내부: Dictionary<int(prefab InstanceID), object(MIObjectPool<T>)>
// 호출부는 제네릭 API만 사용 — 캐스팅 불필요
MIPoolManager.Instance.Get<MITileModel>(prefab, pos, rot);
MIPoolManager.Instance.Return<MITileModel>(tile);
```

역매핑(`_instanceToPool`)으로 인스턴스 → 풀을 O(1)로 조회합니다.

---

### 2. 무한 하강 스테이지 생성 — `MI.Domain.Stage`

**문제**

무한 하강 게임에서 전체 맵을 미리 생성하면 메모리가 감당되지 않습니다. 반대로 플레이 중 동기 생성하면 히치가 발생합니다. 타일 배치는 랜덤이되 자연스럽게 군집을 형성해야 했습니다.

**알고리즘 선택 과정**

기획 요구에 맞는 알고리즘을 두 가지 고안했습니다. (1) 블록 풀을 미리 생성해 랜덤 배치 후 빈 공간을 BFS로 채우는 방식, (2) Flood Fill 기반 군집 생성 방식. 두 알고리즘 모두 텍스트로는 시각적으로 와닿지 않았기 때문에, Unity 구현 전에 AI를 활용해 HTML/JS 프로토타입을 먼저 제작하고 기획자에게 시연했습니다. 기획자가 Flood Fill 결과를 선택했고, 이후 Unity 구현에 들어갔습니다.

→ [타일 생성 시뮬레이터 (HTML 프로토타입)](https://khg12347.github.io/MineMineMine/tile_generation_simulator.html)

**메모리를 고려한 청크 버퍼 설계**

한 화면에 보이는 타일 데이터만 생성해도 충분합니다. 맵 데이터는 청크 버퍼에 미리 캐싱해두고, 실제 MonoBehaviour 인스턴스는 화면에 진입할 때 스폰하고 벗어나면 풀에 반환합니다. 전체 맵을 미리 생성하지 않아 메모리를 일정하게 유지합니다.

**구조**

```
IMITileAlgorithm       — 알고리즘 인터페이스 (교체 가능 설계)
MIFloodFillAlgorithm   — Jittered Grid + BFS Flood-Fill 구현
MISeedPlacer           — 시드 균일 분배 (Jittered Grid)
MILevelResolver        — 절대 행 번호 → 레벨 설정 변환 (블렌딩 지원)
MIChunkBuffer          — 생성된 청크 Queue 관리
MITileSpawner          — 버퍼 소비 → MonoBehaviour 인스턴스화 (풀 경유)
MIDepthTracker         — 곡괭이 Y → 현재/최대 행 변환 및 추적
MIStageOrchestrator    — 전체 Update 루프 조율
```

**알고리즘 3단계 파이프라인**

```
┌──────────────────────────────────────────────────────────────┐
│  Phase 1: 타일 타입 그리드                                     │
│    MISeedPlacer.PlaceSeeds()     ← Jittered Grid 균일 배치    │
│    ExpandSeeds()                 ← 다중 출발점 BFS 확장        │
│    FillRemaining()               ← 가중치 기반 나머지 채우기    │
├──────────────────────────────────────────────────────────────┤
│  Phase 2: 보물 배치                                            │
│    PlaceTreasures()              ← 확률 + 가중치 등급 선택     │
├──────────────────────────────────────────────────────────────┤
│  Phase 3: 광물 오버레이                                        │
│    ApplyMinerals()               ← 섹션 가중치 × 타일 친화도   │
└──────────────────────────────────────────────────────────────┘
```

**레벨 전환 블렌딩**

`MILevelResolver`는 레벨 경계 구간에서 이전/다음 레벨 설정을 `Mathf.Lerp`로 선형 보간합니다. 타일 가중치·광물 가중치가 갑자기 바뀌지 않고 자연스럽게 전환됩니다. 마지막 레벨 도달 후에는 해당 레벨 설정을 무한 반복합니다.

**청크 버퍼 + 컬링**

```
MIChunkBuffer (Queue<FChunkData>)
  ↓ 행 단위 소비
MITileSpawner.SpawnRowsUpTo(currentRow + spawnAhead)   ← 선행 스폰
  ↓ 화면 위쪽 제거
MITileSpawner.CullAbove(currentRow - cullAbove)
  → MIPoolManager.Return(tile)                         ← Destroy 없이 풀 반환
```

스폰과 컬링이 모두 풀을 경유하므로 타일 생성·파괴로 인한 GC 할당을 제거합니다.

**파괴 시 행 목록 O(1) 갱신**

각 타일이 자신의 행 인덱스(`CurrentRow`)를 보유합니다. 파괴 콜백에서 전체 Dictionary를 순회하지 않고 행 인덱스로 직접 접근합니다.

```csharp
// Before: _tilesByRow 전체 순회 → O(행 수 × 행당 타일 수)
// After:  행 인덱스 직접 접근 → O(1)
private void OnTileDestroyed(MITileModel tile)
{
    if (_tilesByRow.TryGetValue(tile.CurrentRow, out var rowTiles))
        rowTiles.Remove(tile);
}
```

**교체 가능한 알고리즘**

`MIStageOrchestrator`는 `IMITileAlgorithm` 인터페이스만 참조합니다. 새 알고리즘(동굴형, 퍼즐형 등)을 추가해 교체해도 스폰·컬링·버퍼 로직은 그대로입니다.

---

### 3. 타일 파괴 시스템 — `MI.Domain.Tile` / `MI.Presentation.World`

**선제적 설계 결정 — 이전 프로젝트 경험에서 출발**

이전 프로젝트에서 뷰 클래스와 시스템 로직 클래스가 섞여 있을 때, 시각 표현이 바뀌면 로직 쪽도 함께 수정해야 하는 양방향 의존 문제를 겪었습니다. 특히 시각적인 부분은 개발 중 언제든 바뀔 수 있다는 것을 회사 프로젝트와 개인 프로젝트 모두에서 체감했습니다. 이 프로젝트는 처음부터 데이터/로직(`MITileModel`)과 시각 표현(`MITileView`)을 분리했습니다. 한쪽이 수정될 때 다른 쪽 구현을 건드리지 않아도 되는 구조가 실제 유지보수에서 이득으로 돌아오고 있습니다.

**구조**

```
IMIBreakable                        — 파괴 가능 계약 인터페이스
  TakeDamage(damage, hitPoint) → EBreakResult
  Break()

EBreakResult                        — Damaged / Destroyed / DestroyWithOneHit
FTileData (struct)                  — 내구도 · 타입 · 광물 등 값 타입 상태
MITileModel (: IMIBreakable)        — 데이터 보유 + 파괴 로직
MITileView                          — 순수 시각 표현 (스프라이트 · 애니메이션 · 이펙트)
```

**파괴 흐름**

```
MIPickaxePartCollider.OnCollisionEnter2D
  → MIPickaxeController.OnPartCollision(EPickaxePart, Collision2D)
    → IMIBreakable.TakeDamage(damage, hitPoint)
      → FTileData.ApplyDamage(damage)        ← 내구도 감소 (순수 값 타입 연산)
        → EBreakResult 반환
      → MITileView.ShowDamageText()
      → MITileView.SetCrackParameter()       ← 균열 단계 Animator 파라미터
      → if Destroyed:
          → MIStatusManager.AddExp()
          → MITileView.PlayBreakEffect()
          → MIPoolManager.Return(this)       ← Destroy 없이 풀 반환
          → _onBroken?.Invoke(this)          ← 스포너 행 목록 갱신
```

**부위별 데미지 — 곡괭이 Head / Handle 분리**

`EPickaxePart.Head` / `Handle`로 구분하며, 각 부위 콜라이더(`MIPickaxePartCollider`)는 별도 자식 GameObject입니다. 충돌 이벤트를 부모 컨트롤러에 위임하므로 부위 추가·제거가 Inspector 레벨에서 가능합니다.

**터치 파괴 — 동일한 인터페이스 재사용**

`MITouchBreaker`는 `IMIInputListener`를 구현합니다. 탭 이벤트를 받으면 `Physics2D.Raycast`로 타일을 찾고 동일한 `IMIBreakable.TakeDamage()`를 호출합니다. 곡괭이 충돌과 터치 파괴가 같은 인터페이스를 공유하므로 파괴 결과 처리 로직이 중복되지 않습니다.



### 4. 입력 추상화 — `MI.Infrastructure.Input`

**문제 — 이전 프로젝트 경험에서 출발**

BB 프로젝트 초기에 입력 처리 코드와 게임 로직이 직접 결합된 구조로 구현했습니다. 입력에 따른 행동이 바뀌거나, UI 입력으로 전환되거나, 캐릭터 동작이 변경될 때 한 곳을 고치면 다른 곳이 터지는 연쇄 문제를 겪었습니다. 데모 제출 후 복기하며 자료조사한 결과, 입력 감지와 입력 의도 해석을 분리하는 구조를 찾아 적용했습니다.

현재 프로젝트는 입력 종류가 적지만, 향후 PC/모바일 크로스 플랫폼을 계획하고 있어 미리 분리해둔 구조입니다.

**구조**

```
IMIInputListener          — OnTap(Vector2 screenPosition)
MIInputHandler            — New Input System 어댑터, 리스너 목록 관리
MIInputActions            — .inputactions 자동 생성 래퍼
```

**흐름**

```
New Input System (Touchscreen / Mouse / 향후 추가 디바이스)
  OnPositionPerformed → _lastPointerPosition 캐시
  OnTapPerformed      → IMIInputListener.OnTap() 브로드캐스트
                              ↓
                    MITouchBreaker.OnTap()     ← 타일 파괴
                    [향후 추가 리스너]
```

게임 로직(`MITouchBreaker`)은 "탭이 발생했고 스크린 좌표는 여기다"라는 의도만 전달받습니다. 입력 디바이스 종류를 알 필요가 없습니다.

**리스너 안전 해제 — 역방향 순회**

`MIInputHandler`는 콜백을 역방향 순회(`for i = count-1 downto 0`)로 호출합니다. 콜백 내에서 `UnregisterListener()`를 호출해도 인덱스 오류가 발생하지 않습니다. `MIStatusManager`도 동일한 패턴을 사용합니다.

---

### 5. ScriptableObject 기반 데이터 설계 — `MI.Data.Config`

**선제적 설계 결정 — 이전 프로젝트 경험에서 출발**

Pixel Poker Defense에서는 하드코딩과 프리팹 직접 수정 기반으로 개발했습니다. 기획자가 에디터에 익숙하지 않아 밸런싱 작업을 개발자가 전담했고, 이 파이프라인이 비효율적이라는 것조차 인식하지 못했습니다.

실무에서 ScriptableObject 기반으로 구현된 프로젝트를 경험하면서, 기획자가 데이터를 직접 세팅하고 컴파일 없이 프로토타입 밸런싱을 반복하는 흐름을 경험했습니다. 이 프로젝트는 처음부터 기획자가 데이터를 편하게 세팅할 수 있도록 한글화된 ScriptableObject 구조로 설계했습니다. 데이터와 로직이 분리되어 있어 구현 수정 시 사이드 이펙트도 줄어드는 효과도 있습니다.

**구조**

```
MIStageConfig    — 그리드 크기, 청크 크기, 레벨 목록
MILevelData      — 레벨별 타일/광물/보물 가중치, 블렌딩 구간
MITileConfig     — 타입별 내구도 · 점수 · 스프라이트 · 이펙트 프리팹
MIPickaxeConfig  — 물리 파라미터 (중력, 탄성, 부위별 데미지)
MIStatusConfig   — 레벨 요구 EXP 테이블, 오버플로 배율
```

**Config → 데이터 팩토리 패턴**

Config 에셋이 런타임 데이터 구조체를 생성하는 팩토리 역할을 합니다. 호출부는 Config만 건네면 됩니다.

```csharp
// MITileConfig
public FTileData CreateTileData() { ... }   // 내구도, 점수 등 조립

// MIPickaxeConfig
public FPickaxeStats CreateStats() { ... }  // 물리 파라미터 조립

// 호출부
tile.ResetTile(config, config.CreateTileData());
pickaxe.InitializeFromConfig(pickaxeConfig);
```

기획 데이터 변경이 코드 수정 없이 에디터에서 즉시 반영됩니다. Odin Inspector(`[FoldoutGroup]`, `[ShowInInspector]`, `[Button]`)로 가중치 테이블·친화도 Dictionary 등 복잡한 설정을 구조화하여 표시합니다.

---

### 6. 플레이어 상태 관리 — `MI.Domain.Status`

**구조**

```
MIStatusManager (Singleton)   — EXP/레벨 계산, 깊이 추적, 리스너 관리
IMIStatusListener             — OnExpChanged / OnLevelUp / OnDepthUpdated
FStatusSnapshot (struct)      — EXP · 레벨 · 누적 EXP 불변 스냅샷
MIStatusConfig (SO)           — 레벨 테이블 + 오버플로 배율
```

**다중 레벨업 처리**

한 번의 타일 파괴로 여러 레벨을 뛰어넘을 수 있습니다. `AddExp()` 루프는 현재 레벨 요구치를 차감하며 레벨업을 반복 처리하고, 매 레벨업마다 `OnLevelUp()` 콜백을 발행합니다.

**무한 레벨 지원**

`MIStatusConfig`의 테이블 범위를 초과하면 `_overflowMultiplier`(기본 1.5×)를 적용해 요구 EXP를 동적으로 계산합니다. 테이블 배열 크기에 관계없이 레벨업이 끊기지 않습니다.

**`FStatusSnapshot` 불변 값 타입**

리스너에 전달되는 상태는 불변 구조체입니다. UI가 내부 상태를 직접 참조하거나 변경할 수 없습니다.

---

## 기술 스택

| 분류 | 내용 |
|------|------|
| 엔진 | Unity 6 (6000.3.10f1) |
| 렌더 파이프라인 | URP 2D |
| 입력 | New Input System 1.18.0 |
| 타일맵 | Tilemap + Tilemap Extras |
| 에디터 확장 | Odin Inspector |
| 세이브/로드 | Easy Save 3 (예정) |
| 테스트 | Unity Test Framework 1.6.0 |
| 빌드 백엔드 | IL2CPP · ARM64 (iOS/Android) |

---

## 개발 현황

| 시스템 | 상태 |
|--------|------|
| 오브젝트 풀링 | ✅ 완료 |
| 타일 스폰/컬링 | ✅ 완료 |
| 스테이지 생성 (Flood Fill) | ✅ 완료 |
| 입력 추상화 | ✅ 완료 |
| 전투 시스템 (TakeDamage · 터치 파괴) | ✅ 완료 |
| 코어 경제 시스템 (재료 · 재화 · 제작 · 강화 · 인벤토리) | 🔧 진행 중 |
| 코어 플로우 + 클라우드 (다시하기 · 클라우드 동기화 · 접속 플로우) | 🔧 시작 전 |
