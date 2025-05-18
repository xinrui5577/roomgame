using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;


namespace Assets.Scripts.Game.brnn.brnn_skin02
{
    public class CardsCtrl02 : CardsCtrl
    {
    
        public override void GetCards(ISFSObject responseData)
        {
            base.GetCards(responseData);

            if (responseData.ContainsKey("cards"))
            {
                Cards = responseData.GetSFSArray("cards");
            }
            CardsArrindex = 0;
        }


        public override void ShowPoints()
        {
            var gdata = App.GetGameData<BrnnGameData>();
            var gMgr = App.GetGameManager<BrnnGameManager>();

            Result[CardsArrindex] = Nn.GetSFSObject(CardsArrindex).GetBool("win");

            //显示牌型,用于动画显示的设置处理
            var point = Points[CardsArrindex];
            var c = point.GetComponent<CardPoint02>();
            c.ShowCardType(Nn.GetSFSObject(CardsArrindex), Cards.GetIntArray(CardsArrindex)); //显示牌型动画

            if (CardsArrindex > 0)
            {
                c.ShowScore(gdata.IsBanker ? Bpg[CardsArrindex - 1] : Pg[CardsArrindex - 1]);
            }
            point.SetActive(true);
            
            CardsArrindex++;
            if (CardsArrindex >= 5)
            {
                gMgr.ResultListCtrl.AddResult(Result);
                ReSetGiveCardsStatus();
            }
        }

        protected override string GetNiuName(ISFSObject responseData)
        {
            return string.Empty;
        }

        protected override void InstantiateChip(GameObject go)
        {

            if (Index >= 25)
            {
                CancelInvoke("GiveCards");
                GiveCardsStatus = 2;
                return;
            }
            
            var temp = Instantiate(go);
            temp.SetActive(true);
            switch (Index % 5)
            {
                case 0:
                    Arrindex++;
                    CommonObject.CardArray0[Arrindex] = temp;
                    break;
                case 1:
                    CommonObject.CardArray1[Arrindex] = temp;
                    break;
                case 2:
                    CommonObject.CardArray2[Arrindex] = temp;
                    break;
                case 3:
                    CommonObject.CardArray3[Arrindex] = temp;
                    break;
                case 4:
                    CommonObject.CardArray4[Arrindex] = temp;
                    break;
               
            }

            var target = Target[Index % 5].GetComponent<CardsPosition02>().PokerPosTrans[Arrindex];
            Facade.Instance<MusicManager>().Play("Card");
            temp.transform.position = go.transform.position;
            temp.transform.parent = target.transform;
            temp.transform.localScale = Vector3.one * 0.3f;
            if(Cards != null)
            {
                var tempSprite = temp.GetComponent<UISprite>();
                if (Cards.Count > 0)
                {
                    var cardIndex = Index / 5;

                    int cardVal = cardIndex == 3 ? 0 : Cards.GetIntArray(CardsArrindex++ % 5)[cardIndex];
                    tempSprite.depth = Chipdepth;
                    tempSprite.spriteName = "0x" + cardVal.ToString("X");
                }
            }
            Chipdepth++;


            var sp = temp.GetComponent<SpringPosition>();
            sp.target = Vector3.zero;
            sp.enabled = true;
            Index++;

            var ts = temp.GetComponent<TweenScale>();
            ts.from = Vector3.one * 0.3f;
            ts.to = Vector3.one;
            ts.duration = 0.3f;
            ts.PlayForward();
        }

        public override void ShowCards()
        {
            if (CardsArrindex >= 4)
            {
                CancelInvoke("ShowCards");
            }

            if (CardsArr[CardsArrindex].Length < 3)
                return;

            CardsArr[CardsArrindex][4].GetComponent<MoveCard02>().BeginMoveCard();
            Facade.Instance<MusicManager>().Play("Open");
            int[] cards = Cards.GetIntArray(CardsArrindex);
            for (int i = 0; i < cards.Length; i++)
            {
                var go = CardsArr[CardsArrindex][i];
                if (go == null) continue;
                go.GetComponent<UISprite>().spriteName = "0x" + cards[i].ToString("X");
            }
            Invoke("ResortCards", 1f);
            Invoke("ShowPoints", 1f);
        }

        public void ResortCards()
        {

            var info = Nn.GetSFSObject(CardsArrindex);
            var niuCards = info.GetIntArray("niuCards");

           
            int[] cards = Cards.GetIntArray(CardsArrindex);

            SortCards(cards, niuCards);

            for (int i = 0; i < cards.Length; i++)
            {
                CardsArr[CardsArrindex][i].GetComponent<UISprite>().spriteName = "0x" + cards[i].ToString("X");
            }

        }




        private void SortCards(int[] cards, int[] niuCards)
        {
            if (niuCards == null || niuCards.Length <= 0)
                return;
            int loopTime = niuCards.Length;
            for (int j = 0; j < loopTime; j++)
            {
                int niuC = niuCards[j];
                int lastIndex =  cards.Length - j - 1;
                for (int i = 0; i < lastIndex; i++)
                {
                    int p = cards[i];
                    //是王牌的情况
                    if (p == niuC || (p % 16 == niuC && NotKingCard(p)))
                    {
                        SwitchItem(cards, i, lastIndex);
                        break;
                    }
                }
            }
        }

        bool NotKingCard(int p)
        {
            return p != 0x51 && p != 0x5e && p != 0x5f && p != 0x61 && p != 0x71;
        }

        /// <summary>
        /// 数字位置更换
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index0"></param>
        /// <param name="index1"></param>
        void SwitchItem(int[] array, int index0, int index1)
        {
            int temp = array[index0];
            array[index0] = array[index1];
            array[index1] = temp;
        }

        public override void OnGiveCardsOver()
        {
            CardsArrindex = 0;
            CardsArr = new[]
            {
                CommonObject.CardArray0,
                CommonObject.CardArray1, 
                CommonObject.CardArray2,
                CommonObject.CardArray3,
                CommonObject.CardArray4
            };
            InvokeRepeating("ShowCards", 0.5f, 1.5f);
        }

        public override void Reset()
        {
            var comps = GetComponentsInChildren<CardPoint02>();
            foreach (var c in comps)
            {
                c.Reset();
            }
            CancelInvoke();
        }
    }
}