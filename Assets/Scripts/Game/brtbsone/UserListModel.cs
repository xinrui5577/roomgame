using System;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.brtbsone
{
    public class UserListModel : MonoBehaviour
    {
        public UILabel NameLabel;
        public UILabel CoinLabel;
        public int UserId;

        public virtual void SetInfo(YxBaseUserInfo user, bool isBanker, int rank = 0)
        {
            NameLabel.text = isBanker ? "[FF0000]" + user.NickM : user.NickM;
            CoinLabel.text = (isBanker ? "[FF0000]" : "") + YxUtiles.ReduceNumber(user.CoinA);
            UserId = user.Id;
        }
        public virtual void SetInfo(string players, int rank = 0)
        {
            var temp = players.Split(',');
            NameLabel.text = temp[0];
            CoinLabel.text = YxUtiles.ReduceNumber(int.Parse(temp[1]), 2, true);
        }
    }
}
