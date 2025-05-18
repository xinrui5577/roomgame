using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Random = System.Random;

namespace Assets.Scripts.Game.nn41
{
    public class NnPlayerSelf : NnPlayer
    {
        public GameObject Calculate;
        public GameObject OpenCard;
        public GameObject BoxClider;
        public List<Vector3> StoreOriginalPos = new List<Vector3>();
        public EventDelegate Del;
        public EventDelegate Dell;
        public List<UITexture> CardsList;

        private List<int> _pokersPos;
        protected bool IsOpenCard;
        private bool _isComplete;


        /// <summary>
        /// 设置结算的时候牌的显示（如果kind=0五张牌的搓牌,kind=1正常的看牌,kind=2 4张牌的搓牌）
        /// </summary>
        /// <param name="kind"></param>
        public override void OnsetResult(int kind)
        {
            var gmanager = App.GetGameManager<NnGameManager>();
            var gdata = App.GetGameData<NnGameData>();

            gmanager.Tip.T("正在等待玩家翻牌");
            switch (kind)
            {
                case 0:
                    if (!IsLiang)
                    {
                        BoxClider.SetActive(true);
                        ExcuteRubCard();
                        IsShowCards = true;
                    }
                    else
                    {
                        NewFanPai();
                        if (gmanager.BlackBg.activeSelf)
                        {
                            gmanager.BlackBg.SetActive(false);
                        }
                        foreach (var t in CardsList)
                        {
                            if (t.gameObject.activeSelf)
                            {
                                t.gameObject.SetActive(false);
                            }
                        }
                    }
                    break;
                case 1:
                    if (!IsLiang)
                    {
                        gmanager.TurnCard.CardValue("0x" + gdata.NewCard.ToString("X"));
                        GameObject.Find("UI Root/Camera/CuopaiPanel/bg").gameObject.SetActive(true);
                    }
                    else
                    {
                        NewFanPai();
                    }
                    break;
                case 2:
                    NewFanPai();
                    break;
                default:
                    NewFanPai();
                    if (gmanager.BlackBg.activeSelf)
                    {
                        gmanager.BlackBg.SetActive(false);
                    }
                    foreach (var t in CardsList)
                    {
                        if (t.gameObject.activeSelf)
                        {
                            t.gameObject.SetActive(false);
                        }
                    }
                    ShowCard();
                    NiuNumShow();
                    break;
            }

            if (gdata.IsEndCardFlop)
            {
                ShowLiangPai(!IsShowCards);
            }
            else
            {
                ShowLiangPai(false);
            }
        }
        /// <summary>
        /// 提示和亮牌按钮的显示
        /// </summary>
        /// <param name="isShow"></param>
        private void ShowLiangPai(bool isShow)
        {
            if (isShow) IsLiang = true;
            if (Calculate != null)
            {
                Calculate.SetActive(isShow);
            }

            if (OpenCard != null)
            {
                OpenCard.SetActive(isShow);
            }
        }

        /// <summary>
        ///翻开最后一张牌的时候的显示
        /// </summary>
        public override void ShowCard41()
        {
            NewFanPai();
        }
        /// <summary>
        /// 翻牌
        /// </summary>
        /// <returns></returns>
        protected override void NewFanPai()
        {
            var gdata = App.GetGameData<NnGameData>();

            IsOpenCard = true;
            Two.Clear();
            _pokersPos = new List<int> { 0, 1, 2, 3, 4 };
            var count = Porkers.Count;
            switch (App.GameKey)
            {
                case "nn41":
                case "nntp":
                    if (gdata.NewCard == 0)
                    {
                        Porkers[count - 1].GetComponent<UISprite>().spriteName = "0x" + Cards[count - 1].ToString("X");
                    }
                    else
                    {
                        Porkers[count - 1].GetComponent<UISprite>().spriteName = "0x" + gdata.NewCard.ToString("X");

                    }
                    break;
                default:
                    for (int i = 0; i < count; i++)
                    {
                        Porkers[i].SetActive(true);
                        Porkers[i].GetComponent<UISprite>().spriteName = "0x" + Cards[i].ToString("X");
                    }
                    break;
            }
            var cVal = CalculateCards();
            if (cVal != null && !_isComplete)
            {
                for (var i = 0; i < cVal.Length; i++)
                {
                    for (var j = 0; j < Porkers.Count; j++)
                    {
                        if (Porkers[j].GetComponent<UISprite>().spriteName != "0x" + cVal[i].ToString("X")) continue;
                        Two.Add(j);
                    }
                }
                foreach (var t in Two)
                {
                    _pokersPos.Remove(t);
                }
            }

            if (!gdata.IsEndCardFlop)
            {
                OnOpenCardBtn();
            }
        }


