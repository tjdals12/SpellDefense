using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Notice {
    using Model.OutGame;

    public abstract class IRepository : MonoBehaviour
    {
        public Notice[] notices { get; protected set; }
        public List<int> readNoticeIds { get; protected set; }
        public abstract Task<Notice[]> FindAll();
        public abstract Task ReadNotice(int id);
    }
}
