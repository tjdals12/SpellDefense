using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Item {
    using SO;
    using Model.Common;

    public abstract class IRepository : MonoBehaviour
    {
        public bool isLoaded { get; protected set; }
        protected ItemImageSO itemImageSO;
        public Item[] items { get; protected set; }
        public abstract Item FindById(int id);
        public abstract Item[] FindAllByType(ItemType type);
    }
}
