using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.OutGame {
    using ChestService = Service.OutGame.ChestService;
    using Model.OutGame;

    public class ChestController : MonoBehaviour
    {
        ChestService chestService;

        #region Unity Method
        void Awake() {
            this.chestService = GameObject.FindObjectOfType<ChestService>();
        }
        #endregion

        public Chest GetSilverChest() {
            Chest silverChest = this.chestService.GetSilverChest();
            return silverChest;
        }
    }
}
