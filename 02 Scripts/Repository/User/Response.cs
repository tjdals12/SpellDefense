using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.User.Response {
    using Model.OutGame;

    public class ReceiveMailResponse {
        public Mail mail { get; private set; }
        public ReceivedReward receivedReward { get; private set; }
        public ReceiveMailResponse(Mail mail, ReceivedReward receivedReward) {
            this.mail = mail;
            this.receivedReward = receivedReward;
        }
    }

    public class ReceiveAllMailResponse {
        public List<Mail> mails { get; private set; }
        public List<ReceivedReward> receivedRewards { get; private set; }
        public ReceiveAllMailResponse(List<Mail> mails, List<ReceivedReward> receivedRewards) {
            this.mails = mails;
            this.receivedRewards = receivedRewards;
        }
    }

    public class BuySkillBookInShopResponse {
        public ShopSkillBook shopSkillBook { get; private set; }
        public ReceivedReward receivedReward { get; private set; }
        public BuySkillBookInShopResponse(ShopSkillBook shopSkillBook, ReceivedReward receivedReward) {
            this.shopSkillBook = shopSkillBook;
            this.receivedReward = receivedReward;
        }
    }

    public class BuyPartInShopResponse {
        public ShopPart shopPart { get; private set; }
        public ReceivedReward receivedReward { get; private set; }
        public BuyPartInShopResponse(ShopPart shopPart, ReceivedReward receivedReward) {
            this.shopPart = shopPart;
            this.receivedReward = receivedReward;
        }
    }

    public class BuyGoldInShopResponse {
        public ShopGold shopGold { get; private set; }
        public ReceivedReward receivedReward { get; private set; }
        public BuyGoldInShopResponse(ShopGold shopGold, ReceivedReward receivedReward) {
            this.shopGold = shopGold;
            this.receivedReward = receivedReward;
        }
    }

    public class BuyEnergyInShopResponse {
        public ShopEnergy shopEnergy { get; private set; }
        public ReceivedReward receivedReward { get; private set; }
        public BuyEnergyInShopResponse(ShopEnergy shopEnergy, ReceivedReward receivedReward) {
            this.shopEnergy = shopEnergy;
            this.receivedReward = receivedReward;
        }
    }

    public class OpenSilverChestResponse {
        public Chest chest { get; private set; }
        public int count { get; private set; }
        public List<ReceivedReward> receivedRewards { get; private set; }
        public OpenSilverChestResponse(Chest chest, int count, List<ReceivedReward> receivedRewards) {
            this.chest = chest;
            this.count = count;
            this.receivedRewards = receivedRewards;
        }
    }

    public class GameStartResponse {
        public Energy energy { get; private set; }
        public bool isEnoughEnergy { get; private set; }
        public GameStartResponse(Energy energy) {
            this.energy = energy;
        }
    }

    public class GameOverResponse {
        public List<ReceivedReward> receivedRewards { get; private set;}
        public GameOverResponse(List<ReceivedReward> receivedRewards) {
            this.receivedRewards = receivedRewards;
        }
    }

    public class WatchAdsResponse {
        public List<ReceivedReward> receivedRewards { get; private set;}
        public WatchAdsResponse(List<ReceivedReward> receivedRewards) {
            this.receivedRewards = receivedRewards;
        }
    }
}
