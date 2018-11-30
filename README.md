## Presto LRC 플러그인
* **Package 폴더를 반드시 포함해서 다운받아주세요.**
* **숨기기 버튼을 누른 후 새로운 곡 재생 시 해당 플레이어가 실생됩니다. (버그 아닙니다.)**
* **offset 기능이 동작하지 않는경우 streamChanged가 다시 발생하면 동작합니다.**
  
### 기능
* **모든 플레이어는 항상 위에 표시됩니다.**
* 가사 출력
  * 가사 점프 기능 (가사를 클릭하면 해당 구간으로 이동)
  * 가사 싱크 조절 (상단의 +, - 버튼)
    * ![image](https://user-images.githubusercontent.com/42190339/49266564-03c23400-f49a-11e8-891a-4e2714d68847.png)
  * 다음과 같은 기능을 처리합니다.
    * [hhh:mm.ss] 등 [hh:mm.ss] 형태가 아닌 가사 처리를 지원합니다.
    * ![image](https://user-images.githubusercontent.com/42190339/49266427-88608280-f499-11e8-8b76-1cb8c69958c8.png)  
        ![image](https://user-images.githubusercontent.com/42190339/49266318-feb0b500-f498-11e8-9115-21b1062935af.png)  
        위와 같이 싱크정보가 담겨있지 않는 정보를 처리합니다. (이전 가사와 같이 출력합니다.)
    * lrc 파일에 담겨있는 정보 출력 (가사 만든이, 작사가, 곡명)
    * UTF8, ansi, 유니코드 인코딩 지원
    * 동일한 시간대가 여러개 있는 가사를 처리합니다.  
      ![image](https://user-images.githubusercontent.com/42190339/49267056-e5f5ce80-f49b-11e8-9883-ac56e7194c73.png)
* 알송 가사 기능 지원
  * mp3ID 태그 기반
  * 유튜브 자동 가사 기능 지원
    * 유튜브 음악 추출 시 입력했던 태그 기반으로 검색합니다. 보다 정확한 결과를 위해서 태그 정보를 정리해주세요.
      **간단한 태그 정보가 검색이 잘 됩니다. ex) 제목이 윤종신 1집 좋니 (MV) 인 것보다는 제목 : 좋니, 가수 : 윤종신 으로 입력이 잘됩니다.**
  * 가사 선택 지원 - 여러 가사가 존재할 경우 선택 기능 (상단의 ↑, ↓ 버튼)
    * ![image](https://user-images.githubusercontent.com/42190339/49266594-26ece380-f49a-11e8-8320-1e65f65efcae.png)
* 미니플레이어 기능
  * 처음 모듈 실행시 미니 플레이어가 재생
  * 미니 플레이어를 더블 클릭 / 가사창 상단의 □ 버튼 클릭으로 창 전환 기능 지원
* 앨범아트 지원
  * 앨범아트가 존재하지 않을경우 maniaDB API 를 통해 앨범아트가 존재할 경우 출력
    * ![image](https://user-images.githubusercontent.com/42190339/49267011-be9f0180-f49b-11e8-9c5d-e6125411d69a.png)
* 플레이어 기본 기능
  * 0.5 간격으로 볼륨 조절 (0~1)
  * 이전곡, 다음곡, 재생, 일시정지, 셔플, 반복 설정 가능
  * 가사창 플레이어에서 현재 재생중인 구간 하단에 표시


### 사용방법 / 디자인
#### 미니 플레이어
![image](https://user-images.githubusercontent.com/42190339/49266273-bc877380-f498-11e8-9a40-404ce7861840.png)  
버튼모양 그대로입니다.  
더블 클릭시 가사 창이 출력됩니다.

#### 큰 플레이어(가사 창)
![image](https://user-images.githubusercontent.com/42190339/49268534-489e9880-f4a3-11e8-82a9-4aeaf43f67aa.png)  
    

### Downlaod
**해당 Presto의 플러그인 폴더에 압축을 풀어주세요.**
[다운로드](https://github.com/kgpyo/PrestoLRCPlugin/raw/master/Presto.Plugin.Lyrics.zip)

### 참여자
* [고경표](https://github.com/kgpyo)
* [김상우](https://github.com/ksw7564)
* [양승빈](https://github.com/xzcv1994)
* [홍나현](https://github.com/abab0528)
* [조영재](https://github.com/jyj94)
