using System;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Game.paijiu.ImgPress.Main;
using Assets.Scripts.Game.paijiu.Tool;
using UnityEngine;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.paijiu.Mgr
{
    /// <summary>
    /// 玩家说话管理类 说话:德州扑克轮到本玩家决定跟牌与否的简称
    /// </summary>
    public class SpeakMgr : MonoBehaviour
    {

        /// <summary>
        /// 按键数组
        /// </summary>
        public GameObject[] Buttons;

        /// <summary>
        /// 其他标签页 加注,说话,结束选项,自动选项
        /// </summary>
        public GameObject[] Pages;

        public AddBetBtnItem[] AddBetBtns;


        // Use this for initialization
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            ShowNothing();
            InitOnClick();
        }

        /// <summary>
        /// 显示说话界面
        /// </summary>
        public virtual void ShowSpeak(GameRequestType rt)
        {
            YxDebug.Log("ShowSpeak rt === " + rt);
            if (rt == GameRequestType.None)
                return;

            switch (rt)
            {
                case GameRequestType.SendCard:

                    Tools.GobShowOnlyOne(Pages, "ZuPaiBtns");
                    break;

                case GameRequestType.BeginBet:
                    if (App.GetGameData<PaiJiuGameData>().BankerSeat != App.GameData.SelfSeat)
                    {
                        Tools.GobShowOnlyOne(Pages, "AddBetBtns");
                    }
                    break;
            }
        }



        /// <summary>
        /// 设置说话界面内容
        /// </summary>
        /// <param name="isput"></param>
        public void RejionRefreshBtns(bool isput)
        {
            int status = App.GetGameData<PaiJiuGameData>().Status;
            switch (status)
            {
                case 1:
                    ShowSpeak(GameRequestType.BeginBet);
                    break;
                case 2:
                    //没有完成组牌的话
                    if (!isput)
                    {
                        ShowSpeak(GameRequestType.SendCard);
                    }
                    break;
                default:
                    ShowNothing();
                    break;
            }
        }


        // ReSharper disable once UnusedMember.Local
        private void SetBtnState(string btnName, bool enable)
        {
            GameObject obj = Tools.GobGet(Buttons, btnName);
            if (obj == null)
            {
                YxDebug.Log(" === The obj ,name == " + btnName + " is null !! === ");
                return;
            }
            obj.GetComponent<BoxCollider>().enabled = enable;
            obj.GetComponent<UIButton>().state = enable ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled;
        }

        // ReSharper disable once UnusedMember.Local
        private void SetItemActive(GameObject[] group, string btnName, bool active)
        {
            GameObject obj = Tools.GobGet(group, btnName);
            if (obj == null)
            {
                YxDebug.Log(" === The obj ,name == " + btnName + " is null !! === ");
                return;
            }
            obj.SetActive(active);
        }



        /// <summary>
        /// 弃牌后不显示按键
        /// </summary>
        public void ShowNothing()
        {
            //Pool.SetActive(false);
            Tools.GobShowOnlyOne(Pages, "");
        }




        /// <summary>
        /// 初始化按键监听
        /// </summary>
        public void InitOnClick()
        {
            foreach (SpeakButton btnid in Enum.GetValues(typeof(SpeakButton)))
            {
                foreach (GameObject btn in Buttons)
                {
                    if (btn.name.Equals(btnid.ToString()))
                    {
                        Tools.NguiAddOnClick(btn, OnClickListener, (int)btnid);
                    }
                }
            }
            /************进度条按键添加事件************/
            //UIEventListener uiel = UIEventListener.Get(AddBtn.gameObject);
            //uiel.onPress = OnBarBtnPress;
            //uiel.parameter = 1;
            //uiel = UIEventListener.Get(SubtractBtn.gameObject);
            //uiel.onPress = OnBarBtnPress;
            //uiel.parameter = 0;
        }

        protected void OnClickListener(GameObject gob)
        {
            SpeakButton btnid = (SpeakButton)UIEventListener.Get(gob).parameter;

            switch (btnid)
            {
                case SpeakButton.CheckBtn:
                    OnClickCheckBtn();

                    break;
                case SpeakButton.FinishBtn:

                    OnClickFinishBtn();

                    break;

                case SpeakButton.ZuPaiBtn:

                    OnClickZuPaiBtn();
                    break;

            }
        }

        private void OnClickCheckBtn()
        {
            YxDebug.Log(" ====== 点击查看按钮 ====== ");
            App.GetGameManager<PaiJiuGameManager>().CheckShowedPanel.ShowCheckView();
        }

        private void OnClickFinishBtn()
        {
            int[] cardArray = App.GetGameData<PaiJiuGameData>().GetPlayer<PaiJiuPlayerSelf>().UserBetPoker.GetPokersArr();
            if (cardArray == null)
            {
                YxDebug.Log(" ==== 没有选择两张牌 ==== ");
                return;
            }
            YxDebug.Log(" === 发送数据 === ");
            Sfs2X.Entities.Data.SFSObject sfs = new Sfs2X.Entities.Data.SFSObject();
            sfs.PutIntArray("cards", cardArray);
            Tools.TestDebug(cardArray);
            sfs.PutInt("type", (int)GameRequestType.PutCard);
            App.GetRServer<PaiJiuGameServer>().SendGameRequest(sfs);
            YxDebug.Log(" === 发送数据结束 === ");
        }

        private int _index;


        private void OnClickZuPaiBtn()
        {
            int index2 = _index + 1;
            var betPoker = App.GetGameData<PaiJiuGameData>().GetPlayer<PaiJiuPlayerSelf>().UserBetPoker;
            betPoker.CleanSelected();
            betPoker.OnClickCard(0);
            betPoker.OnClickCard(index2);

            _index = (_index + 1) % 3;
        }



        /// <summary>
        /// 重置信息
        /// </summary>
        public void Reset()
        {
            ShowNothing();

            _index = 0;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected enum SpeakButton
        {
            ZuPaiBtn,
            CheckBtn,
            FinishBtn,
        }
    }
}
