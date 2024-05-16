using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.SkillBook {
    using SO;
    using Model.Common;

    public abstract class IRepository : MonoBehaviour
    {
        protected ItemImageSO itemImageSO;
        protected SkillPrefabSO skillPrefabSO;
        public SkillBook[] skillBooks;
        public abstract SkillBook FindById(int id);
    }
}
