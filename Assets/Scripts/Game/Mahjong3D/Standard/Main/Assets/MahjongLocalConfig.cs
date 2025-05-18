using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [CreateAssetMenu(fileName = "MahjongLocalConfig", menuName = "Mahjong Scriptable/MahjongLocalConfig", order = 100)]
    public class MahjongLocalConfig : ScriptableObjBast
    {
        //补张
        public bool Buzhang;
        //托管
        public bool AiAgency;
        //Gm
        public bool GmFlag;
        //支持震动
        public bool MobileShake;
        //方言 有方言默认使用方言音效
        public bool Localism;
        //显示房主标记
        public bool ShowOwnerSign;
        //手牌排序动画
        public bool CardSortAni;
        //赖子牌打出
        public bool AllowLaiziPut;
        //不显示牌墙
        public bool NoShowMahjongWall;
        //0,2号玩家的手牌向中心集合
        public bool SortByCenter;
        //牌桌机发牌动画
        public bool MahStartAnimation = true;
        //暗杠一张牌翻开，其余玩家都能看到
        public bool ShowAnGang = true;
        //分享格式 false：本地格式 true：后台配置
        public bool ShareFormat;
        //对面打出的和吃碰杠的牌面向我
        public bool MahjongTowardsMe;
        //是否播放特殊胡牌音效
        public bool PlaySpecialHuSound;
        //是否播放特殊混合胡法特效,例如清一色+飘胡=清一色飘胡
        public bool IsPlayHunHeHuSound;
        //随机播放麻将声音
        public bool RondomPlayMahjongSound;
        //是否播放部分特殊特效,默认播放
        public bool IsPlaySpecialEffects = true;
        //解散界面显示扎码牌
        public bool ResultShowZhongma;
        //copy 房间号
        public bool CopyRoomid;           
        // 手牌检查， 多牌bug 暂时解决方案
        public bool CheckHandCard = false;
        // 查听带倍数
        public bool QueryTingInRate;

        [Header("动画时间")]
        //投票解散时间
        public int TimeHandup = 300;
        //出牌倒计时
        public int TimeOutcard = 15;
        //定缺时间
        public int TimeDingque = 15;
        //换张时间
        public int TimeHuancard = 15;
        //发牌时间      
        public float TimeSendCardUp = 0.08f;
        public float TimeSendCardUpWait = 0.08f;
        public float TimeSendCardInterval = 0.08f;
        //听牌，打牌时间
        public float TimeTingPutCardWait = 0.3f;
        //抓牌时间
        public float TimeGetInCardRote = 0.1f;
        //胡牌操作时间间隔
        public float TimeHuAniInterval = 0.5f;
        //胡牌 推牌间隔
        public float TimePushCardInterval = 1f;
        //扎鸟动画间隔时间
        public float TimeZhaniaoAni = 0.5f;
        //宝牌提示显示时间
        public float TimeBaoTip = 1f;
        //普通話玩家音效播放速率
        public float TimeNormalAudioClipRate = 1f;
        //方言玩家音效播放速率
        public float TimeLocalismAudioClipRate = 1f;
        //补张动画延迟
        public float TimeBuzhangAniDelay = 0.2f;
        //补张消息延迟
        public float TimeBuzhangMessageDelay = 0.5f;

        [Header("游戏默认参数")]
        //默认玩法 例: 番数:4番封顶;玩法:幺九将对,天地胡; 
        public string DefaultGameRule = "";
        //手牌个数
        public int HandCardCount = 13;
        public float MahjongPickupHeight = 0.2f;
        public Vector3 AnbaoPos = new Vector3(0.5f, 3.6f, -0.5f);

        [Header("特殊胡牌参数")]
        public SpecialHuType[] SpecialHuTypes;

    }

    [System.Serializable]
    public class SpecialHuType
    {
        public string HuTypeName;
        public int HuTypeValue;
        public bool IsOnly;
    }
}