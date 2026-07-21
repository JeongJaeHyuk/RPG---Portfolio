# CLAUDE.md

Unity 프로젝트 `Project_T`의 스크립트 구조와 컨벤션 가이드. `Assets/script` 하위 작업 시 이 문서를 따를 것.

## 이 문서와 코드에 대해

이 프로젝트는 개발 도중 AI(Claude Code)와의 대화를 통해 코드 분석·정리·일부 구현이 이루어졌다. 그래서 다음을 감안하고 볼 것:

- 이 문서 자체도 불완전할 수 있다 — 실제 코드와 설명이 어긋나거나, 아직 문서화 안 된 규칙이 있을 수 있다.
- 코드 전반에 네이밍/폴더 규칙이 일관되지 않은 곳이 남아있다("폴더 구조", "네이밍 컨벤션" 참고).
- 책임이 한 파일에 쏠린 곳(god object)이 여전히 여러 군데 있다("컴포넌트 분리 원칙"의 반례 목록 참고).
- **이런 지점들은 방치가 아니라 인지된 상태의 트레이드오프다.** 이미 정상 동작 중이거나, 지금 손대면 코드가 꼬이거나 대공사가 필요해질 위험이 실익보다 커서 AI와 상의 후 의도적으로 그대로 유지하기로 한 것들이다("설계 결정 기록" 참고).
- 반대로 이후에 새로 추가되거나 리팩터링된 부분(예: `Skill_Effect` 상속 구조, "데미지 전달 패턴")은 상대적으로 더 구조적으로 정리된 상태다. 코드를 볼 때 어느 시점 이후에 손댄 부분인지 구분해서 판단할 것.
- 이 문서 자체를 작성한 시점도 남겨둔다. 이 문서는 몬스터 리팩토링·보스 구현만 남은, 사실상 프로젝트 마무리 단계에서 작성됐다. 처음에는 "이제 와서 컨벤션 문서를 새로 만들 필요가 있나" 싶어 넣지 않으려 했으나, 대화 과정에서 남은 작업(스킬 데미지 연산, 맵 추가, 보스 구현)이 구조를 흔들 수 있는 작업들이라 판단해 지금이라도 규칙을 문서화해두는 쪽이 낫다고 결론 내리고 추가하게 됐다.

## 폴더 구조

도메인 단위로 폴더가 나뉘어 있으나 완전히 지켜지지는 않는다.

- `Monster/` — 몬스터 컴포넌트 일체 (State/Move/Range/HitDamage/Spec + 타입별 State 상속)
- `PlayerScript/` — 플레이어 본체, 스탯, 스킬 컴포넌트(`Skill_Effect` 부모 + `Skill_Q/W/E/R`)
- `Data_Manager/` — CSV 파싱 + 데이터 보관 싱글톤 (`*_Data_Manager`, `SaveData`)
- `Manager/` — 런타임 오케스트레이션 싱글톤 (`PlayerManager`, `PlayerResourceManager`, `LoadingSceneManager`, `LoadingData`)
- `UIScript/`, `Inventory/`, `Shop/`, `Quest/`, `Coin/`, `NPC/`, `ObjectPool/` — 각 기능 UI/도메인

**주의할 예외/잔재**:
- 루트에 미분류 파일 다수 존재: `CameraController.cs`, `MobSpawn.cs`, `MobReSpawn.cs`, `PlayerDataManager.cs`, `ResourcesLoadManager.cs`, `Scene_Portal.cs`, `SkillOn.cs`. 새 파일을 여기 추가하지 말고 해당 도메인 폴더에 넣을 것.
- `New Folder/` — 스킬 시스템 전체(`Skill`, `Skill_Manager`, `Skill_Database`, `SkillSlot`, `UI_Skill`)가 이름 없는 폴더에 있음. 정리 전까지는 그대로 두되 새 스킬 관련 파일도 관례상 여기 추가.
- `Manager/` vs `Data_Manager/` 구분 기준: "CSV 파싱/데이터 보관"은 Data_Manager, "런타임 오케스트레이션"은 Manager. 단 `PlayerDataManager.cs`는 이 기준을 어기고 루트에 있음(주의).

## 네이밍 컨벤션

