using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Repository.User {
    using Request;
    using Response;

    public abstract class IRepository : MonoBehaviour
    {
        public abstract Task<Model.OutGame.User> GetUser(string userId);
        public abstract Task<ReceiveMailResponse> ReceiveMail(ReceiveMailRequest request);
        public abstract Task<ReceiveAllMailResponse> ReceiveAllMail(ReceiveAllMailRequest request);
        public abstract Task<List<Model.OutGame.Mail>> RefreshMails(RefreshMailsRequest request);
        public abstract Task UpgradeSkillBook(UpgradeSkillBookRequest request);
        public abstract Task EquipSkillBook(EquipSkillBookRequest request);
        public abstract Task ChangeDeck(ChangeDeckRequest request);
        public abstract Task UpgradePart(UpgradePartRequest request);
        public abstract Task EquipPart(EquipPartRequest request);
        public abstract Task<BuySkillBookInShopResponse> BuySkillBookInShop(BuySkillBookInShopRequest request);
        public abstract Task<Model.OutGame.SkillBookShop> ResetSkillBookShop(ResetSkillBookShopRequest request);
        public abstract Task<Model.OutGame.ShopSkillBook[]> ResetSkillBookShopByAds(ResetSkillBookShopByAdsRequest request);
        public abstract Task<BuyPartInShopResponse> BuyPartInShop(BuyPartInShopRequest request);
        public abstract Task<Model.OutGame.PartShop> ResetPartShop(ResetPartShopRequest request);
        public abstract Task<Model.OutGame.ShopPart[]> ResetPartShopByAds(ResetPartShopByAdsRequest request);
        public abstract Task<BuyGoldInShopResponse> BuyGoldInShop(BuyGoldInShopRequest request);
        public abstract Task<Model.OutGame.GoldShop> ResetGoldShop(ResetGoldShopRequest request);
        public abstract Task<Model.OutGame.ShopGold[]> ResetGoldShopByAds(ResetGoldShopByAdsRequest request);
        public abstract Task<BuyEnergyInShopResponse> BuyEnergyInShop(BuyEnergyInShopRequest request);
        public abstract Task<Model.OutGame.EnergyShop> ResetEnergyShop(ResetEnergyShopRequest request);
        public abstract Task<Model.OutGame.ShopEnergy[]> ResetEnergyShopByAds(ResetEnergyShopByAdsRequest request);
        public abstract Task<OpenSilverChestResponse> OpenSilverChest(OpenSilverChestRequest request);
        public abstract Task<GameStartResponse> GameStart(GameStartRequest request);
        public abstract Task<Model.InGame.User> GetInGameUser();
        public abstract Task<GameOverResponse> GameOver(GameOverRequest request);
        public abstract Task<WatchAdsResponse> WatchAds(WatchAdsRequest request);
    }
}
