using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Service.OutGame {
    using ChestRepository = Repository.Chest.IRepository;
    using Model.OutGame;

    public class ChestService : MonoBehaviour
    {
        ChestRepository chestRepository;

        #region Unity Method
        void Awake() {
            this.chestRepository = GameObject.FindObjectOfType<ChestRepository>();
        }
        #endregion

        public Chest GetSilverChest() {
            Chest chest = this.chestRepository.FindById(id: 100);
            return chest;
        }
    }
}
