using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Tbs
{
    /// <summary>
    /// 选择锅管理 单例类
    /// </summary>
    public class ChooseGuoMgr : MonoBehaviour
    {
        /// <summary>
        /// 选锅界面父节点
        /// </summary>
        public GameObject BG;
        /// <summary>
        /// 总共金币显示
        /// </summary>
        public UILabel TotalGoldLabel;
        /// <summary>
        /// 最小金币显示
        /// </summary>
        public UILabel MinGoldLabel;
        /// <summary>
        /// 最大金币显示
        /// </summary>
        public UILabel MaxGoldLabel;
        /// <summary>
        /// 选择金币显示
        /// </summary>
        public UILabel ChooseGoldLabel;
        /// <summary>
        /// 最小金币
        /// </summary>
        public long MinGold;
        /// <summary>
        /// 最大金币
        /// </summary>
        public long MaxGold;
        /// <summary>
        /// 选择金币
        /// </summary>
        public long ChooseGold;

        /// <summary>
        /// 打开面板
        /// </summary>
        public void OpenPanel(TbsUserInfo selfInfo)
        {
            var ante = App.GetGameManager<TbsGameManager>().BetManager.Ante;
            if (selfInfo.CoinA < ante * 100)
            {
                ClosePanel();
                return;
            }

            BG.SetActive(true);
            MinGold = selfInfo.CoinA > ante * 100 ? ante * 100 : selfInfo.CoinA;
            MaxGold = selfInfo.CoinA / 2;
            MinGoldLabel.text = "最小金额:" + MinGold;
            MaxGoldLabel.text = "最大金额:" + MaxGold;
            ChooseGoldLabel.text = MinGold.ToString(CultureInfo.InvariantCulture);
            TotalGoldLabel.text = selfInfo.CoinA.ToString(CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// 关闭面板
        /// </summary>
        public void ClosePanel()
        {
            BG.SetActive(false);
        }
        /// <summary>
        /// 设置选择的金币
        /// </summary>
        public void SetCurrentGold()
        {
            if (UIProgressBar.current == null)
            {
                return;
            }
            float p = UIProgressBar.current.value;
            ChooseGold = (long)(p*(MaxGold - MinGold) + MinGold);
            ChooseGoldLabel.text = YxUtiles.ReduceNumber((int)ChooseGold); // ChooseGold.ToString();
        }
        /// <summary>
        /// 确认事件 发送锅的值
        /// </summary>
        public void OnClickEnter()
        {
            IDictionary data = new Dictionary<string,object>();
            data.Add("gold",(int)ChooseGold);
            App.GetRServer<TbsRemoteController>().SendRequest(GameRequestType.SetGuo, data);
            ClosePanel();
        }
    }
}