- **오브젝트 이름 → 타입 매칭**: 프리팹/게임오브젝트 이름의 `_` 앞부분이 enum/키로 파싱된다 (`gameObject.name.Split('_')[0]`). 몬스터 스폰·리스폰·스펙 로드·드랍 전부 이 컨벤션에 의존하는 암묵적 계약이다.
  - 예: `RockGolem_1`, `BlueOrc_A` → `MonsterType.RockGolem`, `MonsterType.BlueOrc`
  - 관련 코드: `Monster/Monster_Spec.cs`(LoadSpec), `MobReSpawn.cs`, `MobSpawn.cs`
  - **새 프리팹은 반드시 `EnumName_기타문자열` 형식으로 명명할 것.** 어기면 조용히 null이 반환된다.
- **스킬 이름 매칭**: `SkillAllData.csv`의 `skill_Id`와 `Player_Skill_Data.csv`의 `skillName`이 문자열로 정확히 일치해야 한다(`Skill_Manager`, `UI_Skill.IniSkillSlot`). 오탈자 시 조용히 매칭 실패.
- **클래스명(단어 구분)은 앞으로 언더스코어+PascalCase로 통일한다** (`Monster_State`, `Player_Skill_Stats` 스타일). C# 식별자는 띄어쓰기가 안 되니, 띄어쓰기가 필요한 자리는 전부 `_`로 대신한다는 개인 컨벤션과 일치시킨 것.
  - 기존 코드에는 `PlayerProgression`, `PlayerSpecs`처럼 순수 PascalCase(언더스코어 없음)로 된 파일도 섞여 있다. **기존 파일을 지금 당장 일괄 변경하지 말 것**(참조 범위가 커짐 — 별도 작업). **새 클래스부터 언더스코어+PascalCase로 작성할 것.**
- **매개변수는 항상 `_` 접두사를 붙인다** (`_item`, `_skill`, `_damage`, `_player` 등). 지역변수·필드와 한눈에 구분하기 위한 의도적인 규칙 — 함수 정의만 봐도 어디까지가 매개변수인지 바로 알 수 있게 하기 위함. 새 함수를 작성할 때도 이 규칙을 따를 것.
- **private 필드는 접두사 없이 camelCase로 쓴다** (`basicDamage`, `maxHp`, `gold` 등). 매개변수(`_` 접두사)와 대비되는 자리이므로, 접두사가 없으면 필드/지역변수, 있으면 매개변수로 구분된다.
- **프로퍼티는 전부 대문자(SNAKE_CASE)로 쓴다** (`MAX_HP`, `CURRENT_HP`, `TOTAL_DAMAGE`, `CURRENT_LEVEL` 등). 의도: 소문자 필드/일반 변수와 구분해서 "이건 프로퍼티고, setter에서 이벤트를 Invoke해서 구독 시스템과 연동되어 있을 수 있다"는 걸 이름만 보고 바로 알아보기 위함. `PlayerSpecs`, `PlayerProgression`, `Monster_Spec`, `Skill`, `Quest`(`CURRENTPROGRESS`) 전반에 이 규칙이 지켜지고 있다. (예외: `PlayerSpecs.ComboCount`처럼 PascalCase로 남은 것도 일부 있음 — 새 코드는 대문자 규칙을 따를 것.)
- **bool 변수는 항상 `is` 접두사를 붙인다** (`isDead`, `isAttack`, `isShopOpen`, `isCoolDown` 등). 코드 전체에서 예외 없이 지켜지고 있는 기존 관례이므로 계속 따를 것.
- **상수(const)는 앞으로 `const` 접두사를 붙인 camelCase로 쓴다** (`constCopperMax` 스타일). SCREAMING_SNAKE_CASE로 하지 않는 이유: 프로퍼티가 이미 전부 대문자를 쓰고 있어서, 상수까지 대문자로 하면 프로퍼티와 헷갈린다. `const`라는 단어가 이름 안에 보이면 "이건 상수구나"를 바로 알 수 있다.
  - 기존 코드에는 `LOADING_SCENE_NAME`, `AES_KEY`처럼 SCREAMING_SNAKE_CASE로 된 상수도 있다. **기존 상수를 지금 당장 일괄 변경하지 말 것.** **새 상수부터 `const` 접두사 camelCase로 작성할 것.**
