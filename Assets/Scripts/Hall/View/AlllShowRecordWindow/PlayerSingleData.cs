using UnityEngine;

namespace Assets.Scripts.Hall.View.AlllShowRecordWindow
{
    /// <summary>
    /// 单局单个玩家的数据
    /// </summary>
    public class PlayerSingleData : MonoBehaviour
    {
        public UILabel PlayerName;
        public UILabel CurrentCount;
        public UISprite HuIcon;

        public void InitPlayerData(string playerName, string playerGold,string hu)
        {
            PlayerName.text = playerName;
            CurrentCount.text = playerGold;
            HuIcon.gameObject.SetActive(int.Parse(hu)==1);
        }
    }
}
