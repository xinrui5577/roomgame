using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class PanelTriggerArgs : EvtHandlerArgs
    {
        public bool ReadyState;
        public bool QueryBtnState;
    }

    public class OnTingArgs : EvtHandlerArgs
    {
        public int TingChair = -1;
    }

    public class PlayerInfoArgs : EvtHandlerArgs
    {
        public int Chair = -1;
        public bool State = false;
        private MahjongUserInfo mPlayerData;
        public MahjongUserInfo PlayerData
        {
            get
            {
                if (mPlayerData == null || mPlayerData.Chair != Chair)
                {
                    mPlayerData = GameCenter.DataCenter.Players[Chair];
                }
                return mPlayerData;
            }
        }
    }

    public class GameInfoArgs : EvtHandlerArgs
    {
        public string Round;
        public string MahjongCount;
    }

    public class HandupEventArgs : EvtHandlerArgs
    {
        public int Time;
        public int Chair;
        public string UserName;
        public DismissFeedBack HandupType;
    }

    public class RejoinHandupEventArgs : EvtHandlerArgs
    {
        public class HandupPlayer
        {
            public int Chair;
            public string UserName;
            public DismissFeedBack HandupType;
        }
      
        public int Time;
        public string PlayersId;
        public List<HandupPlayer> Players;
    }

    public class SingleResultArgs : EvtHandlerArgs
    {
        public enum HuResultType
        {
            //胡牌不结算
            HuSingle,
            //胡牌结算
            HuEndu,
        }
        public int Bao;
        public int HuCard;
        public int HuType;
        public bool PiaoHu;
        public bool ChBao;
        public bool MoBao;
        public int[] ZhaMa;
        public int[] ZhongMa;
        public List<int> HuSeats;//胡牌座位
        public List<int> HuCardList;//胡牌座位
        public HuResultType ResultType;
        public List<MahjongResult> Result;
        public Dictionary<int, int> HuSort;//胡牌顺序
    }

    public class ChooseCgArgs : EvtHandlerArgs
    {
        public enum ChooseType
        {
            ChooseTing,
            ChooseCg,
        }
        //打出的牌
        public int OutPutCard;
        //消息类型
        public ChooseType Type;
        //可选排序
        public List<int[]> FindList;
        //确定选择一个牌型
        public Action<int> ConfirmAction;
        //取消听牌
        public UnityEngine.Events.UnityAction CancelTingAction;
    }

    /// <summary>
    /// 查询胡牌
    /// </summary>
    public class QueryHuArgs : EvtHandlerArgs
    {
        //查询的牌
        public int QueryCard;
        //允许胡的牌
        public int[] AllowHuCards;
        //关闭panel
        public bool PanelState = true;
        //倍数
        public int[] RateArray;
    }

    /// <summary>
    /// Op按钮列表
    /// </summary>
    public class OpreateMenuArgs : EvtHandlerArgs
    {
        public List<KeyValuePair<int, bool>> OpMenu;
    }

    /// <summary>
    /// 设置分数
    /// </summary>
    public class SetScoreArgs : EvtHandlerArgs
    {
        public int Type;
        public float DelayTime;
        public Dictionary<int, long> ScoreDic;
    }

    /// <summary>
    /// 提示消息
    /// </summary>
    public class ShowTitleMessageArgs : EvtHandlerArgs
    {
        public int TitleType;
        public Dictionary<int, Variable> Params = new Dictionary<int, Variable>();
    }

    /// <summary>
    /// 定缺和换三张
    /// </summary>
    public class HuanAndDqArgs : EvtHandlerArgs
    {
        //0.定缺|1.换牌|2.胡
        public int Type;
        public int HuanType;
        public List<int> HuSeats;
        public int[] DingqueColors;
        public int[] ChangeAfterCards; //换过来的牌数组
        public int[] ChangeBeforeCards;//扣下去的牌数组
    }

    /// <summary>
    /// 玩家flag
    /// </summary>
    public class PlayerStateFlagArgs : EvtHandlerArgs
    {
        public int SecletColor;
        public Sprite DiySprite;
        public int StateFlagType;
        public bool CtrlState;
    }

    /// <summary>
    /// 加漂界面
    /// </summary>
    public class ScoreDoubleArgs : EvtHandlerArgs
    {
        public int[] ScoreDoubleArray;
    }

    /// <summary>
    /// 流程状态机参数
    /// </summary>
    public class SfsFsmStateArgs : FsmStateArgs
    {
        //显隐控制
        public ISFSObject SFSObject;
    }

    public class AiAgencyArgs : EvtHandlerArgs
    {
        public bool State;
    }

    public class XjfdListArgs : EvtHandlerArgs
    {
        public List<int[]> XjfdList;
    }

    public class PanelGameInfoArgs : EvtHandlerArgs
    {
        public int Laizi;
    }

    public class PanelExhibitionArgs : EvtHandlerArgs
    {
        public List<int> ExhMahjongList = new List<int>();
    }

    public class ZhaniaoArgs : EvtHandlerArgs
    {
        public List<int> ZhaMaList = new List<int>();
        public List<int> ZhongMaAllList = new List<int>();
    }
}