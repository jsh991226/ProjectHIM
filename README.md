# ProjectHIM
Unity / C# / Pun / Spring / Mysql 을 연동해 만든 온라인 게임

### Team INFO
##### 팀장 / 개발 총괄 : 주승환 [ https://github.com/jsh991226 ]
##### 클라이언트 개발 : 권성우 [ https://github.com/giteukham ]
##### 클라이언트 개발 : 이유진 [ https://github.com/Goonbam ]
##### 웹 프론트엔드 : 김수용 [ https://github.com/suyong1213 ]


### Source Code INFO
##### 해당 프로젝트는 유료 에셋을 사용한 프로젝트입니다
##### 현재 깃에 올라온 코드엔 에셋이 포함되어 있지 않아 
##### 프로젝트를 받아서 그대로 실행시 실행되지 않습니다
##### 상세한 내용을 확인하고 싶으시면 메일로 연락주시면 감사하겠습니다
##### 팀장 : jsh991226@gmail.com


# Game Story INFO
플레이어는 중세 몬스터 종족을 선택 해 플레이 하며
함께 마을을 약탈 할 "작업반" 인원들을 모아 시나리오에 진입 해
마을의 시민을 살해하고 물건을 약탈 하여
보스NPC를 처치하고 탈출구가 열리면 시간안에 탈출하는
PVPVE 형태의 타르코프라이크 게임입니다



# Game Main System
메인 타이틀 로비 입니다, 회원가입/로그인을 할 수 있습니다
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/f1184060-b85d-441b-b028-c6c910567ce5)


최초 계정 생성시 종족을 선택할 수 있는 화면 입니다
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/175f453c-82d4-4d71-9223-ea77cc764b5d)


종족 선택 후 접속시 전체 UI 입니다
좌측 상단 체력바/상태, 우측상단 미니맵, 좌측하단 채팅창, 우측하단 현재 장착한 장비 정보 입니다
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/00266260-fbf9-478f-9ff1-1e2ffe637271)


# Inventory System
메인 시스템인 인벤토리 시스템 입니다
다양한 장비와 아이템이 존재 하며 각 아이템 마다 크기와 모양이 각각 존재 해 인벤토리 공간 활용을 잘 해야하는 시스템 입니다
샘플 시스템은 [FarrokhGames](https://github.com/FarrokhGames/Inventory)에서 참조 했으며
실제 응용 및 데이터베이스 연결은 팀에서 직접 처리 하였습니다
![image]()


<img width="500" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/c3ddc1a3-e416-4d1f-ac3e-2aca558c202c">
<img width="500"alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/31cf8e2c-3274-4106-8820-e3bcfc2ba545">


장착시 실제 멀티 연동까지 구현 되어 있습니다
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/f2843089-1fdb-443a-b3d6-8c9d842c780f)


# Item Craft System
위의 인벤토리 시스템에서 파생하여 만들어진 아이템 제조 시스템 입니다
아이템이 차지하는 크기가 클수록 조합 확률이 높아지며 
"룬"아이템의 경우에는 크기에 따른 확률이 아닌 아이템 자체에 기입된 확률이 따로 있습니다
조합에 성공한 아이템은 인벤토리가 아닌 우편함으로 따로 지급되며 해당 기술은 아래에서 따로 서술 하겠습니다


<img width="600" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/32da7f5b-2345-4707-a7ae-1d9d497652e3">
<img width="300" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/ddd6adc4-6dda-41b5-a052-c87265cbb92e">
<img width="700" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/536764f9-62a1-41e0-9f9d-ad0f7019fc05">

# AI based NPC Random Dialogue
ChatGPT API를 스프링에서 Json을 Return하는 페이지를 따로 구축해 두고
게임 클라이언트에서 해당 NPC의 직업과 이름을 웹에 넘겨 AI가 생성한 대사를
돌려받아 인게임 NPC에게 적용하였습니다

![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/bf968aaa-837e-438d-8817-57747b044340)

<img width="900" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/12c26975-3f83-42a0-aaaf-5867ae976623">


# Web Parsing Post Box
저희 게임은 웹 페이지에서 개인 정보를 확인하고 웹 상점에서 아이템을 구매할 수 있습니다
웹 상점에서 구매한 아이템들은 게임 클라이언트에서 우편함을 통해 확인할 수 있습니다
우편함의 아이템들은 모두 웹에서 파싱한 값으로 나타냅니다

