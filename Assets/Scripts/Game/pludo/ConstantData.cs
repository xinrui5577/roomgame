/*===================================================
 *文件名称:     ConstantData.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-18
 *描述:        	常量数据
 *历史记录: 
=====================================================*/

using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.pludo
{
    public class ConstantData 
    {
        #region Cargs Keys
        /// <summary>
        /// Key 解散总时间
        /// </summary>
        public const string KeyCargsHupTime = "-tptout";
        /// <summary>
        /// Key 打骰子时间
        /// </summary>
        public const string KeyCargsRollDicTime = "-rtime";
        /// <summary>
        /// Key 选择飞机时间
        /// </summary>
        public const string KeyCargsCpDicTime = "-cptime";
        #endregion
        #region  Request Key
        /// <summary>
        /// Key 地图信息
        /// </summary>
        public const string KeyMapInfo= "mapinfo";
        /// <summary>
        /// Key 颜色(人物，地图，飞机等各种颜色)
        /// </summary>
        public const string KeyColor = "color";
        /// <summary>
        /// Key 地图Item Id
        /// </summary>
        public const string KeyMapId = "num";
        /// <summary>
        /// Key 地图Item 类型
        /// </summary>
        public const string KeyMapType = "type";
        /// <summary>
        /// Key 房间类型
        /// </summary>
        public const string KeyRoomType = "gtype";
        /// <summary>
        /// Key 地图Item 类型
        /// </summary>
        public const string KeyRule= "rule";
        /// <summary>
        /// Key服务器当前时间
        /// </summary>
        public const string KeyServerNowTime = "svt";
        /// <summary>
        /// Key 投票起始时间
        /// </summary>
        public const string KeyHupStartTime = "hupstart";
        /// <summary>
        /// Key 阶段开始时间
        /// </summary>
        public const string KeyStateStartTime = "st";
        /// <summary>
        /// Key房主投票解散主命令（第一局为开局时使用）
        /// </summary>
        public const string KeyCommondHandsUpByOwner = "dissolve";
        /// <summary>
        /// 命令
        /// </summary>
        public const string KeyCommond = "cmd";
        /// <summary>
        /// Key投票解散主命令（局内通用解散）
        /// </summary>
        public const string KeyCommondHandsUp = "dismiss";
        /// <summary>
        /// Key投票解散子命令（局内通用解散）
        /// </summary>
        public const string KeyHandsUp = "hup";
        /// <summary>
        /// Key游戏总结算
        /// </summary>
        public const string KeyGameOver = "over";
        /// <summary>
        /// Key投票解散子命令（局内通用解散）
        /// </summary>
        public const string KeyCurrentPlayer = "cp";
        /// <summary>
        /// Key玩家 操作状态
        /// </summary>
        public const string KeyPlayerStatus = "status";
        /// <summary>
        /// Key飞机状态
        /// </summary>
        public const string KeyPlaneStatus = "status";
        /// <summary>
        /// Key合体飞机列表
        /// </summary>
        public const string KeyFitList = "fitlist";
        /// <summary>
        /// Key 飞机数量
        /// </summary>
        public const string KeyPlaneNum = "planeNum";
        /// <summary>
        /// Key 遥控骰子消耗钻石数量
        /// </summary>
        public const string KeyControlPointConsume = "controlPointConsume";
        /// <summary>
        /// Key 飞机信息
        /// </summary>
        public const string KeyPlaneInfo = "planeinfo";
        /// <summary>
        /// Key 飞机当前位置
        /// </summary>
        public const string KeyLocalPosition = "local";
        /// <summary>
        /// Key 托管
        /// </summary>
        public const string KeyAuto = "tuoguan";
        /// <summary>
        /// Key 飞机ID
        /// </summary>
        public const string KeyPlaneId = "id";
        /// <summary>
        /// Key 可以选择的飞机
        /// </summary>
        public const string KeyChoosePlane= "chooseplane";
        /// <summary>
        /// Key 骰子点数
        /// </summary>
        public const string KeyRollPoint = "point";
        /// <summary>
        /// Key 默认图片名称
        /// </summary>
        public const string KeyDefaultSpriteName = "DefSpriteName";
        /// <summary>
        /// Key 再来一次
        /// </summary>
        public const string KeyMoreTime = "more";
        /// <summary>
        /// Key 玩家的飞机信息变化状态
        /// </summary>
        public const string KeyChangeInfo = "change";
        /// <summary>
        /// Key 玩家的飞机信息变化状态
        /// </summary>
        public const string KeyMovePath = "path";
        /// <summary>
        /// Key 遥控骰子可以使用数量
        /// </summary>
        public const string KeyControlDiceTime = "canusetime";
        /// <summary>
        /// Key是否使用了遥控骰子(打骰子交互中使用)
        /// </summary>
        public const string KeyUseControlDice = "usecash";
        /// <summary>
        /// Key剩余元宝数量
        /// </summary>
        public const string KeyCash = "cash";
        /// <summary>
        /// Key 遥控骰子可以使用数量
        /// </summary>
        public const string KeyCargsControlDice = "-cotroltimes";
        /// <summary>
        /// Key 攻击次数
        /// </summary>
        public const string KeyAttackNum = "attack";
        /// <summary>
        /// Key 被攻击次数
        /// </summary>
        public const string KeyBeAttackNum = "beattack";
        /// <summary>
        /// Key 是否零起飞
        /// </summary>
        public const string KeyFinishNum = "finish";
        /// <summary>
        /// Key 大赢家
        /// </summary>
        public const string KeyWinner = "winner";
        /// <summary>
        /// Key 局信息
        /// </summary>
        public const string KeyRoundInfo = "ju_info";
        /// <summary>
        /// Key 投票信息分割符
        /// </summary>
        public const char KeyHupSpliteFlag = ',';
        /// <summary>
        /// Key摇色子点格式，参数为显示点数
        /// </summary>
        public const string KeyRollDicePointFormat = "RollPoint_{0}";

        #endregion
        #region Sound Key

        /// <summary>
        /// Key男音效包
        /// </summary>
        public const string KeyManSoundPage = "Man";

        /// <summary>
        /// Key女音效包
        /// </summary>
        public const string KeyWomanSoundPage = "Woman";

        /// <summary>
        /// Key起飞音效
        /// </summary>
        public const string KeyStarFly = "StartFly";

        /// <summary>
        /// Key 游戏开始音效
        /// </summary>
        public const string KeyGameBegin = "GameBegin";
        /// <summary>
        /// Key再来一次音效
        /// </summary>
        public const string KeyRollAgin = "RollAgain";
        /// <summary>
        /// Key遥控骰子音效
        /// </summary>
        public const string KeyControlDice= "ControlDice";
        /// <summary>
        /// Key飞机爆炸音效
        /// </summary>
        public const string KeyPlaneBoom = "PlaneBoom";
        /// <summary>
        /// Key飞机完成音效
        /// </summary>
        public const string KeyPlaneFinish= "PlaneFinish";
        #endregion
        #region Game Constant Data
        /// <summary>
        /// 创建房间状态标识
        /// </summary>
        public const int CreateRoomType = -1;
        /// <summary>
        /// int 默认值（未初始化）
        /// </summary>
        public const int IntDefValue = -1;
        /// <summary>
        /// int 默认值（重置数值）
        /// </summary>
        public const int IntValue = 0;
        /// <summary>
        /// 当前玩家UI座位号
        /// </summary>
        public const int CurUiSeat = 0;
        /// <summary>
        /// 为保持地图id唯一，设置最后一格的ID为指定Id
        /// (服务器返回数据中每种颜色最后一格ID不唯一，不便于统一处理最后一格UI显示)
        /// </summary>
        public const int FinalMapId = 10000;

        /// <summary>
        /// 默认颜色样式(参数1:颜色；参数2:UI样式)
        /// </summary>
        public const string DefColorFormat = "{0}_{1}";
        /// <summary>
        /// 小结算标题格式(参数为颜色)
        /// </summary>
        public const string ResultTitleFormat = "{0}_ResultTitle";
        /// <summary>
        /// 投票默认时长
        /// </summary>
        public const int ValueHupDefTime = 300;
        /// <summary>
        /// 选择飞机默认时长
        /// </summary>
        public const int ValueCpDefTime = 300;
        /// <summary>
        /// 默认打骰子时长
        /// </summary>
        public const int ValueRollDicDefTime = 300;
        /// <summary>
        /// Cd 秒
        /// </summary>
        public const float ValueCdSecond = 1.0f;
        /// <summary>
        /// 飞机旋转z轴默认偏移
        /// </summary>
        public const float ValueRoateOffsetZ = -90;
        /// <summary>
        /// 最后一个飞机的ID
        /// </summary>
        public const int ValueLastPlaneId = 2;
        /// <summary>
        /// 飞机数量
        /// </summary>
        public const int ValuePlaneCount = 3;
        /// <summary>
        /// 准备区地图基础数值（服务器值给了状态值，为了本地区分查找方便，自定义一个数据）
        /// </summary>
        public const int ValueReadyMapItemBase = 10000;
        /// <summary>
        /// 颜色基础值数
        /// </summary>
        public const int ValueReadyMapItemColorBase = 100;
        /// <summary>
        /// 准备区数量
        /// </summary>
        public const int ValueReadyAreaCount = 4;
        /// <summary>
        /// 准备点索引
        /// </summary>
        public const int ValueReadyPosIndex = 3;
        /// <summary>
        /// 起始点基础值
        /// </summary>
        public const int ValueBeginStateBase = 32768;
        /// <summary>
        /// 起始点颜色变化值
        /// </summary>
        public const int ValueBeginStatePerChange = 2;

        /// <summary>
        /// 根据性别播放音效
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="clipName"></param>
        public static void PlaySoundBySex(int sex,string clipName)
        {
            var soundPackageName = sex != 1 ? KeyWomanSoundPage : KeyManSoundPage;
            Facade.Instance<MusicManager>().Play(clipName, soundPackageName);
        }

        #endregion
    }
}
