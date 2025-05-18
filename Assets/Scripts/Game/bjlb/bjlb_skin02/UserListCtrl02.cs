using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.bjlb.bjlb_skin02
{
    public class UserListCtrl02 : UserListCtrl
    {

        public UILabel BankerLimit;

        public void InitBankerLimit(int limit)
        {

            if (BankerLimit == null) return;

            BankerLimit.text = YxFramwork.Tool.YxUtiles.GetShowNumberForm(limit);
        }

        /// <summary>
        /// 刷新banker数据
        /// </summary>
        /// <param name="responseData"></param>
        public override void RefreshBanker(ISFSObject responseData)
        {
            if (!responseData.ContainsKey("bankers"))
                return;
            Debug.Log("Get Bankers Info!!!!");
            var banbers = responseData.GetSFSArray("bankers");
            var b = responseData.GetInt("banker");
            var bankTotal = responseData.ContainsKey("bankTotal") ? responseData.GetLong("bankTotal") : 0;
            var bankerLens = banbers.Count;
            BankersInfo = new YxBaseGameUserInfo[bankerLens];
            var gdata = App.GetGameData<BjlGameData>();
            gdata.BankSeat = b;
            var isHaveMe = false;
            var selfInfo = gdata.GetPlayerInfo();
            var curBanker = gdata.CurrentBanker;
            if (bankerLens < 1)//系统庄
            {
                SetBankerType();
                if (curBanker != null)
                {
                    curBanker.Info = null;
                }
                return;
            }

            var index = 0;
            var bankerInfo = curBanker.GetInfo<BjlUserInfo>();
            //遍历庄家
            foreach (ISFSObject banber in banbers)
            {
                var bseat = banber.GetInt("seat");
                var ttgold = banber.GetLong("ttgold");

                YxBaseGameUserInfo user = gdata.GetPlayerInfo(bseat, true);
                if (user == null) continue;
                user.CoinA = ttgold;

                BankersInfo[index++] = user;

                if (user.Seat == b)
                {
                    if (bankerInfo == null || bankerInfo.Seat != b || bankerInfo.NickM != user.NickM) //换庄家以后局数重新开始计数
                    {
                        user.TotalCount = 0;
                    }
                    else
                    {
                        user.TotalCount = bankerInfo.TotalCount;
                    }
                    user.WinTotalCoin = bankTotal;

                    curBanker.UpdateView(user);
                }

                if (selfInfo.Seat == user.Seat)
                {
                    isHaveMe = true;
                }
            }
            SetBankerType(isHaveMe, b);

            RefreahAll(UserListType.Banker);

        }
    }
}