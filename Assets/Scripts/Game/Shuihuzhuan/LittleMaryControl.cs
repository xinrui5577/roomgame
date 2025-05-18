using System;
using System.Collections;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.Shuihuzhuan
{
    public class LittleMaryControl : MonoBehaviour
    {
        public static LittleMaryControl Instance;
        /// <summary>
        /// 跑马的24个图片
        /// </summary>
        public Transform[] paoma;
        /// <summary>
        /// 四个图片
        /// </summary>
        public Image[] images;
        /// <summary>
        /// 次数
        /// </summary>
        public Text remainText;

        public Text MyMoneyText;

        public Text winMoneyText;

        public Text betNumText;
        /// <summary>
        /// 玛丽
        /// </summary>
        public GameObject maryPanel;
        /// <summary>
        /// 比倍和得分
        /// </summary>
        public GameObject quitPanel;
        /// <summary>
        /// 接受服务器图片
        /// </summary>
        private int[] imageArray;

        private int curretImg = 0;

        private float curretTimer = 0f;

        private int nImg = 0;

        private float MarqueeInterval = .03f; 

        private int resultNum = 80;

        private int result = 24;

        private bool canMove = false;

        public void Awake()
        {
            Instance = this;
            Facade.EventCenter.AddEventListeners<EWmarginEventType, long>(EWmarginEventType.RefreshTotalMoney, OnReshTotalMoney);
        }

        private void OnReshTotalMoney(long obj)
        {
            if (MyMoneyText == null) { return; }
            MyMoneyText.text = YxUtiles.GetShowNumberToString(obj);
        }

        public void OpenMaryPanel()
        {
            RefeshTexts();
            maryPanel.SetActive(true);
        }
        public void CloseMaryPanel()
        {
            quitPanel.SetActive(false);
            maryPanel.SetActive(false);
        }

        public void ShowOverPanel()
        {
            //小玛丽游戏结束  （玛丽次数为0，并且没有赢得金币）
            //还有n次小玛丽，确定后将继续下一次小玛丽！(玛丽次数大于0，并且没有赢取金币)
            //本次玛丽结束，请选择操作！\n还有n次小玛丽！(玛丽次数大于0，并且获得金币)
            var gdata = App.GetGameData<WmarginGameData>();
            string msg;
            string[] showBtns = null;
            if (gdata.iMaliGames > 0)//还有玛丽
            {
                if (gdata.MaliWinMony > 0)
                {
                    msg = string.Format("本次玛丽结束，请选择操作！\n还有{0}次小玛丽！",gdata.iMaliGames);
                    showBtns = new[] { "|得分", "|比倍|btn_bijiao"}; 
                }
                else
                {
                    msg = string.Format("还有{0}次小玛丽，确定后将继续下一次小玛丽！", gdata.iMaliGames);
                }
            }
            else//没有玛丽
            {
                msg = "小玛丽游戏结束！";
                showBtns = gdata.MaliWinMony > 0 ? new[] { "|得分", "|比倍|btn_bijiao" } : new[] { "|返回" };
            }
            var boxData = new YxMessageBoxData
            {
                Msg = msg,
                ShowBtnNames = showBtns,
                Listener = (box, btnName) =>
                {
                    gdata.isMary = gdata.iMaliGames > 0;
                    if (btnName == "btn_bijiao")
                    {
                        BiBeiBtnFun();
                    }
                    else
                    {
                        GetMoneyBtnFun();
                    } 
                }
                
            };
            YxMessageBox.Show(boxData);
        }

        public void GetMoneyBtnFun() //得分
        {
            result = 24;
            var gdata = App.GetGameData<WmarginGameData>();
            if (gdata.iMaliGames <= 0)//没有玛丽次数
            {
                gdata.isMary = false;
                GameStateUiControl .instance .LostWait();
                BottomUIControl.instance.TheincomeMali();
                CloseMaryPanel();
                WinPanelControl.instance.winPanel.SetActive(false);
                Facade.EventCenter.DispatchEvent(EWmarginEventType.RefreshTotalMoney, gdata.GetPlayerInfo().CoinA);
            }
            else//还有马力次数
            {
                gdata.isMary = true;
                Facade.EventCenter.DispatchEvent(EWmarginEventType.RefreshTotalMoney, gdata.GetPlayerInfo().CoinA);
                gdata.MaliWinMony = 0;
                SetWinText(0);
                App.GetRServer<WmarginGameServer>().SendMaLi();
                quitPanel.SetActive(false);
            }
        }

        protected void SetWinText(int coin)
        {
            winMoneyText.text = YxUtiles.GetShowNumberToString(coin);
        }
        protected void SeBetText(int coin)
        {
            betNumText.text = YxUtiles.GetShowNumberToString(coin);
        }
        public void BiBeiBtnFun()//比倍
        {
            App.GetGameData<WmarginGameData>().Malizhuantai = true;
            result = 24;
            BottomUIControl.instance.BigSmallBtnFun();
            CloseMaryPanel();
        }
        public void MaryResultFun()
        { 
            var gdata = App.GetGameData<WmarginGameData>(); 
            result = gdata.iMaliZhuanImage;//转的图片
            resultNum = gdata.iMaliZhuanImage + 48;
            gdata.isMary = true;

            if (maryPanel.activeSelf == false)
            {
                OpenMaryPanel();
            }
            else
            {
                RefeshTexts(false);
            }
            for (var i = 0; i < images.Length; i++)
            {
                images[i].sprite = TurnControl.instance.cardSprites[gdata.iMaliImage[i]];
            }
            canMove = true;   
        }

      

        public void RefeshTexts(bool isAll=true)
        {
            var gdata = App.GetGameData<WmarginGameData>();
            if (isAll)
            {
                SetWinText(gdata.MaliWinMony);
                SeBetText(gdata.BetNum);
            }
            remainText.text = gdata.iMaliGames.ToString();
        }

        private bool _isTurned;
        public void StartMali()
        {
            if (_isTurned) { return;}
            _isTurned = true;
            StartCoroutine(Turnning());
        }

        private IEnumerator Turnning()
        {
            yield return new WaitForSeconds(2);
            if (result == 0 || result == 6 || result == 12 || result == 18)
            {
                //                quitPanel.SetActive(true);
                ShowOverPanel(); 
                canMove = false;
            }
            else
            {
                App.GetRServer<WmarginGameServer>().SendMaLi();
            }
            _isTurned = false;
        }

        public void ClearPaomadeng()
        {
            for (int i = 0; i < paoma.Length; i++)
            {
                paoma[i].gameObject.SetActive(false);
            }
        }
        private void HidePaoma(int curtImg)
        {
            paoma[curtImg].gameObject.SetActive(false);
        }
        void Paoma(int curtImg)
        {
            Facade.Instance<MusicManager>().Play("gundong");
            paoma[curtImg].gameObject.SetActive(true);
            int temp = curtImg == 0 ? paoma.Length - 1 : curtImg - 1;
            StartCoroutine("HidePaoma", temp);
        }

        private float addTime = 0f;
        void Update()
        {
            curretTimer += Time.deltaTime;
            if (curretTimer > MarqueeInterval && canMove == true)
            {
                curretImg = nImg % 24;
                if (nImg == resultNum)
                {
                    YxDebug.LogError(result + "转的图片");
                    Paoma(curretImg);
                    nImg = curretImg;
                    resultNum = -5;
                    canMove = false;
                    RefeshTexts();
                    StartMali();
                }
                else if (nImg < resultNum)
                {
                    Paoma(curretImg);
                    nImg++;
                    curretTimer = 0f;
                }
                else
                {
                    nImg = curretImg;
                }
            }


        }
    }

}