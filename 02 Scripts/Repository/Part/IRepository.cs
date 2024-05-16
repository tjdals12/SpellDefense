using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.Part {
    using SO;
    using Model.Common;

    public abstract class IRepository : MonoBehaviour
    {
        protected ItemImageSO itemImageSO;
        public Part[] parts;
        public abstract Part FindById(int id);
    }
}