# Popup Enhance UI ↔ MIPickaxeEnhanceService 단계별 구현 계획

## Context

`Assets/04_Prefabs/UI/Popup/Enhance Pickaxe/Popup Enhance.prefab` 가 리소스/레이아웃은 조립된 상태지만, 도메인의 `MIPickaxeEnhanceService` 와 미연결이다. Presentation 측에는 빈 스켈레톤만 존재한다:

- `Assets/02_Scripts/Presentation/UI/Popup/Enhance/MIEnhancePopup.cs` — `MIPopupBase` 상속
- `Assets/02_Scripts/Presentation/UI/Popup/Enhance/MIEnhanceTab.cs` — MonoBehaviour
- `Assets/02_Scripts/Presentation/UI/Popup/Enhance/MIAbilityTab.cs` — MonoBehaviour

도메인 자산은 모두 준비됨 (`IMIPickaxeEnhanceService`, `MIEnhanceCostConfig`, `MIEnhanceConfig`, DI 등록은 `MIRootLifetimeScope.cs` 에 완료).

**잠재능력(Ability) 탭은 Step 6 에서 도메인 서비스 신설 후 진행.**

---

## 전체 단계 요약

| Step | 목표 | 상태 |
|------|------|------|
| 1 | 정적 표시 — 팝업을 열면 곡괭이/재료/재화가 표시됨 | ✅ 완료 |
| 2 | 곡괭이 좌/우 선택 + 보유 곡괭이 페이지네이션 | ✅ 완료 |
| 3 | 재료 부족 / MAX 레벨 / 강화 가능 상태 처리 | ✅ 완료 |
| 4 | 강화 버튼 → 결과 처리 → 애니메이션 → 상태 반영 | 🔲 미완료 |
| 5 | 외부 자원 변동(재화/재료 획득·소모) 자동 갱신 | 🔲 미완료 |
| 6 | 잠재능력(Ability) 탭 | ⏸ 보류 |

---

## Step 1. 정적 화면 띄우기 ✅

**목표**: 팝업을 활성화하면 곡괭이 1종 + 그 곡괭이의 현재 강화 비용/확률이 화면에 나타난다.

### 생성/수정 파일

| 파일 | 내용 |
|------|------|
| `MIEnhancePickaxeSelector.cs` (신규) | 보유 곡괭이 첫 항목 표시. 좌/우 버튼 참조는 받되 동작은 Step 2. |
| `MIEnhancePopup.cs` | DI 진입점. `InjectResources` + `Initialize`. `OpenPopup` 오버라이드 시 `RefreshAll` 호출. |
| `MIEnhanceTab.cs` | 재료 슬롯/재화/확률 정적 표시. 버튼은 `interactable = false`. |
| `MIAbilityTab.cs` | 네임스페이스 정정, 본문 비움. 팝업에서 `SetActive(false)`. |
| `MISceneContext.cs` | `_popupEnhance` 필드 + `Construct` 에 `IMIPickaxeEnhanceService` 추가 + 초기화 블록. |

### MISceneContext 초기화 코드

```csharp
if (_popupEnhance != null)
{
    _popupEnhance.InjectResources(_numberResources, _pickaxeIconDataTable, _itemIconDataTable);
    _popupEnhance.Initialize(
        _enhanceService,
        _userState.PickaxeInventory,
        _pickaxeData.EnhanceCostConfig,
        _userState.Wallet,
        _userState.Inventory);
}
```

### 에디터 작업 (사용자)

1. `Popup Enhance` 루트에 `MIEnhancePopup` 컴포넌트 부착, `_enhanceTab` / `_abilityTab` / `_selector` 슬롯 연결
2. `Pickaxe Zone` 에 `MIEnhancePickaxeSelector` 부착, 자식 UI 참조 연결
3. `Enhance Tab` 에 `MIEnhanceTab` 부착, 재료/재화/확률/버튼 참조 연결
4. 씬의 `MISceneContext` 인스펙터에서 `_popupEnhance` 슬롯에 인스턴스 연결

---

## Step 2. 곡괭이 좌/우 선택 ✅

**목표**: 좌/우 버튼으로 보유 곡괭이를 순회하고, 선택이 바뀌면 재료/재화/확률이 함께 갱신된다.

### 변경 내용

