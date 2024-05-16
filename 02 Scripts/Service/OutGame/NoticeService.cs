using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Service.OutGame {
    using INoticeRepository = Repository.Notice.IRepository;
    using Model.OutGame;

    public class NoticeService : MonoBehaviour
    {
        INoticeRepository noticeRepository;
        NoticeModel noticeModel;

        #region Unity Method
        void Awake() {
            this.noticeRepository = GameObject.FindObjectOfType<INoticeRepository>();
            this.noticeModel = GameObject.FindObjectOfType<NoticeModel>();
        }
        #endregion

        public async Task ReadNotice(int noticeId) {
            await this.noticeRepository.ReadNotice(noticeId);
            this.noticeModel.ReadNotice(noticeId);
        }
    }
}