- **싱글톤은 앞으로 소문자 `instance`로 통일한다.** (`public static X instance;`) 기본 규칙: **변수는 소문자로 시작, 함수는 대문자로 시작**. `instance`는 함수가 아니라 참조를 담는 변수이므로 소문자가 맞다는 판단.
  - 기존 코드에는 PascalCase 프로퍼티 스타일(`public static X Instance { get; private set; }`, 예: `PlayerManager`, `CoinManager`, `UI`)과 lowercase 필드 스타일(예: `DropTable_Manager`, `ToolTip`, `UI_Quest`)이 혼재한다 — 같은 `ToolTip/` 폴더 안에서도 파일마다 다를 정도.
  - **기존 파일의 `Instance`를 지금 당장 일괄 변경하지 말 것** (참조하는 곳이 많아 범위가 커짐 — 별도 작업으로 진행). **새로 만드는 싱글톤부터 소문자 `instance`로 작성할 것.**

## 데이터 흐름 패턴

```
CSV (Resources/.../*.csv)
  → *_Data_Manager 싱글톤이 파싱 (Awake에서 로드)
  → 런타임 컴포넌트가 이름/enum으로 조회
```

- 몬스터: `MonsterData.csv` → `Monster_Data_Manager` → `Monster_Spec.LoadSpec()`
- 스킬: `SkillAllData.csv`(레벨별 수치) + `Player_Skill_Data.csv`(보유 스킬) → `Skill_Data_Manager` → `Skill_Manager`
- 세이브: `PlayerDataManager` ↔ `SaveData` ↔ `PlayerManager.SaveCurrentData/ApplyData`

## 데미지 전달 패턴 (받는 쪽이 계산해서 가져간다)

**모든 데미지 계산/적용은 공격을 받는 쪽에서 해결한다.** 공격하는 쪽(스킬 이펙트, 무기 콜라이더 등)이 상대방의 `TakeDamage()`를 직접 호출하지 않는다.

- 공격 판정 콜라이더는 전용 레이어만 갖는다: `PlayerBasicAttack`, `PlayerSkillAttack`, `MonsterAttack`.
- 받는 쪽(`Monster_HitDamage`, 추후 구현할 플레이어 피격 처리)이 자신의 `OnTriggerEnter`에서 그 레이어를 감지하고, `GetComponent`로 공격 쪽 컴포넌트를 가져와 "데미지 값을 계산해서 반환하는 함수"를 호출해 자기 체력에 반영한다.
- 예: `Monster_HitDamage.OnTriggerEnter`가 `PlayerBasicAttack`/`PlayerSkillAttack` 레이어를 감지 → 각각 `PlayerSpecs.GetComboDamage()` / `Skill_Effect.SkillAttack()`을 호출해 자기 `CURRENT_HP`를 깎는다.
- 새 공격 수단(몬스터 공격 등)을 추가할 때도 이 패턴 유지: 공격 쪽에는 "내 데미지가 얼마인지 계산해서 반환하는 함수"만 두고, 실제로 체력을 깎는 코드(`TakeDamage`)는 항상 맞는 쪽에만 둔다.

## 컴포넌트 분리 원칙 (Monster를 표준으로 삼을 것)

Monster 계열은 sibling-component 패턴으로 잘 쪼개져 있다. 새 시스템을 만들 때 이 구조를 기준으로 삼는다.

**리팩토링 배경**: 이 구조는 처음부터 이랬던 게 아니라, 보스몬스터를 추가하기 전에 리팩토링을 거쳐 지금 형태로 재설계된 것이다(git 히스토리: "몬스터 리팩토링" x2, "몬스터 시스템 재설계"). 동기는 "보스 추가 전에 구조를 정리해두면 보스를 얹을 때도 깔끔하게 확장된다"였고, 실제로 이 리팩토링 도중 문제가 발생해 스킬 데미지 연산 쪽도 함께 다시 설계하게 됐다(`Skill_Effect` 상속 구조로 이어짐). 이 경험이 `Player.cs`를 지금 당장 안 건드리는 판단의 근거이기도 하다 — 부분적으로 손대다 끝나지 않고 구조 전체를 다시 짜야 할 위험이 실제로 있었기 때문이다.

| 컴포넌트 | 책임 |
|---|---|
| `Monster_State` | 상태(대기/공격/사망)·애니메이션 허브, virtual 메서드로 확장 지점 제공 |
| `Monster_Move` | 이동 AI (추격/복귀) |
| `Monster_Range` | 플레이어 감지 (트리거) |
| `Monster_HitDamage` | 전투 판정·피격·사망 보상(경험치/골드/드롭) |
| `Monster_Spec` | 스탯 데이터 (CSV에서 로드) |