        /// <summary>
        /// 点击提示按钮
        /// </summary>
        public void OnReminderBtn()
        {
            if (Two.Count > 0 && !_isComplete)
            {
                foreach (var t in _pokersPos)
                {
                    var v = Porkers[t].transform.localPosition;
                    v.y += 20;
                    Porkers[t].transform.localPosition = v;
                }
                _isComplete = true;
            }
            NiuNumShow();
        }

        private void NiuNumShow()
        {
            var sfsObject = UserData.GetSFSObject("niuData");
            if (sfsObject == null)
            {
                StatuSprite.gameObject.SetActive(false);
            }
            else
            {
                SetNiuName(sfsObject);
                var str = GetAudio(sfsObject);
                var rand = new Random();
                int sex = rand.Next(0, 2);

                string source = "";

                if (sex == 0)
                {
                    source = "woman";
                }
                else if (sex == 1)
                {
                    source = "man";
                }
                Facade.Instance<MusicManager>().Play(str, source);
                OnShowNiu(str);
            }
        }
        protected override void ShowCard()
        {
            var v1 = new Vector3(180, 0, 0);
            var v2 = new Vector3(300, 0, 0);
            var backDepth = 4;
            var frontDepth = 1;
            if (CalculateCards() != null)
            {
                foreach (var t in Two)
                {
                    Porkers[t].transform.localPosition = v2;
                    Porkers[t].GetComponent<UISprite>().depth = backDepth++;
                    v2.x += 30;
                }
                foreach (var t in _pokersPos)
                {
                    Porkers[t].transform.localPosition = v1;
                    Porkers[t].GetComponent<UISprite>().depth = frontDepth++;
                    v1.x += 30;
                }
            }
            else
            {
                foreach (var t in Porkers)
                {
                    t.transform.localPosition = v1;
                    v1.x += 30;
                }
            }

        }

        /// <summary>
        /// 点击亮牌按钮
        /// </summary>
        public void OnOpenCardBtn()
        {
            ShowLiangPai(false);
            ShowCard();
            NiuNumShow();
            App.GetRServer<NnGameServer>().SendLiang();
            Two.Clear();
        }

        public override void ShowResult(ISFSObject data)
        {
            base.ShowResult(data);
            ShowLiangPai(false);
            IsOpenCard = false;
            _isComplete = false;
            Facade.Instance<MusicManager>().Play(data.GetInt("gold") >= 0 ? "win" : "lost");
        }
        /// <summary>
        /// 翻牌
        /// </summary>
        /// <returns></returns>
        protected override IEnumerator FanPai()
        {
            var gmanager = App.GetGameManager<NnGameManager>();

            if (gmanager.BlackBg.activeSelf)
            {
                gmanager.BlackBg.SetActive(false);
            }

            if (Porkers.Count > 5 || Cards.Count > 5)
            {
                yield return null;
            }
            NewFanPai();
            OpenCard.transform.localPosition = new Vector3(1134, -500, 0);
            Calculate.transform.localPosition = new Vector3(947, -500, 0);
            gmanager.Target[0].transform.localPosition = new Vector3(329, -500, 0);

            var wait = new WaitForSeconds(0.5f);
            yield return wait;
            iTween.MoveTo(OpenCard, iTween.Hash("position", new Vector3(1134, 9, 0), "time", 0.5f, "islocal", true));
            iTween.MoveTo(Calculate, iTween.Hash("position", new Vector3(947, 9, 0), "time", 0.5f, "islocal", true));
            iTween.MoveTo(gmanager.Target[0], iTween.Hash("position", new Vector3(329, 9, 0), "time", 0.5f, "islocal", true));

            IsShowCards = false;
            ShowLiangPai(!IsShowCards);
        }
        /// <summary>
        /// 显示牛数
        /// </summary>
        /// <param name="str"></param>
        protected override void OnShowNiu(string str)
        {
            var gmanager = App.GetGameManager<NnGameManager>();

            if (str == "n10" || str == "nwhn" || str == "nzdn" || str == "nwxn" || str == "nszn" || str == "nthn" || str == "nhln" || str == "nkln")
            {
                gmanager.Ecffect.SetWinEffect();
            }
            if (str == "n0")
            {
                gmanager.Ecffect.SetLostEffect();
            }
        }
        /// <summary>
        /// 游戏重置
        /// </summary>
        public override void ReSetGame()
        {
            base.ReSetGame();
            UpCards.Clear();
        }

        protected List<GameObject> UpCards = new List<GameObject>();