### 웹 상점에서 아이템을 구매할 때
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/ce114215-c964-49af-a16c-232c0fd77dae)


### 웹에서 구매한 아이템이 우편함에 도착했을 시 나오는 공지사항
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/81742443-362a-455b-b978-b2ad241fc29b)


### 구매한 아이템이 들어있는 우편함
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/7d7ad31b-cf60-427c-9b71-2ca075b4e23f)


# Scenario Party System
일반적으로 PhotonNetwork Pun2 에서는 한번 Room으로 입장 하면 다른 Room 목록을 불러올 수 없어
저희 기획인 3D형태의 인게임 로비 에서 활동 하다가 인원을 모아 활성화된 Room으로 넘길 수 없었습니다
그래서 데이터베이스와 코드를 활용 하여 한번 들어온 Room에서 다른 활성화된 방의 정보와 인원을 받고
전체 인원이 Ready 상태가 되면 새로운 Room을 만들어 일부 유저만 다른 게임으로 옮겨가는 시스템을 만들어 냈습니다

<img width="450" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/2eec9285-fe8b-4f48-b7c9-3992fc6548f6">
<img width="450" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/76991bdf-3a8b-47cb-bf94-bcf81059455f">

##### 시나리오 참여시 상단에 상태 정보가 나타나며
##### TAB키를 눌러 현재 참여한 인원 정보를 확인할 수 있습니다
##### 유저가 입장시 실시간으로 반영되며 방장을 제외한 다른 유저는
##### 준비 완료 버튼을 누를시 프로필 테두리가 붉은색으로 변경되어 확인할 수 있습니다
![습격반 생성](https://github.com/jsh991226/ProjectHIM/assets/81565737/e72a44db-d77d-431f-96c0-f30ae46f8cf9)



### 프리팹을 이용하여 간단히 새로운 시나리오를 추가할 수 있습니다
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/6e460b2a-781b-4dd9-91ff-15cd01e7b507)
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/6ac0d734-d655-45bd-bc2e-bc967e1cf54d)


### 컨트롤러 스크립트에 실제 현재 인원 현황을 확인하는 코드를 통해 연동하는 기능을 확인할 수 있습니다\
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/73daf585-b372-4c50-98ac-9cba54c89c4f)


# Scenario System InGame Rule
최소 플레이 인원이 충족되면 방장은 게임 시작 버튼을 통해 시나리오에 진입할 수 있습니다
서로 다른 위치에 스폰 되며 플레이어들은 서로 죽일수도 있고 협동해서 시나리오를 탈출할 수도 있습니다
게임 입장 후 스테이지 기본 클리어시간안에 후에 서술할 시나리오의 보스를 처치해야 합니다
보스를 처치하지 못한다면 탈출구는 생성되지 않으며 모든 작업반 인원은 사망 후 로비로 돌아갑니다

### 스테이지 클리어 시간을 보여주는 UI
<img width="674" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/e2384680-1912-4758-b10a-d1f5f5c6141b">

### 시나리오 보스 처치 시 변경되는 상단 UI
탈출구의 최대 개수는 현재 남은 작업반 인원 수 입니다, 랜덤한 위치에 한개씩 만 생성됩니다
<img width="469" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/ef65af05-1e73-44dd-845e-9078a2b88e3f">



### 멀티 플레이 탈출구 이용
탈출구는 한명이 이용시 해당 위치의 탈출구가 사라지고 다른 위치에 생성됩니다
탈출구는 1분에 한번씩 사라지며 위치를 이동하기 때문에
만약 탈출구를 시간안에 못찾았다면 개수가 줄어들어 서로 싸워서 남은 사람만 탈출하도록 유도하는 시스템을 만들었습니다.

![포탈 탈출 멀티](https://github.com/jsh991226/ProjectHIM/assets/81565737/7ac466a3-4b6a-448d-897d-b3dc7cc06e69)

### 모든 탈출구가 사라질때 사망 처리
시간이 경과하여 시나리오의 모든 탈출구가 사라진다면 플레이어는 해당 위치에서 사망하여 로비로 돌아갑니다
![시간 경과 사망](https://github.com/jsh991226/ProjectHIM/assets/81565737/569268c8-5e05-4af1-a586-a6a39d778e9f)



