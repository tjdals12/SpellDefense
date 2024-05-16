using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.User.Request {
    public class ReceiveMailRequest {
        public string mailId { get; private set; }
        public ReceiveMailRequest(string mailId) {
            this.mailId = mailId;
        }
    }

    public class ReceiveAllMailRequest {
    }

    public class RefreshMailsRequest {
    }

    public class UpgradeSkillBookRequest {
        public int skillBookId { get; private set; }
        public UpgradeSkillBookRequest(int skillBookId) {
            this.skillBookId = skillBookId;
        }
    }

    public class EquipSkillBookRequest {
        public int index { get; private set; }
        public int skillBookId { get; private set; }
        public EquipSkillBookRequest(int index, int skillBookId) {
            this.index = index;
            this.skillBookId = skillBookId;
        }
    }

    public class ChangeDeckRequest {
        public int index { get; private set; }
        public ChangeDeckRequest(int index) {
            this.index = index;
        }
    }

    public class UpgradePartRequest {
        public string id { get; private set; }
        public string[] materialIds { get; private set; }
        public UpgradePartRequest(string id, string[] materialIds) {
            this.id = id;
            this.materialIds = materialIds;
        }
    }

    public class EquipPartRequest {
        public string id { get; private set; }
        public EquipPartRequest(string id) {
            this.id = id;
        }
    }

    public class BuySkillBookInShopRequest {
        public int index { get; private set; }
        public BuySkillBookInShopRequest(int index) {
            this.index = index;
        }
    }

    public class ResetSkillBookShopRequest {}

    public class ResetSkillBookShopByAdsRequest {}

    public class BuyPartInShopRequest {
        public int index { get; private set; }
        public BuyPartInShopRequest(int index) {
            this.index = index;
        }
    }

    public class ResetPartShopRequest {}

    public class ResetPartShopByAdsRequest {}

    public class BuyGoldInShopRequest {
        public int index { get; private set; }
        public BuyGoldInShopRequest(int index) {
            this.index = index;
        }
    }

    public class ResetGoldShopRequest {}
    
    public class ResetGoldShopByAdsRequest {}

    public class BuyEnergyInShopRequest {
        public int index { get; private set; }
        public BuyEnergyInShopRequest(int index) {
            this.index = index;
        }
    }
    
    public class ResetEnergyShopRequest {}

    public class ResetEnergyShopByAdsRequest {}

    public class OpenSilverChestRequest {
        public int count { get; private set; }
        public OpenSilverChestRequest(int count) {
            this.count = count;
        }
    }

    public class GameStartRequest {}
    public class GameOverRequest {
        public int wave { get; private set; }
        public GameOverRequest(int wave) {
            this.wave = wave;
        }
    }
    public class WatchAdsRequest {
        public int wave { get; private set; }
        public WatchAdsRequest(int wave) {
            this.wave = wave;
        }
    } 
}