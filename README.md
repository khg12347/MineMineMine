# Mine Mine Mine

> 광물을 캐고 곡괭이를 성장시켜 최강의 곡괭이를 만드는 모바일 캐주얼 게임
> 
> **개발 중인 개인 프로젝트입니다.**

![gameplay](docs/gameplay.gif)

---

## Overview

| 항목 | 내용 |
|------|------|
| 엔진 | Unity 6 (6000.3.10f1) |
| 언어 | C# |
| 플랫폼 | Android / iOS |
| 개발 인원 | 1인 |
| 상태 | 개발 중 |

---

## Architecture

레이어 간 단방향 의존성을 원칙으로 한 레이어드 아키텍처를 적용했습니다.
상위 레이어가 하위 레이어를 참조하며, 역방향 의존은 허용하지 않습니다.

```
MI
├── Core          // 범용 유틸리티, 오브젝트 풀링 — 다른 레이어에 의존하지 않음
├── Data          // ScriptableObject 기반 설정 데이터 (MITileConfig 등)
├── Domain        // 게임 규칙·상태 (MITileModel, MITileSpawner, MIChunkBuffer)
├── Infrastructure// 외부 연동 (저장, 플랫폼 API)
├── Presentation  // 시각 표현 (MITileView, VFX, UI) — Domain을 구독
├── Utility       // 수학, 확장 메서드 등 순수 유틸
└── Editor        // 에디터 전용 툴링
```

**의존 방향**

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

상위 레이어가 하위 레이어를 참조합니다. 역방향 참조는 허용하지 않습니다.
`Utility`와 `Editor`는 어느 레이어에서도 참조 가능한 횡단 레이어입니다.

**설계 의도**

- `Domain`은 `Presentation`을 직접 참조하지 않습니다. 상태 변화는 콜백/이벤트로 전달하여 게임 규칙과 시각 표현의 결합도를 낮췄습니다.
- `Core`는 게임 도메인 지식이 없는 순수 범용 레이어로 유지합니다. 어느 프로젝트에도 이식 가능한 수준을 목표로 합니다.

---

## Key Systems

### 1. Generic Object Pool — `MI.Core.Pool`

`Component`를 상속하는 모든 타입에 대해 동작하는 범용 풀입니다.

**핵심 설계 결정**

중복 반환 방지를 위한 containment 체크를 `Queue<T>.Contains` (O(n)) 대신 `HashSet<T>` 병행으로 처리합니다.

```csharp
public void Return(T obj)
{
    if (obj == null) return;
    if (!_inPool.Add(obj)) return;  // HashSet.Add가 false면 이미 풀에 있음 → O(1)
    obj.gameObject.SetActive(false);
    obj.transform.SetParent(_parent);
    _pool.Enqueue(obj);
}
```

| | Queue.Contains | HashSet 병행 |
|---|---|---|
| containment 체크 | O(n) | O(1) |
| Get / Return | O(1) / O(n) | O(1) / O(1) |

**자동 확장**

풀 소진 시 `growSize` 단위로 확장합니다. 한 번에 확장하지 않고 단위 확장하여 GC 스파이크를 분산시킵니다.

```csharp
// 풀 소진 시
else if (_autoExpand && _totalCreated < _maxSize)
{
    for (int i = 0; i < _growSize && _totalCreated < _maxSize; i++)
        Expand();
}
```

---

### 2. Tile System — `MI.Domain.Stage`

스크롤되는 스테이지를 행(row) 단위로 스폰/컬링합니다.

**행 단위 관리**

```csharp
// 행 인덱스 → 해당 행의 타일 목록
private readonly Dictionary<int, List<MITileModel>> _tilesByRow;
```

**파괴 콜백 O(1) 처리**

타일 파괴 시 전체 Dictionary를 순회하지 않도록 각 타일이 자신의 행 인덱스(`CurrentRow`)를 보유합니다.

```csharp
// Before: _tilesByRow 전체 순회 → O(행 수 × 행당 타일 수)
// After:  행 인덱스로 직접 접근 → O(1)
private void OnTileDestroyed(MITileModel tile)
{
    if (tile == null) return;
    if (_tilesByRow.TryGetValue(tile.CurrentRow, out var rowTiles))
    {
        rowTiles.Remove(tile);
        tile.CurrentRow = -1;
    }
}
```

**스테이지 생성**

데이터 기반 설정(`MITileConfig`)과 Flood Fill 알고리즘을 결합하여 매 스테이지마다 다른 광물 배치를 생성합니다.

Unity 구현 전에 HTML/JS로 알고리즘을 시각적으로 검증하는 프로토타입을 먼저 제작했습니다.
파라미터(광물 비율, 클러스터 크기 등)를 실시간으로 조정하며 결과를 확인할 수 있습니다.
AI로 제작한 프로토타입을 바탕으로 기획자와 최종 논의 후 작업에 들어갔습니다.

→ [타일 생성 시뮬레이터 (HTML 프로토타입)](https://khg12347.github.io/MineMineMine/docs/tile_generation_simulator.html)

---

### 3. Input Abstraction — `MI.Presentation`

입력 소스와 게임플레이 기능의 직접 결합을 방지하기 위해 추상화 레이어를 분리했습니다.

```
InputHandler (입력 소스 감지)
    └─▶ IInputListener.OnTap(Vector2 worldPos)
            └─▶ 구현 객체 (게임플레이 기능)
```

`IInputListener`를 구현한 객체는 입력 소스(터치/마우스/외부 SDK)가 무엇인지 알 필요가 없습니다. 입력 소스 교체 시 `InputHandler`만 수정하면 됩니다.

---

## Project Status

| 시스템 | 상태 |
|--------|------|
| 오브젝트 풀링 | ✅ 완료 |
| 타일 스폰/컬링 | ✅ 완료 |
| 스테이지 생성 (Flood Fill) | ✅ 완료 |
| Input 추상화 | ✅ 완료 |
| 전투 시스템 | 🔧 진행 중(TakeDamage시스템, 터치 데미지 시스템) |
| 코어 경제 시스템 (재료, 재화, 제작, 강화, 인벤토리) | 🔧 시작 전 |
| 코어 플로우 + 클라우드 (다시하기, 클라우드 동기화, 접속 플로우) | 🔧 시작 전 |
| 확장 컨텐츠 (던전, 룬, 특성 시스템) | 🔧 시작 전 |
| 라이브 서비스 + BM (퀘스트, 업적, 광고 시스템) | 🔧 시작 전 |
| 온보딩 + 튜토리얼 | 🔧 시작 전 |

---

