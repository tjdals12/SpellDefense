using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Firebase.Database;
using Newtonsoft.Json;

namespace Repository.Notice {
    using LocalizationRepository = Repository.Localization.IRepository;
    using Model.OutGame;

    public class FirebaseRepository : IRepository
    {
        LocalizationRepository localizationRepository;
        DatabaseReference _noticesRef;
        DatabaseReference noticesRef {
            get {
                if (this._noticesRef == null) {
                    this._noticesRef = FirebaseDatabase.DefaultInstance.RootReference.Child("notices");
                }
                return this._noticesRef;
            }
        }

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
            this.localizationRepository = GameObject.FindObjectOfType<LocalizationRepository>();
            if (localizationRepository.isLoaded == false) {
                yield return null;
            }
        }

        public override async Task<Notice[]> FindAll()
        {
            if (this.notices == null) {
                DataSnapshot dataSnapshot = await this.noticesRef.GetValueAsync();
                string rawJson = dataSnapshot.GetRawJsonValue();
                if (rawJson == null) {
                    this.notices = new Notice[0];
                } else {
                    Dictionary<string, Entity> entityDict = JsonConvert.DeserializeObject<Dictionary<string, Entity>>(rawJson);
                    List<Notice> notices = new();
                    string currentLanguage = this.localizationRepository.GetCurrentLanguage();
                    foreach (var kv in entityDict) {
                        Entity entity = kv.Value;
                        bool isRead = this.readNoticeIds.Contains(entity.id);
                        notices.Add(new Notice(
                            id: entity.id,
                            type: entity.type,
                            title: entity.GetTitle(currentLanguage),
                            content: entity.GetContent(currentLanguage),
                            createdAt: entity.createdAt,
                            isRead
                        ));
                    }
                    this.notices = notices.OrderByDescending((notice) => notice.createdAt).ToArray();
                }
            } else {
                this.notices = new Notice[0];
            }
            return this.notices;
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
