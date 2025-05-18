using System;
using System.Collections;
using System.Collections.Generic;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.Tbs
{
    /// <summary>
    /// 扑克管理 单例类    
    /// </summary>
    public class PokerMgr : MonoBehaviour
    {
        /// <summary>
        /// 新牌堆
        /// </summary>
        public UIGrid NewPoker;
        /// <summary>
        /// 扑克牌prefab
        /// </summary>
        public GameObject PokerPrefab;
        /// <summary>
        /// 扑克数量
        /// </summary>
        public int PokerNum;
        /// <summary>
        /// 扑克容器
        /// </summary>
        public List<GameObject> PokerList;
        /// <summary>
        /// 分牌距离
        /// </summary>
        public int CutDis;
        /// <summary>
        /// 扑克最小的深度
        /// </summary>
        public int PokerMinDepth;
        /// <summary>
        /// 是否切过牌了
        /// </summary>
        public bool IsCut;
        /// <summary>
        /// 扑克的缩放比
        /// </summary>
        public Vector3 PokerScale;

        // Use this for initialization
        protected void Start()
        {
            PokerList = new List<GameObject>();

            for (int i = 0; i < PokerNum; i++)
            {
                GameObject gob = Instantiate(PokerPrefab);
                gob.transform.parent = NewPoker.transform;
                gob.transform.localScale = PokerScale;
                PokerList.Add(gob);
                gob.transform.name = i + "_poker";
                SetPokerDepth(gob, PokerMinDepth + i);
            }

            NewPoker.repositionNow = true;

            ClickOpenEvent = new EventDelegate(AutoOpen);
        }

        /// <summary>
        /// 右侧牌堆
        /// </summary>
        public GameObject CutLeft;
        /// <summary>
        /// 左侧牌堆
        /// </summary>
        public List<GameObject> LeftList;
        /// <summary>
        /// 右侧牌堆
        /// </summary>
        public GameObject CutRight;
        /// <summary>
        /// 右侧牌堆
        /// </summary>
        public List<GameObject> RightList;

        /// <summary>
        /// 发送切牌交互
        /// </summary>
        public void SendBeginCut()
        {
            App.GetRServer<TbsRemoteController>().SendRequest(GameRequestType.CutPoker, null);
        }

        /// <summary>
        /// 切牌
        /// </summary>
        public void BeginCut(bool showProcess = true, int showPokerNum = 52)
        {
            if (IsCut)
            {
                return;
            }

            IsCut = true;

            if (LeftList == null)
                LeftList = new List<GameObject>();
            else
                LeftList.Clear();

            if (RightList == null)
                RightList = new List<GameObject>();
            else
                RightList.Clear();


            CutRight.SetActive(true);
            CutLeft.SetActive(true);
            var index = PokerNum >> 1;

            for (int i = 0; i < PokerNum; i++)
            {
                if (i % index == index / 2)
                {
                    if (i >= index)
                        CutRight.transform.localPosition = PokerList[i].transform.localPosition;
                    else
                        CutLeft.transform.localPosition = PokerList[i].transform.localPosition;
                }
            }
            var showN = showPokerNum / 2;
            for (int i = 0; i < PokerNum; i++)
            {
                if (i % 26 >= showN)
                {
                    PokerList[i].SetActive(false);
                    PokerList[i].transform.parent = App.GetGameManager<TbsGameManager>().transform;
                    continue;
                }

                if (i >= index)
                    RightList.Add(PokerList[i]);
                else
                    LeftList.Add(PokerList[i]);

                PokerList[i].transform.parent = i >= index ? CutRight.transform : CutLeft.transform;
            }

            if (showProcess)
            {
                CutRight.GetComponent<TweenPosition>().from = CutRight.transform.localPosition;
                CutLeft.GetComponent<TweenPosition>().from = CutLeft.transform.localPosition;

                CutRight.GetComponent<TweenPosition>().to = new Vector3(CutDis, 0f, 0f);
                CutLeft.GetComponent<TweenPosition>().to = new Vector3(-CutDis, 0f, 0f);

                CutRight.GetComponent<TweenPosition>().ResetToBeginning();
                CutLeft.GetComponent<TweenPosition>().ResetToBeginning();

                CutRight.GetComponent<TweenPosition>().PlayForward();
                CutLeft.GetComponent<TweenPosition>().PlayForward();
            }
            else
            {
                CutRight.transform.localPosition = new Vector3(CutDis, 0f, 0f);
                CutLeft.transform.localPosition = new Vector3(-CutDis, 0f, 0f);
            }
        }
        /// <summary>
        /// 发哪边的牌
        /// </summary>
        private bool _leftOrRight;
        /// <summary>
        /// 发牌方法
        /// </summary>
        public GameObject Deal(GameObject traget, float delayed = 0f, int pokerV = 0)
        {
            List<GameObject> pokerList = _leftOrRight ? LeftList : RightList;

            if (pokerList.Count <= 0)
            {
                YxDebug.Log("牌堆没牌了!!");
                return null;
            }
            GameObject gob = pokerList[pokerList.Count - 1];
            gob.SetActive(true);
            pokerList.RemoveAt(pokerList.Count - 1);

            gob.GetComponent<TweenTransform>().from = gob.transform;
            gob.GetComponent<TweenTransform>().to = traget.transform;

            gob.GetComponent<TweenTransform>().delay = delayed;

            ShowPokerV(gob, "0x" + pokerV.ToString("X"));

            gob.GetComponent<TweenTransform>().RemoveOnFinished(ClickOpenEvent);
            gob.GetComponent<TweenTransform>().AddOnFinished(ClickOpenEvent);
            gob.GetComponent<TweenTransform>().ResetToBeginning();
            gob.GetComponent<TweenTransform>().PlayForward();

            YxDebug.Log("tween走了");

            _leftOrRight = !_leftOrRight;

            return gob;
        }

        /// <summary>
        /// 点击翻牌代理
        /// </summary>
        public EventDelegate ClickOpenEvent;
        /// <summary>
        /// 自动翻牌
        /// </summary>
        public void AutoOpen()
        {

            for (int i = 0; i < App.GetGameData<TbsGameData>().PlayerList.Length; i++)
            {
                App.GetGameData<TbsGameData>().GetPlayer<TbsPlayer>(i, true).UserBetPoker.OnDealFinish();
            }
        }

        /// <summary>
        /// 重置扑克类
        /// </summary>
        public IEnumerator Reset(float ftime, Action<bool, int> action)
        {
            IsCut = false;
            _leftOrRight = false;

            CutLeft.SetActive(false);
            CutRight.SetActive(false);
            for (int i = 0; i < PokerList.Count; i++)
            {
                var o = PokerList[i];
                o.transform.parent = NewPoker.transform;
                o.transform.localScale = Vector3.one;
                o.transform.localRotation = Quaternion.Euler(Vector3.zero);
                o.SetActive(true);

                HidePokerV(o);
                SetPokerDepth(o, PokerMinDepth + i);
            }

            NewPoker.repositionNow = true;

            yield return new WaitForSeconds(ftime);

            action(true, 52);
        }
        /// <summary>
        /// 隐藏手牌
        /// </summary>
        public void HideHandPoker()
        {
            var gdata = App.GetGameData<TbsGameData>();
            for (int i = 0; i < gdata.PlayerList.Length; i++)
            {
                if (gdata.GetPlayer<TbsPlayer>(i, true).UserBetPoker.LeftPoker != null)
                {
                    gdata.GetPlayer<TbsPlayer>(i, true).UserBetPoker.LeftPoker.SetActive(false);
                    gdata.GetPlayer<TbsPlayer>(i, true).UserBetPoker.LeftPoker = null;
                }

                if (gdata.GetPlayer<TbsPlayer>(i, true).UserBetPoker.RightPoker == null) continue;
                gdata.GetPlayer<TbsPlayer>(i, true).UserBetPoker.RightPoker.SetActive(false);
                gdata.GetPlayer<TbsPlayer>(i, true).UserBetPoker.RightPoker = null;
            }
        }

        /// <summary>
        /// 显示牌值
        /// </summary>
        public void ShowPokerV(GameObject poker, string v)
        {
            poker.transform.FindChild("Sprite").GetComponent<UISprite>().spriteName = v;
        }
        /// <summary>
        /// 隐藏牌值
        /// </summary>
        public void HidePokerV(GameObject poker)
        {
            poker.transform.FindChild("Sprite").GetComponent<UISprite>().spriteName = "0x0";
        }
        /// <summary>
        /// 设置扑克层级
        /// </summary>
        public void SetPokerDepth(GameObject poker, int depth)
        {
            poker.transform.FindChild("Sprite").GetComponent<UISprite>().depth = depth * 2;
            poker.transform.FindChild("bg").GetComponent<UISprite>().depth = depth * 2 - 1;
        }
        /// <summary>
        /// 关闭扑克的所有tween
        /// </summary>
        /// <param name="poker"></param>
        public void CloseAllTween(GameObject poker)
        {
            poker.GetComponent<TweenTransform>().enabled = false;
            poker.GetComponent<TweenPosition>().enabled = false;
            poker.GetComponent<TweenRotation>().enabled = false;
        }
    }
}