**`MIEnhancePickaxeSelector.cs`**
- `Initialize` 에서 `_btnLeft` / `_btnRight` 에 `onClick` 리스너 등록
- `OnLeftClicked` / `OnRightClicked` — 인덱스 ±1 후 `RefreshVisual` + `RefreshNavButtons` + `OnSelectionChanged` 발행
- `RefreshNavButtons` — `_index == 0` 이면 좌 버튼 비활성, `_index == Count-1` 이면 우 버튼 비활성
- `RefreshOwned` — 보유 목록 재구성 (Step 5의 `OnPickaxeAdded` 핸들러에서 사용)

**`MIEnhancePopup.cs`**
- `Initialize` 에서 `_selector.OnSelectionChanged += HandleSelectionChanged` 구독
- `OnDestroy` 에서 구독 해제
- `HandleSelectionChanged(type)` → `_enhanceTab.Refresh(type)` 호출

### 검증 포인트

- 보유 곡괭이가 2종 이상일 때 우 버튼 활성 → 클릭하면 다음 곡괭이 아이콘/이름/공격력/재료/확률이 갱신
- 첫 항목에서 좌 버튼 비활성, 마지막 항목에서 우 버튼 비활성
- 보유 곡괭이가 1종이면 양 버튼 모두 비활성

---

## Step 3. 재료 부족 / MAX 레벨 / 강화 가능 처리 ✅

**목표**: 강화 불가 상태에서는 버튼이 비활성·시각적으로 구분된다.

### 변경 내용

**`MIEnhanceTab.cs`**
- `RefreshMaterials` — `service.GetMaterialAmount(mat)` 로 보유량 조회, `보유/요구` 형태 표시. `HasEnoughMaterial` false 면 빨간색
- `RefreshCurrency` — `HasEnoughCurrency` false 면 금액 텍스트 빨간색
- `Refresh` 끝 — `_btnEnhance.interactable = service.CanEnhance(type)`

**`MIPickaxeEnhanceService.cs`** (별도 요청)
- `CanEnhance` 의 false 반환 분기마다 `MILog.Log` 로 원인 출력

| 분기 | 로그 내용 |
|------|-----------|
| type == None | `type이 None` |
| 미보유 | `{type} 미보유` |
| 인스턴스 조회 실패 | `{type} 인스턴스 조회 실패` |
| MAX 레벨 | `{type} 최대 레벨 도달 (Lv현재/최대)` |
| 비용 데이터 없음 | `{type} Lv{n} 비용 데이터 없음` |
| 재료 부족 | `{type} 재료 부족: {ItemType} (보유/요구)` |
| 재화 부족 | `{type} 재화 부족: {CurrencyType} (필요 n)` |

### 검증 포인트

- 재료 충분 → 텍스트 흰색, 버튼 활성
- 재료 부족 → 해당 슬롯 텍스트 빨간색, 버튼 비활성
- MAX 레벨 → 강화 영역 전체 숨김, `_maxLevelLabel` 표시

---

## Step 4. 강화 클릭 → 결과 처리 → 상태 반영 🔲

**목표**: 버튼 클릭 시 도메인 서비스가 결과를 결정하고, UI 는 그 결과에 맞춰 애니메이션 후 상태를 반영. 실패 시 동일 곡괭이로 재시도 가능.

### 결과 흐름

```
Button_Enhance 클릭
  → state = Playing, 버튼 비활성
  → service.TryEnhance(type) 호출 (즉시 재료/재화 차감 + 확률 판정)
  → PlayResultAsync(result) — UniTask
      성공: OnPlaySuccessFx() 훅 호출 후 Refresh
      실패: OnPlayFailFx() 훅 호출 후 Refresh
  → state = Idle, Refresh(type)
```

### 구현 내용

**`MIEnhanceTab.cs`**

```csharp
private enum EEnhanceUIState { Idle, Playing }
private EEnhanceUIState _state = EEnhanceUIState.Idle;
private EPickaxeType _currentType;
```

- `Initialize` 에서 `_btnEnhance.onClick` 에 `OnEnhanceClicked` 리스너 등록
- `OnEnhanceClicked` — `_state != Idle` 이면 리턴 (중복 클릭 방지)
- `PlayResultAsync(FEnhanceAttemptResult)` — `async UniTaskVoid`
  - 현 단계: 짧은 `UniTask.Delay` + `MILog` 출력
  - 향후 VFX/사운드 훅 `virtual void OnPlaySuccessFx()` / `OnPlayFailFx()` 만 마련

