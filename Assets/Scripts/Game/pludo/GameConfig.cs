using UnityEngine;

/*===================================================
 *文件名称:     GameConfig.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-07
 *描述:        	游戏中的部分信息有可能需要进行修改，配置数据在这个脚本里
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo
{
    public class GameConfig : MonoBehaviour
    {
        [HideInInspector]
        public int MessageQueueMaxLenth = 10;
        [Tooltip("大结算窗口名称")]
        public string GameOverWindowName = "GameOverWindow";
        [Tooltip("小结算窗口名称")]
        public string GameResultWindoName = "ResultWindow";
        [Tooltip("设置窗口名称")]
        public string SettingWindoName = "SettingWindow";
        [Tooltip("小结算窗口名称")]
        public string HandWindoName = "HupWindow";
    }
}
