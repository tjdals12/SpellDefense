using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.PartShop {
    public abstract class IRepository : MonoBehaviour
    {
        protected Dictionary<int, Entity[]> partShops;
        public abstract Entity[] FindAllByIndex(int index);
    }
}