**`MIEnhancePopup.cs`**

- `service.OnEnhanceAttempted` 구독 → `_selector.RefreshVisual()` 호출 (공격력 텍스트 갱신)
- `OnDestroy` 에서 구독 해제

### 재시도 처리

- 실패 시에도 `_state = Idle` 복귀 → 동일 곡괭이로 재클릭 가능
- `CanEnhance` 가 false 면 버튼 비활성 → Step 3 로직이 자동 처리

### 검증 포인트

- 성공 → 레벨 +1, 공격력 증가, 재료/재화 감소, 새 레벨 비용/확률 표시
- 실패 → 레벨 유지, 재료/재화 감소, 동일 비용 재표시 + 버튼 재활성 (재료 있으면)
- 연출 중 더블 클릭 무시
- MAX 도달 시 강화 영역 즉시 숨김

---

## Step 5. 외부 자원 변동 자동 갱신 🔲

**목표**: 다른 시스템이 재료/재화를 변경할 때 팝업이 열려 있으면 즉시 갱신.

### 구현 내용

**`MIEnhancePopup.cs`** — `Initialize` 에서 구독, `OnDestroy` 에서 해제

```csharp
_userInventory.OnInventoryUpdated  += HandleInventoryUpdated;
_userWallet.OnCurrencyUpdated      += HandleCurrencyUpdated;
_pickaxeInventory.OnPickaxeAdded   += HandlePickaxeAdded;
```

각 핸들러:

```csharp
private void HandleInventoryUpdated(Dictionary<EItemType, int> _)
    => _enhanceTab.Refresh(_selector.Current);

private void HandleCurrencyUpdated(ECurrencyType _, long __)
    => _enhanceTab.Refresh(_selector.Current);

private void HandlePickaxeAdded(EPickaxeType _)
{
    _selector.RefreshOwned();
    _enhanceTab.Refresh(_selector.Current);
}
```

### 검증 포인트

- 팝업을 연 채 게임 내에서 재료를 획득 → 보유량/버튼 활성 즉시 갱신
- 새 곡괭이 제작 완료 시 selector 보유 목록에 추가됨

---

## Step 6. 잠재능력(Ability) 탭 ⏸ 보류

도메인 서비스 미존재. 진행 시 필요한 것:

- `IMIPickaxeAbilityService` (재추첨/잠금/Exchange currency 로직)
- `MIAbilityTab` 본격 구현 + 4 슬롯 (`Image_Ability Slot Frame 01~04`) 바인딩
- 새로운 Config (`MIAbilityConfig`)

---

## 영향받는 파일 전체 목록

| 파일 | 신규/수정 | Step |
|------|-----------|------|
| `Presentation/UI/Popup/Enhance/MIEnhancePickaxeSelector.cs` | 신규 | 1, 2 |
| `Presentation/UI/Popup/Enhance/MIEnhancePopup.cs` | 수정 | 1, 2, 4, 5 |
| `Presentation/UI/Popup/Enhance/MIEnhanceTab.cs` | 수정 | 1, 3, 4 |
| `Presentation/UI/Popup/Enhance/MIAbilityTab.cs` | 수정 | 1 |
| `Presentation/MISceneContext.cs` | 수정 | 1 |
| `Domain/Pickaxe/Enhance/MIPickaxeEnhanceService.cs` | 수정 | 3 |
| `04_Prefabs/UI/Popup/Enhance Pickaxe/Popup Enhance.prefab` | 에디터 | 1 |

---

## 참고 패턴

| 참고 항목 | 파일 |
|----------|------|
| DI/이벤트 구독·해제 패턴 | `Presentation/UI/Popup/Craft/MIPopupCraft.cs:55-87` |
| 재료/재화 슬롯 표시 | `Presentation/UI/Popup/Craft/MICraftDetailPanel.cs:56-110` |
| 팝업 베이스 | `Presentation/UI/Popup/MIPopupBase.cs` |
| 숫자 스프라이트 표기 | `Presentation/UI/Common/MIImageGroups.cs` |
| DI 등록 | `Core/DI/MIRootLifetimeScope.cs:48` |
