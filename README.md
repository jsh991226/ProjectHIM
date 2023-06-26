# ProjectHIM
Unity / C# / Pun / Spring / Mysql 을 연동해 만든 온라인 게임

# Game Info
플레이어는 중세 몬스터 종족을 선택 해 플레이 하며
함께 마을을 약탈 할 "작업반" 인원들을 모아 시나리오에 진입 해
마을의 시민을 살해하고 물건을 약탈 하여
보스NPC를 처치하고 탈출구가 열리면 시간안에 탈출하는
PVPVE 형태의 타르코프 라이크 게임입니다



# Game System
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
조합에 성공한 아이템은 인벤토리가 아닌 우편함으로 따로 지급되며
해당 기술은 아래에서 따로 서술 하겠습니다
<img width="700" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/928f6393-edbf-4ce9-82d4-4222f72a33b9">
<img width="235" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/9fe815dd-e16e-41c7-8abc-545dae8bd6c8">
<img width="900" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/fefafba7-570c-48b4-956e-99df966e11ed">

# AI based NPC Random Dialogue
ChatGPT API를 스프링에서 Json을 Return하는 페이지를 따로 구축해 두고
게임 클라이언트에서 해당 NPC의 직업과 이름을 웹에 넘겨 AI가 생성한 대사를
돌려받아 인게임 NPC에게 적용하였습니다

![image](https://github.com/jsh991226/ProjectHIM/assets/81565737/9b14f3f9-d10e-49ec-84fb-843b51b3c433)
<img width="900" alt="image" src="https://github.com/jsh991226/ProjectHIM/assets/81565737/c404bad9-488b-48f1-879d-4dc44da44d51">
