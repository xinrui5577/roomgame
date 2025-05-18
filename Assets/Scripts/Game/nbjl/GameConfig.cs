using System.Collections.Generic;
using UnityEngine;

/*===================================================
 *文件名称:     GameConfig.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-23
 *描述:        	游戏中的部分信息有可能需要进行修改，配置数据在这个脚本里
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class GameConfig : MonoBehaviour
    {
        [Tooltip("下注筹码Grid间隔")]
        public List<int> BetChipGridCells=new List<int>();
        [Tooltip("下注区域名称")]
        public string[] RateNames = { "z", "x", "h", "zd", "xd" };
        [Tooltip("开始下注窗口名称")]
        public string BeginBetWidnowName = "BeginBet";
        [Tooltip("停止下注窗口名称")]
        public string EndBetWidnowName = "EndBet";
        [Tooltip("非下注阶段提示")]
        public string OutOfBetStateNotice = "对不起，未在下注时间不能下注!";
        [Tooltip("下注金额不足")]
        public string MoneyIsNotEnough = "您的金额不足，无法下注!";
        [Tooltip("未下注")]
        public string HaveNotBet = "您未曾下过注，请下注后使用!";
        [Tooltip("新一轮提示")]
        public string NewBigRoundNotice = "新一轮游戏开始!";
        [Tooltip("上庄不能下注提示")]
        public string OnBankerCanNotBet = "正在上庄中，无法下注!";
        [Tooltip("庄分数音效格式")]
        public string ZhuangNumFormat = "zhuang{0}";
        [Tooltip("闲分数音效格式")]
        public string XianNumFormat = "xian{0}";
        [Tooltip("上庄金额不足提示")]
        public string BankerGoldNotEnough = "金额不足，无法申请上庄!";
        [Tooltip("游戏中，玩家处于庄位，下庄申请提示")]
        public string QuitBankerNotice = "正在上庄中，本局结束自动下庄!";
        [Tooltip("牌显示完毕，等待时间")]
        public float CardShowFinishedWaitTime = 0.8f;
    }
}
