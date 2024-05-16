using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Chest {
    using Model.OutGame;
    using SO;

    public abstract class IRepository : MonoBehaviour
    {
        protected ItemImageSO itemImageSO;
        public Chest[] chests { get; protected set; }
        public abstract Chest FindById(int id);
    }
}
