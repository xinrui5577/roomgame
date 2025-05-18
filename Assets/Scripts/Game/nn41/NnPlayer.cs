using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Framework;
using YxFramwork.Tool;
using Random = System.Random;

namespace Assets.Scripts.Game.nn41
{
    public class NnPlayer : YxBaseGamePlayer
    {
        public UILabel AnteLabel;
        public UISprite StatuSprite;
        public List<GameObject> Porkers;

        public List<int> Cards = new List<int>();
        public GameObject Target;
        public GameObject Banker;
        public List<UILabel> CardLabels;
        public bool IsShowCards = false;
        public bool IsLiang = false;
        public GameObject WinEffect;
        public GameObject CardTypeEffect;


        private UISprite _bgKuang;
        protected List<int> Two = new List<int>();
        protected override void OnAwake()
        {
            base.OnAwake();
            _bgKuang = transform.FindChild("Kuang").GetComponent<UISprite>();
            _bgKuang.gameObject.SetActive(false); ReSetGame();
        }

        public void SetBanker(bool isBank)
        {
            _bgKuang.gameObject.SetActive(isBank);
            Banker.SetActive(isBank);
        }

        public void SetBankAnte(int ante)
        {
            AnteLabel.text = ante > 0 ? ante + "倍" : "不抢";
            ReSetStatus();
        }

        public void HideResult()
        {
            AnteLabel.text = "";
            var gr = transform.FindChild("GameResult").GetComponent<GameResult>();
            gr.Hide();
        }
        public virtual void ShowResult(ISFSObject data)
        {
            var gr = transform.FindChild("GameResult").GetComponent<GameResult>();
            gr.SetGameResult(data);
        }


        public void SetResult(ISFSObject userData)
        {
            Debug.Log("SetResult:" + IsShowCards + "---" + enabled);
            if (IsShowCards || !enabled)
            {
                return;
            }
            IsShowCards = true;
            UserData = userData;
            if (userData.ContainsKey(RequestKey.KeyCards))
            {
                SetCardsValue(userData.GetIntArray(RequestKey.KeyCards));
            }

            OnsetResult(App.GetGameData<NnGameData>().Kind);
        }

        /// <summary>
        /// 设置纸牌值
        /// </summary>
        public void SetCardsValue(int[] cardsValue)
        {
            Cards.Clear();
            var count = cardsValue.Length;
            for (var i = 0; i < count; i++)
            {
                Cards.Add(cardsValue[i]);
            }
        }

        public virtual void OnsetResult(int kind)
        {
            if (gameObject.activeSelf)
            {
                NewFanPai();
            }
        }

        protected string GetAudio(ISFSObject responseData)
        {
            var niu = responseData.GetInt(InteractParameter.Niu);
            var type = responseData.GetInt(InteractParameter.Type);
            var str = "n" + niu;
            if (type > 10)
            {
                str = "n" + type;
            }
            switch (type)
            {
                case 11:
                    str = "nszn";
                    break;
                case 12:
                    str = "nshn";
                    break;
                case 13:
                    str = "nwhn";
                    break;
                case 14:
                    str = "nthn";
                    break;
                case 15:
                    str = "nhln";
                    break;
                case 16:
                    str = "nzdn";
                    break;
                case 17:
                    str = "nwxn";
                    break;
                case 18:
                    str = "nkln";
                    break;
            }
            return str;
        }
        public ISFSObject UserData;

        public int FpIndex;

