using System.Collections; 
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Shuihuzhuan
{
    public class BigOrSmallControl : MonoBehaviour
    {
        public static BigOrSmallControl Instance;
        /// <summary>
        /// 骰子
        /// </summary>
        public Sprite[] resultSprites;
        /// <summary>
        /// 大小和
        /// </summary>
        int[] history;

        /// <summary>
        /// 历史ui数组
        /// </summary>
        public Image[] history_img;
        /// <summary>
        /// 大小和十张图片
        /// </summary>
        public Sprite[] history_Sprite;
        /// <summary>
        /// 我的钱数
        /// </summary>
        public Text MyMoneyText;
        /// <summary>
        /// 当前所得
        /// </summary>
        public Text winText; 
        /// <summary>
        /// 下注倍数
        /// </summary>
        public Text betText;
        /// <summary>
        /// 小  按钮
        /// </summary>
        public Button smallBtn;
        /// <summary>
        /// 和 按钮
        /// </summary>
        public Button middleBtn;
        /// <summary>
        /// 大 按钮
        /// </summary>
        public Button bigBtn;
        /// <summary>
        /// 骰子位置
        /// </summary>
        public Image resultImage1;
        /// <summary>
        /// 骰子位置
        /// </summary>
        public Image resultImage2;
        /// <summary>
        ///  自己
        /// </summary>
        public GameObject thisPanel;
        /// <summary>
        /// 比倍和得分
        /// </summary>
        public GameObject nextPanel;

        private int iSone = 1;

        private int iSsec = 1;
        /// <summary>
        /// 中间的人
        /// </summary>
        public Image bossImage;
        /// <summary>
        /// 左边的人
        /// </summary>
        public Image leftImage;
        /// <summary>
        /// 右边的人
        /// </summary>
        public Image rightImage;

        public GameObject rightjinbi;
        public GameObject leftjinbi;
        public GameObject bossjinbi;

        private int Lishi = 0;
        private int Shaizi = 0;
        private void Awake()
        {

            Instance = this;
            history = new int[] { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 };
            for (int i = 0; i < 10; i++)
            {
                history_img[i].enabled = false;
            }
            Facade.EventCenter.AddEventListeners<EWmarginEventType, long>(EWmarginEventType.RefreshTotalMoney,OnReshTotalMoney);
        }

        private void OnReshTotalMoney(long obj)
        {
            if (MyMoneyText == null) { return; }
            MyMoneyText.text = YxUtiles.GetShowNumberToString(obj);
        }

        /// <summary>
        /// 服务器初始化数据
        /// </summary>
        public void GameBiBeiResultFun()
        {
            Invoke("Chushihua", 2f);
        }

        private void Chushihua()
        {
            HideBetButtons();
            var gdata = App.GetGameData<WmarginGameData>();
            iSone = gdata.iDice1;
            iSsec = gdata.iDice2;
            var animator = bossImage.GetComponent<Animator>(); 
            animator.Play("boss_3");
            AnimatorFun();
        }
         
        public void History()
        {
            int index;
            if ((iSone + iSsec) > 7)
            {
                index = 2;
            }
            else if ((iSone + iSsec) < 7)
            {
                index = 0;
            }
            else
            {
                index = 1;
            }

            for (int i = 0; i < 10; i++)
            {
                if (i < 9)
                {
                    history[i] = history[i + 1];
                }
                else
                {
                    history[i] = index;
                }
            }

            for (int i = 0; i < 10; i++)
            {
                if (history[i] == 0)
                {
                    history_img[i].sprite = history_Sprite[0];
                    history_img[i].enabled = true;
                }
                else if (history[i] == 1)
                {
                    history_img[i].sprite = history_Sprite[1];
                    history_img[i].enabled = true;
                }
                else if (history[i] == 2)
                {
                    history_img[i].sprite = history_Sprite[2];
                    history_img[i].enabled = true;
                }
            } 
        }

        public void bibeiFun()
        {
            var gdata = App.GetGameData<WmarginGameData>();
            if (gdata.Malizhuantai)
            {
                gdata.Malizhuantai = false;
                //                YxDebug.LogError(App.GetGameData<GlobalData>().MaliWinMony );
                SetWinText(gdata.MaliWinMony);
                gdata.iWinMoney = gdata.MaliWinMony;
            }
            else
            {
                SetWinText(gdata.iWinMoney);
            }
//            MyMoneyText.text = gdata.GetPlayerInfo().CoinA.ToString();
            SetBetText(gdata.BetNum);
            bossImage.GetComponent<Animator>().Play("boss_1");
            Invoke("ShowBetButtons", 2);
            Invoke("ShowBigSmallBtnFun", 2);//19
            Facade.Instance<MusicManager>().Play("yaosaizi");

        }

        protected void SetWinText(int coin)
        {
            winText.text = YxUtiles.GetShowNumberToString(coin);
        }
        protected void SetBetText(int coin)
        {
            betText.text = YxUtiles.GetShowNumberToString(coin);
        }

        /// <summary>
        /// 关闭大小和
        /// </summary>
        public void HideBetButtons()
        {

            smallBtn.interactable = false;
            bigBtn.interactable = false;
            middleBtn.interactable = false;
        }
        public void ShowBigSmallBtnFun()
        {
            Facade.Instance<MusicManager>().Play("xia");
            smallBtn.gameObject.SetActive(true);
            bigBtn.gameObject.SetActive(true);
            middleBtn.gameObject.SetActive(true);
        }
        /// <summary>
        /// 比倍
        /// </summary>
        public void BiBeiBtnFun()
        {
            Facade.Instance<MusicManager>().Play("yaosaizi");
            bossImage.GetComponent<Animator>().Play("boss_1");
            leftImage.GetComponent<Animator>().Play("left_1");
            rightImage.GetComponent<Animator>().Play("right_1");
            Invoke("ShowBetButtons", 2);
            Invoke("ShowBigSmallBtnFun", 2);
            nextPanel.SetActive(false);//比倍和得分关闭
            resultImage1.gameObject.SetActive(false);//骰子打开
            resultImage2.gameObject.SetActive(false);
        }
        /// <summary>
        /// 得分
        /// </summary>
        public void GetMoneyBtnFun()
        { 
            CloseSelf();
        }
        /// <summary>
        /// 小
        /// </summary>
        public void SmallBtnFun()
        {
            BigBtnFun(App.GetGameData<WmarginGameData>().Yazhu1);
        }
        /// <summary>
        /// 和
        /// </summary>
        public void MiddleBtnFun()
        {
            BigBtnFun(App.GetGameData<WmarginGameData>().Yazhu3);
        }
        /// <summary>
        /// 大
        /// </summary>
        public void BigBtnFun()
        {
            BigBtnFun(App.GetGameData<WmarginGameData>().Yazhu2);
        }

        private void BigBtnFun(int type)
        {
            var gdata = App.GetGameData<WmarginGameData>(); 
            Facade.Instance<MusicManager>().Play("xia1");
            leftjinbi.SetActive(type == gdata.Yazhu1);
            bossjinbi.SetActive(type == gdata.Yazhu3);
            rightjinbi.SetActive(type == gdata.Yazhu2);
            App.GetRServer<WmarginGameServer>().SendMyDaXiaoHe(App.GetGameData<WmarginGameData>().iWinMoney, type);
            HideBetButtons();
        }

        public void AnimatorFun()
        {
            StartCoroutine(ShowCard());
        }

        public IEnumerator ShowCard()
        {
            var musicMgr = Facade.Instance<MusicManager>();
            yield return new WaitForSeconds(0.5f);
            resultImage1.gameObject.SetActive(true);//骰子打开
            resultImage2.gameObject.SetActive(true);
            resultImage1.sprite = resultSprites[iSone - 1];//骰子的图片
            resultImage2.sprite = resultSprites[iSsec - 1];//骰子的图片
            SetWinText(App.GetGameData<WmarginGameData>().iWinMoney);
            //            ShowHistory(App.GetGameData<GlobalData>().iHistory);
            if (App.GetGameData<WmarginGameData>().iWinMoney == 0)//输了
            {
                musicMgr.Play(iSone + iSsec + "dian");
                leftImage.GetComponent<Animator>().Play("left_3");
                rightImage.GetComponent<Animator>().Play("right_3");
                yield return new WaitForSeconds(2);
                musicMgr.Play("shu");
                Invoke("CloseSelf", 3);
                BottomUIControl.instance.Theincome();

            }
            else
            {
                musicMgr.Play(iSone + iSsec + "dian");
                leftImage.GetComponent<Animator>().Play("left_2");
                rightImage.GetComponent<Animator>().Play("right_2");
                HideBetButtons();
                musicMgr.Play("ying");
                Invoke("ReShowSelf", 3);
            }
            History();
        }
        public void CloseSelf()//输了
        {
            //Lishijilu();
            Facade.Instance<MusicManager>().ChangeBackSound(0);
            ReHideBetButtons();
            resultImage1.gameObject.SetActive(false);//骰子关闭
            resultImage2.gameObject.SetActive(false);
            nextPanel.SetActive(false);//比倍和得分关闭
            WinPanelControl.instance.winPanel.SetActive(false);
            GameStateUiControl.instance.LostWait();
            GameStateUiControl.instance.TiShi.interactable = true;
            thisPanel.SetActive(false);//自己关闭
            leftjinbi.SetActive(false);
            rightjinbi.SetActive(false);
            bossjinbi.SetActive(false);
            BottomUIControl.instance.DaxiaoheFun();
            var gdata = App.GetGameData<WmarginGameData>();
            // Lishijilu();
            if (gdata.isMary)//玛丽
            {
                gdata.iWinMoney = 0;
                gdata.MaliWinMony = 0;
                LittleMaryControl.Instance.OpenMaryPanel();
                LittleMaryControl.Instance.StartMali();
                gdata.IsAuto = false;
            }
        }

        public void ReShowSelf()
        {
            ReHideBetButtons();
            nextPanel.SetActive(true);//得分和比倍打开
        }
        public void ShowBetButtons()//大小和按钮点击事件打开
        {
            smallBtn.interactable = true;
            bigBtn.interactable = true;
            middleBtn.interactable = true;

        }
        public void ReHideBetButtons()
        {
            ShowBetButtons();
            smallBtn.gameObject.SetActive(false);//大小和按钮关闭
            bigBtn.gameObject.SetActive(false);
            middleBtn.gameObject.SetActive(false);
        }
        public void ThisPlaneFun()
        {
            thisPanel.SetActive(true);
            Facade.Instance<MusicManager>().ChangeBackSound(1);
        }
    }


}
