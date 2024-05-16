using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.EnergyShop {
    public abstract class IRepository : MonoBehaviour
    {
        public Entity[] energyShops { get; protected set; }
    }
}