타입별 확장은 `Monster_State`를 상속(`Monster_State_BlueOrc`, `Monster_State_RockGolem`, `Monster_State_Special` 등)하여 virtual 메서드를 override하는 방식.

**반례(피해야 할 패턴)**: `Player.cs`는 이동·기본공격·스킬입력·스킬이동 코루틴·피격을 한 클래스에 몰아넣은 god object다. Monster처럼 컴포넌트 분리가 안 되어 있다는 것을 인지하고, **더 키우지 말 것**. 새 플레이어 기능은 가능하면 별도 컴포넌트로 뺄 것 (완전한 리팩터링은 별도 작업으로 논의 후 진행).

다른 god object 후보(파일 하나가 여러 무관한 책임을 가짐): `SkillOn.cs`(모든 스킬 이펙트 로드/배치), `ResourcesLoadManager.cs`(몬스터/스킬아이콘/아이템아이콘/코인 4종 리소스), `PlayerManager.cs`(세이브/로드 전체 인라인). 이 파일들을 수정할 때 책임을 더 추가하기보다 기존 패턴(switch/case 추가) 안에서 최소 변경으로 처리할 것.

## 이미 잘 되어 있는 부분 (참고할 기존 패턴)

Monster/`Skill_Effect` 말고도 프로젝트 전반에 이미 일관되게 잘 지켜지고 있는 패턴들. 새 코드를 짤 때 아래 기존 관례를 우선 따를 것 — 새로 방식을 고안하지 말 것.

- **`BaseShop` 상속 구조**: `Consume_Shop`(고정 슬롯)/`Equip_Shop`(동적 생성)이 `BaseShop`을 상속해 `LoadShopItems`/`ClearShopItems`/`CreatedShopItems`만 override한다. 공통 로직(구매, 골드 갱신, 탭 전환)은 부모에 있음. `Skill_Effect`와 같은 template method 패턴이 먼저 있던 사례이므로, 새 상속 구조를 만들 때 이 두 사례를 함께 참고할 것.
- **복사 생성자 패턴**: `Item(Item other)`, `Quest(Quest other)` — CSV에서 로드된 "원본 템플릿"과 플레이어가 실제로 들고 있는 "인스턴스"를 분리한다. 새로운 데이터 클래스(예: 미래에 장비 강화, 인챈트 등)를 추가할 때도 원본을 직접 공유하지 말고 이 패턴을 따를 것 — 참조 공유로 인한 수량/상태 오염을 막기 위함.
- **이벤트 기반 스탯→UI 갱신**: `PlayerSpecs`/`PlayerProgression`의 프로퍼티 setter가 `HpChage`/`MpChage`/`ExpChage`/`LevChage` 등 이벤트를 Invoke하고, UI(`HpBar` 등)는 그 이벤트를 구독해서 값이 바뀔 때만 갱신한다. 매 프레임 폴링하지 말고 이 구독 패턴을 따를 것. 구독 해제(`OnDestroy`에서 `-=`)도 항상 같이 챙길 것.
- **오브젝트 풀 (`Dictionary<string, Queue<GameObject>>` / `Queue<T>`)**: `CoinManager`, `LootBagPool`이 동일한 큐 기반 풀링 방식을 쓴다. 오브젝트는 `OnDisable()`(또는 `Clear()`)에서 스스로 풀에 반환한다. 새로운 소모성 오브젝트(투사체, 이펙트 등)를 풀링할 때 이 방식을 그대로 따를 것.
- **`*_Data_Manager` CSV 파싱 뼈대**: `Monster_Data_Manager`/`Skill_Data_Manager`/`Quest_Data_Manager`/`Item_Data_Manager`/`DropTable_Manager` 전부 "싱글톤 + Awake에서 CSV 로드 + Dictionary/List 보관 + Get 함수로 조회" 구조가 동일하다. 새 CSV 데이터를 추가할 때 이 뼈대를 그대로 복제할 것.

## 새 요소 추가 시 규칙

### 스킬(데미지 연산 포함) 추가

