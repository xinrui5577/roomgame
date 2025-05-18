using System.Globalization;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.brnn
{
    public class CardsCtrl : MonoBehaviour
    {

        public GameObject[] Target;
        public GameObject[] Points;
        public GameObject ClonedCards;
        protected int Index;
        protected int Arrindex = -1;
        protected int Pontion;
        protected int Chipdepth = 2;
        protected ISFSArray Cards = new SFSArray();
        protected ISFSArray Nn = new SFSArray();
        internal bool[] Result = { true, true, false, false, true };
        protected int[] Cardv = new int[4];
        public static CardsCtrl Instance;
        public int[] Pg;

        /// <summary>
        /// 庄家每门的输赢
        /// </summary>
        internal int[] Bpg;

        public virtual void Start()
        {
            Instance = this;
        }

        protected int GiveCardsStatus;

        public void ReSetGiveCardsStatus()
        {
            GiveCardsStatus = 0;
            CancelInvoke("GiveCards");
        }

        public void BeginGiveCards(ISFSObject responseData)
        {
            var gdata = App.GetGameData<BrnnGameData>();
            Pg = new[] { 0, 0, 0, 0 };
            Bpg = new[] { 0, 0, 0, 0 };
            if (responseData.ContainsKey("total"))
            {
                gdata.GetPlayerInfo().CoinA = responseData.GetLong("total");
            }
            if (responseData.ContainsKey("pg"))
            {
                Pg = responseData.GetIntArray("pg");
            }
            if (responseData.ContainsKey("bpg"))
            {
                Bpg = responseData.GetIntArray("bpg");
            }
            gdata.ResultUserTotal = responseData.ContainsKey("win") ? responseData.GetInt("win") : 0;
            gdata.ResultBnakerTotal = responseData.ContainsKey("bwin") ? responseData.GetLong("bwin") : 0;
            Cards = responseData.GetSFSArray("cards");

            Nn = responseData.GetSFSArray("nn");
            BeginClone();
            InvokeRepeating("ReadyGiveCards", 0, 0.1f);
        }

        public virtual void GetCards(ISFSObject responseData)
        {
            
        }

        public void ReceiveResult(ISFSObject responseData)
        {
           
            if (responseData.ContainsKey("total"))
            {
                App.GameData.GetPlayerInfo().CoinA = responseData.GetLong("total");
            }
            if (responseData.ContainsKey("pg"))
            {
                Pg = new[] { 0, 0, 0, 0 };
                Pg = responseData.GetIntArray("pg");
            }
            if (responseData.ContainsKey("bpg"))
            {
                Bpg = new[] { 0, 0, 0, 0 };
                Bpg = responseData.GetIntArray("bpg");
            }
            App.GetGameData<BrnnGameData>().ResultUserTotal = responseData.ContainsKey("win") ? responseData.GetInt("win") : 0;
            App.GetGameData<BrnnGameData>().ResultBnakerTotal = responseData.ContainsKey("bwin") ? responseData.GetLong("bwin") : 0;
        }

        public void ReadyGiveCards()
        {
            if (GiveCardsStatus == 2)
            {
                OnGiveCardsOver();
                CancelInvoke("ReadyGiveCards");
            }
        }

        public void BeginClone()
        {
            if (GiveCardsStatus == 0)
            {
                GiveCardsStatus = 1;
                Index = 0;
                Arrindex = -1;
                Pontion = 0;
                Result = new bool[5];
                InvokeRepeating("GiveCards", 0.5f, 0.15f);
            }
        }

        protected UISprite XianShi;
        public virtual void ShowPoints()
        {
            var gdata = App.GetGameData<BrnnGameData>();
            Points[CardsArrindex].SetActive(true);
            Points[CardsArrindex].transform.GetChild(0).GetComponent<UILabel>().text = GetNiuName(Nn.GetSFSObject(CardsArrindex));
            if (CardsArrindex > 0)
            {
                var l = Points[CardsArrindex].transform.GetChild(1).GetComponent<UILabel>();
                l.color = Color.white;
                l.text = "无成绩";
                XianShi = Points[CardsArrindex].transform.GetChild(2).GetComponent<UISprite>();

                XianShi.spriteName = Result[CardsArrindex] ? "16" : "15";
                if (gdata.IsBanker)
                {
                    if (Bpg[CardsArrindex - 1] != 0)
                    {
                        if (Bpg[CardsArrindex - 1] > 0)
                        {
                            l.text = "+" + YxUtiles.ReduceNumber(Bpg[CardsArrindex - 1]);
                            l.color = Color.red;
                        }
                        else
                        {
                            l.text = YxUtiles.ReduceNumber(Bpg[CardsArrindex - 1]);

                            l.color = Color.green;
                        }
                    }
                }
                else
                {
                    if (Pg[CardsArrindex - 1] != 0)
                    {
                        if (Pg[CardsArrindex - 1] > 0)
                        {
                            l.text = "+" + Pg[CardsArrindex - 1];
                            l.color = Color.red;
                        }
                        else
                        {
                            l.text = Pg[CardsArrindex - 1].ToString(CultureInfo.InvariantCulture);
                            l.color = Color.green;
                        }
                    }
                }
            }
            CardsArrindex++;
            if (CardsArrindex >= 5)
            {
                var gMgr = App.GetGameManager<BrnnGameManager>();
                gMgr.ResultListCtrl.AddResult(Result);
                ReSetGiveCardsStatus();
            }
        }

        public virtual void ShowCards()
        {
            if (CardsArrindex >= 4)
            {
                CancelInvoke("ShowCards");
            }
            Facade.Instance<MusicManager>().Play("Open");
            int[] cards = Cards.GetIntArray(CardsArrindex);
            for (int i = 0; i < cards.Length; i++)
            {
                var go = CardsArr[CardsArrindex][i];
                if(go == null) continue;
                go.GetComponent<UISprite>().spriteName = "0x" + cards[i].ToString("X");
            }
            Invoke("ShowPoints", 1f);
        }

        protected int[] CalculateCards(int[] cards, int niu)
        {
            if (niu == 10) niu = 0;
            var cVal = new int[cards.Length];
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
                            return new[] { i, j };
                        }
                    }

                }
            }
            return null;
        }

        protected virtual string GetNiuName(ISFSObject responseData)
        {
            //结算时，弹出来两张牌

            int niu = responseData.GetInt("niu");
            int type = responseData.GetInt("type");
            Result[CardsArrindex] = responseData.GetBool("win");
            if (niu > 0 && type < 11)
            {
                int[] cVal = CalculateCards(Cards.GetIntArray(CardsArrindex), niu);
                Vector3 v1 = CardsArr[CardsArrindex][cVal[0]].transform.localPosition;
                v1.y = v1.y + 15;
                CardsArr[CardsArrindex][cVal[0]].transform.localPosition = v1;

                Vector3 v2 = CardsArr[CardsArrindex][cVal[1]].transform.localPosition;
                v2.y = v2.y + 15;
                CardsArr[CardsArrindex][cVal[1]].transform.localPosition = v2;
            }
            string[] daxie = { "无牛", "牛一", "牛二", "牛三", "牛四", "牛五", "牛六", "牛七", "牛八", "牛九", "牛牛" };
            string str = daxie[niu];
            if (type > 10)
            {
                switch (type)
                {
                    case 11:
                        str += "·四花牛";
                        Facade.Instance<MusicManager>().Play("bull10");
                        break;
                    case 12:
                        str += "·五花牛";
                        Facade.Instance<MusicManager>().Play("bullwuhua");
                        break;
                    case 13:
                        str += "·炸弹牛";
                        Facade.Instance<MusicManager>().Play("bullzhadan");
                        break;
                    case 14:
                        str += "·小五牛";
                        Facade.Instance<MusicManager>().Play("bullwuxiao");
                        break;
                }
            }
            else
            {
                Facade.Instance<MusicManager>().Play("bull" + niu);
            }
            return str;
        }

        protected GameObject[][] CardsArr;

        /// <summary>
        /// 牌堆索引
        /// </summary>
        protected int CardsArrindex;

        public void GiveCards()
        {
            InstantiateChip(ClonedCards);
        }

        public virtual void OnGiveCardsOver()
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
            InvokeRepeating("ShowCards", 0.5f, 1.7f);
        }
        protected virtual void InstantiateChip(GameObject go)
        {
            if (Index >= 25)
            {
                CancelInvoke("GiveCards");
                GiveCardsStatus = 2;
                return;
            }
            Facade.Instance<MusicManager>().Play("Card");
            var temp = Instantiate(go);
            temp.transform.position = go.transform.position;
            temp.transform.parent = Target[Index % 5].transform;
            temp.transform.localScale = Vector3.one;
            temp.GetComponent<UISprite>().depth = Chipdepth;
            temp.SetActive(true);
            Chipdepth++;
            var sp = temp.GetComponent<SpringPosition>();

            switch (Index % 5)
            {
                case 0:
                    Arrindex++;
                    Pontion = Pontion + 27;
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
            var v = new Vector3(Pontion, 0, 0);
            sp.target = v;
            sp.enabled = true;
            Index++;
        }

        public void ReSetPonits()
        {
            foreach (GameObject i in Points)
            {
                i.SetActive(false);
            }
        }

        public virtual void Reset()
        {
            CancelInvoke();
        }

    }
}
