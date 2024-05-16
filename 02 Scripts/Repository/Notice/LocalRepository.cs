using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Repository.Notice {
    using LocalizationRepository = Repository.Localization.IRepository;
    using Model.OutGame;

    public class LocalRepository : IRepository
    {
        #region Unity Method
        void Awake() {
            var obj = GameObject.FindObjectsOfType<IRepository>();
            if (obj.Length == 1) {
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
            if (PlayerPrefs.HasKey("readNoticeIds")) {
                string rawJson = PlayerPrefs.GetString("readNoticeIds");
                this.readNoticeIds = JsonConvert.DeserializeObject<List<int>>(rawJson);
            } else {
                this.readNoticeIds = new();
            }
            StartCoroutine(this.LoadAfterResolveDependencies());
        }
        #endregion

        IEnumerator LoadAfterResolveDependencies() {
            LocalizationRepository localizationRepository = GameObject.FindObjectOfType<LocalizationRepository>();
            while (localizationRepository.isLoaded == false) {
                yield return null;
            }
            string currentLanguage = localizationRepository.GetCurrentLanguage();
            Entity[] entities = new Entity[10] {
                new Entity(
                    id: 0,
                    type: NoticeType.Announcement,
                    titles: new Dictionary<string, string>() {
                        { "ko", "테스트 공지사항 1" },
                        { "en", "Test Notice 1" },
                    },
                    contents: new Dictionary<string, string>() {
                        { "ko", "슥저휘도 든자선함후야말로 길토나의 레뎌가 디두앙 교으란가롸시가, 뉘닌하왈이니. 니막앙아는 사알딕게 덜겨다붐이라 어옴밭그를 범뭔 호채올 베털엉바왏머다 킬알싼카아 은기줌맜에 은넴서여의. 존됴저 아금멘하다 서오로 냐연음 딜율사괴다. 염다으강습니다 비지지만, 랄저보미를 시항졷기다, 혀실층이 센자거, 째졀로를 질론잔에서 드켠끌재냔염 뜰강. 짔더짠츰습니다 수잘을 자란가시와 긍바딤롼위억알론에서 다십아지에 단육으니.\n셔다 히나다탄롱무에 라하까눈 웡잔라을라 딜우임닥을, 박사거마재롸에토는. 멍가대로 순수사타 이엔이 더리으가 왈아나샌 안딘반차. 자껄귬한늘은 랠게골과 가로칌톤 우티홤너민래기공 됴분란던하가 상기웅앗하알지 핬눕올이고 도도즜울닽. 꾼믕제긱도 은씨레민 팀동벼오목부터 처아그똬로 불사홈투소술도 서민은 이라기, 으타카아 엲드나아. 느네느지면 치새기밴 아앰으라 어리싈자그소에 체감으젹댤울좐그를. 조꺼지 죠힉은 에너 너헤잋부터 딘그앵궁 알만구게다. 아곤겨윽비를 현르어아도 스상아, 어우시붢고 오다더는 기겕아가궝오 니하 걸놩에, 노타계를. 뇰소귀교의 옹이는, 장랑앤녀에다 이룽듷으면서 변인의 즈이안게큰다." },
                        { "en", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus mauris ante, ornare sed mi in, ultrices lobortis felis. Nullam ac purus sit amet metus mollis aliquam. Phasellus nibh enim, scelerisque ac fringilla ac, scelerisque ut nunc. Vestibulum rutrum dignissim erat, a mollis quam. Ut odio dolor, facilisis id justo at, molestie consequat dolor. Proin semper gravida metus, vel ullamcorper tortor placerat nec. Praesent vitae nibh eget nisl placerat ornare. In ullamcorper venenatis diam eu porta." },
                    },
                    createdAt: DateTime.UtcNow
                ),
                new Entity(
                    id: 1,
                    type: NoticeType.Update,
                    titles: new Dictionary<string, string>() {
                        { "ko", "테스트 공지사항 1" },
                        { "en", "Test Notice 1" },
                    },
                    contents: new Dictionary<string, string>() {
                        { "ko", "슥저휘도 든자선함후야말로 길토나의 레뎌가 디두앙 교으란가롸시가, 뉘닌하왈이니. 니막앙아는 사알딕게 덜겨다붐이라 어옴밭그를 범뭔 호채올 베털엉바왏머다 킬알싼카아 은기줌맜에 은넴서여의. 존됴저 아금멘하다 서오로 냐연음 딜율사괴다. 염다으강습니다 비지지만, 랄저보미를 시항졷기다, 혀실층이 센자거, 째졀로를 질론잔에서 드켠끌재냔염 뜰강. 짔더짠츰습니다 수잘을 자란가시와 긍바딤롼위억알론에서 다십아지에 단육으니.\n셔다 히나다탄롱무에 라하까눈 웡잔라을라 딜우임닥을, 박사거마재롸에토는. 멍가대로 순수사타 이엔이 더리으가 왈아나샌 안딘반차. 자껄귬한늘은 랠게골과 가로칌톤 우티홤너민래기공 됴분란던하가 상기웅앗하알지 핬눕올이고 도도즜울닽. 꾼믕제긱도 은씨레민 팀동벼오목부터 처아그똬로 불사홈투소술도 서민은 이라기, 으타카아 엲드나아. 느네느지면 치새기밴 아앰으라 어리싈자그소에 체감으젹댤울좐그를. 조꺼지 죠힉은 에너 너헤잋부터 딘그앵궁 알만구게다. 아곤겨윽비를 현르어아도 스상아, 어우시붢고 오다더는 기겕아가궝오 니하 걸놩에, 노타계를. 뇰소귀교의 옹이는, 장랑앤녀에다 이룽듷으면서 변인의 즈이안게큰다." },
                        { "en", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus mauris ante, ornare sed mi in, ultrices lobortis felis. Nullam ac purus sit amet metus mollis aliquam. Phasellus nibh enim, scelerisque ac fringilla ac, scelerisque ut nunc. Vestibulum rutrum dignissim erat, a mollis quam. Ut odio dolor, facilisis id justo at, molestie consequat dolor. Proin semper gravida metus, vel ullamcorper tortor placerat nec. Praesent vitae nibh eget nisl placerat ornare. In ullamcorper venenatis diam eu porta." },
                    },
                    createdAt: DateTime.UtcNow.AddDays(-2)
                ),
                new Entity(
                    id: 2,
                    type: NoticeType.Update,
                    titles: new Dictionary<string, string>() {
                        { "ko", "테스트 공지사항 1" },
                        { "en", "Test Notice 1" },
                    },
                    contents: new Dictionary<string, string>() {
                        { "ko", "슥저휘도 든자선함후야말로 길토나의 레뎌가 디두앙 교으란가롸시가, 뉘닌하왈이니. 니막앙아는 사알딕게 덜겨다붐이라 어옴밭그를 범뭔 호채올 베털엉바왏머다 킬알싼카아 은기줌맜에 은넴서여의. 존됴저 아금멘하다 서오로 냐연음 딜율사괴다. 염다으강습니다 비지지만, 랄저보미를 시항졷기다, 혀실층이 센자거, 째졀로를 질론잔에서 드켠끌재냔염 뜰강. 짔더짠츰습니다 수잘을 자란가시와 긍바딤롼위억알론에서 다십아지에 단육으니.\n셔다 히나다탄롱무에 라하까눈 웡잔라을라 딜우임닥을, 박사거마재롸에토는. 멍가대로 순수사타 이엔이 더리으가 왈아나샌 안딘반차. 자껄귬한늘은 랠게골과 가로칌톤 우티홤너민래기공 됴분란던하가 상기웅앗하알지 핬눕올이고 도도즜울닽. 꾼믕제긱도 은씨레민 팀동벼오목부터 처아그똬로 불사홈투소술도 서민은 이라기, 으타카아 엲드나아. 느네느지면 치새기밴 아앰으라 어리싈자그소에 체감으젹댤울좐그를. 조꺼지 죠힉은 에너 너헤잋부터 딘그앵궁 알만구게다. 아곤겨윽비를 현르어아도 스상아, 어우시붢고 오다더는 기겕아가궝오 니하 걸놩에, 노타계를. 뇰소귀교의 옹이는, 장랑앤녀에다 이룽듷으면서 변인의 즈이안게큰다." },
                        { "en", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus mauris ante, ornare sed mi in, ultrices lobortis felis. Nullam ac purus sit amet metus mollis aliquam. Phasellus nibh enim, scelerisque ac fringilla ac, scelerisque ut nunc. Vestibulum rutrum dignissim erat, a mollis quam. Ut odio dolor, facilisis id justo at, molestie consequat dolor. Proin semper gravida metus, vel ullamcorper tortor placerat nec. Praesent vitae nibh eget nisl placerat ornare. In ullamcorper venenatis diam eu porta." },
                    },
                    createdAt: DateTime.UtcNow.AddDays(-10)
                ),
                new Entity(
                    id: 3,
                    type: NoticeType.Announcement,
                    titles: new Dictionary<string, string>() {
                        { "ko", "테스트 공지사항 1" },
                        { "en", "Test Notice 1" },
                    },
                    contents: new Dictionary<string, string>() {
                        { "ko", "슥저휘도 든자선함후야말로 길토나의 레뎌가 디두앙 교으란가롸시가, 뉘닌하왈이니. 니막앙아는 사알딕게 덜겨다붐이라 어옴밭그를 범뭔 호채올 베털엉바왏머다 킬알싼카아 은기줌맜에 은넴서여의. 존됴저 아금멘하다 서오로 냐연음 딜율사괴다. 염다으강습니다 비지지만, 랄저보미를 시항졷기다, 혀실층이 센자거, 째졀로를 질론잔에서 드켠끌재냔염 뜰강. 짔더짠츰습니다 수잘을 자란가시와 긍바딤롼위억알론에서 다십아지에 단육으니.\n셔다 히나다탄롱무에 라하까눈 웡잔라을라 딜우임닥을, 박사거마재롸에토는. 멍가대로 순수사타 이엔이 더리으가 왈아나샌 안딘반차. 자껄귬한늘은 랠게골과 가로칌톤 우티홤너민래기공 됴분란던하가 상기웅앗하알지 핬눕올이고 도도즜울닽. 꾼믕제긱도 은씨레민 팀동벼오목부터 처아그똬로 불사홈투소술도 서민은 이라기, 으타카아 엲드나아. 느네느지면 치새기밴 아앰으라 어리싈자그소에 체감으젹댤울좐그를. 조꺼지 죠힉은 에너 너헤잋부터 딘그앵궁 알만구게다. 아곤겨윽비를 현르어아도 스상아, 어우시붢고 오다더는 기겕아가궝오 니하 걸놩에, 노타계를. 뇰소귀교의 옹이는, 장랑앤녀에다 이룽듷으면서 변인의 즈이안게큰다." },
                        { "en", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus mauris ante, ornare sed mi in, ultrices lobortis felis. Nullam ac purus sit amet metus mollis aliquam. Phasellus nibh enim, scelerisque ac fringilla ac, scelerisque ut nunc. Vestibulum rutrum dignissim erat, a mollis quam. Ut odio dolor, facilisis id justo at, molestie consequat dolor. Proin semper gravida metus, vel ullamcorper tortor placerat nec. Praesent vitae nibh eget nisl placerat ornare. In ullamcorper venenatis diam eu porta." },
                    },
                    createdAt: DateTime.UtcNow.AddDays(-11)
                ),
                new Entity(
                    id: 4,
                    type: NoticeType.Announcement,
                    titles: new Dictionary<string, string>() {
                        { "ko", "테스트 공지사항 1" },
                        { "en", "Test Notice 1" },
                    },
                    contents: new Dictionary<string, string>() {
                        { "ko", "슥저휘도 든자선함후야말로 길토나의 레뎌가 디두앙 교으란가롸시가, 뉘닌하왈이니. 니막앙아는 사알딕게 덜겨다붐이라 어옴밭그를 범뭔 호채올 베털엉바왏머다 킬알싼카아 은기줌맜에 은넴서여의. 존됴저 아금멘하다 서오로 냐연음 딜율사괴다. 염다으강습니다 비지지만, 랄저보미를 시항졷기다, 혀실층이 센자거, 째졀로를 질론잔에서 드켠끌재냔염 뜰강. 짔더짠츰습니다 수잘을 자란가시와 긍바딤롼위억알론에서 다십아지에 단육으니.\n셔다 히나다탄롱무에 라하까눈 웡잔라을라 딜우임닥을, 박사거마재롸에토는. 멍가대로 순수사타 이엔이 더리으가 왈아나샌 안딘반차. 자껄귬한늘은 랠게골과 가로칌톤 우티홤너민래기공 됴분란던하가 상기웅앗하알지 핬눕올이고 도도즜울닽. 꾼믕제긱도 은씨레민 팀동벼오목부터 처아그똬로 불사홈투소술도 서민은 이라기, 으타카아 엲드나아. 느네느지면 치새기밴 아앰으라 어리싈자그소에 체감으젹댤울좐그를. 조꺼지 죠힉은 에너 너헤잋부터 딘그앵궁 알만구게다. 아곤겨윽비를 현르어아도 스상아, 어우시붢고 오다더는 기겕아가궝오 니하 걸놩에, 노타계를. 뇰소귀교의 옹이는, 장랑앤녀에다 이룽듷으면서 변인의 즈이안게큰다." },
                        { "en", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus mauris ante, ornare sed mi in, ultrices lobortis felis. Nullam ac purus sit amet metus mollis aliquam. Phasellus nibh enim, scelerisque ac fringilla ac, scelerisque ut nunc. Vestibulum rutrum dignissim erat, a mollis quam. Ut odio dolor, facilisis id justo at, molestie consequat dolor. Proin semper gravida metus, vel ullamcorper tortor placerat nec. Praesent vitae nibh eget nisl placerat ornare. In ullamcorper venenatis diam eu porta." },
                    },
                    createdAt: DateTime.UtcNow.AddDays(-15)
                ),
                new Entity(
                    id: 5,
                    type: NoticeType.Announcement,
                    titles: new Dictionary<string, string>() {
                        { "ko", "테스트 공지사항 1" },
                        { "en", "Test Notice 1" },
                    },
                    contents: new Dictionary<string, string>() {
                        { "ko", "슥저휘도 든자선함후야말로 길토나의 레뎌가 디두앙 교으란가롸시가, 뉘닌하왈이니. 니막앙아는 사알딕게 덜겨다붐이라 어옴밭그를 범뭔 호채올 베털엉바왏머다 킬알싼카아 은기줌맜에 은넴서여의. 존됴저 아금멘하다 서오로 냐연음 딜율사괴다. 염다으강습니다 비지지만, 랄저보미를 시항졷기다, 혀실층이 센자거, 째졀로를 질론잔에서 드켠끌재냔염 뜰강. 짔더짠츰습니다 수잘을 자란가시와 긍바딤롼위억알론에서 다십아지에 단육으니.\n셔다 히나다탄롱무에 라하까눈 웡잔라을라 딜우임닥을, 박사거마재롸에토는. 멍가대로 순수사타 이엔이 더리으가 왈아나샌 안딘반차. 자껄귬한늘은 랠게골과 가로칌톤 우티홤너민래기공 됴분란던하가 상기웅앗하알지 핬눕올이고 도도즜울닽. 꾼믕제긱도 은씨레민 팀동벼오목부터 처아그똬로 불사홈투소술도 서민은 이라기, 으타카아 엲드나아. 느네느지면 치새기밴 아앰으라 어리싈자그소에 체감으젹댤울좐그를. 조꺼지 죠힉은 에너 너헤잋부터 딘그앵궁 알만구게다. 아곤겨윽비를 현르어아도 스상아, 어우시붢고 오다더는 기겕아가궝오 니하 걸놩에, 노타계를. 뇰소귀교의 옹이는, 장랑앤녀에다 이룽듷으면서 변인의 즈이안게큰다." },
                        { "en", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus mauris ante, ornare sed mi in, ultrices lobortis felis. Nullam ac purus sit amet metus mollis aliquam. Phasellus nibh enim, scelerisque ac fringilla ac, scelerisque ut nunc. Vestibulum rutrum dignissim erat, a mollis quam. Ut odio dolor, facilisis id justo at, molestie consequat dolor. Proin semper gravida metus, vel ullamcorper tortor placerat nec. Praesent vitae nibh eget nisl placerat ornare. In ullamcorper venenatis diam eu porta." },
                    },
                    createdAt: DateTime.UtcNow.AddDays(-18)
                ),
                new Entity(
                    id: 6,
                    type: NoticeType.Update,
                    titles: new Dictionary<string, string>() {
                        { "ko", "테스트 공지사항 1" },
                        { "en", "Test Notice 1" },
                    },
                    contents: new Dictionary<string, string>() {
                        { "ko", "슥저휘도 든자선함후야말로 길토나의 레뎌가 디두앙 교으란가롸시가, 뉘닌하왈이니. 니막앙아는 사알딕게 덜겨다붐이라 어옴밭그를 범뭔 호채올 베털엉바왏머다 킬알싼카아 은기줌맜에 은넴서여의. 존됴저 아금멘하다 서오로 냐연음 딜율사괴다. 염다으강습니다 비지지만, 랄저보미를 시항졷기다, 혀실층이 센자거, 째졀로를 질론잔에서 드켠끌재냔염 뜰강. 짔더짠츰습니다 수잘을 자란가시와 긍바딤롼위억알론에서 다십아지에 단육으니.\n셔다 히나다탄롱무에 라하까눈 웡잔라을라 딜우임닥을, 박사거마재롸에토는. 멍가대로 순수사타 이엔이 더리으가 왈아나샌 안딘반차. 자껄귬한늘은 랠게골과 가로칌톤 우티홤너민래기공 됴분란던하가 상기웅앗하알지 핬눕올이고 도도즜울닽. 꾼믕제긱도 은씨레민 팀동벼오목부터 처아그똬로 불사홈투소술도 서민은 이라기, 으타카아 엲드나아. 느네느지면 치새기밴 아앰으라 어리싈자그소에 체감으젹댤울좐그를. 조꺼지 죠힉은 에너 너헤잋부터 딘그앵궁 알만구게다. 아곤겨윽비를 현르어아도 스상아, 어우시붢고 오다더는 기겕아가궝오 니하 걸놩에, 노타계를. 뇰소귀교의 옹이는, 장랑앤녀에다 이룽듷으면서 변인의 즈이안게큰다." },
                        { "en", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus mauris ante, ornare sed mi in, ultrices lobortis felis. Nullam ac purus sit amet metus mollis aliquam. Phasellus nibh enim, scelerisque ac fringilla ac, scelerisque ut nunc. Vestibulum rutrum dignissim erat, a mollis quam. Ut odio dolor, facilisis id justo at, molestie consequat dolor. Proin semper gravida metus, vel ullamcorper tortor placerat nec. Praesent vitae nibh eget nisl placerat ornare. In ullamcorper venenatis diam eu porta." },
                    },
                    createdAt: DateTime.UtcNow.AddDays(-20)
                ),
                new Entity(
                    id: 7,
                    type: NoticeType.Announcement,
                    titles: new Dictionary<string, string>() {
                        { "ko", "테스트 공지사항 1" },
                        { "en", "Test Notice 1" },
                    },
                    contents: new Dictionary<string, string>() {
                        { "ko", "슥저휘도 든자선함후야말로 길토나의 레뎌가 디두앙 교으란가롸시가, 뉘닌하왈이니. 니막앙아는 사알딕게 덜겨다붐이라 어옴밭그를 범뭔 호채올 베털엉바왏머다 킬알싼카아 은기줌맜에 은넴서여의. 존됴저 아금멘하다 서오로 냐연음 딜율사괴다. 염다으강습니다 비지지만, 랄저보미를 시항졷기다, 혀실층이 센자거, 째졀로를 질론잔에서 드켠끌재냔염 뜰강. 짔더짠츰습니다 수잘을 자란가시와 긍바딤롼위억알론에서 다십아지에 단육으니.\n셔다 히나다탄롱무에 라하까눈 웡잔라을라 딜우임닥을, 박사거마재롸에토는. 멍가대로 순수사타 이엔이 더리으가 왈아나샌 안딘반차. 자껄귬한늘은 랠게골과 가로칌톤 우티홤너민래기공 됴분란던하가 상기웅앗하알지 핬눕올이고 도도즜울닽. 꾼믕제긱도 은씨레민 팀동벼오목부터 처아그똬로 불사홈투소술도 서민은 이라기, 으타카아 엲드나아. 느네느지면 치새기밴 아앰으라 어리싈자그소에 체감으젹댤울좐그를. 조꺼지 죠힉은 에너 너헤잋부터 딘그앵궁 알만구게다. 아곤겨윽비를 현르어아도 스상아, 어우시붢고 오다더는 기겕아가궝오 니하 걸놩에, 노타계를. 뇰소귀교의 옹이는, 장랑앤녀에다 이룽듷으면서 변인의 즈이안게큰다." },
                        { "en", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus mauris ante, ornare sed mi in, ultrices lobortis felis. Nullam ac purus sit amet metus mollis aliquam. Phasellus nibh enim, scelerisque ac fringilla ac, scelerisque ut nunc. Vestibulum rutrum dignissim erat, a mollis quam. Ut odio dolor, facilisis id justo at, molestie consequat dolor. Proin semper gravida metus, vel ullamcorper tortor placerat nec. Praesent vitae nibh eget nisl placerat ornare. In ullamcorper venenatis diam eu porta." },
                    },
                    createdAt: DateTime.UtcNow.AddDays(-21)
                ),
                new Entity(
                    id: 8,
                    type: NoticeType.Update,
                    titles: new Dictionary<string, string>() {
                        { "ko", "테스트 공지사항 1" },
                        { "en", "Test Notice 1" },
                    },
                    contents: new Dictionary<string, string>() {
                        { "ko", "슥저휘도 든자선함후야말로 길토나의 레뎌가 디두앙 교으란가롸시가, 뉘닌하왈이니. 니막앙아는 사알딕게 덜겨다붐이라 어옴밭그를 범뭔 호채올 베털엉바왏머다 킬알싼카아 은기줌맜에 은넴서여의. 존됴저 아금멘하다 서오로 냐연음 딜율사괴다. 염다으강습니다 비지지만, 랄저보미를 시항졷기다, 혀실층이 센자거, 째졀로를 질론잔에서 드켠끌재냔염 뜰강. 짔더짠츰습니다 수잘을 자란가시와 긍바딤롼위억알론에서 다십아지에 단육으니.\n셔다 히나다탄롱무에 라하까눈 웡잔라을라 딜우임닥을, 박사거마재롸에토는. 멍가대로 순수사타 이엔이 더리으가 왈아나샌 안딘반차. 자껄귬한늘은 랠게골과 가로칌톤 우티홤너민래기공 됴분란던하가 상기웅앗하알지 핬눕올이고 도도즜울닽. 꾼믕제긱도 은씨레민 팀동벼오목부터 처아그똬로 불사홈투소술도 서민은 이라기, 으타카아 엲드나아. 느네느지면 치새기밴 아앰으라 어리싈자그소에 체감으젹댤울좐그를. 조꺼지 죠힉은 에너 너헤잋부터 딘그앵궁 알만구게다. 아곤겨윽비를 현르어아도 스상아, 어우시붢고 오다더는 기겕아가궝오 니하 걸놩에, 노타계를. 뇰소귀교의 옹이는, 장랑앤녀에다 이룽듷으면서 변인의 즈이안게큰다." },
                        { "en", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus mauris ante, ornare sed mi in, ultrices lobortis felis. Nullam ac purus sit amet metus mollis aliquam. Phasellus nibh enim, scelerisque ac fringilla ac, scelerisque ut nunc. Vestibulum rutrum dignissim erat, a mollis quam. Ut odio dolor, facilisis id justo at, molestie consequat dolor. Proin semper gravida metus, vel ullamcorper tortor placerat nec. Praesent vitae nibh eget nisl placerat ornare. In ullamcorper venenatis diam eu porta." },
                    },
                    createdAt: DateTime.UtcNow.AddDays(-25)
                ),
                new Entity(
                    id: 10,
                    type: NoticeType.Announcement,
                    titles: new Dictionary<string, string>() {
                        { "ko", "테스트 공지사항 1" },
                        { "en", "Test Notice 1" },
                    },
                    contents: new Dictionary<string, string>() {
                        { "ko", "슥저휘도 든자선함후야말로 길토나의 레뎌가 디두앙 교으란가롸시가, 뉘닌하왈이니. 니막앙아는 사알딕게 덜겨다붐이라 어옴밭그를 범뭔 호채올 베털엉바왏머다 킬알싼카아 은기줌맜에 은넴서여의. 존됴저 아금멘하다 서오로 냐연음 딜율사괴다. 염다으강습니다 비지지만, 랄저보미를 시항졷기다, 혀실층이 센자거, 째졀로를 질론잔에서 드켠끌재냔염 뜰강. 짔더짠츰습니다 수잘을 자란가시와 긍바딤롼위억알론에서 다십아지에 단육으니.\n셔다 히나다탄롱무에 라하까눈 웡잔라을라 딜우임닥을, 박사거마재롸에토는. 멍가대로 순수사타 이엔이 더리으가 왈아나샌 안딘반차. 자껄귬한늘은 랠게골과 가로칌톤 우티홤너민래기공 됴분란던하가 상기웅앗하알지 핬눕올이고 도도즜울닽. 꾼믕제긱도 은씨레민 팀동벼오목부터 처아그똬로 불사홈투소술도 서민은 이라기, 으타카아 엲드나아. 느네느지면 치새기밴 아앰으라 어리싈자그소에 체감으젹댤울좐그를. 조꺼지 죠힉은 에너 너헤잋부터 딘그앵궁 알만구게다. 아곤겨윽비를 현르어아도 스상아, 어우시붢고 오다더는 기겕아가궝오 니하 걸놩에, 노타계를. 뇰소귀교의 옹이는, 장랑앤녀에다 이룽듷으면서 변인의 즈이안게큰다." },
                        { "en", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus mauris ante, ornare sed mi in, ultrices lobortis felis. Nullam ac purus sit amet metus mollis aliquam. Phasellus nibh enim, scelerisque ac fringilla ac, scelerisque ut nunc. Vestibulum rutrum dignissim erat, a mollis quam. Ut odio dolor, facilisis id justo at, molestie consequat dolor. Proin semper gravida metus, vel ullamcorper tortor placerat nec. Praesent vitae nibh eget nisl placerat ornare. In ullamcorper venenatis diam eu porta." },
                    },
                    createdAt: DateTime.UtcNow.AddDays(-27)
                ),
            };
            this.notices = new Notice[entities.Length];
            for (int i = 0; i < this.notices.Length; i++) {
                Entity entity = entities[i];
                bool isRead = this.readNoticeIds.Contains(entity.id);
                this.notices[i] = new Notice(
                    id: entity.id,
                    type: entity.type,
                    title: entity.GetTitle(currentLanguage),
                    content: entity.GetContent(currentLanguage),
                    createdAt: entity.createdAt,
                    isRead: isRead
                );
            }
        }

        public override Task<Notice[]> FindAll()
        {
            return Task.FromResult(this.notices);
        }

        public override Task ReadNotice(int id)
        {
            if (this.readNoticeIds.Contains(id) == false) {
                this.readNoticeIds.Add(id);
                string rawJson = JsonConvert.SerializeObject(this.readNoticeIds);
                PlayerPrefs.SetString("readNoticeIds", rawJson);
            }
            return Task.CompletedTask;
        }
    }
}
