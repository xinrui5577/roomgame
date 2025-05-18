using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.jlgame.EventII;
using Assets.Scripts.Game.jlgame.Modle;
using Assets.Scripts.Game.jlgame.Sound;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Abstracts;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameTableView : MonoBehaviour
    {
        public EventObject EventObj;

        public JlGameCardItem JlGameCardItem;

        public Transform StartArea;

        public UISpriteAnimation Ani;
        public GameObject RoomInfoView;
        public GameObject NormalInfoView;

        public GameObject ReadyBtn;

        public GameObject InviteBtn;

        public UILabel RoomId;

        public UILabel Round;

        public List<JlGameCardItem> CardsList;

        public List<UISprite> DragonSprites;

        public GameObject KillDragonEffect;

        public GameObject IsDragonEffect;

        public List<JlGameOutCardsArea> OutCardAreas;

        private JlGameGameTable _gdata;

        protected void Start()
        {
            _gdata = App.GetGameData<JlGameGameTable>();
        }

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {

                case "RoomInfo":
                    OnFreshRoomInfo(data.Data);
                    break;
                case "Rejoin":
                    RejoinCardDeal(data.Data);
                    break;
                case "FreshCurRound":
                    OnFreshCurRound(data.Data);
                    break;
                case "Allocate":
                    PlayWashCardAni(data.Data);
                    break;
                case "OutCard":
                    OnOutCard(data.Data);
                    break;
                case "Reset":
                    Reset();
                    break;
                case "Ready":
                    ReadyBtn.SetActive(true);
                    break;
            }
        }


        protected void OnFreshRoomInfo(object data)
        {
            RoomInfoView.SetActive(true);
            NormalInfoView.SetActive(false);
            var roomData = (ISFSObject)data;
            var roomId = roomData.GetInt("roomId");
            var curRound = roomData.GetInt("curRound");
            var maxRound = roomData.GetInt("maxRound");
            InviteBtn.SetActive(curRound <= 0);
            var round = string.Format("{0}/{1}", curRound, maxRound);
            RoomId.text = roomId.ToString();
            Round.text = round;
        }

        protected void OnFreshCurRound(object data)
        {
            InviteBtn.SetActive(false);
            var roomData = data as YxCreateRoomInfo;
            if (roomData != null)
            {
                Round.text = string.Format("{0}/{1}", roomData.CurRound, roomData.MaxRound);
            }
        }

        protected void PlayWashCardAni(object data)
        {
            Ani.gameObject.SetActive(true);
            Ani.enabled = true;
            StartCoroutine(StopWash(1, data));
        }

        IEnumerator StopWash(int time, object data)
        {
            yield return new WaitForSeconds(time);
            Ani.gameObject.SetActive(false);
            Ani.enabled = false;
            CreateAllCards(data);
        }

        public void CreateAllCards(object data)
        {
            var cardData = (ISFSObject)data;
            var cards = cardData.GetIntArray("cards");
            var cardsNum = cardData.GetUtfStringArray("cardsNum");
            var selfSeat = cardData.GetInt("selfSeat");
            var allCardsCount = 0;
            _gdata.GetPlayer<JlGameSelfPlayer>().RejoinFresh = new EventDelegate(OnTrusteeshipBtnClick);
            foreach (var t in cardsNum)
            {
                allCardsCount += int.Parse(t);
            }
            for (int i = 0; i < allCardsCount; i++)
            {
                JlGameCardItem mJlGameCard = (JlGameCardItem)Instantiate(JlGameCardItem, new Vector3(0.002f * i - 0.1f, 0, 0), Quaternion.identity);
                mJlGameCard.transform.SetParent(StartArea);
                mJlGameCard.SetCardDepth(i);
                mJlGameCard.transform.localScale = Vector3.one * 0.4f;
                mJlGameCard.GetComponent<UIEventListener>().onClick = OnClickCard;
                CardsList.Add(mJlGameCard);
            }
            StartCoroutine(AllocateCard(cards, cardsNum, selfSeat));
        }

        IEnumerator AllocateCard(int[] cards, string[] everyCardNum, int selfSeat)
        {
            var playerArr = _gdata.PlayerList;
           
            for (int i = 0; i < playerArr.Length; i++)
            {
                for (int j = 0; j < int.Parse(everyCardNum[i]); j++)
                {
                    yield return new WaitForSeconds(0.0001f);
                    bool isSelf = selfSeat == i;
                    _gdata.GetPlayer<JlGamePlayer>(i).GetCardsOnStart(CardsList[CardsList.Count - 1], isSelf, int.Parse(everyCardNum[i]));
                    CardsList.RemoveAt(CardsList.Count - 1);
                    EventObj.SendEvent("SoundEvent", "PlayEffect", new JlGameSound.SoundData(JlGameSound.EnAudio.Sendcard, _gdata.GetPlayerInfo<JlGameUserInfo>().SexI));
                }
            }
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < playerArr.Length; i++)
            {
                _gdata.GetPlayer<JlGamePlayer>(i).SetCardValue(cards, cards.Length);
            }
        }

        private void RejoinCardDeal(object data)
        {
            var playerArr = _gdata.PlayerList;

            _gdata.GetPlayer<JlGameSelfPlayer>().RejoinFresh = new EventDelegate(OnTrusteeshipBtnClick);

            for (int i = 0; i < playerArr.Length; i++)
            {
                _gdata.GetPlayer<JlGamePlayer>(i).CreateCards(JlGameCardItem, i==0, _gdata.GetPlayerInfo<JlGameUserInfo>(i).CardLen, _gdata.GetPlayerInfo<JlGameUserInfo>(i).Cards, OnClickCard);
            }

            ReadyBtn.gameObject.SetActive(!_gdata.GetPlayer<JlGamePlayer>().ReadyState);
            var cardData = (ISFSObject)data;
            var color1 = cardData.GetIntArray("color1");
            var color2 = cardData.GetIntArray("color2");
            var color3 = cardData.GetIntArray("color3");
            var color4 = cardData.GetIntArray("color4");

            if (color1.Length != 0)
            {
                foreach (var t in color1)
                {
                    JlGameCardItem jlGameCard = Instantiate(JlGameCardItem);
                    jlGameCard.Value = t;
                    OutCardAreas[0].FromHandToOutArea(jlGameCard, true);
                }
            }
            if (color2.Length != 0)
            {
                foreach (var t in color2)
                {
                    JlGameCardItem jlGameCard = Instantiate(JlGameCardItem);
                    jlGameCard.Value = t;
                    OutCardAreas[1].FromHandToOutArea(jlGameCard, true);
                }
            }
            if (color3.Length != 0)
            {
                foreach (var t in color3)
                {
                    JlGameCardItem jlGameCard = Instantiate(JlGameCardItem);
                    jlGameCard.Value = t;
                    OutCardAreas[2].FromHandToOutArea(jlGameCard, true);
                }
            }
            if (color4.Length != 0)
            {
                foreach (var t in color4)
                {
                    JlGameCardItem jlGameCard = Instantiate(JlGameCardItem);
                    jlGameCard.Value = t;
                    OutCardAreas[3].FromHandToOutArea(jlGameCard, true);
                }
            }

            var killDragon = cardData.ContainsKey("killDragon") ? cardData.GetBoolArray("killDragon") : null;
            var isDragon = cardData.ContainsKey("isDragon") ? cardData.GetBoolArray("isDragon") : null;
            if (killDragon != null || isDragon != null)
            {
                StartCoroutine(PlayDragon(killDragon, isDragon,true));
            }

        }

        public void OnOutCard(object data)
        {
            var cardData = (ISFSObject)data;
            var card = cardData.GetInt("card");
            var seat = cardData.GetInt("seat");
            _gdata.GetPlayer<JlGamePlayer>(seat, true).OutCard(OutCardAreas[(card >> 4) - 1], card);

            var killDragon = cardData.ContainsKey("killDragon") ? cardData.GetBoolArray("killDragon") : null;
            var isDragon = cardData.ContainsKey("isDragon") ? cardData.GetBoolArray("isDragon") : null;
            if (killDragon != null || isDragon != null)
            {
                StartCoroutine(PlayDragon(killDragon, isDragon));
            }
        }

        IEnumerator PlayDragon(bool[] killDragon, bool[] isDragon, bool rejoin = false)
        {
            bool[] dragon = new bool[] { };
            bool isKillDragon = false;
            if (killDragon != null)
            {
                isKillDragon = true;
                dragon = killDragon;
                KillDragonEffect.SetActive(false);
            }

            if (isDragon != null)
            {
                dragon = isDragon;
                IsDragonEffect.SetActive(false);
            }

            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < dragon.Length; i++)
            {
                if (dragon[i])
                {
                    if (DragonSprites[i].gameObject.activeSelf) continue;
                    if (isKillDragon)
                    {
                        EventObj.SendEvent("SoundEvent", "RemindSound", new JlGameSound.SoundData(JlGameSound.EnAudio.KillDragon, _gdata.GetPlayerInfo<JlGameUserInfo>().SexI));
                        KillDragonEffect.SetActive(!rejoin);
                    }
                    else
                    {
                        EventObj.SendEvent("SoundEvent", "PlayEffect", new JlGameSound.SoundData(JlGameSound.EnAudio.Dragon, _gdata.GetPlayerInfo<JlGameUserInfo>().SexI));
                        IsDragonEffect.transform.position=new Vector3(IsDragonEffect.transform.position.x, OutCardAreas[i].transform.position.y,0);
                        IsDragonEffect.SetActive(!rejoin);
                        DragonSprites[i].spriteName = "public_018";
                    }
                    if (!rejoin) yield return new WaitForSeconds(0.2f);


                    DragonSprites[i].gameObject.SetActive(true);
                    if (OutCardAreas[i].OutCards[1] != null)
                    {
                        DragonSprites[i].transform.parent = OutCardAreas[i].OutCards[1].transform;
                        DragonSprites[i].transform.position = OutCardAreas[i].OutCards[1].transform.position;
                    }
                    OutCardAreas[i].ChangeBlack();
                }
            }
        }

        public void OnReadyBtnClick()
        {
            EventObj.SendEvent("ServerEvent", "ReadyReq", null);
            ReadyBtn.SetActive(false);
        }

        public void OnTrusteeshipBtnClick()
        {
            EventObj.SendEvent("ServerEvent", "Trusteeship", null);
        }

        public void OnClickCard(GameObject cardItem)
        {
            var clickCard = cardItem.GetComponent<JlGameCardItem>();
            EventObj.SendEvent("SoundEvent", "PlayEffect", new JlGameSound.SoundData(JlGameSound.EnAudio.Click, _gdata.GetPlayerInfo<JlGameUserInfo>().SexI));
            if (!clickCard.IsOutCard && !clickCard.IsFoldCard) return;

            if (clickCard.MyCardFlag && clickCard.IsCardSelect)
            {
                if (clickCard.IsOutCard)
                {
                    EventObj.SendEvent("ServerEvent", "OutCard", clickCard.Value);
                }
                if (clickCard.IsFoldCard)
                {
                    EventObj.SendEvent("ServerEvent", "FoldCard", clickCard.Value);
                }
                return;
            }
            _gdata.GetPlayer<JlGameSelfPlayer>().SortCard();
            clickCard.SetCardUp();
        }

       

        public void OnSettingClick()
        {
            EventObj.SendEvent("SettingEvent", "Open", null);
        }

        public void OnClickChatInvite()
        {
            var roomInfo = _gdata.CreateRoomInfo;
            YxTools.ShareFriend(roomInfo.RoomId.ToString(), roomInfo.RuleInfo);
        }

        public void Reset()
        {
            foreach (var t in DragonSprites)
            {
                t.transform.parent = transform;
                t.gameObject.SetActive(false);
            }
            foreach (var t in OutCardAreas)
            {
                t.Reset();
            }
            var playerArr = _gdata.PlayerList;

            for (int i = 0; i < playerArr.Length; i++)
            {
                _gdata.GetPlayer<JlGamePlayer>(i).Reset();
            }
        }
    }
}