`Skill_Q/W/E/R`는 공통 부모 `Skill_Effect`(`PlayerScript/Skill_Effect.cs`)를 상속한다. 공통 필드(`plsp`/`pps`/콜라이더)와 `Awake/OnEnable/OnDisable/SetSkillData`, 기본 데미지 공식(`virtual SkillAttack()` = `TOTAL_DAMAGE × CURRENT_DAMAGE`)은 부모가 갖고 있다.

- **배율 없는 스킬(Q/W처럼)**: `public class Skill_X : Skill_Effect { }` — 빈 클래스로 끝. override 불필요.
- **타격당 배율이 있는 스킬(E/R처럼)**: `SkillAttack()`만 override해서 `return base.SkillAttack() * damageMultiplier;`.
- 몬스터에게 데미지가 전달되는 경로는 스킬 이펙트 쪽이 아니라 **몬스터 쪽**(`Monster_HitDamage.OnTriggerEnter`)에서 처리한다. `PlayerBasicAttack` 레이어를 감지하는 기존 패턴과 동일하게, `PlayerSkillAttack` 레이어를 감지하면 `GetComponent<Skill_Effect>()`로 (Q/W/E/R 구분 없이) `SkillAttack()`을 호출해 데미지를 뺀다.
- **새 스킬 이펙트 콜라이더 오브젝트는 반드시 `PlayerSkillAttack` 레이어로 설정할 것.** 레이어는 자동 상속되지 않으므로 (다타수 스킬처럼 자식 오브젝트가 여러 개면) 부모에서 레이어 변경 시 뜨는 "change children" 팝업에서 Yes를 눌러 일괄 적용해야 한다. 빠뜨리면 콜라이더는 있어도 데미지가 조용히 안 들어간다.
- CSV/아이콘/이펙트 등록은 기존과 동일하게 필요:
  1. `SkillAllData.csv`와 `Player_Skill_Data.csv` 양쪽에 **동일한 스킬 이름 문자열**로 행 추가.
  2. `ResourcesLoadManager.GetRcSkillIcon()` switch에 아이콘 case 추가.
  3. `SkillOn.cs`에 이펙트 오브젝트 필드 + 로드 함수 + `SkillX_ON()` 함수 3종 추가.
- `Player.cs`에 스킬 데미지/판정 로직을 직접 추가하지 말 것. `Player.cs`는 입력 처리·애니메이션 트리거만 담당(스킬 개수가 고정이라 지금 당장 더 쪼갤 이유는 없음, 아래 "설계 결정 기록" 참고).

**타수별 배율 설정은 인스펙터에서 인스턴스별로 한다** (`Skill_R`처럼 자식 오브젝트마다 별도 `Skill_R` 컴포넌트 + 별도 `damageMultiplier` 값). 코드로 통합 관리하지 말 것 — 지금 스킬 4개·타수 몇 개 고정 규모에서는 인스펙터 값이 코드보다 짧고 재컴파일 없이 밸런스 조절도 가능해서 더 적합하다.

### 맵(씬) 추가
1. `Manager/LoadingData.cs`의 `SceneName` enum에 항목 추가.
2. Unity Build Settings에 **동일한 이름**으로 씬 등록 (enum.ToString()이 곧 씬 이름).
3. 씬에 `PlayerResourceManager`, `ResourcesLoadManager`, `MobSpawn`(자식으로 `MobReSpawn` N개) 배치.
4. 몬스터 스폰 자식 오브젝트 이름은 `"{MonsterType}_기타문자열"` 형식 필수.

### 보스 추가
1. `Monster_State_Boss : Monster_State`를 만들어 `EnterAttack`/`AttackColliderOn` 등 virtual 메서드 override. (`Monster_State_Special`이 스킬형 몬스터용 훅으로 이미 존재)
2. `Monster_Data_Manager`의 `MonsterType` enum + `MonsterData.csv`에 스탯 행 추가.
3. `Monster_HitDamage`/`Monster_Move`는 virtual이 없으므로 페이즈 전환·원거리 공격 등 보스 전용 로직은 오버라이드로 넣을 수 없다. **새 sibling 컴포넌트를 추가**(`GetComponent`로 연결)하는 방식으로 확장할 것 — 기존 두 클래스를 뜯어고치지 말 것.
4. 몬스터 진영에는 플레이어의 `SkillOn.cs` 같은 이펙트 매니저가 없다(현재 몬스터 공격은 애니메이션 이벤트로 콜라이더 on/off만 함, `Monster_State.AttackColliderOn/Off`). 원거리/스킬형 보스가 필요하면 이 부분을 먼저 설계해야 한다.

