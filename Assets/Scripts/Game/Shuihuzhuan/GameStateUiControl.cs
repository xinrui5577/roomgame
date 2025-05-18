using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Enums;

namespace Assets.Scripts.Game.Shuihuzhuan
{
    public class GameStateUiControl : MonoBehaviour
    {
        public static GameStateUiControl instance;
        public Button BeginButton;

        public Button StopButton;

        public Button DefenButton;

        public Button JianZhuButton;

        public Button BiBeiButton;

        public Button ZiDongButton;

        public Button ShouDongButton;
        public Button Esc;
        public Button TiShi;
        void Awake()
        {
            instance = this;
             ChangeToWait(); 
        }
        public void ChangeToWait()//初始化
         {
            ZiDongButton.interactable = false;
            ShouDongButton.interactable = false;
            BeginButton.interactable = false;
            //StopButton.interactable = false;
            DefenButton.interactable = false;
            JianZhuButton.interactable = false;
            BiBeiButton.interactable = false;
            Esc.interactable = false;
            TiShi.interactable = false;

        }

        public void Isaudt()//自动设置
        {
            App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
            ZiDongButton.interactable = true ;
            ShouDongButton.interactable = true;
            BeginButton.interactable = true;
            //StopButton.interactable = true;
            DefenButton.interactable = false;
            JianZhuButton.interactable = false;
            BiBeiButton.interactable = false;
            Esc.interactable = false;
            TiShi.interactable = false;
        }

        public void StartWait()//点击开始
        {
            App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
            BeginButton.interactable = false;
            StopButton.interactable = true;
            DefenButton.interactable = false;
            JianZhuButton.interactable = false;
            BiBeiButton.interactable = false;
            ZiDongButton.interactable = true;
            ShouDongButton.interactable = true;
            Esc.interactable = false;
            TiShi.interactable =false;

        }
        /// <summary>
        /// 输了
        /// </summary>
        public void LostWait()
        {
            App.GameData.GStatus = YxEGameStatus.Normal;
            BeginButton.interactable = true;
            //StopButton.interactable = true;
            DefenButton.interactable = false;
            JianZhuButton.interactable = true;
            BiBeiButton.interactable = false;
            ZiDongButton.interactable = true;
            ShouDongButton.interactable = true;
            Esc.interactable = true;
            TiShi.interactable = true;
        }
        /// <summary>
        /// 赢了
        /// </summary>
        public void WinWait()
        {
            BeginButton.interactable = false ;
            StopButton.interactable = false ;
            DefenButton.interactable = true ;
            JianZhuButton.interactable = false ;
            BiBeiButton.interactable = true ;
            ZiDongButton.interactable = true;
            ShouDongButton.interactable = true;
            TiShi.interactable = false;

        }
        /// <summary>
        /// 赢了
        /// </summary>
        public void DeFenWait()
        {
            App.GameData.GStatus = YxEGameStatus.Normal;
            BeginButton.interactable = true;
            StopButton.interactable = true;
            DefenButton.interactable = false ;
            JianZhuButton.interactable = true;
            BiBeiButton.interactable = false ;
            ZiDongButton.interactable = true;
            ShouDongButton.interactable = true;
            Esc.interactable = true ;
            TiShi.interactable = true;

        }

    }
}