        /// <summary>
        /// 调用错牌事件
        /// </summary>
        public void ExcuteRubCard()
        {
            StartCoroutine(RubCard());
        }
        /// <summary>
        /// 点击搓牌之后的显示
        /// </summary>
        public IEnumerator RubCard()
        {
            Del = new EventDelegate(Move);
            Dell = new EventDelegate(StopMove);
            var count = Porkers.Count;
            var wait = new WaitForSeconds(0.5f);
            yield return wait;
            FpIndex = 0;
            var gmanager = App.GetGameManager<NnGameManager>();

            for (FpIndex = 0; FpIndex < count; FpIndex++)
            {
                var bundleName = string.Format("pai/{0}", "0x" + Cards[FpIndex].ToString("X"));
                CardsList[FpIndex].GetComponent<UITexture>().mainTexture = Instantiate(ResourceManager.LoadAsset<Texture>(App.GameKey, bundleName, "0x" + Cards[FpIndex].ToString("X")));
            }
            for (FpIndex = 0; FpIndex < count; FpIndex++)
            {
                Porkers[FpIndex].SetActive(false);
                var card = CardsList[FpIndex].transform;
                card.gameObject.SetActive(true);
                card.parent = gmanager.BlackBg.transform;
                card.localPosition = new Vector3(-226, -372, 0);
                card.localEulerAngles = Vector3.zero;
                card.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                CardsList[FpIndex].GetComponent<UITexture>().depth = 45 - FpIndex;
            }
            yield return wait;
            for (FpIndex = count - 1; FpIndex > 0; FpIndex--)
            {
                iTween.RotateBy(CardsList[FpIndex].gameObject, iTween.Hash("z", .0047 * FpIndex, "easeType", "easeInOutBack", "loopType", "none", "delay", .2));
                StoreOriginalPos.Add(CardsList[FpIndex].transform.localPosition);
            }
            FpIndex = 0;
            CardsList[FpIndex].gameObject.AddComponent<UIEventTrigger>().onPress.Add(Del);
            CardsList[FpIndex].gameObject.GetComponent<UIEventTrigger>().onRelease.Add(Dell);
            yield return wait;
            yield return wait;
            yield return wait;
            BoxClider.SetActive(false);
            IsShowCards = false;
        }
        /// <summary>
        /// 移动之后
        /// </summary>
        /// <returns></returns>
        private IEnumerator MoveBack()
        {
            var v = CardsList[2].gameObject.transform.localPosition;
            var count = Porkers.Count;
            var wait = new WaitForSeconds(0.5f);
            yield return wait;
            FpIndex = 0;
            for (FpIndex = 0; FpIndex < count; FpIndex++)
            {
                iTween.MoveTo(CardsList[FpIndex].gameObject, iTween.Hash("position", v, "time", 0.1f * FpIndex + 0.1f, "islocal", true, "delay", .2));
            }
            yield return wait;
            yield return wait;
            for (FpIndex = 0; FpIndex < count; FpIndex++)
            {
                CardsList[FpIndex].gameObject.SetActive(false);
            }
            StartCoroutine(FanPai());

        }
        /// <summary>
        /// 鼠标压上之后
        /// </summary>
        public void Move()
        {
            if (CardsList[FpIndex].GetComponent<UIDragObject>() == null)
            {
                CardsList[FpIndex].gameObject.AddComponent<UIDragObject>().target = CardsList[FpIndex].transform;
                CardsList[FpIndex].gameObject.transform.localEulerAngles = Vector3.zero;
            }
            else
            {
                CardsList[FpIndex].GetComponent<UIDragObject>().target = CardsList[FpIndex].transform;
                CardsList[FpIndex].gameObject.transform.localEulerAngles = Vector3.zero;
            }
        }
        /// <summary>
        /// 按钮释放之后
        /// </summary>
        public void StopMove()
        {
            var dis = Vector3.Distance(CardsList[FpIndex].transform.localPosition, StoreOriginalPos[FpIndex]);
            if (dis > 10)
            {
                if (FpIndex == 3)
                {
                    CardsList[4].transform.localEulerAngles = Vector3.zero;
                    StartCoroutine(MoveBack());
                    return;
                }
                Destroy(CardsList[FpIndex].GetComponent<UIDragObject>());
                Destroy(CardsList[FpIndex].GetComponent<UIEventTrigger>());
                FpIndex++;
                CardsList[FpIndex].gameObject.AddComponent<UIEventTrigger>().onPress.Add(Del);
                CardsList[FpIndex].gameObject.GetComponent<UIEventTrigger>().onRelease.Add(Dell);
            }
        }
    }
}
