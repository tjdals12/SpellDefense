using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.OutGame {
    using ValidationException = Core.CustomException.ValidationException;
    using NoticeService = Service.OutGame.NoticeService;

    public class NoticeController : MonoBehaviour
    {
        NoticeService noticeService;

        public event Action OnSuccess;
        public event Action<string> OnAlert;
        public event Action OnError;

        #region Unity Method
        void Awake() {
            this.noticeService = GameObject.FindObjectOfType<NoticeService>();
        }
        #endregion

        public async void ReadNotice(int noticeId) {
            try {
                await this.noticeService.ReadNotice(noticeId);
                this.OnSuccess?.Invoke();
            } catch (ValidationException ve) {
                Debug.Log(ve.Message);
                this.OnAlert?.Invoke(ve.Message);
            } catch (Exception e) {
                Debug.Log(e.Message);
                this.OnError?.Invoke();
            }
        }
    }
}
