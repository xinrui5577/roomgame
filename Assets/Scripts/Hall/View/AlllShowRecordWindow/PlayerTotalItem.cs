using Assets.Scripts.Common.components;
using UnityEngine;

namespace Assets.Scripts.Hall.View.AlllShowRecordWindow
{
    /// <summary>
    /// 每个房卡中玩家的总信息
    /// </summary>
    public class PlayerTotalItem : MonoBehaviour
    {
        public UITexture HeadTexture;
        public UILabel PlayerName;
        public UILabel PlayerGold;

        public void InitPlayerData(string playerName,string playerGold,string avatar="")
        {
            PlayerName.text = playerName;
            PlayerGold.text = playerGold;
            PortraitRes.SetPortrait(avatar, HeadTexture,1);
        }
    }
}
