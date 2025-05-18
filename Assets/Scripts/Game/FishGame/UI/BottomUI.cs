using Assets.Scripts.Game.FishGame.ChessCommon;
using Assets.Scripts.Game.FishGame.Common.Servers;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Sfs2X.Entities.Data;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Model;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.FishGame.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class BottomUI : UIView
    {
        public YxBaseTextureAdapter HeadTexture;
        public Text UserNameText;
        public NumberLabel UserCoin;


        public void OnF1Click()
        {
            GameMain.Singleton.Operation.BuyCoin();
        }
        public void OnF2Click()
        {
            GameMain.Singleton.Operation.Retrieve();
        }

        public void OnF3Click()
        {
            GameMain.Singleton.Operation.ChangeNextGunStyle();
        }
        public void OnF4Click()
        {
            GameMain.Singleton.Operation.ChangePriorGunStyle();
        }

        /// <summary>
        /// 设置玩家名称
        /// </summary>
        /// <param name="userName"></param>
        public void SetUserName(string userName)
        {
            if (UserNameText == null) return;
            UserNameText.text = userName;
        }

        /// <summary>
        /// 设置玩家名称
        /// </summary>
        /// <param name="head"></param>
        /// <param name="sex"></param>
        public void SetHead(string head,int sex)
        {
            if (HeadTexture == null) return;
            PortraitDb.SetPortrait(head, HeadTexture, sex);
        }

        /// <summary>
        /// 设置总金币
        /// </summary>
        /// <param name="totalCoin"></param>
        public void SetUserCoin(long totalCoin)
        {
            if (UserCoin == null) return;
            if(gameObject.activeSelf)UserCoin.SetNumber(totalCoin, 0.2f);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="gameInfo"></param>
        public void Init(ISFSObject gameInfo)
        {
            if (!gameInfo.ContainsKey(RequestKey.KeyUser)) return;
            var userInfo = gameInfo.GetSFSObject(RequestKey.KeyUser);
            var user = new YxBaseGameUserInfo();
            user.Parse(userInfo);

            //名字
            SetUserName(user.NickM);
            SetHead(user.AvatarX, user.SexI);
            //总金币
            var totalCoin = user.CoinA;
            App.GetGameData<FishGameData>().TotalCoin = totalCoin;
            SetUserCoin(totalCoin);
            if (!App.GetGameData<FishGameData>().NeedUpperScore)
            {
                App.GetRServer<FishGameServer>().SendBuyCoin((int)totalCoin);
            }
        }
    }
}
