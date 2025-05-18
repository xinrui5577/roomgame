using System.Globalization;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.nn41
{
    public class RoomInfoCtrl : MonoBehaviour
    {
        /// <summary>
        /// 房间号
        /// </summary>
        public UILabel RoomIdNum;
        /// <summary>
        /// 游戏局数
        /// </summary>
        public UILabel Jushu;
        /// <summary>
        /// 游戏背景上的底分
        /// </summary>
        public UILabel DiFen;
        /// <summary>
        /// 是什么座位选择
        /// </summary>
        public UILabel ZuoWei;
        /// <summary>
        /// 游戏规则的界面
        /// </summary>
        public GameObject GameRule;
        /// <summary>
        /// 面板中需要显示的label
        /// </summary>
        public UILabel[] RulesLabels;

        protected void Awake()
        {
            TableShowCtrl();
            GameObject.Find("UI Root/Camera/Game/RoomInfo/").SetActive(false);
        }

        private void TableShowCtrl(bool isHide=false)
        {
            GameObject.Find("UI Root/Camera/Game/RoomInfo/Grid/RoomID").SetActive(isHide);
            GameObject.Find("UI Root/Camera/Game/RoomInfo/Grid/GameNum").SetActive(isHide);
            GameObject.Find("UI Root/Camera/Game/RoomInfo/Grid/Seat").SetActive(isHide);
            GameObject.Find("UI Root/Camera/Game/RoomInfo/Grid/Ante").SetActive(isHide);
        }

        /// <summary>
        /// 初始化房间信息
        /// </summary>
        public void InitRoomInfo(int roomNum, string[] infos, string bankType)
        {
            var gdata = App.GetGameData<NnGameData>();
            if (gdata.IsKaiFang)
            {
                var curNum = "[ffff00]" + gdata.CurrentRound;
                var totalNum = "[ffffff]" + gdata.MaxRound;

                if (RoomIdNum != null)
                {
                    RoomIdNum.text = roomNum.ToString(CultureInfo.InvariantCulture);
                }

                if (Jushu != null)
                {
                    Jushu.text = string.Format("{0}/{1}", curNum, totalNum);
                }

                TableShowCtrl(true);
            }
            GameObject.Find("UI Root/Camera/Game/RoomInfo/").SetActive(!gdata.IsHideRule);
            
            RulesLabels[0].text = bankType;
            foreach (var t in infos)
            {
                if (RulesLabels[1].text == "") RulesLabels[1].text = t.IndexOf("翻倍规则", System.StringComparison.Ordinal) > -1 ? t.Split(':')[1] : "";
                if (RulesLabels[2].text == "") RulesLabels[2].text = t.IndexOf("特殊牌型", System.StringComparison.Ordinal) > -1 ? t.Split(':')[1] : "";
                if (RulesLabels[3].text == "") RulesLabels[3].text = t.IndexOf("高级选项", System.StringComparison.Ordinal) > -1 ? t.Split(':')[1] : "";
                if (RulesLabels[4].text != "") continue;
                RulesLabels[4].text = t.IndexOf("底", System.StringComparison.Ordinal) > -1 ? t.Split(':')[1] : "";
                DiFen.text = t.IndexOf("底", System.StringComparison.Ordinal) > -1 ? t.Split(':')[1] : "";
            }
            ZuoWei.text = bankType;
        }

        /// <summary>
        /// 局数的改变
        /// </summary>
        /// <param name="num"></param>
        public void GameNumChange(int num)
        {
            var gdata = App.GetGameData<NnGameData>();
            var gmanager = App.GetGameManager<NnGameManager>();

            var curNum = "[ffff00]" + num;
            var totalRound = "[ffffff]" + App.GetGameData<NnGameData>().MaxRound;
            if (!gdata.IsKaiFang) return;
            if (Jushu != null)
            {
                Jushu.text = string.Format("{0}/{1}", curNum, totalRound);
            }

            gmanager.MenuCtrl.CreatRoomNormalShow();
        }

        public void ShowGameRule()
        {
            GameRule.SetActive(!GameRule.activeSelf);
        }
    }
}
