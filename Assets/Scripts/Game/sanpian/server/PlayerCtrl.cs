using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.sanpian.DataStore;
using Assets.Scripts.Game.sanpian.item;
using Assets.Scripts.Game.sanpian.Tool;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.sanpian.server
{
    public class PlayerCtrl : MonoBehaviour
    {
        [HideInInspector]
        public UserInfo userInfo;

        [HideInInspector]
        public UIPlayerInfo UIInfo;

        public List<CardItem> CardItemList = new List<CardItem>();

        public List<CardItem> OutCardItemList = new List<CardItem>();

        [HideInInspector]
        public int position;

        public int TeamNum = 0;

        protected UIGrid grid;

        public bool myFriend = false;

        protected int startCardIndex = 0;

        public bool IsOnline = false;

        public Texture HeadTexture;

        public float IsFirstRow = -1;//第一排为-1，第二排为1

        //牌的飞行时间
        public float flyTime = 1f;

        public void Start()
        {
            grid = UIInfo.CardsArea.GetComponent<UIGrid>();
        }

        //更新头像处信息
        public void UpdateUIInfo()
        {
            UIInfo.gameObject.SetActive(true);
            UIInfo.GoldLabel.text = YxUtiles.ReduceNumber(userInfo.Gold+userInfo.PianScore);
            UIInfo.NameLabel.text = userInfo.Name;
            PortraitDb.SetPortrait(userInfo.HeadImage ,UIInfo.HeadTexture,userInfo.Sex);
            UIInfo.IsOnLineSprite.SetActive(!userInfo.IsOnline);
        }


        //开始时接牌
        public void GetCardsOnStart(CardItem card)
        {
            CardItemList.Add(card);
            card.transform.SetParent(UIInfo.CardsArea);
            Vector3 PosTemp = UIInfo.CardsArea.position;
            Vector3 ScaleTemp = Vector3.one;
            switch (position)
            {
                case 1:
                case 3:
                    ScaleTemp = Vector3.one * 0.35f;
                    PosTemp = new Vector3(PosTemp.x, PosTemp.y - startCardIndex * 0.003f, PosTemp.z);
                    startCardIndex++;
                    break;
                case 2:
                    ScaleTemp = Vector3.one * 0.5f;
                    PosTemp = new Vector3(PosTemp.x + startCardIndex * 0.03f, PosTemp.y, PosTemp.z);
                    startCardIndex++;
                    break;
                case 0:
                    card.MyCardFlag = true;
                    ScaleTemp = Vector3.one * 0.8f;
                    PosTemp = new Vector3(PosTemp.x + (startCardIndex - 13) * 0.1226f, PosTemp.y+0.106f*IsFirstRow, PosTemp.z);
                    startCardIndex++;
                    if (startCardIndex>26)
                    {
                        startCardIndex = 0;
                        IsFirstRow = 1;
                    }
                    break;
            }

            card.SetCardDepth(startCardIndex + (int)IsFirstRow*-50);
            //iTween.MoveTo(card.gameObject, PosTemp, flyTime);
            //iTween.ScaleTo(card.gameObject, ScaleTemp, flyTime);
            card.transform.position = PosTemp;
            card.transform.localScale = ScaleTemp;
        }

        public void CreateCards(int len)
        {
            for (int i = 0; i < len; i++)
            {
                CardItem m_card = (CardItem)Instantiate(App.GetGameManager<SanPianGameManager>().cardItem, new Vector3(0.002f * i - 0.1f, 0, 0), Quaternion.identity);
                m_card.transform.SetParent(UIInfo.CardsArea);
                m_card.transform.localScale = Vector3.one * 0.6f;
                m_card.SetCardDepth(i);
                m_card.SetBackKey();
                Vector3 ScaleTemp = Vector3.one;
                switch (position)
                {
                    case 1:
                    case 3:
                        ScaleTemp = Vector3.one * 0.35f;
                        startCardIndex++;
                        break;
                    case 2:
                        ScaleTemp = Vector3.one * 0.5f;
                        startCardIndex++;
                        break;
                    case 0:
                        m_card.MyCardFlag = true;
                        ScaleTemp = Vector3.one * 0.8f;
                        startCardIndex++;
                        break;
                }
                m_card.transform.localScale = ScaleTemp;
                CardItemList.Add(m_card);
                App.GetGameManager<SanPianGameManager>().XiaoHuiCardList.Add(m_card);
                grid.Reposition();
            }
            SetCardValue();
        }

        public virtual void SetCardValue(int[] cards = null)
        {
            int len = CardItemList.Count;
            if (len <= 0)
            {
                return;
            }
            CardItemList[len - 1].SetLen(len);
        }

        public virtual void Reset()
        {
            startCardIndex = 0;
            myFriend = false;
            CardItemList.Clear();
            OutCardItemList.Clear();
            IsFirstRow = -1;
        }

        //出牌減手牌
        protected CardItem[] SubCards(int[] cards)
        {
            List<int> OutList = new List<int>(cards);
            SortCardsTool.SortCards(OutList);
            int len = OutList.Count;
            CardItem[] listTemp = new CardItem[len];
            for (int i = 0; i < len; i++)
            {
                int index = CardItemList.Count - 1;
                listTemp[i] = CardItemList[index];
                listTemp[i].Value = OutList[i];
                CardItemList.RemoveAt(index);
            }
            if (CardItemList.Count > 0)
            {
                CardItemList[CardItemList.Count - 1].SetLen(CardItemList.Count);
            }
            return listTemp;
        }

        public virtual void OutCards(int[] cards)
        {
            CardItem[] outCards = SubCards(cards);
            for (int i = 0; i < outCards.Length; i++)
            {
                OutCardItemList.Add(outCards[i]);
                outCards[i].transform.SetParent(UIInfo.OutCardsArea);
                outCards[i].transform.localScale = Vector3.one * 0.5f;
                outCards[i].SetCardDepth(i);
            }
            UIInfo.OutCardsArea.GetComponent<UIGrid>().Reposition();
        }

        public bool  IsPlayerOut()
        {
            return CardItemList.Count==0;
        }


        //分组显示红6,
        public void ShowSix(int Depth)
        {
            //CardItem item = (CardItem)Instantiate(SanPianGameManager.instance.cardItem, Vector3.zero, Quaternion.identity);
            //item.transform.parent = UIInfo.OutCardsArea;
            //item.transform.localScale = Vector3.one * 0.5f;
            //item.SetCardDepth(Depth);
            //item.Value = 38;
            //UIInfo.OutCardsArea.GetComponent<UIGrid>().Reposition();
            //StartCoroutine(DisSix(item.gameObject));
        }

        IEnumerator DisSix(GameObject go)
        {
            yield return new WaitForSeconds(2f);
            Destroy(go);
        }

        public int OldCheckThree()
        {
            int len = 10;
            var cards = new int[10] {8, 8, 8, 7, 7, 7, 5, 5, 3, 3};
            Array.Sort(cards);
            if (len > 9 && len%5 == 0)
            {
                var flag = true;
                List<int> three = new List<int>();
                List<int> two = new List<int>();
                int countIndex = 0;
                for (int i = 0; i < len - 1; i++)
                {
                    countIndex++;
                    if (cards[0] == 15 || cards[0] == 17 || cards[0] == 18)
                    {
                        break;
                    }
                    if (cards[i] != cards[i + 1])
                    {
                        if (countIndex == 2)
                        {
                            two.Add(cards[i]);
                            countIndex = 0;
                        }
                        else if (countIndex == 3)
                        {
                            if (three.Count == 0 || three[three.Count - 1] + 1 == cards[i])
                            {
                                three.Add(cards[i]);
                                countIndex = 0;
                            }
                            else
                            {
                                flag = false;
                                break;
                            }

                        }
                        else
                        {
                            flag = false;
                            break;
                        }
                    }
                    else if (countIndex == 3)
                    {
                        if (three.Count == 0 || three[three.Count - 1] + 1 == cards[i])
                        {
                            three.Add(cards[i]);
                            countIndex = 0;
                        }
                        else
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (i == len - 2)
                    {
                        if (countIndex == 2)
                        {
                            three.Add(cards[i]);
                        }
                        else if (countIndex == 1)
                        {
                            two.Add(cards[i]);
                        }
                    }
                }
                if (three.Count != two.Count)
                {
                    flag = false;
                }
                if (flag && three.Count >= 2)
                {
                    Debug.LogError(80000 + three[0] * 100 + len);
                    return 80000 + three[0]*100 + len;
                }
                Debug.LogError(-1);
            }
            return -1;
        }

        public void CardsMusicPlay(int[] reqcards)
        {
            int[] cards = new int[reqcards.Length];
            Array.Copy(reqcards, cards, reqcards.Length);
            int sex = userInfo.Sex;
            int lu = 0;
            int card = 0;
            int randomNum = 0;
            //-------------------------------
            int len = cards.Length;
            Array.Sort(cards);
            var pianCount=CheckPians(cards);
            if(pianCount!=0)
            {
                lu = 9;
            }
            else if (len > 3)
            {
                for (int i = 0; i < len; i++)
                {
                    if (cards[i] != 81 && cards[i] != 97)
                    {
                        cards[i] = cards[i] % 16;
                    }
                }
                Array.Sort(cards);
                var threeNum = 0;
                var sanDaiErCount = CheckSanDaiEr(cards,out threeNum);
                if (sanDaiErCount != 0)
                {
                    if (sanDaiErCount == 1)
                    {
                        lu = 50;
                    }
                    else
                    {
                        lu = 60;
                    }
                }
                else if (cards[0] == cards[1] && cards[1] + 1 == cards[2] && cards[2] == cards[3])
                {
                    lu = 40;
                }
                else if (cards[0] + 1 == cards[1] && cards[1] + 1 == cards[2])
                {
                    lu = 30;
                }
                else
                {
                    var groupNum = 0;
                    var groupCount=CheckGroupSame(cards,out groupNum);
                    if (groupCount>0)
                    {
                        if (groupNum==3)
                        {
                            lu = 70;
                        }
                        else
                        {
                            lu = 4;
                        }
                    }
                }
            }else if (len == 3)
            {
               if (cards[0] == cards[1] && cards[1] == cards[2])
                {
                    lu = 9;
                    randomNum = UnityEngine.Random.Range(0, 2);
                }
               else
               {
                   lu = 3;
                   card = cards[0];
               }
            }
            else if (len == 2)
            {
                lu = 2;
                card = cards[0];
            }
            else if (len == 1)
            {
                lu = 1;
                card = cards[0];
            }
            if (lu == 4)
            {
                randomNum = UnityEngine.Random.Range(0, 6);
                var boom = "k-boom-" + UnityEngine.Random.Range(0, 2);
                Facade.Instance<MusicManager>().Play(boom);
            }
            if (card != 81 && card != 97)
            {
                card = card%16;
            }
            string voiceName = sex + "-" + lu + "-" + card + "-" + randomNum;
            Facade.Instance<MusicManager>().Play(voiceName);
        }

        protected int CheckSanDaiEr(int [] cards, out int san)
        {
            var pairCount = 0;
            san = 0;
            var cardCount = cards.Length;
            if (cardCount%5==0)
            {
                Dictionary<int,int> _dic=new Dictionary<int, int>();
                for (int i = 0; i < cardCount; i++)
                {
                    var item = cards[i];
                    if(_dic.ContainsKey(item))
                    {
                        _dic[item] += 1;
                    }
                    else
                    {
                        _dic.Add(item,1);
                    }
                }
                List<int> threeList=new List<int>();
                List<int> twoList=new List<int>();
                foreach (var keyPair in _dic)
                {
                    if (keyPair.Value == 3)
                    {
                        threeList.Add(keyPair.Key);
                    }
                    else if (keyPair.Value == 2)
                    {
                        twoList.Add(keyPair.Key);
                    }
                }
                if (threeList.Count==twoList.Count&& twoList.Count!=0)
                {
                    threeList.Sort();
                    var threeCount=threeList.Count;
                    var inOrder = true;
                    for (int i = 0; i < threeCount; i++)
                    {
                        if (i+1< threeCount)
                        {
                            if (threeList[i]+1!= threeList[i+1])
                            {
                                inOrder = false;
                                break;
                            }
                        }
                    }
                    if (inOrder)
                    {
                        for (int i = 0; i < threeCount; i++)
                        {
                            if (i + 1 < threeCount)
                            {
                                if (twoList[i] + 1 != twoList[i + 1])
                                {
                                    inOrder = false;
                                    break;
                                }
                            }
                        }
                        if (inOrder)
                        {
                            pairCount = threeCount;
                            san = threeList[0];
                        }
                    }
                    else
                    {
                        pairCount = 0;
                    }
                }
            }
            return pairCount;
        }

        private int CheckGroupSame(int[] cards,out int shouldCount)
        {
            var groupCount = 0;
            var same = true;
            var count = cards.Length;
            Dictionary<int, int> _dic = new Dictionary<int, int>();
            for (int i = 0; i < count; i++)
            {
                var item = cards[i];
                if (_dic.ContainsKey(item))
                {
                    _dic[item] += 1;
                }
                else
                {
                    _dic.Add(item, 1);
                }
            }
            bool numSame = true;
            var dicCount = _dic.Count;
            if (dicCount!=0)
            {
                shouldCount = count / dicCount;
            }
            else
            {
                shouldCount = 0;
            }
           
            var list = new List<int>();
            foreach (var item in _dic)
            {
                if (item.Value != shouldCount)
                {
                    numSame = false;
                    break;
                }
                else
                {
                    list.Add(item.Key);
                }
            }
            if (numSame)
            {
                bool inOrder = true;
                for (int i = 0; i < dicCount; i++)
                {
                    if (i + 1 < dicCount)
                    {
                        if (list[i] + 1 != list[i + 1])
                        {
                            inOrder = false;
                            break;
                        }
                    }
                }
                if (inOrder)
                {
                    groupCount = dicCount;
                }
            }
            return groupCount;
        }

        /// <summary>
        /// 检查片
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        private int CheckPians(int[] cards)
        {
            var groupCount = 0;
            var same = true;
            var count = cards.Length;
            if (count % 3 == 0)
            {
                Dictionary<int, int> _dic = new Dictionary<int, int>();
                for (int i = 0; i < count; i++)
                {
                    var item = cards[i];
                    if (_dic.ContainsKey(item))
                    {
                        _dic[item] += 1;
                    }
                    else
                    {
                        _dic.Add(item, 1);
                    }
                }
                bool numSame = true;
                var dicCount = _dic.Count;
                var shouldCount = 3;
                var list = new List<int>();
                foreach (var item in _dic)
                {
                    if (item.Value != shouldCount)
                    {
                        numSame = false;
                        break;
                    }
                    else
                    {
                        list.Add(item.Key);
                    }
                }
                if (numSame)
                {
                    bool inOrder = true;
                    for (int i = 0; i < dicCount; i++)
                    {
                        if (i + 1 < dicCount)
                        {
                            if (list[i] + 1 != list[i + 1])
                            {
                                inOrder = false;
                                break;
                            }
                        }
                    }
                    if (inOrder)
                    {
                        groupCount = dicCount;
                    }
                }
            }
            return groupCount;
        }

        public void TalkVoicePlay(int commonIndex)
        {
            int sex = userInfo.Sex;
            string voiceName = "talk-" + sex + "-" + commonIndex;
            Facade.Instance<MusicManager>().Play(voiceName);
        }

        public void PlayEffect(string name, int index)
        {
            int sex = userInfo.Sex;
            sex = sex == 0 ? 1 : sex == 1 ? 0 : sex;
            string voiceName = sex + "-" + name + "-" + index;
            Facade.Instance<MusicManager>().Play(voiceName);
        }

    }
}
