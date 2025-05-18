
namespace Assets.Scripts.Game.LXGameScripts
{
    public class EventID
    {
        public enum GameEventId
        {
            /// <summary>
            /// 更改彩池的金币
            /// </summary>
            AlterPotGold = 100,
            /// <summary>
            /// 赢的时候显示红线
            /// </summary>
            ShowRedLineIfWin,
            /// <summary>
            /// 更改本局得分
            /// </summary>
            AlterRoundScore,
            /// <summary>
            /// 点击开始按钮
            /// </summary>
            OnStartClick,
            /// <summary>
            /// 点击停止按钮
            /// </summary>
            OnStopClick,
            /// <summary>
            /// 开始滚动图片
            /// </summary>
            StartRollJetton,
            /// <summary>
            /// 初始化下注数据
            /// </summary>
            InitBottomPourData,
            /// <summary>
            /// 开启托管
            /// </summary>
            OnTrusteeshipOpen,
            /// <summary>
            /// 取消托管
            /// </summary>
            OnCancelTrusteeship,
            /// <summary>
            /// 当图片停止的时候
            /// </summary>
            WhenIconStop,
            /// <summary>
            /// 获取自动停止时图片的显示顺序
            /// </summary>
            GetIconOrder,
            /// <summary>
            /// 初始本局得分
            /// </summary>
            ClearRoundScore,
            /// <summary>
            /// 显示设置面板
            /// </summary>
            SettingBtnClick,
            /// <summary>
            /// 退出游戏到大厅
            /// </summary>
            OnCloseBtnClick,
            /// <summary>
            /// 隐藏所有特效
            /// </summary>
            HideAllEffect,
            /// <summary>
            /// 播放除砸蛋的特效
            /// </summary>
            PlayEffect,
            /// <summary>
            /// 播放砸蛋特效
            /// </summary>
            PlayZaDanEffect,
            /// <summary>
            /// 更改按钮贴图
            /// </summary>
            ChangeButtonIcon,
            /// <summary>
            /// 播放投币动画
            /// </summary>
            PlayInsertCoinsAnim,
        }
    }
}