### NPC 대화 (계획, 미구현)
보스 몬스터 구현 이후 진행 예정. 테이블(CSV) 기반, 퀘스트/보스 진행 상태에 따라 조건부로 대화 전환. 착수 시 사용자가 먼저 말할 예정 — 자세한 내용은 그때 논의.

## 설계 결정 기록 (일부러 안 한 것들)

이 프로젝트는 포트폴리오용으로 **캐릭터 1종, 스킬 Q/W/E/R 4개 고정**이며 확장 계획이 없다. 그래서 "나중에 늘어날 걸 대비해 일반화"하는 작업을 의도적으로 하지 않은 지점들이 있다. 규모에 안 맞는 선반영이 아니라 현재 스코프에 맞춘 판단이므로, 아래 항목들을 "미완성"으로 오해하지 말 것.

- **`Player.cs`를 컴포넌트로 안 쪼갬**: 이동·기본공격·스킬 입력·애니메이션 트리거가 한 클래스에 있다. 캐릭터가 하나뿐이라 늘어날 일이 없고, 스킬 트리거 함수들은 Unity 애니메이션 이벤트에 메서드 이름으로 바인딩돼 있어 쪼개면 애니메이션 클립을 전부 다시 연결해야 하는 리스크 대비 실익이 없다.
- **스킬 타수별 `damageMultiplier`를 코드로 통합 관리 안 함**: `Skill_R`처럼 자식 오브젝트마다 별도 인스턴스가 각자 `[SerializeField] damageMultiplier`를 가진다. 스킬 종류/타수가 지금처럼 적고 고정이면 인스펙터 값이 더 간단하고 실용적이다. **만약 나중에 스킬 종류가 크게 늘어난다면**, 그때는 아래처럼 CSV/코드 기반으로 바꾸는 게 맞다 — 지금 방식을 유지하면서 스킬만 계속 늘리면 인스펙터 설정이 스킬 수만큼 반복 작업이 된다.

  #### (참고, 미구현) 나중에 스킬이 다양해질 경우의 확장 설계

  **1. CSV에는 "타격별 비율 리스트"를 문자열 하나로 저장한다.**
  타수를 별도 컬럼으로 두거나(`attack_1`,`attack_2`...) "타수" 개수를 따로 저장할 필요 없음 — 리스트 길이가 곧 타수다. 지금 `skill_CoolTime`/`skill_Damage`/`skill_MpCost`를 레벨별로 배열로 파싱하는 것과 동일한 방식으로, 구분자로 묶은 문자열 하나를 컬럼에 넣고 파싱 시점에 배열로 변환한다.

  ```csv
  name,Level,coolTime,Damage,mpCost,hitRatio
  Finish Attack,1,30,2.5,200,"0.1;0.1;0.2;0.6"
  ```
  ```csharp
  // Skill_Data_Manager에서 skill_CoolTime 등과 동일한 방식으로 파싱
  float[] hitRatios = data[5].Trim().Split(';').Select(float.Parse).ToArray();
  ```
  균등분배(`100 / 타수`)는 이 리스트의 모든 값이 같은 특수한 경우일 뿐이다. 마지막 타격이 더 센 피니셔처럼 타수마다 다른 비율을 주려면 처음부터 리스트로 저장해야 한다.

  **2. "몇 번째 타격인지"를 알아내는 방법은 파티클(이펙트) 구조에 따라 둘로 갈린다.**

  | 구조 | 방법 |
  |---|---|
  | **독립 오브젝트 구조 (지금 이 프로젝트)** — 타격마다 별도 자식 오브젝트 + 별도 콜라이더 | `SkillOn.cs`가 `SkillR1_ON()`~`SkillR4_ON()` 중 어느 함수를 호출하는지 이미 알고 있으므로, 그 자리에서 인덱스를 직접 넘긴다(`SetHitIndex(i)`). 카운트를 셀 필요가 없다. |
  | **단일 오브젝트 구조** — 파티클 하나로 애니메이션 전체를 표현하고, 애니메이션 이벤트 키에 맞춰 같은 데미지 센서가 반복적으로 켜졌다 꺼짐 | 오브젝트가 하나뿐이라 인덱스를 직접 넘길 대상이 없다. 대신 애니메이션 이벤트가 호출될 때마다 **카운트를 1씩 증가**시키고, 그 값을 인덱스로 쓴다. |

  카운트 방식을 쓸 경우, **리셋은 애니메이션이 끝날 때가 아니라 시작할 때 해야 한다.** 애니메이션이 중간에 캔슬(피격, 다른 입력 등)되면 종료 이벤트가 아예 호출되지 않을 수 있어서, "끝날 때 리셋"은 다음 시전의 인덱스가 밀리는 버그로 이어질 수 있다. "시작할 때 무조건 0으로 초기화"하면 이전 시전이 어떻게 끝났든 항상 깨끗한 상태로 시작한다.

  **3. (참고) 실무에서는 VFX와 데미지 판정을 아예 분리하기도 한다.** 파티클이 몇 개든, 데미지가 몇 번 들어가는지는 애니메이션 이벤트가 코드 함수를 직접 호출하는 타이밍만으로 결정하고, 눈에 보이는 이펙트 개수와는 무관하게 짜는 경우가 많다. 이 프로젝트는 "파티클 오브젝트 하나 = 콜라이더 하나 = 데미지 판정 하나"로 묶어서 단순화한 것으로, 규모에 맞는 실용적 선택이지 유일한 정답은 아니다.

