using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.sanpian.DataStore;
using Assets.Scripts.Game.sanpian.item;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.sanpian.server
{
    public class UIButtonCtrl : MonoBehaviour
    {
        //准备按钮
        public GameObject ReadyBt;

        //操作按钮3个
        public GameObject OpBt;

        //中间的分
        public UILabel TableScore;

        public UILabel RoomId;

        public UILabel RoundNum;

        public UILabel RoomIdBig;

        //开始后桌面显示的东西
        public GameObject TableInfoForStart;

        public UILabel MyScore;

        public UILabel EnemyScore;


        public GameObject XueObj;//雪不雪两个按钮

        public UIButton PassBt;//不出按钮

        public GameObject XueTip; //雪牌提示

        private bool Auto = false;

        public Texture2D DefHeadTexture2D;

        public GameObject FriendCardsIcon;

        //正在选择雪牌字
        public GameObject XuePaiZhong;

        public GameObject WeiChatBt;

        public UIButton JieSan;

      
        public void RoJoin(ISFSObject data)
        {
            ReadyBt.SetActive(false);
            AddScore(data.GetInt("tableScore"));
            DisableBigRoomId();
        }


        //本轮的分
        public void AddScore(int value)
        {
            TableScore.text = value+"";
        }

        

        public void DisableBigRoomId()
        {
            if (!App.GetGameData<SanPianGameData>().IsRoomGame)
            {
                RoomId.gameObject.SetActive(false);
                RoomIdBig.gameObject.SetActive(false);
                RoundNum.gameObject.SetActive(false);
            }
            else
            {
                TableInfoForStart.SetActive(true);
                RoomIdBig.gameObject.SetActive(false);
            }
            
        }


        //桌面的分清零
        public void ScoreToZero()
        {
            TableScore.text = "0";
        }

        //准备
        public void ClickReady()
        {
            App.GetRServer<SanPianGameServer>().ClickReadyBt();
        }
        //出牌
        public void ClickOutCards()
        {
            App.GetGameManager<SanPianGameManager>().RealPlayer.ClickOutCards();
        }
        //提示
        public void ClickTiShi()
        {
            App.GetGameManager<SanPianGameManager>().RealPlayer.TiShi();
        }
        //不出
        public void ClickPass()
        {
            ISFSObject data = new SFSObject();
            data.PutInt("realSeat", App.GetGameManager<SanPianGameManager>().RealPlayer.userInfo.Seat);
            App.GetGameManager<SanPianGameManager>().RealPlayer.TiShiIndex = -1;
            data.PutInt(RequestKey.KeyType, (int)LandRequestData.GameRequestType.TypeBuChu);
            App.GetRServer<SanPianGameServer>().SendGameRequest(data);
            OpBt.SetActive(false);
        }

        //雪
        public void ClickXue()
        {
            XueObj.gameObject.SetActive(false);
            ISFSObject data = new SFSObject();
            data.PutInt(RequestKey.KeyType, (int)LandRequestData.GameRequestType.Snow);
            data.PutBool("snow", true);
            App.GetRServer<SanPianGameServer>().SendGameRequest(data);
        }

        //设置
        public void ClickSetting()
        {
            YxWindowManager.OpenWindow("SettingPanel");
        }

        //设置
        public void OnRuleClick()
        {
            YxWindowManager.OpenWindow("RulePanel");
        }

        //不雪
        public void ClickCancelXue()
        {
            XueObj.gameObject.SetActive(false);
            ISFSObject data=new SFSObject();
            data.PutInt(RequestKey.KeyType, (int)LandRequestData.GameRequestType.Snow);
            data.PutBool("snow", false);
            App.GetRServer<SanPianGameServer>().SendGameRequest(data);
        }

        public void ShowOpBt()
        {
            OpBt.SetActive(true);
            if (App.GetGameManager<SanPianGameManager>().lastV==0)
            {
                PassBt.isEnabled = false;

            }
            else
            {
                PassBt.isEnabled = true;
                if (Auto)
                {
                    ClickPass();
                }
            }
            
        }

        public void ShowXue(bool xue)
        {
            GameObject gj = (GameObject)Instantiate(XueTip, Vector3.zero, Quaternion.identity);
            XueTipAni xueTip = gj.GetComponent<XueTipAni>();
            xueTip.StartAni(xue);
        }

        public void AutoPass(UILabel label)
        {
            Auto = !Auto;
            if (Auto)
            {
                label.text = "取消托管";
            }
            else
            {
                label.text = "托管";
            }
        }



        public void Reset()
        {
            OpBt.SetActive(false);
            TableScore.transform.parent.gameObject.SetActive(false);
            ScoreToZero();
            PassBt.isEnabled = true;
            MyScore.text = "0";
            EnemyScore.text = "0";
            FriendCardsIcon.SetActive(false);
        }

        public void OnClickWeiChatBtn()
        {
            var gameData = App.GetGameData<SanPianGameData>();
            if (gameData)
            {
                YxTools.ShareFriend(gameData.RoomID.ToString(), gameData.Rule);
            }
        }

        public void ChangeJieSanSprite(bool LiKai=false)
        {
            string up = "but_034_up";
            string over = "but_034_over";
            if (LiKai)
            {
                up = "but_072_up";
                over = "but_072_over";
            }
            JieSan.normalSprite =up ;
            JieSan.pressedSprite = over;
            Transform son=JieSan.transform.FindChild("Sprite");
            UISprite sonSprite = son.GetComponent<UISprite>();
            sonSprite.MakePixelPerfect();
        }
        
    }
}
