using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jsys
{
    public class TurnGroupsManager : MonoBehaviour
    {
        //跑马灯的数组
        public Transform[] paoma;
        //当前图片的位置
        public int CurretImg;
        //当前时间
        private float _curretTimer;
        private int _nImg;

        public GameConfig GameConfig;
        //彩金的text
        public Text CaiJin;
        //彩金的面板
        public Image HandselUI;

        //动物的类型
        private int[] AnimalType =
            {
                8, 3, 3, 3, 9, 7, 7, 7, 8,//9个
                6,6,9,5,5,//5个
                8,4,4,4,9,1,1,1,8,//9个
                2,2,9,0,0//5个
            };

        public void Awake()
        {
            GameConfig = new GameConfig();
        }

        /// <summary>
        /// 显示彩金
        /// </summary>
        public void showWinning()
        {
            if (App.GetGameData<JsysGameData>().Winning == 0)
            {
                HandselUI.transform.gameObject.SetActive(false);
                return;
            }
            HandselUI.transform.gameObject.SetActive(true);
            var win = App.GetGameData<JsysGameData>().Winning;
            CaiJin.text = YxUtiles.GetShowNumberForm(win);
        }

        /// <summary>
        /// 隐藏彩金
        /// </summary>
        public void HideWinning()
        {
            HandselUI.transform.gameObject.SetActive(false);
        }
        public bool isWait = true;
        /// <summary>
        /// 游戏开始
        /// </summary>
        /// <param name="data"></param>
        public void PlayGame()
        {
            var gameMgr = App.GetGameManager<JsysGameManager>();
            var gdata = App.GetGameData<JsysGameData>();
            gdata.Judge = true;
            _addTime = 0f;
            // Debug.Log("@@@@@@@@@@@@@开始转圈!!!!!!!!!!!!!!!!!!!!");
            //判断最后位置是编号几的动物
            gdata.EndAnimal = AnimalType[gdata.EndPos];
            //Debug.Log("最后一个动物的数字" + App.GetGameData<GlobalData>().EndAnimal);
            GameConfig.MarqueeInterval = 0.01f;
            CurretImg = gdata.StarPos;
            _nImg = CurretImg;
            if (gdata.IsShark)
            {
                GameConfig.TurnTableResult = 28 * 3 + gdata.FishIdx;
            }
            if (gdata.IsShark == false)
            {
                GameConfig.TurnTableResult = 28 * 3 + gdata.EndPos;
            }
            gdata.SharkPos = AnimalType[gdata.FishIdx];
            var musicMgr = Facade.Instance<MusicManager>();
            if (gdata.IsShark && (gdata.SharkPos == 8 || gdata.SharkPos == 9))
            {
                Debug.Log("");
                gdata.EndAnimal = gdata.SharkPos;
                musicMgr.Play("Dajiang");
                gdata.Judge = false;
                ChangeState();
                paoma[CurretImg].gameObject.SetActive(false);
                isWait = false;
            }
            if (isWait)
            {
                StartCoroutine("Wait");
                paoma[CurretImg].gameObject.SetActive(false);
            }
            isWait = true;
            musicMgr.Stop();

            musicMgr.Play("Paodeng");

            if (GameConfig.IsBetPanelOnShow)
            {
                gameMgr.BetPanelMgr.HideUI();
            }
        }
        public void PlayS()
        {
            Facade.Instance<MusicManager>().Play("Animal" + App.GetGameData<JsysGameData>().SharkPos + "");
        }
        IEnumerator Wait()
        {
            yield return new WaitForSeconds(2f);
            ChangeState();
        }

        /// <summary>
        /// 改变游戏状态
        /// </summary>
        public void ChangeState()
        {
            GameConfig.TurnTableState = (int)GameConfig.GoldSharkState.Marquee;
        }

        private void HidePaoma(int curretImg)
        {
            if (curretImg >= 1)
            {
                paoma[curretImg - 1].gameObject.SetActive(false);
            }
            else if (curretImg == 0)
            {
                paoma[paoma.Length - 1].gameObject.SetActive(false);
            }
        }

        void Paoma(int curretImg)
        {
            paoma[curretImg].gameObject.SetActive(true);
            StartCoroutine("HidePaoma", curretImg);
        }

        /// <summary>
        /// 被调用的方法
        /// </summary>
        public void DiaoYong()
        {
            App.GameData.GStatus = YxEGameStatus.Normal;
            App.GetGameManager<JsysGameManager>().ResultUIMgr.GameFinish();
        }
        private float _addTime;

        private void Update()
        {
            _curretTimer += Time.deltaTime;
            if (_curretTimer > GameConfig.MarqueeInterval && GameConfig.TurnTableState == (int)GameConfig.GoldSharkState.Marquee)
            {
                CurretImg = _nImg % 28;
                if (_nImg == GameConfig.TurnTableResult)
                {
                    var gameMgr = App.GetGameManager<JsysGameManager>();
                    Paoma(CurretImg);
                    GameConfig.MarqueeInterval = 0.01f;
                    GameConfig.TurnTableResult = 0;
                    _nImg = 0;
                    _addTime = 0f;
                    GameConfig.TurnTableState = (int)GameConfig.GoldSharkState.Finish;
                    gameMgr.AnimationMgr.ShowAnimation();
                    gameMgr.ModelMgr.GotoKaiJiang();
                    Invoke("DiaoYong", 3f);
                    paoma[0].gameObject.SetActive(false);
                }
                else if (_nImg < GameConfig.TurnTableResult)
                {
                    if (GameConfig.TurnTableResult - _nImg < 28)
                    {
                        GameConfig.MarqueeInterval += _addTime;
                        _addTime += 0.001f;
                    }
                    Paoma(CurretImg);
                    _nImg++;
                    _curretTimer = 0f;
                }
            }
        }

    }

}

