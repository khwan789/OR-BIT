# ORBIT — Design Document

## 1. 게임 개요

**ORBIT**은 작은 원형 행성을 자동으로 공전하는 캐릭터를 조작하는 모바일 하이퍼 캐주얼 액션 게임이다. 플레이어는 다가오는 장애물의 형태에 맞춰 점프와 슬라이드를 사용하고, 아이템을 모으며 가능한 오래 생존해 최고 점수와 라운드를 갱신한다.

- 장르: 2D 하이퍼 캐주얼 / 엔드리스 러너
- 플랫폼: Android, iOS
- 화면: 세로형 모바일 UI
- 현재 Unity 버전: `6000.3.24f1`
- 시작 씬: `Assets/Scenes/Menu.unity`
- 플레이 씬: `Assets/Scenes/Main.unity`

## 2. 플레이 경험

게임의 핵심은 한 손으로 즉시 이해 가능한 **점프 / 슬라이드** 판단과, 라운드가 오를수록 빨라지는 공전 속도다. 원형 행성이라는 이동 경로가 일반적인 횡스크롤 러너와 다른 시각적 정체성을 만든다.

### 기본 루프

1. 메뉴에서 캐릭터와 설정을 확인하고 게임을 시작한다.
2. 캐릭터가 `Ground` 태그의 행성을 중심으로 자동 공전한다.
3. 화면의 점프·슬라이드 버튼(PC에서는 Space·아래 방향키)으로 장애물을 피한다.
4. 수집품으로 점수와 골드를 얻고, 일부 아이템은 무적 또는 감속 효과를 준다.
5. 한 바퀴를 통과할 때마다 라운드와 속도가 증가한다.
6. 충돌하면 보상형 광고 부활 기회가 제공될 수 있으며, 최종 게임 오버 후 기록과 업적을 저장한다.
7. 골드/플레이 횟수/누적 라운드로 캐릭터를 해금하고 다시 도전한다.

## 3. 조작과 규칙

| 행동 | 모바일 | 에디터/PC | 결과 |
| --- | --- | --- | --- |
| 점프 | Jump 버튼 누름 | Space | 행성 바깥 방향으로 점프 |
| 슬라이드 | Slide 버튼 누름 | 아래 방향키 | 슬라이드 상태 유지 |
| 버튼 위치 변경 | 메뉴 Swap 버튼 | 메뉴 Swap 버튼 | 점프·슬라이드 UI 위치 교환 |

- 점수는 공전 이동 거리와 수집 아이템 보상으로 증가한다.
- 라운드 통과 시 `speedMultiplier`가 0.1씩 증가한다.
- 장애물 충돌은 기본적으로 게임 오버다.
- 무적 아이템 중에는 속도가 증가하고 장애물을 제거할 수 있는 상태가 있다.
- 감속 아이템은 `slowDownValue`를 증가시켜 실질 공전 속도를 낮춘다.

## 4. 콘텐츠와 보상

### 캐릭터 및 해금

`CharacterDatabase`가 캐릭터 ScriptableObject 목록을 보유하고 `CharacterManager`가 소유/장착 상태를 `PlayerPrefs`에 저장한다.

- 기본 캐릭터: 최초 실행 시 자동 소유/장착
- Cat: 누적 9회 플레이 시 해금
- Rocket: 누적 100라운드 달성 시 해금
- 해금 시 메뉴 캐릭터 선택 버튼에 알림 점이 표시된다.

### 저장 데이터

| 키 | 용도 |
| --- | --- |
| `HighScore` | 최고 점수 |
| `TotalGold` | 누적 보유 골드 |
| `TotalRound` | 누적 통과 라운드 |
| `TotalGamePlay` | 누적 플레이 횟수 |
| `TutorialCleared` | 튜토리얼 완료 여부 |
| `OwnedCharacters`, `EquippedCharacter` | 캐릭터 소유/장착 정보 |
| `BGM`, `SFX`, `SwapButtons` | 사용자 설정 |

## 5. 씬과 시스템 구조

### 씬 흐름

```text
Menu
 ├─ 기록/골드 표시, 캐릭터 선택, 사운드·조작 설정
 └─ Start Game
       ↓
Main
 ├─ CharacterLoader가 장착 캐릭터 생성
 ├─ ObjectPlayer가 행성 공전 및 입력 처리
 ├─ WeightedObjectSpawner/ObjectPoolManager가 장애물·수집품 생성
 ├─ RoundDetector가 라운드·속도 상승 처리
 └─ 충돌 → 부활 광고(조건부) → 게임 오버 → Menu 또는 재시작
```

### 주요 런타임 컴포넌트

| 컴포넌트 | 책임 |
| --- | --- |
| `GameManager` | 전역 게임 상태, 점수·저장·속도·게임 오버 |
| `ObjectPlayer` / `BitPlayer` | 공전 이동, 점프/슬라이드, 충돌 및 무적 |
| `ObjectPoolManager` | 장애물, 수집품, 이펙트, 플로팅 텍스트 재사용 |
| `WeightedObjectSpawner` | 라운드별 가중치 기반 장애물과 수집품 생성 |
| `RoundDetector` | 한 바퀴 통과 감지 및 난이도 상승 |
| `InGameUIManager` / `MainMenuUIManager` | HUD, 부활/게임 오버, 메뉴 UI |
| `AchievementManager` | 캐릭터 해금 조건 검사 |
| `AudioManager` | BGM/SFX 및 사용자 설정 |
| `AdObserver` | LevelPlay 전면·보상형 광고 |
| `GoogleObserver` / `IOSObserver` | 플랫폼별 리더보드·로그인 |

## 6. 튜토리얼과 UX

첫 플레이에서는 튜토리얼 트리거가 점프와 슬라이드를 순서대로 안내한다. 안내가 열려 있는 동안 `Time.timeScale`을 0으로 두고, 사용자가 Continue를 누르면 게임을 재개한다. 튜토리얼 완료는 영구 저장된다.

메뉴는 최고점, 골드, 누적 라운드, 플레이 횟수를 보여 주며 BGM/SFX 토글과 조작 버튼 교환을 제공한다.

## 7. 외부 서비스와 빌드

- 광고: Unity LevelPlay (`com.unity.services.levelplay` 8.10.1)
- Android: Google Play Games 및 Play In-App Update
- iOS: Apple GameKit
- 사운드: `Assets/Sounds/` 및 8-bit BGM 에셋
- 빌드 대상: Android/iOS. Android application id는 `com.PearTree.ORBIT`이다.

## 8. 유지보수 메모

- 싱글턴(`GameManager`, `AudioManager`, `AchievementManager`)은 씬 전환 뒤에도 유지된다.
- 광고 키, 리더보드 ID, 스토어 업데이트 동작은 실제 기기/스토어 환경에서 별도 검증이 필요하다.
- Unity 업그레이드 뒤에는 Android/iOS 각각에서 광고 초기화, 리더보드 로그인, 부활 보상, 첫 실행 튜토리얼을 반드시 스모크 테스트한다.
