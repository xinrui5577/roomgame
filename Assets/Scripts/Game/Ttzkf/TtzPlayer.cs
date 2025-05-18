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

namespace Assets.Scripts.Game.Ttzkf
{
    public class TtzPlayer : YxBaseGamePlayer
    {
        public UILabel AnteLabel;
        public UISprite StatuSprite;
        public List<CardCtrl> Card;

        public List<int> Cards = new List<int>();
        public GameObject Target;
        public GameObject Banker;
        public List<UILabel> CardLabels;
        public bool IsShowCards = false;
        public bool IsLiang = false;
        public GameObject WinEffect;
        public ParticleSystem[] CardTypeEffects;
        public UISprite[] DianSprite;
        public GameObject Ready;
        public UISprite BgKuang;

        protected override void OnAwake()
        {
            base.OnAwake();
            ReSetGame();
        }

        public void SetBanker(bool isBank, bool Frist = true)
        {
            BgKuang.gameObject.SetActive(isBank);
            Banker.SetActive(isBank);
            if (isBank && Frist)
            {
                var temp = Banker.GetComponent<TweenPosition>();
                if (temp != null)
                {
                    temp.ResetToBeginning();
                    temp.PlayForward();
                }
            }
        }

        public void SetBankAnte(int ante)
        {
            AnteLabel.text = ante > 0 ? ante + "倍" : "不抢";
            ReSetStatus();
        }

        public void HideResult()
        {
            //AnteLabel.text = "";
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

            OnsetResult(App.GetGameData<TtzGameData>().Kind);
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
            var value = responseData.GetInt(InteractParameter.Value);
            var type = responseData.GetInt(InteractParameter.Type);
            var str = "";
            switch (type)
            {
                case 0:
                    str = "dian0";
                    break;
                case 1:
                    str = "dian" + value;
                    break;
                case 2:
                    str = "dian" + value;
                    break;
                case 3:
                    str = "erbagang";
                    break;
                case 4:
                    str = "duizi";
                    break;
                case 5:
                    str = "duizi";
                    break;
            }
            return str;
        }
        public ISFSObject UserData;

        public int FpIndex;



        protected virtual void NewFanPai()
        {
            var count = Card.Count;
            for (FpIndex = 0; FpIndex < count; FpIndex++)
            {
                var cardsprite = Card[FpIndex].GetComponent<UISprite>();
                cardsprite.spriteName = Card[FpIndex].Mahjong;
                cardsprite.MakePixelPerfect();
                Card[FpIndex].TargetSprite.spriteName = "A_" + Cards[FpIndex];
                Card[FpIndex].TargetSprite.gameObject.SetActive(true);
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
            }
        }

        protected int[] CalculateCards()
        {
            var niuData = UserData.GetSFSObject(InteractParameter.NiuData);
            var cards = UserData.GetIntArray(InteractParameter.Cards);
            if (cards == null) return null;
            var cVal = new int[cards.Length];
            var niu = niuData == null ? 0 : niuData.GetInt(InteractParameter.Value);
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

        public void SetNiuName(ISFSObject responseData)
        {
            int value = responseData.GetInt(InteractParameter.Value);
            int bei = responseData.GetInt(InteractParameter.Rate);
            int type = responseData.GetInt(InteractParameter.Type);
            // CardTypeEffect.gameObject.SetActive(value != 0);
            if (type <= 1)
            {
                StatuSprite.gameObject.SetActive(true);
                StatuSprite.spriteName = "dian_" + value;
                DianSprite[3].gameObject.SetActive(type == 1);
                if (type == 1) CardTypeEffects[0].Play();
                StatuSprite.MakePixelPerfect();
            }
            else
            {
                bool isNeedScake = false;
                string str = "";
                switch (type)
                {
                    case 2:
                        str = "dian_" + value;
                        break;
                    case 3:
                        str = "erbagang";
                        break;
                    case 4:
                        str = "duizi";
                        isNeedScake = true;
                        break;
                    case 5:
                        str = "tiandui";
                        isNeedScake = true;
                        break;
                }
                CardTypeEffects[1].Stop();
                DianSprite[0].spriteName = str;

                DianSprite[1].spriteName = "bei_" + bei;
                CardTypeEffects[1].Play();
                DianSprite[0].gameObject.SetActive(true);
                DianSprite[0].MakePixelPerfect();
                if (isNeedScake)
                    DianSprite[0].transform.localScale = Vector3.one * 0.9f;
                else
                    DianSprite[0].transform.localScale = Vector3.one;
                DianSprite[1].gameObject.SetActive(true);
                DianSprite[2].gameObject.SetActive(type == 2);
            }
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
            foreach (var effect in CardTypeEffects)
            {
                effect.Stop();
            }
            Ready.SetActive(true);
        }

        public void RemoveUser()
        {
            //            YxDebug.Log("----------------------RemoveUser");
            ReSetGame();
        }

        public void ReSetStatus()
        {
            StatuSprite.gameObject.SetActive(false);
            foreach (var sprite in DianSprite)
            {
                sprite.gameObject.SetActive(false);
            }
            foreach (var effect in CardTypeEffects)
            {
                effect.Stop();
            }
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
            BgKuang.gameObject.SetActive(true);
            BgKuang.spriteName = isBank ? "2" : "";
        }

        public void HideBankKuang()
        {
            BgKuang.gameObject.SetActive(false);
        }

        /// <summary>
        /// 重新设置房间
        /// </summary>
        public virtual void ReSetGame()
        {
            Banker.SetActive(false);
            foreach (var porker in Card)
            {
                porker.gameObject.SetActive(false);
                Destroy(porker.gameObject);
            }
            Ready.SetActive(false);
            IsLiang = false;
            IsShowCards = false;
            Card.Clear();
            WinEffect.TrySetComponentValue(false);
            Cards.Clear();
            ReSetStatus();
            HideResult();
        }


        protected override void SetWinCoin(long winCoin)
        {
            base.SetWinCoin(winCoin);
            WinEffect.TrySetComponentValue(true);
        }
    }
}
