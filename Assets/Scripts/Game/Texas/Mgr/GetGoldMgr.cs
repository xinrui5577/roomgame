using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Game.Texas.Main;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Texas.Mgr
{
    public class GetGoldMgr : MonoBehaviour
    {
        public GameObject Bg;

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
        public NguiLabelAdapter ChooseGoldLabelAdp;
        /// <summary>
        /// 最小金币
        /// </summary>
        [HideInInspector]
        public long MinGold;
        /// <summary>
        /// 最大金币
        /// </summary>
        [HideInInspector]
        public long MaxGold;

        /// <summary>
        /// 选择金币
        /// </summary>
        [HideInInspector]
        public long ChooseGold;
        /// <summary>
        /// 选择带入进度条
        /// </summary>
        public UISlider ChooseSlider;
        /// <summary>
        /// 自动补充选项
        /// </summary>
        public UIToggle AutoAdd;

        public void AutoShowPanel(YxBaseGameUserInfo selfInfo = null)
        {
            if (selfInfo == null) { return; }
            var ante = App.GetGameData<TexasGameData>().Ante;
            MaxGold = (selfInfo.CoinA + selfInfo.RoomCoin) > ante * 100 ? ante * 100 : selfInfo.CoinA + selfInfo.RoomCoin;
            MaxGold = selfInfo.RoomCoin >= ante * 100 ? 0 : MaxGold - selfInfo.RoomCoin;

            if (MaxGold > 0)
            {
                OpenPanel(selfInfo);
            }
        }


        public void OpenPanel(YxBaseGameUserInfo selfInfo = null)
        {
            Bg.SetActive(true);
            if (selfInfo == null) { return; }
            selfInfo.State = false;

            int ante = App.GetGameData<TexasGameData>().Ante;
            //MinGold = selfInfo.RoomGold >= App.GetGameData<GlobalData>().Ante * 10 ? App.GetGameData<GlobalData>().Ante * 10 : selfInfo.RoomGold;
            MinGold = selfInfo.RoomCoin >= ante * 10 ? 0 : ante * 10 - selfInfo.RoomCoin;
            MinGold = MinGold > selfInfo.CoinA ? selfInfo.CoinA : MinGold;

            MaxGold = (selfInfo.CoinA + selfInfo.RoomCoin) > ante * 100 ? ante * 100 : selfInfo.CoinA + selfInfo.RoomCoin; //App.GetGameData<GlobalData>().Ante * 100 ? App.GetGameData<GlobalData>().Ante * 100 : selfInfo.Gold;
            MaxGold = selfInfo.RoomCoin >= ante * 100
                ? 0
                : MaxGold - selfInfo.RoomCoin; //App.GetGameData<GlobalData>().Ante*100 - selfInfo.RoomGold;
         
            Debug.Log(" ==== Ante == " + ante + " , MinGold == " + MinGold + " , MaxGold == " + MaxGold + " ==== ");

            MinGoldLabel.text = string.Format("最小:{0}", YxUtiles.ReduceNumber(MinGold));
            MaxGoldLabel.text = string.Format("最大:{0}", YxUtiles.ReduceNumber(MaxGold));
            TotalGoldLabel.text = string.Format("现有金额:{0}", YxUtiles.ReduceNumber(selfInfo.CoinA));

            ChooseSlider.value = 1f;
            ChooseGold = MaxGold;

            //显示要带入筹码金额
            ChooseGoldLabelAdp.Text(YxUtiles.ReduceNumber(ChooseGold));
        }


        public void AutoMaxGold(YxBaseGameUserInfo selfInfo)
        {
            var gdata = App.GetGameData<TexasGameData>();
            int ante = gdata.Ante;
            MaxGold = selfInfo.CoinA > ante * 100 ? ante * 100 : selfInfo.CoinA;
            MaxGold = selfInfo.RoomCoin >= ante * 100
                ? 0
                : MaxGold - selfInfo.RoomCoin;


            Dictionary<string, object> data = new Dictionary<string, object>() { { "gold", (int)MaxGold } };
            App.GetRServer<TexasGameServer>().SendRequest(GameRequestType.SetGold, data);

            if (!gdata.IsRoomGame && selfInfo.State)
            {
                //自动准备
                App.GetRServer<TexasGameServer>().SendReadyGame();
            }
        }

        public void ClosePanel()
        {
            Bg.SetActive(false);
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
            var gdata = App.GetGameData<TexasGameData>();
            ChooseGold = (long)(p * (MaxGold - MinGold) + MinGold);

            if (ChooseGold != MaxGold && ChooseGold != MinGold)
            {
                ChooseGold -= ChooseGold % gdata.Ante;
            }

            //显示要带入筹码金额
            ChooseGoldLabelAdp.Text(YxUtiles.ReduceNumber(ChooseGold));
        }


        /// <summary>
        /// 确定按键
        /// </summary>
        public virtual void OnEnterClick()
        {
            var data = new Dictionary<string, object>() { { "gold", (int)ChooseGold } };
            App.GetRServer<TexasGameServer>().SendRequest(GameRequestType.SetGold, data);

            ClosePanel();
        }
    }

}