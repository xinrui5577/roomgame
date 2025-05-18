using System.Collections.Generic;
using Assets.Scripts.Game.fillpit.ImgPress.Main;
using Assets.Scripts.Game.fillpit.Mgr;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.fillpit.skin1
{
    public class FillpitGameManagerSk1 : FillpitGameManager
    {

        protected bool IsRub;

        public PubCardScript PubCardScript;

        public WmFriend WmFriend;

        public RuleViewMgr RuleViewMgr;

        public UISprite GameIcon;
        


        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            base.OnGetGameInfo(gameInfo);
            WmFriend.OnGetGameInfoData(gameInfo); //初始化邀请牌友信息
            if(gameInfo.ContainsKey("cargs2"))
            {
                var cargs2 = gameInfo.GetSFSObject("cargs2");
                if (GameIcon != null && cargs2.ContainsKey("-gicon"))
                {
                    GameIcon.spriteName = cargs2.GetUtfString("-gicon");
                    GameIcon.MakePixelPerfect();
                }
            }
        }


        public override void InitRoom(ISFSObject gameInfo)
        {
            base.InitRoom(gameInfo);
            if (gameInfo.ContainsKey("isRub"))
            {
                IsRub = gameInfo.GetBool("isRub");
            }

            //获取房间配置
            if (gameInfo.ContainsKey("rid"))
            {
                bool played = gameInfo.GetBool("playing");
                WeiChatInvite.SetWeiChatBtnActive(!played);
            }

            if (RuleViewMgr != null)
                RuleViewMgr.SetRuleViewInfo(gameInfo);
        }

        public override void BeginNewGame(ISFSObject sfsObject)
        {
            base.BeginNewGame(sfsObject);
            WmFriend.OnGameBegin();
            var gdata = App.GetGameData<FillpitGameData>();
            if (gdata.IsRoomGame && gdata.IsPlayed) return;
            Facade.Instance<MusicManager>().Play(string.Format("Begin{0}{1}", UnityEngine.Random.Range(1, 3), 1));
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            if (!App.GetRServer<FillpitGameServer>().HasGetGameInfo)
            {
                return;
            }
            GameRequestType gameType = (GameRequestType) type;
            base.GameResponseStatus(type, response);
            var gdata = App.GameData;
            switch (gameType)
            {
                case GameRequestType.Card:
                    //搓牌
                    if (response.ContainsKey("isRub"))
                    {
                        IsRub = response.GetBool("isRub");
                        if (!IsRub || PubCardScript == null) break;

                        int[] cardSeats = response.GetIntArray("seats");

                        if (response.ContainsKey("selfCard") && response.GetIntArray("selfCard").Length >= 0)
                        {
                            var cardValList = new List<int>();
                            AddCards(cardValList, response.GetIntArray("selfCard"));
                            if (response.ContainsKey("cardsArr"))
                            {
                                ISFSArray cardArray = response.GetSFSArray("cardsArr");
                                int count = cardArray.Count;
                                for (int i = 0; i < count; i++)
                                {
                                    int[] cards = cardArray.GetIntArray(i);
                                    AddCards(cardValList, GetSelfCard(cards, cardSeats));
                                }
                            }
                            else
                            {
                                int[] cards = response.GetIntArray("cards");
                                AddCards(cardValList, GetSelfCard(cards, cardSeats));
                            }
                            PubCardScript.ShowView(cardValList.ToArray());
                        }
                        SetPlayersRunMarkActive(cardSeats, true);
                    }
                    break;

                case GameRequestType.BetSpeak:
                    if (PubCardScript != null)
                        PubCardScript.PlayHide();
                    SetAllPlayerRubMarkActive(false);
                    break;

                case GameRequestType.RubDone:
                    int seat = response.GetInt("seat");
                    gdata.GetPlayer<PlayerPanel>(seat, true).SetRubMark(false);
                    if (seat == gdata.SelfSeat)
                    {
                        if (PubCardScript != null)
                            PubCardScript.PlayHide();
                    }
                    break;
            }
        }

        private void AddCards(List<int> cardValList, IList<int> cards)
        {
            int count = cards.Count;
            if (count <= 0) return;
            for (int i = 0; i < count; i++)
            {
                cardValList.Add(cards[i]);
            }
        }
     

        List<int> GetSelfCard(int[] cards , int[] seats)
        {
            var list = new List<int>();
            int cardCount = cards.Length;
            int playerCount = seats.Length;
            int selfSeat = App.GameData.SelfSeat;
            for (int i = 0; i < cardCount; i++)
            {
                int curSeat = seats[i%playerCount];
                if (selfSeat == curSeat)
                {
                    list.Add(cards[i]);
                }
            }
            return list;
        }

        public void SendFinishRubDone()
        {
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("type", (int)GameRequestType.RubDone);
            App.RServer.SendGameRequest(sfsObject);
        }

        void SetAllPlayerRubMarkActive(bool active)
        {
            var gdata = App.GameData;
            foreach (var key in gdata.UserInfoDict.Keys)
            {
                gdata.GetPlayer<PlayerPanel>(key).SetRubMark(active);
            }
        }

        void SetPlayersRunMarkActive(int[] seats, bool active)
        {
            var gdata = App.GameData;
            int len = seats.Length;
            for (int i = 0; i < len; i++)
            {
                gdata.GetPlayer<PlayerPanel>(seats[i], true).SetRubMark(active);
            }
        }
    }
}
