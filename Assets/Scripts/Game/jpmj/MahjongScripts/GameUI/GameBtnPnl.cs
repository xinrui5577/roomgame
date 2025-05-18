using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public.Adpater;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.View;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class GameBtnPnl : MonoBehaviour
    {
        private EnRoomType RoomType;

        public GameObject ShareBtn;

        public GameObject ZiDong;

        public GameObject GameHelp;

        /// <summary>
        /// GPS界面按钮
        /// </summary>
        public GameObject GPSBtn;

        /// <summary>
        /// 游戏准备按钮
        /// </summary>
        public GameObject GameReadyBtn;

        public void OnSettingBtnClick()
        {
            CloseXiaLa();
            EventDispatch.Dispatch((int)UIEventId.ShowSettintPnl, new EventData());
        }

        public void OnChatBtnClick()
        {
            EventDispatch.Dispatch((int)UIEventId.ShowChatPnl, new EventData());

        }

        public void SetSharBtnActive(bool active)
        {
            if (ShareBtn!=null) ShareBtn.gameObject.SetActive(active);
            fanhuiFlag = active;
        }

        public void OnShareBtnClick()
        {
            EventDispatch.Dispatch((int)ShareEventID.OnWeiChatShareTableInfo, new EventData());
        }

        public void SetRoomInfo(RoomInfo info)
        {
            RoomType = info.RoomType;
            if (info.RoomType==EnRoomType.YuLe)
            {
                SetSharBtnActive(false);
                
            }
        }

        private bool zidongdapai_bool = false;
        public void ZiDongDaPai(Text text)
        {
            zidongdapai_bool = !zidongdapai_bool;
            if (zidongdapai_bool)
            {
                text.text = "取消自动";
            }
            else
            {
                text.text = "自动打牌";
            }
            EventDispatch.Dispatch((int)NetEventId.OnZiDongDaPai, new EventData());
        }

        void Start()
        {
            if (null != GameAdpaterManager.Singleton)
            {
                transform.FindChild("ChatBtn").GetComponent<RectTransform>().anchoredPosition3D = GameAdpaterManager.Singleton.GetConfig.ChatBtn_Pos;
            }          

            for (int i = 0; i < XiaLaArr.Length; i++)
            {
                XiaLaArr[i].SetActive(false);
            }
            if (XiaLaPostion)
            {
                BtVec = XiaLaPostion.position;
            }
            if (GroupBg)
            {
                GroupBg.SetActive(false);
            }
            for (int i = 0; i < XiaLaArr.Length; i++)
            {
                XiaLaArr[i].transform.position = new Vector3(XiaLaArr[i].transform.position.x, XiaLaPostion.position.y);
            }
#if MJ_AUTO
            ZiDong.SetActive(true);
#else
            if (ZiDong==null)
            {
                return;
            }
            ZiDong.SetActive(false);
#endif
        }
        /// <summary>
        /// 把解散房间倒数第二,返回大厅放在最后一个来匹配娱乐房
        /// </summary>
        public GameObject[] XiaLaArr;
        private float move_y = 1.3f;
        public Transform XiaLaPostion;
        private Vector3 BtVec=Vector3.zero;
        private bool IsMove = false;
        public GameObject GroupBg;
        //开放模式的返回按钮是否显示，跟微信邀请按钮同步
        private bool fanhuiFlag = true;
        public GameObject ChangRoomBt;
        public void XiaLa()
        {
            if (RoomType == EnRoomType.YuLe&&XiaLaArr.Length>1)
            {
                XiaLaArr[XiaLaArr.Length - 2] = ChangRoomBt;
            }
            if ( XiaLaArr == null || XiaLaArr.Length == 0 || !XiaLaPostion )
            {
                return;
            }
            IsMove = true;

            if (XiaLaArr[0].transform.position.y < XiaLaPostion.position.y - 0.5f)
            {
                CloseXiaLa();
                return;
            }
            GroupBg.SetActive(true);
            for (int i = 0; i < XiaLaArr.Length; i++)
            {
                if (i != XiaLaArr.Length - 1)
                {
                    XiaLaArr[i].SetActive(true);
                }
                else
                {
                    XiaLaArr[i].SetActive(false);
                }
                XiaLaArr[i].transform.position = new Vector3(XiaLaArr[i].transform.position.x,XiaLaPostion.position.y);
                if ((App.GameData.GStatus == YxEGameStatus.Over||App.GameData.GStatus==YxEGameStatus.Normal)&&RoomType==EnRoomType.YuLe)
                {
                    XiaLaArr[i].SetActive(true);
                }
                if (RoomType==EnRoomType.FanKa&&fanhuiFlag)
                {
                    XiaLaArr[i].SetActive(true);
                }
                
            }
            for (int i = 0; i < XiaLaArr.Length; i++)
            {
                Debug.Log(" ===== pos == " + new Vector3(XiaLaArr[i].transform.position.x, XiaLaPostion.position.y - move_y * (i + 1)) + " === ");
                iTween.MoveTo(XiaLaArr[i], new Vector3(XiaLaArr[i].transform.position.x, XiaLaPostion.position.y-move_y * (i + 1)), 0.5f);
            }            
        }


        public void CloseXiaLa()
        {
            if (XiaLaArr==null||XiaLaArr.Length<1)
            {
                return;
            }
            if ( XiaLaArr == null || XiaLaArr.Length == 0 || !XiaLaPostion )
            {
                return;
            }
            GroupBg.SetActive(false);
            for (int i = 0; i < XiaLaArr.Length; i++)
            {
                iTween.MoveTo(XiaLaArr[i], new Vector3(XiaLaArr[i].transform.position.x, XiaLaPostion.position.y), 0.5f);
            }
        }
                
        public void OnShowDismissPnlClick()
        {
            EventDispatch.Dispatch((int)UIEventId.OnShowDismissPnl);        
        }

        public void OnExistRoomForSure()
        {
            //CloseXiaLa();
            YxMessageBox.Show("是否要解散房间？", null, (box, btnName) =>
            {
                if (btnName.Equals(YxMessageBox.BtnLeft))
                {
                    EventDispatch.Dispatch((int)NetEventId.OnDismissRoom, new EventData(EnDismissFeedBack.ApplyFor));
                }
            }, false, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
        }

        public void OnChangeRoomClick()
        {
            CloseXiaLa();
            if (App.GameData.GStatus != YxEGameStatus.Normal)
            {
                YxMessageBox.Show("游戏中不能换房间！");
            }
            else
            {
                EventDispatch.Dispatch((int)NetEventId.OnChangeRoom);
            }
        }

        public void OnExistRoom()
        {
            CloseXiaLa();
            App.QuitGame() ;
        }

        public void OnGameHelp()
        {
            GameHelp.SetActive(true);
        }

        public void OnShowGPSClick()
        {
            EventDispatch.Dispatch((int)UIEventId.ShowGPSInfo, new EventData());
        }

        public void InitGPSBtnActive()
        {
            if (GPSBtn == null)
                return;

            GPSBtn.SetActive(Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.UtilData.CurrGamePalyerCnt > 2);

        }

        public void OnGameReadyClick()
        {
            EventDispatch.Dispatch((int)NetEventId.OnUserReady, new EventData());
        }
    }
}
