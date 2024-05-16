using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.SkillBookShop {
    public abstract class IRepository : MonoBehaviour
    {
        protected Dictionary<int, Entity[]> skillBookShops;
        public abstract Entity[] FindAllByIndex(int index);
    }
}