        protected virtual void ShowCard()
        {
            Two.Clear();
            var cVal = CalculateCards();
            var v1 = new Vector3(0, 0, 0);
            var v2 = new Vector3(110, 0, 0);
            var list = new List<int> { 0, 1, 2, 3, 4 };
            var backDepth = 4;
            var frontDepth = 1;
            if (cVal != null)
            {
                for (int i = 0; i < cVal.Length; i++)
                {
                    for (int j = 0; j < Porkers.Count; j++)
                    {
                        if (Porkers[j].GetComponent<UISprite>().spriteName != "0x" + cVal[i].ToString("X")) continue;
                        Two.Add(j);
                    }
                }
                foreach (int t in Two)
                {
                    list.Remove(t);

                    Porkers[t].transform.localPosition = v2;
                    Porkers[t].GetComponent<UISprite>().depth = backDepth++;

                    var tempSposition = Porkers[t].GetComponent<SpringPosition>();
                    tempSposition.target = v2;
                    tempSposition.enabled = true;
                    v2.x += 25;
                }
                foreach (int t in list)
                {
                    Porkers[t].transform.localPosition = v1;
                    Porkers[t].GetComponent<UISprite>().depth = frontDepth++;

                    var tempSposition = Porkers[t].GetComponent<SpringPosition>();
                    tempSposition.target = v1;
                    tempSposition.enabled = true;

                    v1.x += 25;
                }
            }
            else
            {
                foreach (GameObject t in Porkers)
                {
                    t.transform.localPosition = v1;
                    v1.x += 25;
                }
            }

        }
        protected virtual void NewFanPai()
        {
            var count = Porkers.Count;
            for (FpIndex = 0; FpIndex < count; FpIndex++)
            {
                Porkers[FpIndex].GetComponent<UISprite>().spriteName = "0x" + Cards[FpIndex].ToString("X");
            }

            ShowCard();
            var sfsObject = UserData.GetSFSObject(InteractParameter.NiuData);
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
        /// <summary>
        /// 暂时留着 防止以后用到
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator FanPai()
        {
            var count = Porkers.Count;
            Debug.Log("扑克：" + count + "         值：" + Cards.Count);
            var wait = new WaitForSeconds(0.5f);
            yield return wait;
            //亮最后一张
            Porkers[count - 1].GetComponent<UISprite>().spriteName = "0x" + Cards[count - 1].ToString("X");
            var cVal = CalculateCards();
            if (cVal != null)
            {
                for (FpIndex = 0; FpIndex < cVal.Length; FpIndex++)
                {
                    foreach (var t in Porkers)
                    {
                        if (t.GetComponent<UISprite>().spriteName != "0x" + cVal[FpIndex].ToString("X")) continue;
                        Vector3 v = t.transform.localPosition;
                        v.y += 20;
                        t.transform.localPosition = v;
                    }
                }
            }
            var sfsObject = UserData.GetSFSObject(InteractParameter.NiuData);
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

        protected virtual void OnShowNiu(string str)
        {
        }

        protected int[] CalculateCards()
        {
            var niuData = UserData.GetSFSObject(InteractParameter.NiuData);
            var cards = UserData.GetIntArray(InteractParameter.Cards);
            if (cards == null) return null;
            var cVal = new int[cards.Length];
            var niu = niuData == null ? 0 : niuData.GetInt(InteractParameter.Niu);
            if (niuData != null)
            {
                var type = niuData.GetInt(InteractParameter.Type);
                if (type > 10)
                {
                    if (type == 16)
                    {
                        var myDic = new Dictionary<int, int>();
                        foreach (int t in cards)
                        {
                            if (myDic.Keys.Contains(t % 16))
                            {
                                myDic[t % 16]++;
                            }
                            else
                            {
                                myDic[t % 16] = 1;
                            }
                        }
                        var numArr = myDic.Where(t => t.Value == 1).ToDictionary(t => t.Key, t => t.Value).Keys.ToArray();
                        return cards.Where(t => numArr[0] != t % 16).ToArray();
                    }
                    return cards;
                }
            }
            if (niu > 0)
            {
                if (niu == 10) niu = 0;
                for (int i = 0; i < cards.Length; i++)
                {
                    cVal[i] = cards[i] % 16 > 10 ? 10 : cards[i] % 16;
                }
                for (int i = 0; i < cVal.Length; i++)
                {
                    for (int j = 0; j < cVal.Length; j++)
                    {
                        if (i != j)
                        {
                            if ((cVal[i] + cVal[j]) % 10 == niu)
                            {
                                return new[] { cards[i], cards[j] };
                            }
                        }
                    }
                }
            }
            return null;
        }

        protected string GetNiuName(ISFSObject responseData)
        {
            int niu = responseData.GetInt(InteractParameter.Niu);
            int type = responseData.GetInt(InteractParameter.Type);
            CardTypeEffect.gameObject.SetActive(niu != 0);

            string str = "n" + niu;
            switch (type)
            {
                case 11:
                    str = "nszn";
                    break;
                case 12:
                    str = "nshn";
                    break;
                case 13:
                    str = "nwhn";
                    break;
                case 14:
                    str = "nthn";
                    break;
                case 15:
                    str = "nhln";
                    break;
                case 16:
                    str = "nzdn";
                    break;
                case 17:
                    str = "nwxn";
                    break;
                case 18:
                    str = "nkln";
                    break;
            }
            return str;
        }

        public void SetNiuName(ISFSObject responseData)
        {
            AnteLabel.text = "";
            StatuSprite.spriteName = GetNiuName(responseData);
            StatuSprite.gameObject.SetActive(true);
            StatuSprite.MakePixelPerfect();
        }

        /// <summary>
        /// 隐藏StatuSprite
        /// </summary>
        public void HideStatuSprite()
        {
            StatuSprite.gameObject.SetActive(false);
        }

        /// <summary>
        /// 准备
        /// </summary>
        public void SetReady()
        {
            AnteLabel.text = "";
            StatuSprite.gameObject.SetActive(true);
            CardTypeEffect.gameObject.SetActive(false);
            StatuSprite.spriteName = "zb";
            StatuSprite.MakePixelPerfect();
        }

        public void RemoveUser()
        {
            //            YxDebug.Log("----------------------RemoveUser");
            ReSetGame();
        }

        public void ReSetStatus()
        {
            StatuSprite.gameObject.SetActive(false);
        }

        /// <summary>
        /// 注
        /// </summary>
        /// <param name="gold">金额：小的为倍数.大的为金币数</param>
        public void SetUserAnte(int gold)
        {
            AnteLabel.text = YxUtiles.GetShowNumberForm(gold);
            AnteLabel.gameObject.SetActive(true);
            StatuSprite.gameObject.SetActive(false);
        }

        public void SetBankKuang(bool isBank)
        {
            if (isBank) Facade.Instance<MusicManager>().Play(GameSounds.SetBanker);
            _bgKuang.gameObject.SetActive(true);
            _bgKuang.spriteName = isBank ? "2" : "";
        }

        public void HideBankKuang()
        {
            _bgKuang.gameObject.SetActive(false);
        }

        /// <summary>
        /// 重新设置房间
        /// </summary>
        public virtual void ReSetGame()
        {
            Banker.SetActive(false);
            foreach (var porker in Porkers)
            {
                porker.SetActive(false);
                Destroy(porker);
            }
            IsLiang = false;
            IsShowCards = false;
            Porkers.Clear();
            WinEffect.TrySetComponentValue(false);
            Cards.Clear();
            ReSetStatus();
            HideResult();
        }

        /// <summary>
        ///翻开最后一张牌的时候的显示
        /// </summary>
        public virtual void ShowCard41()
        {

        }

        protected override void SetWinCoin(long winCoin)
        {
            base.SetWinCoin(winCoin);
            WinEffect.TrySetComponentValue(true);

        }
    }
}
