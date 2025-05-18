using System.Collections.Generic;
using Assets.Scripts.Game.lyzz2d.Game;
using Assets.Scripts.Game.lyzz2d.Game.Data;
using Assets.Scripts.Game.lyzz2d.Game.Item;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Utils
{
    public class ConstantData
    {
        /// <summary>
        ///     默认头像
        /// </summary>
        public const string DefaultHeadName = "HS_9";

        private ConstantData()
        {
        }

        #region GameKeys

        #region 游戏中通用的Key

        #endregion

        /// <summary>
        ///     任务ID
        /// </summary>
        public const string KeyTaskId = "taskId";

        /// <summary>
        ///     玩家ID
        /// </summary>
        public const string KeyUserId = "uid";

        /// <summary>
        ///     游戏中的状态
        /// </summary>
        public const string KeyStatus = "status";


        /// <summary>
        ///     CD
        /// </summary>
        public const string KeyCDTime = "cdTime";

        #endregion

        #region 音乐相关 

        /// <summary>
        ///     默认背景音乐
        /// </summary>
        public const string Music_DefBackground = "beijingyinyue";

        /// <summary>
        ///     游戏开始音效
        /// </summary>
        public const string Voice_GameBegin = "duijukaishi";

        /// <summary>
        ///     亮牌音效(好像是没有，用这个先用着)
        /// </summary>
        public const string Voice_LiangPai = "Tap";

        /// <summary>
        ///     抓牌音效
        /// </summary>
        public const string Voice_ZhuaPai = "zhuapai";

        /// <summary>
        ///     碰的音效
        /// </summary>
        public const string Voice_Peng = "peng";

        /// <summary>
        ///     杠的音效
        /// </summary>
        public const string Voice_Gang = "gang";

        /// <summary>
        ///     吃牌的音效
        /// </summary>
        public const string Voice_Chi = "chi";

        /// <summary>
        ///     自摸音效
        /// </summary>
        public const string Voice_ZiMo = "zimo";

        /// <summary>
        ///     胡音效
        /// </summary>
        public const string Voice_Hu = "hu";

        /// <summary>
        ///     听音效
        /// </summary>
        public const string Voice_Ting = "ting";

        #endregion

        #region 常量数据相关

        /// <summary>
        ///     Waiting面板消失时间
        /// </summary>
        public const float WaitingEscapeTime = 70;

        #region 麻将常量数据

        /// <summary>
        ///     水平飞的大小X
        /// </summary>
        public const int Bg_Horizontal_Fly_SizeX = 130;

        /// <summary>
        ///     水平飞的大小Y
        /// </summary>
        public const int Bg_Horizontal_Fly_SizeY = 82;

        /// <summary>
        ///     放倒水平X
        /// </summary>
        public const int Bg_Horizontal_Lie_SizeX = 78;

        /// <summary>
        ///     放倒水平Y
        /// </summary>
        public const int Bg_Horizontal_Lie_SizeY = 75;

        /// <summary>
        ///     水平方向的牌站立的大小X
        /// </summary>
        public const int Bg_Horizontal_Stand_SizeX = 44;

        /// <summary>
        ///     水平方向的牌站立的大小Y
        /// </summary>
        public const int Bg_Horizontal_Stand_SizeY = 112;

        /// <summary>
        ///     竖直飞的大小X
        /// </summary>
        public const int Bg_Vertical_Fly_SizeX = 78;

        /// <summary>
        ///     竖直飞的大小Y
        /// </summary>
        public const int Bg_Vertical_Fly_SizeY = 139;

        /// <summary>
        ///     水平方向显示value的X
        /// </summary>
        public const int Value_Horizontal_Lie_SizeX = 66;

        /// <summary>
        ///     水平方向显示value的Y
        /// </summary>
        public const int Value_Horizontal_Lie_SizeY = 101;

        /// <summary>
        ///     常规背景X
        /// </summary>
        public const int Bg_Normal_SizeX = 78;

        /// <summary>
        ///     常规背景Y
        /// </summary>
        public const int Bg_Normal_SizeY = 112;

        /// <summary>
        ///     竖直方向麻将站立时，value显示的位置X
        /// </summary>
        public const int Value_Vertical_Stand_PosY = 0;

        /// <summary>
        ///     竖直方向麻将站立时，value显示的位置X
        /// </summary>
        public const int Value_Vertical_Stand_PosX = 0;

        /// <summary>
        ///     右侧方向麻将放倒时，value显示的位置X
        /// </summary>
        public const int Value_Horizontal_LiePosX = -6;

        /// <summary>
        ///     右侧方向麻将放倒时，value显示的位置Y
        /// </summary>
        public const int Value_Horizontal_LiePosY = 12;

        /// <summary>
        ///     竖直方向麻将放倒时，value显示的位置X
        /// </summary>
        public const int Value_Vertical_Lie_PosX = 0;

        /// <summary>
        ///     竖直方向麻将放倒时，value显示的位置Y
        /// </summary>
        public const int Value_Vertical_Lie_PosY = 20;

        /// <summary>
        ///     对面玩家亮牌时，牌值的X坐标
        /// </summary>
        public const int Value_Oppset_Lie_PosX = -2;

        /// <summary>
        ///     对面玩家亮牌时，牌值的Y坐标
        /// </summary>
        public const int Value_Oppser_lie_PosY = 6;

        /// <summary>
        ///     对面玩家亮牌时，牌值的X坐标
        /// </summary>
        public const int Value_Oppset_Stand_PosX = -2;

        /// <summary>
        ///     对面玩家亮牌时，牌值的Y坐标
        /// </summary>
        public const int Value_Oppser_Stand_PosY = -20;

        /// <summary>
        ///     水平方向标记位置X
        /// </summary>
        public const int Tag_Horizontal_PosX = 5;

        /// <summary>
        ///     水平方向标记位置Y
        /// </summary>
        public const int Tag_Horizontal_PosY = 12;

        /// <summary>
        ///     竖直方向标记位置X
        /// </summary>
        public const int Tag_Vertical_PosX = 15;

        /// <summary>
        ///     竖直方向标记位置Y
        /// </summary>
        public const int Tag_Vertical_PosY = 10;

        /// <summary>
        ///     选段门
        /// </summary>
        public const int DuanMenSelect = 1;

        /// <summary>
        ///     选段门默认移动时间
        /// </summary>
        public const float DuanMenMoveTime = 3;

        /// <summary>
        ///     默认的显示的牌的层级
        /// </summary>
        public const int ShowItemLayler = 105;

        /// <summary>
        ///     常用语
        /// </summary>
        public static readonly string[] Common =
        {
            "大家好,很高心见到各位",
            "和你合作真是太愉快了",
            "快点啊,等到花儿都都谢了",
            "你的牌打的也太好了",
            "不要吵了,不要吵了,专心玩游戏吧",
            "怎么又断线,网络怎么这么差啊",
            "不好意思,我要离开一会",
            "不要走,决战到天亮",
            "你是MM还是GG",
            "交个朋友吧,能告诉我联系方式吗",
            "再见了,我会想念大家的"
        };

        ///// <summary>
        ///// 水平方向标记位置X
        ///// </summary>
        //public const int Tag_Horizontal_PosX = 5;
        ///// <summary>
        ///// 水平方向标记位置Y
        ///// </summary>
        //public const int Tag_Horizontal_PosY = 12;

        #endregion

        #endregion
    }

    #region  枚举
    /// <summary>
    /// 服务器返回的操作小类型，用于显示菜单使用
    /// </summary>
    public enum Enum_CPGHMenuType
    {
        None = 0,                               //无
        Chi = 1,                                //吃
        Peng = 1 << 1,                          //碰
        Gang = 1 << 2,                          //杠
        Hu = 1 << 3,                            //胡
        Liang = 1 << 4,                         //亮
        FengGang = 1 << 5,                      //旋风杠
        JueGang = 1 << 6,                       //绝杠
        Ting = 1 << 7,                          //听
        CaiGang = 1 << 8,                           //彩杠
        GuoDan = 1 << 9,							//长春麻将中的过蛋
        DanSelect = 1 << 10                         //过蛋选择（盛将特殊要求，不在菜单中显示，但是为了服务器端处理重连状态，这个地方使用了op接口来处理）
    }

    /// <summary>
    /// 吃碰杠胡确认小类型
    /// </summary>
    public enum Enum_CPGType
    {
        None = 0,                                //无
        Chi,                                     //吃
        Peng,                                    //碰
        ZhuaGang = 4,                            //有刻子，抓一张相同的
        PengGang,                                //有三张，别人打一张
        MingGang,                                //自己有四张，服务器返回明杠标记（正面朝上？）
        AnGang,                                  //自己有四张，服务器返回暗杠标记（反面朝上？）
        LaiZiGang,                               //癞子杠，应该会有特殊的标记显示癞子牌吧
        Hu,                                      //胡
        ZiMo,                                    //自摸
        MoBao,                                   //摸宝
        PiaoHu,                                  //飘胡
        ChongBao,                                //冲宝
        HuanBao,                                 //换宝
        NiuBiHu,                                 //牛x
        Ting,                                    //听
        CaiGang,                                 //彩钢
        Xst,                                     //潇洒听
        QiangGangHu                               //抢杠胡
    }

    /// <summary>
    ///     游戏状态
    /// </summary>
    public enum EnumGameState
    {
        Init,
        Waitting,
        Gaming,
        Account,
        Over
    }

    /// <summary>
    /// 麻将的几种状态
    /// Fly: 飞入过场
    /// Push:扣下，处于俯卧状态
    /// StandNo:站立没有数值状态，即为背面状态
    /// Lie：亮开状态，处于仰卧状态
    /// StandWith:站立且存在值，即当前玩家的手牌
    /// </summary>
    public enum EnumMahJongAction
    {
        Fly,
        Push,
        StandNo,
        Lie,
        StandWith,
    }
    /// <summary>
    /// 麻将摆放方向，当前玩家与对家显示为竖直的，左右玩家显示为水平的
    /// </summary>
    public enum EnumMahJongDirection
    {
        Horizontal,
        Vertical,
    }

    /// <summary>
    /// 麻将的值
    /// </summary>
    public enum EnumMahjongValue
    {
        None = 0,
        Wan_1 = 17,
        Wan_2 = 18,
        Wan_3,
        Wan_4,
        Wan_5,
        Wan_6,
        Wan_7,
        Wan_8,
        Wan_9,
        Tiao_1 = 33,
        Tiao_2,
        Tiao_3,
        Tiao_4,
        Tiao_5,
        Tiao_6,
        Tiao_7,
        Tiao_8,
        Tiao_9,
        Bing_1 = 49,
        Bing_2,
        Bing_3,
        Bing_4,
        Bing_5,
        Bing_6,
        Bing_7,
        Bing_8,
        Bing_9,
        Dong = 65,
        Nan = 68,
        Xi = 71,
        Bei = 74,
        Zhong = 81,
        Fa = 84,
        Bai = 87,
        ChunF = 96,
        XiaF = 97,
        QiuF = 98,
        DongF,
        MeiF,
        LanF,
        ZuF,
        JuF,
        AnBao = 10086,
    }

    /// <summary>
    /// 服务器返回的消息类型
    /// </summary>
    public enum EnumRequest
    {
        None, //无
        SelectPiao = 0x1, //弹出选择漂按钮
        ShowPiao = 0x2, //前台玩家选择漂、后台发送展示玩家选择的漂
        ShowRate = 0x3, //显示底注
        GiveHun = 0x4, //会，也就是癞子
        XuanDuanMen = 0x5, //选断门菜单提示
        AlloCate = 0x11, //分牌
        GetCard = 0x12, //玩家抓牌
        ThrowOutCard = 0x13, //玩家打牌
        LiangCard = 0x14, //亮牌
        ZiMo = 0x15, //自摸
        MoreTime = 0x16, //申请更多时间
        BaoCard = 0x17, //宝牌
        CPGHMenu = 0x18, //吃碰杠胡菜单提示
        JueGang = 0x1a, //绝杠
        Card = 0x20, //获得一张牌
        AllowResponse = 0x21, //服务器允许玩家请求？？？
        QiangGangHu = 0x23, //询问玩家是否抢杠胡
        CheckCards = 0x40, //客户端出现问题时，从服务器同步数据
        CPHType = 0x50, //吃碰杠胡统一标记响应
        Ting = 0x51, //打听牌
        Hu = 0x54, //胡牌
        SelfGang = 0x55, //抓杠
        LiuJu = 0x5A, //流局
        HaiDi = 0x5C, //海底牌 ctype:0:显示“要”按钮,1玩家选择要海底牌,-1玩家不要海底牌
        XFG = 0x56, //旋风杠
        CaiGang = 0x57, //彩杠（中发白三张加上任意中发白）
        GuoDan = 0x62,    //过蛋
        GuoDanSelect = 0x63,   //过蛋选择
    }


    /// <summary>
    /// 显示方向
    /// </summary>
    public enum EnumShowDirection
    {
        Self,
        Right,
        Oppset,
        Left,
    }

    /// <summary>
    /// 牌组的类型
    /// </summary>
    public enum GroupType
    {
        Chi = 1,
        Peng = 2,
        WZhuaGang = 3,
        /// <summary>
        /// 先碰，后杠
        /// </summary>
        ZhuaGang = 4,

        /// <summary>
        /// 直接杠别人的
        /// </summary>
        PengGang = 5,

        /// <summary>
        /// 明杠
        /// </summary>
        MingGang = 6,

        /// <summary>
        /// 暗杠
        /// </summary>
        AnGang = 7,

        /// <summary>
        /// 赖子杠
        /// </summary>
        LaiZiGang = 8,

        /// <summary>
        /// 粘牌
        /// </summary>
        NianPai = 9,

        /// <summary>
        /// 旋风杠
        /// </summary>
        FengGang = 10,

        /// <summary>
        /// 绝杠
        /// </summary>
        JueGang = 11,
    }

    public enum TotalState
    {
        Init = 0, //游戏初始化状态 玩家不够，或者玩家够了，但是未准备状态
        Waiting, // 游戏准备状态
        Gaming, // 游戏开始中
        Account, // 结算 等待玩家点击连局 如果有人退出 则进入Waiting状态
        Over, // 游戏结束 不连局
    }

    #region 麻将相关

    #endregion

    #region 交互

    #endregion

    #endregion

    #region Class
    /// <summary>
    /// 游戏内的工具类
    /// </summary>
    public class GameTools
    {
        /// <summary>
        /// 检查一个值是否存在
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="key"></param>
        public static void TryGetValueWitheKey(ISFSObject data, out int value, string key)
        {
            if (data.ContainsKey(key))
            {
                value = data.GetInt(key);
            }
            else
            {
                value = 0;
                TryGetLog(key);
            }
        }
        public static void TryGetValueWitheKey(ISFSObject data, out long value, string key)
        {
            if (data.ContainsKey(key))
            {
                value = data.GetLong(key);
            }
            else
            {
                value = 0;
                TryGetLog(key);
            }
        }
        public static void TryGetValueWitheKey(ISFSObject data, out short value, string key)
        {
            if (data.ContainsKey(key))
            {
                value = data.GetShort(key);
            }
            else
            {
                value = 0;
                TryGetLog(key);
            }
        }


        public static void TryGetValueWitheKey(ISFSObject data, out string value, string key)
        {
            if (data.ContainsKey(key))
            {
                value = data.GetUtfString(key);
            }
            else
            {
                value = "";
                TryGetLog(key);
            }
        }
        public static void TryGetValueWitheKey(ISFSObject data, out bool value, string key)
        {
            if (data.ContainsKey(key))
            {
                value = data.GetBool(key);
            }
            else
            {
                value = false;
                TryGetLog(key);
            }
        }
        public static void TryGetValueWitheKey(ISFSObject data, out int[] value, string key)
        {
            if (data.ContainsKey(key))
            {
                int[] arr = data.GetIntArray(key);
                value = (int[])arr.Clone();
            }
            else
            {
                value = new int[0];
                TryGetLog(key);
            }
        }
        public static void TryGetValueWitheKey(ISFSObject data, out ISFSArray value, string key)
        {
            if (data.ContainsKey(key))
            {
                value = data.GetSFSArray(key);
            }
            else
            {
                value = new SFSArray();
                TryGetLog(key);
            }
        }
        public static void TryGetValueWitheKey(ISFSObject data, out ISFSObject value, string key)
        {
            if (data.ContainsKey(key))
            {
                value = data.GetSFSObject(key);
            }
            else
            {
                value = new SFSObject();
                TryGetLog(key);
            }
        }

        public static void TryGetLog(string key)
        {
            //YxDebug.LogError(string.Format("There is not exist a value with Key \"{0}\" from Server", key));
        }

        public static SFSObject getSFSObject(int type)
        {
            SFSObject sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, type);
            return sfsObject;
        }

        public static string GetNormalTalkVoice(int sex, int index)
        {
            return string.Format("p{0}_Chat_{1}", sex, index);
        }

        public static string GetOperationVoice(int sex, string audioName, int randNum)
        {
            return string.Format("{0}-{1}-{2}", sex, audioName, randNum);
        }

        public static List<Transform> CreatMahjongItems(List<int> values)
        {
            List<Transform> _maList = new List<Transform>();
            for (int i = 0, lenth = values.Count; i < lenth; i++)
            {
                _maList.Add(CreateMahjong(values[i]));
            }
            return _maList;
        }

        static public Transform AddChild(Transform parent, Transform trans, float offsetScaleX = 1, float offsetScaleY = 1, bool setPosition = true)
        {
            if (trans != null && parent != null)
            {
                trans.parent = parent;
                if (setPosition)
                {
                    trans.localPosition = Vector3.zero;
                }
                trans.localRotation = Quaternion.identity;
                trans.localScale = new Vector3(offsetScaleX, offsetScaleY);
                trans.gameObject.layer = parent.gameObject.layer;
            }
            return trans;
        }
        public static Transform CreateMahjong(int value, bool numBerChange = true)
        {
            MahjongItem item = App.GetGameManager<Lyzz2DGameManager>().GetNextMahjong(numBerChange);
            item.Value = value;
            return item.transform;
        }
        /// <summary>
        /// 排序，带癞子，从大到小排序，癞子默认最小（牌局中使用排序）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="laiZiNum"></param>
        /// <returns></returns>
        public static List<int> SortCardWithLaiZi(List<int> list, int laiZiNum)
        {
            int lenth = list.Count;
            for (int i = 0; i < lenth - 1; i++)
            {
                for (int j = i + 1; j < lenth; j++)
                {
                    if (list[j].Equals(laiZiNum))
                    {
                        continue;
                    }
                    if (list[i] < list[j] || list[i].Equals(laiZiNum))
                    {
                        int temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 排序，带癞子，从小到大排序，癞子默认最小（结算时的处理）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="laiZiNum"></param>
        /// <returns></returns>
        public static List<int> SortCardWithLaiZiOppset(List<int> list, int laiZiNum)
        {
            int lenth = list.Count;
            for (int i = 0; i < lenth - 1; i++)
            {
                if (list[i].Equals(laiZiNum))
                {
                    continue;
                }
                for (int j = i + 1; j < lenth; j++)
                {
                    if (list[j].Equals(laiZiNum))
                    {
                        int temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                        break;
                    }
                    if (list[i] > list[j])
                    {
                        int temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                    }
                }
            }
            return list;
        }

        public static List<MahjongGroupData> GetGroupData(ISFSArray groups)
        {
            List<MahjongGroupData> datas = new List<MahjongGroupData>();
            for (int i = 0, lenth = groups.Size(); i < lenth; i++)
            {
                ISFSObject g = groups.GetSFSObject(i);
                MahjongGroupData groupData = CteateGroupFromSfsObject(g);
                datas.Add(groupData);
            }
            return datas;
        }
        /// <summary>
        /// 创建一个牌组(重连时使用这个，这个时候是需要处理Group中的数据即可)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static MahjongGroupData CteateGroupFromSfsObject(ISFSObject data)
        {
            int gtype;
            int value;
            int[] cards;
            GameTools.TryGetValueWitheKey(data, out gtype, RequestKey.KeyType);
            GameTools.TryGetValueWitheKey(data, out value, RequestKey.KeyCard);
            GameTools.TryGetValueWitheKey(data, out cards, RequestKey.KeyCards);
            GroupType type = (GroupType)gtype;
            MahjongGroupData groupData = new MahjongGroupData(type);
            switch (type)
            {
                case GroupType.Chi:
                    groupData.AddValue(cards[0]);
                    groupData.AddValue(value);
                    groupData.AddValue(cards[1]);
                    break;
                default:
                    for (int i = 0, max = groupData.values.Length; i < max; i++)
                    {
                        groupData.AddValue(value);
                    }
                    break;
            }
            return groupData;
        }
        /// <summary>
        /// 删除麻将控制脚本
        /// </summary>
        /// <param name="item"></param>
        public static void DestroyUserContorl(MahjongItem item)
        {
            UserContorl uc = item.GetComponent<UserContorl>();
            if (uc != null)
                GameObject.Destroy(uc);
        }

        /// <summary>
        /// 排序，移除癞子
        /// </summary>
        /// <param name="list"></param>
        /// <param name="laiZiNum"></param>
        /// <returns></returns>
        public static List<int> SortCardWithOutLaiZi(List<int> list, int laiZiNum)
        {
            int lenth = list.Count;
            for (int i = 0; i < lenth - 1; i++)
            {
                for (int j = i + 1; j < lenth; j++)
                {
                    if (list[i] > list[j])
                    {
                        int temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                    }
                }
            }
            list.Remove(laiZiNum);
            return list;
        }
        /// <summary>
        /// 删除拖拽组件
        /// </summary>
        /// <param name="item"></param>
        public static void DestroyDragObject(MahjongItem item)
        {
            UIDragObject drag = item.GetComponent<UIDragObject>();
            if (drag != null)
                GameObject.Destroy(drag);
        }
    }

    public class RequestData
    {
        /// <summary>
        /// 响应类型
        /// </summary>
        public int Type;

        /// <summary>
        /// 当前玩家的手牌
        /// </summary>
        public int[] Cards = new[] { 0, 0, 0, 0 };

        /// <summary>
        /// 每次用于接收的牌(如果返回的值为0，则从wall中取)
        /// </summary>
        public int OpCard;

        /// <summary>
        /// 服务器返回的用于操作的牌
        /// </summary>
        public int Card;

        /// <summary>
        /// 桌位
        /// </summary>
        public int Sit;

        /// <summary>
        /// 吃碰杠胡类型
        /// </summary>
        public int Op;
        public int[] GenZhuang;
        public int[] TingOutCards;
        public int Xs;
        /// <summary>
        /// 当前操作的seq
        /// </summary>
        public int Seq;

        public ISFSObject Data;

        public RequestData(ISFSObject data)
        {
            Data = data;
            Parse(data);
        }

        private void Parse(ISFSObject data)
        {
            GameTools.TryGetValueWitheKey(data, out Type, RequestKey.KeyType);
            GameTools.TryGetValueWitheKey(data, out Cards, RequestKey.KeyCards);
            GameTools.TryGetValueWitheKey(data, out Sit, RequestKey.KeySeat);
            GameTools.TryGetValueWitheKey(data, out OpCard, RequestKey.KeyOpCard);
            GameTools.TryGetValueWitheKey(data, out Card, RequestKey.KeyCard);
            GameTools.TryGetValueWitheKey(data, out Op, RequestKey.KeyMenuOperation);
            GameTools.TryGetValueWitheKey(data, out Seq, RequestKey.KeySeq);
            if (Cards.Length > 0)
            {
                if (Card == 0)
                {
                    Card = Cards[0];
                }
            }
            if (Op > 0)
            {
                GameTools.TryGetValueWitheKey(data, out TingOutCards, RequestKey.KeyTingOutCards);
            }
        }
    }

    /// <summary>
    /// 大结算的数据
    /// </summary>
    public class OverData
    {
        public int gang;
        public int anGang;
        public int gold;
        public string nick;
        public int pao;
        public int hu;
        public int id;
        public int seat;
        public int zimo;
        /// <summary>
        /// 目前进入游戏后玩家的座位号是不变的，头像直接从对应玩家的牌桌上拿来，节省一次下载头像的操作。
        /// </summary>
        public UITexture avatar;
        public bool isPaoShou;
        public bool isYingJia;

        public OverData(ISFSObject data)
        {
            Parse(data);
        }

        private void Parse(ISFSObject user)
        {
            GameTools.TryGetValueWitheKey(user, out gang, RequestKey.KeyGangScore);
            GameTools.TryGetValueWitheKey(user, out anGang, RequestKey.KeyAnGangScore);
            GameTools.TryGetValueWitheKey(user, out gold, RequestKey.KeyGold);
            GameTools.TryGetValueWitheKey(user, out nick, RequestKey.KeyNickName);
            GameTools.TryGetValueWitheKey(user, out pao, RequestKey.KeyPaoScore);
            GameTools.TryGetValueWitheKey(user, out hu, RequestKey.KeyHuScore);
            GameTools.TryGetValueWitheKey(user, out id, RequestKey.KeyId);
            GameTools.TryGetValueWitheKey(user, out zimo, RequestKey.KeyZimoScore);
            GameTools.TryGetValueWitheKey(user, out seat, RequestKey.KeySeat);
        }
    }
    /// <summary>
    /// 投票数据
    /// </summary>
    public class HupData
    {
        public int ID;
        public string Name;
        public int Operation;//-1:拒绝 2.发起 3同意。

        public void Parse(ISFSObject data)
        {

        }
    }
    /// <summary>
    /// 聊天数据
    /// </summary>
    public class TalkData
    {
        public AudioClip Clip;
        public int Seat;
        public int Lenth;
    }
    #endregion
}