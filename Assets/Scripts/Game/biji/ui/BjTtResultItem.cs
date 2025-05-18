using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Game.biji.network;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjTtResultItem : MonoBehaviour
    {
        public GameObject BigWiner;
        public GameObject RoomOwner;
        public NguiTextureAdapter UserHead;
        public UILabel UserName;
        public UILabel UserId;
        public UISprite NumItem;
        public UIGrid XiPaiGrid;
        public UIGrid GiveUpGrid;
        public UIGrid TotalScoreGrid;
        public UISprite TotalScoreItem;

        public void SetView(TtResultUserData ttResultUserData)
        {
            BigWiner.SetActive(ttResultUserData.IsWinner);
            RoomOwner.SetActive(ttResultUserData.IsRoomOwner);
            PortraitDb.SetPortrait(ttResultUserData.UserHead, UserHead, ttResultUserData.Sex);
            UserName.text = ttResultUserData.UserName;
            UserId.text = ttResultUserData.UserId.ToString();

            SetDaoScore(NumItem, ttResultUserData.XiPaiCnt, XiPaiGrid);
            SetDaoScore(NumItem, ttResultUserData.TouXiangCnt, GiveUpGrid);
            SetDaoScore(TotalScoreItem, ttResultUserData.UserGold, TotalScoreGrid, true);
        }
        public void SetDaoScore(UISprite numItem, int score, UIGrid grid, bool needAdd = false)
        {
            var scoreStr = score.ToString();
            if (score >= 0)
            {
                if (needAdd)
                {
                    var obj = YxWindowUtils.CreateItem(numItem, grid.transform);
                    obj.spriteName = "win";
                    obj.MakePixelPerfect();
                }

                for (int i = 0; i < scoreStr.Length; i++)
                {
                    var item = YxWindowUtils.CreateItem(numItem, grid.transform);
                    item.spriteName = "win" + scoreStr[i];
                    item.MakePixelPerfect();
                }
            }
            else
            {
                if (needAdd)
                {
                    var obj = YxWindowUtils.CreateItem(numItem, grid.transform);
                    obj.spriteName = "lose";
                    obj.MakePixelPerfect();
                }

                for (int i = 1; i < scoreStr.Length; i++)
                {
                    var item = YxWindowUtils.CreateItem(numItem, grid.transform);
                    item.spriteName = "lose" + scoreStr[i];
                    item.MakePixelPerfect();
                }
            }

            grid.repositionNow = true;
        }
    }
}
