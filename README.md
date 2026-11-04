니케 PC 클라이언트에서 사용하는 "tbs_browser.exe"가 메모리 누수가 심해서 만들었던 프로그램.

게임을 종료 해도 계속 메모리에 남아서 강제 종료 용도로 만들었으나 
2.5주년 쯤 tbs_browser 가 PV 등의 영상을 자동으로 재생하는 문제가 있어서 
방화벽 차단 규칙 기능을 추가 하였는데 2026-11-04 니케 런처 내의 "tbs_browser"를 이용한 
해킹 사건이 발생했으나 방화벽으로 차단 해두면 해당 웹사이트에 접속이 불가능하여 피해를 예방 할 수 있었음.

(이번 이슈는 런처가 실행중일 때 tbs_browser가 자동으로 접속하는 CDN에서 퍼진 이슈로 
해당 웹뷰어가 외부로 나가고 들어오는 데이터를 차단하면 별도의 해킹 피해가 발생하지 않음)


[사용방법]

<img width="317" height="260" alt="1" src="https://github.com/user-attachments/assets/cb7183fd-07ad-431a-a3c1-399c8ae41622" />


클릭후 니케가 설치된 폴더 
(예시: "C:\NIKKE\Launcher\intl_service") 에서 

"tbs_browser.exe" 를 선택 하면

자동으로 방화벽 규칙에 적용됨


<img width="1344" height="900" alt="2" src="https://github.com/user-attachments/assets/9fc12ac9-5cd1-431e-b97e-9ec3989dd631" />

그럼 웹뷰어가 인터넷 사용이 차단되어 

"https://gist.github.com/MotherSphere/d6c049a09ba8ac3085c51ee9df24a4e5#file-nikke-pc-launcher-compromise_ko-md" 

이슈와 상관없이 사용 가능함
