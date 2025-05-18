using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Shuihuzhuan
{
    public class ShowResult : MonoBehaviour {

        public static ShowResult instance;

        public Transform[] endPositions;

        public Transform[] homePositions;

        public GameObject[] lines;
        private bool mAnimation = true;
        public Image[] resultImages;
        void Awake()
        {
            instance = this;

        }
        /// <summary>
        /// 开奖数据图片替换
        /// </summary>
        public void SetResultSprite()
        {
            for (int i = 0; i < TurnControl.instance.resultItems.Length; i++)
            {
                resultImages[i].sprite = TurnControl.instance.cardSprites[App.GetGameData<WmarginGameData>().iTypeImgid[i]];
            }
         
        }

        /// <summary>
        /// 旋转动画
        /// </summary>
        /// <param name="timeKind"></param>
        public void Moveing(int timeKind)
        {
           
            TurnControl.instance.resultItems[timeKind - 1].transform.localPosition =
                TurnControl.instance.homeTransforms[timeKind - 1].localPosition;

            if (timeKind == 15)
            {
                Invoke("GoBackHome", 1.5f);
            }
        }
        public void OnClick()
        {
            var gdata = App.GetGameData<WmarginGameData>();
            GameStateUiControl.instance.TiShi.interactable = false;
            App.GetGameManager<WmarginGameManager>().ClearData();//清楚数据
            var turnCtr = TurnControl.instance;
            turnCtr.canMove = true;
            for (var i = 0; i < 15; i++)
            {
                turnCtr.resultImages[i].GetComponent<Animator>().enabled = false;
                gdata.iTypeImgid[i] = 0;
                resultImages[i].sprite = turnCtr.cardSprites[0];
            }
            var turnItems = turnCtr.turnItems;
            var turnItemCount = turnCtr.turnItems.Length;
            for (var i = 0; i < turnItemCount; i++)
            {
                var turnItem = turnItems[i];
                turnItem.SetActive(true);
                turnItem.GetComponent<Image>().sprite = turnCtr.graySprites[0];
            }

            gdata.changeState = false;
            for (var i = 0; i < 18; i++)
            {
                gdata.iLineImgid[i] = 0;
            }
            
            for (var i = 0; i < 9; i++)
            {
                gdata.m_LineType[i] = 0;
            }
            StartCoroutine(ShowAwardEffect());
        }

        /// <summary>
        /// 转动
        /// </summary>
        public void GoBackHome()
        {
            var gdata = App.GetGameData<WmarginGameData>();
            var gMgr = App.GetGameManager<WmarginGameManager>();
            for (var i = 0; i < TurnControl.instance.resultItems.Length; i++)
            {
                var ccc = gdata.iTypeImgid[i];

                TurnControl.instance.resultImages[i].sprite = TurnControl.instance.graySprites[ccc];
            }
            for (var i = 0; i < TurnControl.instance.resultItems.Length; i++)
            {
                TurnControl.instance.resultItems[i].transform.localPosition =
                    TurnControl.instance.bornTransforms[i / 3].localPosition;
            }
            for (var i = 0; i < TurnControl.instance.turnItems.Length; i++)
            {
                TurnControl.instance.turnItems[i].SetActive(true);

            }
            if (gdata.NeedShowAnim && !BottomUIControl.instance.skip_bool)//开场动画运行一次
            {
                GameStateUiControl.instance.TiShi.interactable = false;
                gdata.changeState = false;
                StartCoroutine(ShowAwardEffect());
            }
            else
            { 
                GameStateUiControl.instance.TiShi.interactable = true;
                BottomUIControl.instance.SetMoney();//开始显示金钱和线数
                gdata.IsAotozhuangtai = true;
                GameStateUiControl.instance.LostWait();
                gMgr.ClearData();//清楚数据
            }
            gdata.NeedShowAnim = false;
            //切换开始结束状态按钮
            TurnControl.instance.SetStop(true);
            //切换到赢了或输了状态
            if (gdata.iWinMoney > 0)//赢了
            {
                App.GetGameData<WmarginGameData>().ZhuanState = 3;
                gMgr.LotteryJudge(); //计算
                gMgr.Theincome();//当前所赢的钱数
                GameStateUiControl.instance.ChangeToWait();//按钮全部关闭
            }

            if (gdata.iWinMoney == 0)//输了
            {
                gdata.IsAotozhuangtai = true;

                gdata.ZhuanState = 2;
                gMgr.ClearData();//清楚数据
                if (gdata.IsAuto)
                {
                    Invoke("IsAutoFun", 2f);
                    GameStateUiControl.instance.BeginButton.interactable = false;
                }
                else
                {
                    GameStateUiControl.instance.LostWait();//输了的按钮
                    GameStateUiControl.instance.TiShi.interactable = true;
                }
            }

        }

        public void IsAutoFun()
        {
            GameStateUiControl.instance.Isaudt();
            BottomUIControl.instance.BeginTurn();//向服务器发送数据

        }
        
        //显示开奖动画
        public IEnumerator ShowAwardEffect()
        {
            var gdata = App.GetGameData<WmarginGameData>();
            CheckLine();
            var lineTypes = gdata.m_LineType;
            var lineTypeCount = lineTypes.Length;
            var typeArray = gdata.m_TypeArray;
            var turnCtrl = TurnControl.instance;
            var resultImageArray = turnCtrl.resultImages;
            var secAnimaates = gdata.m_ShowSecAnimate;
            var typeImgIds = gdata.iTypeImgid;
            for (var i = 0; i < lineTypeCount; i++)
            {
                if (lineTypes[i] != 1) { continue;}
                for (var j = 0; j < 5; j++)
                {
                    var type = typeArray[i, j]; 
                    var resultImage = resultImageArray[type];
                    var animator = resultImage.GetComponent<Animator>();
                    if (gdata.m_ResultArray[i, j] == 1)
                    {
                        secAnimaates[type] = 1;
                        var aniStr = typeImgIds[type] + "_" + "0";
                        animator.enabled = true;
                        resultImage.gameObject.SetActive(true);
                        animator.Play(aniStr);
                    }
                    else
                    {
                        secAnimaates[type] = 0;
                    }
                }
                yield return new WaitForSeconds(1f);
            }

            for (var i = 0; i < 15; i++)
            {
                if (secAnimaates[i] != 1) { continue;}
                var resultImage = resultImageArray[i];
                resultImage.gameObject.SetActive(false);
                resultImage.sprite = turnCtrl.cardSprites[typeImgIds[i]];
                resultImage.GetComponent<Animator>().enabled = false;
                resultImage.gameObject.SetActive(true);
            }
            Facade.Instance<MusicManager>().Play("winsound");
            BottomUIControl.instance.SetMoney();//开始显示金钱和线数
            gdata.IsAotozhuangtai = true;
            GameStateUiControl.instance.LostWait();
            App.GetGameManager<WmarginGameManager>().ClearData();//清楚数据

            var turnItems = turnCtrl.turnItems;
            var turnItemCount = turnItems.Length;
            for (var i = 0; i < turnItemCount; i++)
            {
                var item = turnItems[i];
                item.SetActive(true);
                item.GetComponent<Image>().sprite = turnCtrl.graySprites[0];
            }
            GameStateUiControl.instance.TiShi.interactable = true;
        }
      
        public void CheckLine()
        {
            for (int j = 0; j < 9; j++)
            {
                int CountSame = 0;

                int[] tempint = new int[6];

                for (int i = 0; i < 5; i++)
                {
                    tempint[i] = App.GetGameData<WmarginGameData>().iTypeImgid[App.GetGameData<WmarginGameData>().m_TypeArray[j, i]];
                }

                tempint[5] = 100;

                for (int i = 0; i < 5; i++)
                {
                    if (tempint[i] != 0 && tempint[i + 1] != 0 && tempint[i + 1] != 100)
                    {
                        if (tempint[i] == tempint[i + 1]) CountSame++;
                        else break;
                    }
                    else if (tempint[i] == 0 && tempint[i + 1] != 0 && tempint[i + 1] != 100)
                    {
                        CountSame++;
                    }
                    else if (tempint[i] == 0 && tempint[i + 1] == 0)
                    {
                        CountSame++;
                    }
                    else if (tempint[i] != 0 && tempint[i + 1] == 0)
                    {
                        CountSame++;
                        tempint[i + 1] = tempint[i];
                    }
                }

                //5连线
                if (CountSame == 4)
                {
                    App.GetGameData<WmarginGameData>().m_LineType[j] = 1;
                    for (int i = 0; i < 5; i++)
                    {
                        App.GetGameData<WmarginGameData>().m_ResultArray[j, i] = 1;
                    }

                }

                //4连线
                if (CountSame == 3)
                {
                    App.GetGameData<WmarginGameData>().m_LineType[j] = 1;
                    for (int i = 0; i < 4; i++)
                    {
                        App.GetGameData<WmarginGameData>().m_ResultArray[j, i] = 1;
                    }
                }
                //3连线
                if (CountSame == 2)
                {
                    App.GetGameData<WmarginGameData>().m_LineType[j] = 1;
                    for (int i = 0; i < 3; i++)
                    {
                        App.GetGameData<WmarginGameData>().m_ResultArray[j, i] = 1;
                    }
                }
                CountSame = 0;
                for (int i = 1; i < 6; i++)
                {
                    tempint[i] = App.GetGameData<WmarginGameData>().iTypeImgid[App.GetGameData<WmarginGameData>().m_TypeArray[j, i - 1]];
                }
                tempint[0] = 100;
                for (int i = 5; i >= 1; i--)
                {
                    if (tempint[i] != 0 && tempint[i - 1] != 0)
                    {
                        if (tempint[i] == tempint[i - 1])
                            CountSame++;
                        else break;
                    }
                    else if (tempint[i] == 0 && tempint[i - 1] != 0)
                    {
                        CountSame++;
                    }
                    else if (tempint[i] == 0 && tempint[i - 1] == 0)
                    {
                        CountSame++;

                    }
                    else if (tempint[i] != 0 && tempint[i - 1] == 0)
                    {
                        CountSame++;
                        tempint[i - 1] = tempint[i];
                    }
                }
                //4连线
                if (CountSame == 3)
                {
                    App.GetGameData<WmarginGameData>().m_LineType[j] = 1;
                    for (int i = 4; i > 0; i--)
                    {
                        App.GetGameData<WmarginGameData>().m_ResultArray[j, i] = 1;
                    }
                }
                //3连线
                if (CountSame == 2)
                {
                    App.GetGameData<WmarginGameData>().m_LineType[j] = 1;
                    for (int i = 4; i > 1; i--)
                    {
                        App.GetGameData<WmarginGameData>().m_ResultArray[j, i] = 1;
                    }
                }

            }
            //获取对应积分
        }
    }
}
