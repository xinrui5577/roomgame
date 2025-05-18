using UnityEngine;
using YxFramwork.Common;
using Sfs2X.Entities.Data;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;


namespace Assets.Scripts.Game.brnn.brnn_skin01
{
    public class CardCtrl01 : CardsCtrl
    {
        public override void ShowPoints()
        {
            Points[CardsArrindex].SetActive(true);
            var spr = Points[CardsArrindex].transform.GetChild(0).GetComponent<UISprite>();
            spr.spriteName = GetNiuName(Nn.GetSFSObject(CardsArrindex));
            spr.MakePixelPerfect();

            if (CardsArrindex > 0)
            {
                var l = Points[CardsArrindex].transform.GetChild(1).GetComponent<UILabel>();
                l.color = Color.white;
                l.text = "无成绩";
                XianShi = Points[CardsArrindex].transform.GetChild(2).GetComponent<UISprite>();
                XianShi.spriteName = Result[CardsArrindex] ? "16" : "15";
                if (App.GetGameData<BrnnGameData>().IsBanker)
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
                            l.text = "+" + YxUtiles.ReduceNumber(Pg[CardsArrindex - 1]);
                            l.color = Color.red;
                        }
                        else
                        {
                            l.text = YxUtiles.ReduceNumber(Pg[CardsArrindex - 1]);
                            l.color = Color.green;
                        }
                    }
                }
            }
            CardsArrindex++;
            if (CardsArrindex >= 5)
            {
                App.GetGameManager<BrnnGameManager>().ResultListCtrl.AddResult(Result);
                //App.GetRServer<GameServer_01>().ResultListCtrl.AddResult(_result);
                ReSetGiveCardsStatus();
            }
        }

        protected override string GetNiuName(ISFSObject responseData)
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
            string[] daxie = { "n0", "n1", "n2", "n3", "n4", "n5", "n6", "n7", "n8", "n9", "n10" };
            string str = daxie[niu];

            if (type > 10)
            {
                switch (type)
                {
                    case 11:
                        str = "72";
                        Facade.Instance<MusicManager>().Play("bull10");
                        break;
                    case 12:
                        str = "74";
                        Facade.Instance<MusicManager>().Play("bullwuhua");
                        break;
                    case 13:
                        str = "75";
                        Facade.Instance<MusicManager>().Play("bullzhadan");
                        break;
                    case 14:
                        str = "73";
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

        protected override void InstantiateChip(GameObject go)
        {
            if (Index >= 25)
            {
                CancelInvoke("GiveCards");
                GiveCardsStatus = 2;
                return;
            }
            Facade.Instance<MusicManager>().Play("Card");
            GameObject temp = Instantiate(go);
            temp.transform.parent = Target[Index % 5].transform;
            temp.transform.position = go.transform.position;
            temp.transform.localScale = Vector3.one;
            temp.GetComponent<UISprite>().depth = Chipdepth;
            temp.SetActive(true);
            Chipdepth++;
            var sp = temp.GetComponent<SpringPosition>();

            switch (Index % 5)
            {
                case 0: Arrindex++;
                    Pontion = Pontion + 30;
                    CommonObject.CardArray0[Arrindex] = temp;
                    break;
                case 1: CommonObject.CardArray1[Arrindex] = temp;
                    break;
                case 2: CommonObject.CardArray2[Arrindex] = temp;
                    break;
                case 3: CommonObject.CardArray3[Arrindex] = temp;
                    break;
                case 4: CommonObject.CardArray4[Arrindex] = temp;
                    break;
            }
            var v = new Vector3(Pontion, 0, 0);
            sp.target = v;
            sp.enabled = true;
            Index++;
        }
    }
}