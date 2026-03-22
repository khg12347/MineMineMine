# Easy Save 3 — 한국어 완전 가이드

> Unity용 완전한 세이브/로드 솔루션  
> 원문: [docs.moodkie.com/product/easy-save-3](https://docs.moodkie.com/product/easy-save-3/)  
> 대상 독자: Unity 주니어 개발자

---

## 목차

1. [Easy Save 3 란?](#1-easy-save-3-란)
2. [설치 및 시작하기](#2-설치-및-시작하기)
3. [기본 저장 & 불러오기](#3-기본-저장--불러오기)
4. [클래스 · 컴포넌트 · ScriptableObject 저장](#4-클래스--컴포넌트--scriptableobject-저장)
5. [GameObject & 프리팹 저장](#5-gameobject--프리팹-저장)
6. [컬렉션 저장 (배열, 리스트, 딕셔너리 등)](#6-컬렉션-저장)
7. [Auto Save — 코드 없이 저장하기](#7-auto-save--코드-없이-저장하기)
8. [파일 경로 & 저장 위치](#8-파일-경로--저장-위치)
9. [삭제 (키 / 파일 / 디렉토리)](#9-삭제)
10. [암호화 & 압축](#10-암호화--압축)
11. [캐싱으로 성능 높이기](#11-캐싱으로-성능-높이기)
12. [세이브 슬롯 만들기](#12-세이브-슬롯-만들기)
13. [백업 & 복원](#13-백업--복원)
14. [저장할 필드 선택하기 (ES3Type)](#14-저장할-필드-선택하기-es3type)
15. [오류 처리](#15-오류-처리)
16. [ES3 Cloud — 서버 업로드/다운로드](#16-es3-cloud--서버-업로드다운로드)
17. [어셈블리 정의 파일 (Assembly Definitions)](#17-어셈블리-정의-파일)
18. [자주 하는 실수 & 주의사항](#18-자주-하는-실수--주의사항)

---

## 1. Easy Save 3 란?

Easy Save 3(이하 **ES3**)는 Unity에서 게임 데이터를 저장하고 불러오는 작업을 아주 간단하게 해주는 에셋입니다.  
`PlayerPrefs`보다 훨씬 강력하며, 복잡한 파일 IO 코드를 작성하지 않아도 됩니다.

**주요 특징**

- 단 한 줄 코드로 저장/불러오기
- int, float, string 같은 기본 타입부터 클래스, GameObject, 프리팹까지 모두 지원
- 코드 없이 저장하는 **Auto Save** 기능
- AES 암호화 & Gzip 압축 지원
- 세이브 슬롯, 백업, 클라우드 연동 지원
- 모바일(iOS/Android), PC, WebGL 크로스 플랫폼

---

## 2. 설치 및 시작하기

1. Unity Asset Store에서 Easy Save 3를 구매 후 import합니다.
2. import가 완료되면 별도 설정 없이 바로 스크립트에서 사용할 수 있습니다.
3. 에디터 메뉴 `Tools > Easy Save 3`에서 각종 설정을 확인할 수 있습니다.

> **어셈블리 정의(Assembly Definition)를 사용하는 경우**  
> `Tools > Easy Save 3 > Enable Assembly Definition Files`를 먼저 활성화해야 합니다.  
> 이후 자신의 asmdef에 `EasySave3` 어셈블리를 참조로 추가하세요.

---

## 3. 기본 저장 & 불러오기

ES3는 데이터를 **키(key) + 값(value)** 쌍으로 저장합니다. 딕셔너리와 비슷한 구조입니다.

### 저장

```csharp
ES3.Save("myInt", 123);
ES3.Save("myString", "Hello");
ES3.Save("myFloat", 3.14f);
```

### 불러오기

```csharp
// 기본값(defaultValue)을 함께 지정하는 방법 (권장)
int myInt = ES3.Load<int>("myInt", 0);
string myString = ES3.Load<string>("myString", "기본값");
```

기본값을 지정하면 저장된 데이터가 없을 때 기본값을 반환하므로, 첫 실행 시에도 안전합니다.

### 키 존재 여부 확인 후 불러오기

```csharp
if (ES3.KeyExists("myInt"))
{
    int myInt = ES3.Load<int>("myInt");
}
```

### 기존 오브젝트에 데이터 덮어쓰기 (LoadInto)

새 오브젝트를 생성하지 않고, 이미 존재하는 오브젝트에 데이터를 로드할 때 사용합니다.

```csharp
// 씬에 이미 있는 Transform에 저장된 Transform 데이터를 불러옴
ES3.LoadInto("myTransform", this.transform);
```

> **주의:** `LoadInto`는 클래스 같은 참조 타입에만 사용할 수 있습니다.  
> int, float 같은 값 타입(기본형)이나 struct에는 사용할 수 없습니다.

### 언제 저장/불러오기를 호출해야 할까?

| 시점                        | 메서드                      | 설명                        |
|           ------           |--------                    |------                       |
|        게임 시작 시          | `Start()`                  | 저장 데이터를 불러오기에 적합   |
|           앱 종료 시 (PC)   | `OnApplicationQuit()`       | 데이터를 저장하기에 적합       |
| 앱 백그라운드 전환 시 (모바일) | `OnApplicationPause(true)` | 모바일에서 저장하기에 적합      |

---

## 4. 클래스 · 컴포넌트 · ScriptableObject 저장

클래스나 컴포넌트도 기본 타입과 동일한 방식으로 저장합니다.

```csharp
[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int score;
    public float health;
}

// 저장
var data = new PlayerData { playerName = "형관", score = 1000, health = 100f };
ES3.Save("playerData", data);

// 불러오기
var loaded = ES3.Load<PlayerData>("playerData", new PlayerData());
```

### 어떤 필드가 저장되나요?

다음 조건을 **모두** 만족하는 필드가 저장됩니다:

- `public`이거나 `[SerializeField]` 어트리뷰트가 있을 것
- `const` 또는 `readonly`가 아닐 것
- `[Obsolete]` 또는 `[NonSerialized]` 어트리뷰트가 없을 것
- ES3가 지원하는 타입일 것

### UnityEngine.Object 타입 저장 시 주의

`Component`, `ScriptableObject`, `Texture2D` 같은 `UnityEngine.Object` 계열 필드는 **참조**로 저장됩니다.  
즉, 해당 인스턴스가 씬에 없으면 로드 시 `null`이 됩니다.  
값으로 저장하려면 별도로 `ES3.Save`를 호출하고, 참조하는 데이터보다 **먼저** 로드해야 합니다.

---

## 5. GameObject & 프리팹 저장

```csharp
// GameObject 저장
ES3.Save("myGameObject", gameObject);

// GameObject 불러오기
var go = ES3.Load<GameObject>("myGameObject");
```

**저장되는 항목:** `layer`, `tag`, `name`, `hideFlags`, 지원되는 컴포넌트들, 모든 자식 오브젝트

> 프리팹 인스턴스를 저장하려면 프리팹을 우클릭 후 `Easy Save 3 > Enable Easy Save for Prefab`을 먼저 실행해야 합니다.

### Transform(위치) 저장

오브젝트의 위치는 Transform 컴포넌트의 `localPosition`과 `localRotation`으로 정의됩니다.  
Transform 컴포넌트 자체를 저장하면 위치가 함께 저장됩니다.

```csharp
ES3.Save("myTransform", transform);
ES3.LoadInto("myTransform", transform);
```

---

## 6. 컬렉션 저장

배열, 리스트, 딕셔너리 등 컬렉션도 동일한 방법으로 저장합니다.

### Array (배열)

```csharp
int[] myArray = { 1, 2, 3 };
ES3.Save("myArray", myArray);
myArray = ES3.Load<int[]>("myArray", new int[0]);
```

### List

```csharp
var myList = new List<string> { "사과", "바나나" };
ES3.Save("myList", myList);
myList = ES3.Load<List<string>>("myList", new List<string>());
```

### Dictionary

```csharp
var myDict = new Dictionary<string, int> { { "점수", 100 } };
ES3.Save("myDictionary", myDict);
myDict = ES3.Load<Dictionary<string, int>>("myDictionary", new Dictionary<string, int>());
```

### 2D 배열

```csharp
int[,] my2DArray = new int[3, 3];
ES3.Save("my2DArray", my2DArray);
my2DArray = ES3.Load<int[,]>("my2DArray", new int[0, 0]);
```

### Queue / Stack / HashSet

```csharp
// Queue
ES3.Save("myQueue", myQueue);
myQueue = ES3.Load<Queue<int>>("myQueue", new Queue<int>());

// Stack
ES3.Save("myStack", myStack);
myStack = ES3.Load<Stack<int>>("myStack", new Stack<int>());

// HashSet
ES3.Save("myHashSet", myHashSet);
myHashSet = ES3.Load<HashSet<int>>("myHashSet", new HashSet<int>());
```

---

## 7. Auto Save — 코드 없이 저장하기

**Auto Save**는 코드를 작성하지 않고도 씬의 컴포넌트를 저장할 수 있는 기능입니다.  
같은 씬에서 저장하고 불러오는 경우에 적합합니다. 씬 간 데이터 공유는 코드 API를 사용하세요.

### 활성화 방법

1. `Window > Easy Save 3 > Auto Save` 창을 열고 **'Enable Auto Save for this scene'** 버튼 클릭
2. 저장할 컴포넌트를 씬 계층(Hierarchy)에서 선택
3. 컴포넌트 옆 톱니바퀴 아이콘을 눌러 저장할 변수를 세부 설정

### 프리팹 인스턴스 Auto Save

1. 프리팹 우클릭 → `Easy Save 3 > Enable Easy Save for Prefab`
2. `Window > Easy Save 3 > Auto Save > Prefabs` 탭에서 저장할 컴포넌트 선택

### 저장/불러오기 시점 변경

Auto Save 창의 **Save Event**와 **Load Event** 드롭다운에서 원하는 시점을 선택합니다.

> **참고:** GameObject 로드 순서가 보장되지 않으므로, 프리팹 인스턴스 간의 참조가 실패할 수 있습니다.  
> 이 경우 아래처럼 코드로 두 번 로드하는 방법을 사용합니다.

### 코드로 Auto Save 직접 트리거

```csharp
// Save Event / Load Event를 None으로 설정한 뒤 코드로 직접 호출
ES3AutoSaveMgr.Current.Save();
ES3AutoSaveMgr.Current.Load();
```

### 저장 파일 경로 변경 (코드)

```csharp
ES3AutoSaveMgr.Current.settings.path = "MySaveFile.es3";
```

### 계층 필터링

Auto Save 창 상단 검색창에 이름을 입력하면 특정 GameObject만 표시됩니다.  
`tag:` 를 앞에 붙이면 태그로 필터링합니다.

---

## 8. 파일 경로 & 저장 위치

### 기본 저장 위치

기본적으로 `Application.persistentDataPath` 아래에 저장됩니다.  
에디터에서 `Tools > Easy Save 3 > Open Persistent Data Path`로 해당 폴더를 열 수 있습니다.

| 플랫폼 | 저장 위치 |
|--------|-----------|
| Android / iOS / PC | `Application.persistentDataPath` |
| WebGL | PlayerPrefs (→ IndexedDB) |

### 파일명 지정

```csharp
// 기본 위치에 "myFile.es3"로 저장
ES3.Save("myKey", myValue, "myFile.es3");
```

### 상대 경로

```csharp
// 기본 저장 위치 아래 폴더/파일로 저장
ES3.Save("myKey", myValue, "myFolder/myFile.es3");
```

### 절대 경로

```csharp
// 절대 경로 (특수한 경우에만 사용 권장)
ES3.Save("myKey", myValue, "C:/Users/User/Documents/myFile.es3");
```

> **권장:** 절대 경로는 모든 사용자 환경에서 동일하게 동작한다는 보장이 없으므로,  
> 특수한 경우가 아니면 상대 경로 또는 기본 위치를 사용하세요.

### 설정 변경

에디터에서 기본 설정: `Window > Easy Save 3 > Settings`  
런타임에서 설정을 바꾸려면 `ES3Settings` 객체를 사용합니다.

```csharp
// 예: 기본 파일명 변경
var settings = new ES3Settings("myCustomFile.es3");
ES3.Save("myKey", myValue, settings);
```

---

## 9. 삭제

### 특정 키 삭제

```csharp
ES3.DeleteKey("myKey");
// 특정 파일에서 키 삭제
ES3.DeleteKey("myKey", "myFolder/myFile.es3");
```

### 파일 삭제

```csharp
ES3.DeleteFile("myFile.es3");
ES3.DeleteFile("myFolder/myFile.es3");
```

### 디렉토리 삭제

```csharp
ES3.DeleteDirectory("myFolder/");
```

---

## 10. 암호화 & 압축

### AES 암호화

저장 데이터를 암호화하여 사용자가 파일을 직접 편집하기 어렵게 만듭니다.

```csharp
var settings = new ES3Settings(ES3.EncryptionType.AES, "비밀번호1234");

// 저장 (암호화)
ES3.Save("myKey", myValue, settings);

// 불러오기 (동일한 settings 사용 필수)
var myValue = ES3.Load<int>("myKey", 0, settings);
```

> **중요:** 암호화를 활성화하기 전에 기존 세이브 데이터를 삭제해야 합니다.  
> `Tools > Easy Save 3 > Clear Persistent Data Path`로 삭제할 수 있습니다.

**암호화를 더 안전하게 사용하는 팁:**
- 비밀번호를 코드에 하드코딩하지 말고, 서버에서 받아와서 사용자별로 고유하게 설정
- IL2CPP 스크립팅 백엔드를 사용해 역공학 난이도를 높임 (이미 프로젝트에서 사용 중)
- 소스코드를 공개하지 말 것
- 비밀번호는 길고 무작위 문자열로 구성

### Gzip 압축

파일 크기를 평균 약 85% 줄여줍니다.

```csharp
var settings = new ES3Settings(ES3.CompressionType.Gzip);
ES3.Save("myKey", myValue, settings);
var myValue = ES3.Load<int>("myKey", 0, settings);
```

---

## 11. 캐싱으로 성능 높이기

여러 키를 한 파일에서 반복해서 읽고 쓸 때, 매번 파일을 열고 닫으면 성능이 저하됩니다.  
**캐시(Cache)**를 사용하면 파일을 메모리에 올려두고 한 번에 읽고 쓸 수 있습니다.

### 전역 캐시 설정 (Settings에서)

1. `Window > Easy Save 3 > Settings`에서 Location을 **Cache**로 변경
2. 앱 시작 시 기본 파일이 자동으로 캐시에 로드됨
3. 매 프레임 끝에 변경된 캐시가 자동으로 파일에 저장됨

### 코드로 캐시 사용

```csharp
// 파일을 메모리(캐시)에 로드
ES3.CacheFile("myFile.es3");

// 캐시에서 읽고 쓰기 (동일한 ES3.Save/Load 사용)
var settings = new ES3Settings("myFile.es3", ES3.Location.Cache);
ES3.Save("myKey", myValue, settings);
int val = ES3.Load<int>("myKey", 0, settings);

// 캐시를 파일에 저장 (영구 저장)
ES3.StoreCachedFile("myFile.es3");
```

> **팁:** 씬 시작 시 CacheFile, 씬 종료 시 StoreCachedFile을 호출하면 성능을 크게 높일 수 있습니다.

### 레퍼런스 매니저 최적화

씬에 Easy Save 3 Manager가 있을 때, 더 이상 참조되지 않는 오브젝트가 매니저에 남아있을 수 있습니다.  
`ES3ReferenceMgr` 컴포넌트의 **Optimize** 버튼을 눌러 정리하세요.

대형 씬(의존성 100,000개 이상)에서는 `Tools > Easy Save 3 > Settings > Editor Settings`에서  
**Auto Update References**를 끄고 수동으로 레퍼런스를 관리하는 것이 좋습니다.

---

## 12. 세이브 슬롯 만들기

플레이어가 여러 저장 파일(슬롯)을 선택할 수 있는 UI를 손쉽게 추가할 수 있습니다.  
Unity 2020.3 이상, TextMeshPro 패키지가 필요합니다.

### 씬에 슬롯 추가

1. `Assets > Easy Save 3 > Add Save Slots to Scene` (저장용) 또는  
   `Assets > Easy Save 3 > Add Load Slots to Scene` (불러오기용) 선택
2. 별도 코드 없이 슬롯 선택 시 해당 슬롯 파일이 자동으로 사용됨

### 슬롯 선택 후 동작 설정

- **Load Scene After Select Slot**: 슬롯 선택 후 씬 전환
- **On After Select Slot**: 슬롯 선택 후 특정 메서드 호출

### 관련 스크립트 (커스터마이징 참고)

`Assets/Plugins/Easy Save 3/Scripts/Save Slots/` 폴더의 스크립트를 참고하세요.  
수정이 필요하면 원본 대신 **복사본**을 만들어 수정하거나 상속하여 오버라이드하는 방법을 권장합니다.

| 스크립트 | 역할 |
|----------|------|
| `ES3SlotManager.cs` | 설정 관리, 슬롯 생성 담당 |
| `ES3Slot.cs` | 슬롯 선택, 삭제, 덮어쓰기 처리 |
| `ES3CreateSlot.cs` | 새 슬롯 생성, 중복 방지 |
| `ES3SlotDialog.cs` | 다이얼로그(확인/취소) 처리 |

---

## 13. 백업 & 복원

저장 파일을 백업해두면 오류 발생 시 안전하게 복원할 수 있습니다.  
백업 파일은 원본 파일명에 `.bak` 확장자를 붙여 저장됩니다.

```csharp
// 백업 생성 (기존 백업이 있으면 덮어씀)
ES3.CreateBackup("myFile.es3");

// 백업 복원
ES3.RestoreBackup("myFile.es3");
```

**권장 사용 패턴:**

```csharp
// 저장 전에 백업을 만들고, 저장이 완료되면 백업을 삭제
ES3.CreateBackup("save.es3");
try
{
    ES3.Save("myKey", myValue, "save.es3");
    // 저장 성공 — 백업 유지 여부는 자유
}
catch (System.Exception)
{
    // 저장 실패 — 백업 복원
    ES3.RestoreBackup("save.es3");
}
```

---

## 14. 저장할 필드 선택하기 (ES3Type)

기본적으로 자동 직렬화되지 않는 타입이거나, 저장할 필드를 직접 선택하고 싶을 때 사용합니다.

### ES3Type 생성

1. `Window > Easy Save 3 > Types` 탭 열기
2. 목록에서 저장할 타입 선택
3. 저장할 필드/프로퍼티 체크박스 선택

> 타입 옆에 체크 원형 아이콘이 있으면 ES3Type 파일이 이미 존재한다는 뜻입니다.

### ES3Type을 수동으로 수정해야 하는 경우

- 해당 타입에 **파라미터 없는 생성자(기본 생성자)**가 없을 때
- 변수 접근에 특정 메서드 호출이 필요할 때

생성된 ES3Type 파일은 `/Assets/Easy Save 3/Types/` 폴더에 있습니다.  
`Write`, `Read`, `ReadInto` 메서드를 수정하여 직렬화 동작을 제어할 수 있습니다.

> **주의:** Types 패널에서 변경하면 수동 수정 내용이 덮어써집니다.

---

## 15. 오류 처리

저장/불러오기 중 발생하는 오류를 `try/catch`로 처리하는 것을 권장합니다.

```csharp
try
{
    ES3.Save("key", 123);
    ES3.Save("key2", 456);
}
catch (System.IO.IOException)
{
    // 파일이 다른 곳에서 열려있거나, 저장 공간이 부족할 때
    Debug.LogError("파일 저장 실패: 저장 공간 부족 또는 파일이 사용 중입니다.");
}
catch (System.Security.SecurityException)
{
    // 파일 접근 권한이 없을 때
    Debug.LogError("파일 저장 실패: 접근 권한이 없습니다.");
}
catch (System.Exception e)
{
    Debug.LogError($"예상치 못한 오류: {e.Message}");
}
```

---

## 16. ES3 Cloud — 서버 업로드/다운로드

ES3 Cloud를 사용하면 MySQL 서버에 세이브 파일을 업로드/다운로드할 수 있습니다.  
크로스 플랫폼 동기화에 유용합니다.

> **참고:** Steam Auto Cloud, Android Auto Backup, Apple iCloud Backup을 사용하면  
> ES3 Cloud 없이도 기본적인 백업이 지원됩니다. 플랫폼 타겟에 따라 선택하세요.

### 서버 설정

1. `Assets/Plugins/Easy Save 3/Web/` 폴더의 `ES3Cloud.php`를 서버에 업로드
2. 설치 완료 후 제공받은 **API Key** 저장
   - `ES3Variables.php` 파일에서도 확인 가능

### 기본 사용법

```csharp
// ES3Cloud 컴포넌트가 씬에 있어야 합니다
var cloud = FindObjectOfType<ES3Cloud>();

// 파일 업로드
yield return StartCoroutine(cloud.UploadFile("save.es3"));
if (cloud.isError)
    Debug.LogError(cloud.error);

// 파일 다운로드
yield return StartCoroutine(cloud.DownloadFile("save.es3"));
if (cloud.isError)
    Debug.LogError(cloud.error);
```

---

## 17. 어셈블리 정의 파일

프로젝트에서 `.asmdef` 파일을 사용하는 경우 아래 설정이 필요합니다.

1. `Tools > Easy Save 3 > Enable Assembly Definition Files` 실행
2. 자신의 어셈블리 `.asmdef`에서 `Assembly Definition References` 목록에 `EasySave3` 추가

---

## 18. 자주 하는 실수 & 주의사항

| 상황 | 권장 대처법 |
|------|-------------|
| 암호화 활성화 후 기존 데이터가 안 불러와짐 | 기존 저장 파일 삭제 후 다시 저장 |
| 프리팹 인스턴스 저장이 안 됨 | 우클릭 → `Easy Save 3 > Enable Easy Save for Prefab` 먼저 실행 |
| 로드 후 오브젝트 위치가 변경되지 않음 | `PlayerController` 등 위치 캐시 컴포넌트를 비활성화 후 로드, 이후 재활성화 |
| 참조가 null로 로드됨 | 참조 대상이 씬에 존재하는지 확인. 런타임 생성 오브젝트는 씬 재시작 시 사라짐 |
| 필요 없는 데이터까지 저장함 | 전체 GameObject 저장 대신 필요한 컴포넌트만 선택하여 저장 |
| 저장이 너무 느림 | 캐싱(Section 11) 사용 고려, 저장 데이터 양 최소화 |
| Assembly Definition 오류 | `Enable Assembly Definition Files` 실행 후 asmdef에 EasySave3 참조 추가 |
| WebGL에서 파일 저장 안 됨 | WebGL은 자동으로 PlayerPrefs(IndexedDB)에 저장됨, 별도 설정 불필요 |

---

## 빠른 참조 치트시트

```csharp
// ✅ 저장
ES3.Save("키", 값);
ES3.Save("키", 값, "파일명.es3");

// ✅ 불러오기
var val = ES3.Load<타입>("키", 기본값);
ES3.LoadInto("키", 기존오브젝트); // 참조 타입만 가능

// ✅ 존재 확인
bool exists = ES3.KeyExists("키");
bool fileExists = ES3.FileExists("파일명.es3");

// ✅ 삭제
ES3.DeleteKey("키");
ES3.DeleteFile("파일명.es3");
ES3.DeleteDirectory("폴더명/");

// ✅ 암호화
var settings = new ES3Settings(ES3.EncryptionType.AES, "비밀번호");
ES3.Save("키", 값, settings);

// ✅ 압축
var settings = new ES3Settings(ES3.CompressionType.Gzip);
ES3.Save("키", 값, settings);

// ✅ 캐싱
ES3.CacheFile("파일명.es3"); // 메모리에 로드
ES3.StoreCachedFile("파일명.es3"); //캐싱한 파일을 한꺼번에 저장

// ✅ 백업
ES3.CreateBackup("파일명.es3");
var settings = new ES3Settings("파일명.es3", ES3.Location.Cache); //경로 대신 쓸 settings
ES3.Save("키1번", "값1번", settings); // IO 없음, 메모리에만 쓰임
ES3.Save("키2번", "값2번", settings); // ``
ES3.RestoreBackup("파일명.es3");

// ✅ Auto Save (코드)
ES3AutoSaveMgr.Current.Save();
ES3AutoSaveMgr.Current.Load();
```

---

> 이 문서는 [Easy Save 3 공식 문서](https://docs.moodkie.com/product/easy-save-3/)를 바탕으로 작성한 한국어 가이드입니다.  
> 최신 API 변경사항은 공식 문서를 함께 참고하세요.