## 빌드 전 반드시 고쳐야 하는 것 (알려진 문제, 미수정)

**CSV 데이터 로드 경로가 지금은 에디터에서만 동작하고 실제 빌드(.exe)에서는 깨진다.** 원인과 고칠 방법을 미리 기록해둔다 — 완성 단계에서 실제 빌드 테스트하기 전에 반드시 처리할 것.

- **원인**: `Monster_Data_Manager`, `Skill_Data_Manager`, `Item_Data_Manager`, `Quest_Data_Manager`, `DropTable_Manager` 다섯 개가 전부 CSV 경로를 `Application.dataPath + "/Resources/..."`로 만들고 `File.Exists`/`StreamReader`(System.IO)로 직접 읽는다. 이 방식은 유니티 에디터에서 `Application.dataPath`가 실제 `Assets` 폴더를 가리키기 때문에 우연히 동작하는 것뿐이다.
- **왜 빌드에서 깨지는가**: 유니티는 빌드할 때 `Resources` 폴더 안의 내용을 원본 파일 그대로 두지 않고 압축된 내부 리소스 번들로 묶어버린다. 그래서 빌드 결과물 안에는 `MonsterData.csv` 같은 파일이 눈에 보이는 형태로 아예 존재하지 않는다 — "빌드하고 나서 경로를 찾아 넣기"로는 해결 불가능(찾을 파일 자체가 없음).
- **고치는 방법**: CSV 파일들을 `Assets/Resources/...`에서 `Assets/StreamingAssets/...`로 옮기고, 경로 생성 코드를 `Application.dataPath + "/Resources/..."` → `Application.streamingAssetsPath + "/..."`로 교체한다. `StreamingAssets` 폴더는 유니티가 압축하지 않고 원본 그대로 빌드에 포함시켜주므로, 경로만 바꾸면 `File.Exists`/`StreamReader` 나머지 코드는 그대로 재사용 가능하다.
  - (대안) `Resources.Load<TextAsset>()`로 완전히 바꾸는 방법도 있음 — 파일을 옮길 필요는 없지만 읽는 코드(`File.Exists`/`StreamReader` → `Resources.Load` + `.text`)를 다시 써야 해서 수정량이 더 크다.
- **영향 범위**: 위 5개 `*_Data_Manager`가 전부 CSV를 못 읽으면 몬스터/스킬/아이템/퀘스트/드랍 데이터가 게임 시작부터 전부 비어있게 된다 — 빌드 후 첫 실행에서 바로 드러나는 심각한 문제이므로 빌드 테스트 체크리스트 1순위로 확인할 것.

## 공통 실패 지점 (조용히 null이 되는 곳)

새 데이터 추가 시 아래 4단계 중 하나라도 빠지면 예외 없이 조용히 실패(null 리턴/로그만 남음)하므로 빠짐없이 확인:
1. CSV 행 추가
2. `*_Data_Manager` 파싱 로직에 컬럼 매핑 확인
3. enum/이름 문자열 매칭 확인 (오탈자 주의)
4. `ResourcesLoadManager`/`SkillOn` 등 switch/case에 새 항목 추가
