using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.GoldShop {
    public abstract class IRepository : MonoBehaviour
    {
        public Entity[] goldShops { get; protected set; }
    }
}
