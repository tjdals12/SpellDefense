using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model.OutGame {
    public abstract class INoticeModel : MonoBehaviour
    {
        protected Notice[] _notices;
        public INotice[] notices { get => this._notices; } 

        public Action OnInitialize;
        public Action<int> OnReadNotice;
    }

    public class NoticeModel : INoticeModel {
        public void Initialize(Notice[] notices) {
            this._notices = notices;
            this.OnInitialize?.Invoke();
        }
        public void ReadNotice(int noticeId) {
            Notice notice = Array.Find(this._notices, (e) => e.id == noticeId);
            notice.ToRead();
            this.OnReadNotice?.Invoke(notice.id);
        }
    }
}
