# ProjectHIM
Unity / C# / Pun / Spring / Mysql 을 연동해 만든 온라인 게임

# Game Info
플레이어는 중세 몬스터 종족을 선택 해 플레이 하며
함께 마을을 약탈 할 "작업반" 인원들을 모아 시나리오에 진입 해
마을의 시민을 살해하고 물건을 약탈 하여
보스NPC를 처치하고 탈출구가 열리면 시간안에 탈출하는
PVPVE 형태의 타르코프 라이크 게임입니다



# Game Main System
메인 타이틀 로비 입니다, 회원가입/로그인을 할 수 있습니다
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/fc7ac30f-59b2-4bb8-acbd-60997db4e828)

최초 계정 생성시 종족을 선택할 수 있는 화면 입니다
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/d64e7832-b9d1-4955-9c83-addfaf400198)

종족 선택 후 접속시 전체 UI 입니다
좌측 상단 체력바/상태, 우측상단 미니맵, 좌측하단 채팅창, 우측하단 현재 장착한 장비 정보 입니다
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/fc4a4864-44cb-46d1-8c76-7ab7b465e2a0)

# Inventory System
메인 시스템인 인벤토리 시스템 입니다
다양한 장비와 아이템이 존재 하며 각 아이템 마다 크기와 모양이 각각 존재 해 인벤토리 공간 활용을 잘 해야하는 시스템 입니다
샘플 시스템은 @@@에서 참조 했으며
실제 응용 및 데이터베이스 연결은 팀에서 직접 처리 하였습니다


<img width="500" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/19d429d6-1394-4c14-9ba9-ecd56d6b7977">
<img width="500"alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/80828b24-7a53-41f1-bc22-3b4c27a70cff">


장착시 실제 멀티 연동까지 구현 되어 있습니다
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/b8045897-b1c7-4ae4-99fc-ab3a550b0bd5)

# Item Craft System
위의 인벤토리 시스템에서 파생하여 만들어진 아이템 제조 시스템 입니다
아이템이 차지하는 크기가 클수록 조합 확률이 높아지며 
"룬"아이템의 경우에는 크기에 따른 확률이 아닌 아이템 자체에 기입된 확률이 따로 있습니다
조합에 성공한 아이템은 인벤토리가 아닌 우편함으로 따로 지급되며 해당 기술은 아래에서 따로 서술 하겠습니다

<img width="600" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/928f6393-edbf-4ce9-82d4-4222f72a33b9">
<img width="300" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/9fe815dd-e16e-41c7-8abc-545dae8bd6c8">
<img width="700" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/fefafba7-570c-48b4-956e-99df966e11ed">

# AI based NPC Random Dialogue
ChatGPT API를 스프링에서 Json을 Return하는 페이지를 따로 구축해 두고
게임 클라이언트에서 해당 NPC의 직업과 이름을 웹에 넘겨 AI가 생성한 대사를
돌려받아 인게임 NPC에게 적용하였습니다

![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/9b14f3f9-d10e-49ec-84fb-843b51b3c433)
<img width="900" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/c404bad9-488b-48f1-879d-4dc44da44d51">


# Web Parsing Post Box
저희 게임은 웹 페이지에서 개인 정보를 확인하고 웹 상점에서 아이템을 구매할 수 있습니다
웹 상점에서 구매한 아이템들은 게임 클라이언트에서 우편함을 통해 확인할 수 있습니다
우편함의 아이템들은 모두 웹에서 파싱한 값으로 나타냅니다

### 웹 상점에서 아이템을 구매할 때
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/df4aca1a-79e3-47b5-bd2f-515bedf2c011)


### 웹에서 구매한 아이템이 우편함에 도착했을 시 나오는 공지사항
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/f4fc4817-d6b5-4277-82aa-c5a12419e323)

### 구매한 아이템이 들어있는 우편함
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/1bcdb42d-aad7-454c-85a9-5a05ae57b3e7)

# Scenario Party System
일반적으로 PhotonNetwork Pun2 에서는 한번 Room으로 입장 하면 다른 Room 목록을 불러올 수 없어
저희 기획인 3D형태의 인게임 로비 에서 활동 하다가 인원을 모아 활성화된 Room으로 넘길 수 없었습니다
그래서 데이터베이스와 코드를 활용 하여 한번 들어온 Room에서 다른 활성화된 방의 정보와 인원을 받고
전체 인원이 Ready 상태가 되면 새로운 Room을 만들어 일부 유저만 다른 게임으로 옮겨가는 시스템을 만들어 냈습니다


<img width="450" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/1af510f5-0b52-49c8-bd5e-0fa298296063">
<img width="450" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/2d585afc-520c-4ee1-94ca-4980044ecaad">
<img width="900" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/9e882003-934e-4b25-b7c7-df0410a4b568">

### 프리팹을 이용하여 간단히 새로운 시나리오를 추가할 수 있습니다
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/a45eea9e-191a-4471-a24b-1498edb25ebe)
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/656efd5a-b0ac-4f0b-ae5f-c37250f59da1)

### 컨트롤러 스크립트에 실제 현재 인원 현황을 확인하는 코드를 통해 연동하는 기능을 확인할 수 있습니다
![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/32100038-a7d6-42a8-bc54-746d048e1c83)

# Scenario System InGame Rule
최소 플레이 인원이 충족되면 방장은 게임 시작 버튼을 통해 시나리오에 진입할 수 있습니다
서로 다른 위치에 스폰 되며 플레이어들은 서로 죽일수도 있고 협동해서 시나리오를 탈출할 수도 있습니다
게임 입장 후 스테이지 기본 클리어시간안에 후에 서술할 시나리오의 보스를 처치해야 합니다
보스를 처치하지 못한다면 탈출구는 생성되지 않으며 모든 작업반 인원은 사망 후 로비로 돌아갑니다

### 스테이지 클리어 시간을 보여주는 UI
<img width="674" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/b9f4d815-8c25-484a-8f1f-69347f90689f">

### 시나리오 보스 처치 시 변경되는 상단 UI
탈출구는 현재 남은 인원 수 만큼 랜덤 위치에 생성됩니다
<img width="469" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/5b154c4c-1cf1-45ce-8532-f1646a821d29">

### 멀티 플레이 탈출구 이용
탈출구는 한명이 이용시 해당 위치의 탈출구가 사라지고 다른 위치에 생성됩니다
탈출구는 1분에 한번씩 사라지며 위치를 이동하기 때문에
만약 탈출구를 시간안에 못찾았다면 개수가 줄어들어 서로 싸워서 남은 사람만 탈출하도록 유도하는 시스템을 만들었습니다.
![포탈 탈출 멀티](https://github.com/jsh991226/ProjectHIM/assets/81565737/75240d22-8d80-4c7e-b754-7ac4012345ea)

### 모든 탈출구가 사라질때 사망 처리
시간이 경과하여 시나리오의 모든 탈출구가 사라진다면 플레이어는 해당 위치에서 사망하여 로비로 돌아갑니다
![시간 경과 사망](https://github.com/jsh991226/ProjectHIM/assets/81565737/40550889-1750-4fdf-9447-74cacdec2c28)


