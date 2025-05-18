using System;
using Assets.Scripts.Game.jh.EventII;
using Assets.Scripts.Game.jh.Modle;
using Assets.Scripts.Game.jh.Sound;
using Sfs2X.Entities.Data;
using UnityEngine.SocialPlatforms;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.jh.network
{
    public class JhGameManager : YxGameManager
    {

        public EventObject EventObj;

        protected YxEventDispatchManager EventMng;

        protected JhGameTable GameData;

        protected override void OnStart()
        {
            base.OnStart();
            EventMng = Facade.EventCenter;
            GameData = App.GetGameData<JhGameTable>();
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            GameData.SetGameStatus();
            JhUserInfo player = GameData.GetPlayerInfo<JhUserInfo>();
            if (GameData.IsCreatRoom)
            {
                GameData.HupUp.OnGameInfo(gameInfo);
                GameData.GStatus = gameInfo.GetBool("playing") ? YxEGameStatus.Play : YxEGameStatus.Normal;
            }
            else
            {
                if (GameData.RStatus == RoomStatus.Over && player.IsPlaying())
                {
                    GameData.GStatus = YxEGameStatus.Over;
                }
                else if (GameData.RStatus > RoomStatus.CanStart && player.IsPlaying())
                {
                    GameData.GStatus = YxEGameStatus.PlayAndConfine;
                }
                else
                {
                    GameData.GStatus = YxEGameStatus.Ready;
                }
            }
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {

        }

        public override void GameStatus(int status, ISFSObject info)
        {
            
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            switch (type)
            {
                case JhTypeKey.TypeReady://准备
                    
                    break;
                case JhTypeKey.TypeFaPai://发牌
                    GameData.GStatus = YxEGameStatus.PlayAndConfine;
                    GameData.OnFapai(response);
                    break;
                case JhTypeKey.TypeGenZhu://跟诅
                    GameData.OnGenZhu(response);
                    break;
                case JhTypeKey.TypeQiPai://弃牌
                    GameData.OnQiPai(response);
                    if (!GameData.IsCreatRoom&&!GameData.GetPlayerInfo<JhUserInfo>().IsPlaying())
                    {
                        GameData.GStatus = YxEGameStatus.Over;
                    }
                    break;
                case JhTypeKey.TypeKanPai://看牌
                    GameData.OnLookCards(response);
                    break;
                case JhTypeKey.TypeBiPai://比牌
                    GameData.OnCompare(response);
                    if (!GameData.IsCreatRoom && !GameData.GetPlayerInfo<JhUserInfo>().IsPlaying())
                    {
                        GameData.GStatus = YxEGameStatus.Over;
                    }
                    break;
                case JhTypeKey.TypeOver:
                    if (!GameData.IsCreatRoom)
                    {
                        GameData.GStatus = YxEGameStatus.Over;    
                    }
                    GameData.OnResult(response);
                    break;
                case JhTypeKey.TypeDefault://20轮比牌
                    GameData.On20Compare(response);
                    break;
                case JhTypeKey.TypeCurInfo:
                    GameData.OnCurPlayer(response);
                    break;
                case JhTypeKey.TypeStart:
                    GameData.OnStart(response);
                    break;
                case JhTypeKey.TypeGameReady:
                    if (!GameData.IsCreatRoom)
                    {
                        GameData.GStatus = YxEGameStatus.Ready;
                    }
                    GameData.OnGameReady(response);
                    break;
                case JhTypeKey.TypeGZYZ:
                    GameData.OnGuZhuYiZhi(response);
                    if (!GameData.IsCreatRoom&&!GameData.GetPlayerInfo<JhUserInfo>().IsPlaying())
                    {
                        GameData.GStatus = YxEGameStatus.Over;
                    }
                    break;
                case JhTypeKey.TypeShowCard:
                    GameData.OnShowCard(response);
                    break;
            }
        }


        public override void BeginNewGame(ISFSObject sfsObject)
        {
            if (GameData.IsCreatRoom)
            {
                Random rd = new Random();
                var index=rd.Next(1, 3);
                EventObj.SendEvent("SoundEvent", "PersonSound", new JhSound.SoundData(JhSound.EnAudio.Start,1, index));
                GameData.CreateRoomInfo.AddRound();
            }
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            base.OnOtherPlayerJoinRoom(sfsObject);
            if (GameData.IsCreatRoom)
            {
                GameData.HupUp.FrashHupUpUser();
            }
        }

        public override void UserReady(int localSeat, ISFSObject responseData)
        {
            GameData.OnReady(localSeat,responseData);
        }

        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "Quit":
                    OnQuitGameClick();
                    break;
                case "ChangeRoom":
                    if (GameData.GetPlayerInfo<JhUserInfo>().IsPlaying()&&GameData.RStatus > RoomStatus.Ready)
                    {
                        YxMessageBox.Show("已在游戏中，无法更换房间！");
                    }
                    else
                    {
                        OnChangeRoomClick();
                    }
                    break;
            }
        }

        public override void UserIdle(int localSeat, ISFSObject responseData)
        {
            base.UserIdle(localSeat, responseData);
            EventObj.SendEvent("PlayersViewEvent","UserIdle",localSeat);
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
            base.UserIdle(localSeat, responseData);
            GameData.SetGameStatus();
        }
    }
}
