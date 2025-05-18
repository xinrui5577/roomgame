using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class GameInfoSkin1 : AbsGameInfo
    {
        public Text RoomID;
        public Text MahjongCnt;
        public GameInfoLoopType LoopType;
        public RectTransform PhoneInfo;
        public RectTransform RoomidRect;
        public RectTransform Up;
        public RectTransform Down;

        public GameObject HuangZhuangImage;
        public GameObject HuangZhuangSign;
        public Text HuangZhuangCnt;
        public int TwinkleNum = 1;
        public float ShowTime = 0.5f;
        public float HideTime = 0.3f;

        protected bool IsFristShow = false;

        private void Awake()
        {
            GameCenter.EventHandle.Subscriber((int)EventKeys.HuangzhuangTip, HuangzhuangTip);
        }

        private void SetMahjongCntFont(bool isOn)
        {
            MahjongCnt.color = isOn ? Color.white : Color.red;
            MahjongCnt.GetComponent<Gradient>().enabled = isOn;
        }

        public override void OnGetInfoRefresh()
        {
            var db = GameCenter.DataCenter;
            MahjongRoomData data = db.Room;
            ResetHuangZhuangData();
            MahjongCnt.text = db.LeaveMahjongCnt.ToString();
            CheackHuangZhuangAndShow();
            if (db.Room.RoomType == MahRoomType.YuLe)
            {
                LoopType.SetloopType = (int)MahGameLoopType.Round;
                OnYuLeFang();
            }
            else
            {
                LoopType.SetRecord = data.CurrRound + "/" + data.MaxRound;
                LoopType.SetloopType = (int)data.LoopType;
                RoomID.text = "" + data.RoomID;
            }
        }

        public override void OnReadyRefresh()
        {
            MahjongCnt.color = Color.white;
            MahjongRoomData data = GameCenter.DataCenter.Room;
            ResetHuangZhuangData();
            MahjongCnt.text = GameCenter.DataCenter.LeaveMahjongCnt.ToString();
            if (GameCenter.DataCenter.Room.RoomType == MahRoomType.YuLe)
            {
                OnYuLeFang();
            }
            else
            {
                LoopType.SetRecord = data.CurrRound + "/" + data.MaxRound;
                RoomID.text = "" + data.RoomID;
            }
        }

        //lisi--游戏开始刷新局数--start--
        public override void OnStartGameUpdate()
        {
            base.OnStartGameUpdate();
            if (GameCenter.DataCenter.Room.RoomType == MahRoomType.FanKa)
            {
                MahjongRoomData data = GameCenter.DataCenter.Room;
                LoopType.SetRecord = data.CurrRound + "/" + data.MaxRound;
                LoopType.SetloopType = (int)data.LoopType;
                RoomID.text = "" + data.RoomID;
            }
        }
        //lisi--end--
        public override void UpdateMahjongCount(GameInfoArgs args)
        {
            MahjongCnt.text = GameCenter.DataCenter.LeaveMahjongCnt.ToString();
            //if (HuangZhuangCnt != null) HuangZhuangCnt.text = GameCenter.DataCenter.LeaveMahjongCnt.ToString();
            CheackHuangZhuangAndShow();
        }

        public void HuangzhuangTip(EvtHandlerArgs args)
        {
            //if (HuangZhuangCnt != null) HuangZhuangCnt.text = GameCenter.DataCenter.LeaveMahjongCnt.ToString();          
            CheackHuangZhuangAndShow();
        }

        public virtual void OnYuLeFang()
        {
            if (Up != null && Down != null && PhoneInfo != null && RoomidRect != null)
            {
                Down.localPosition = Up.localPosition;
                Up.gameObject.SetActive(false);
                PhoneInfo.SetParent(RoomidRect.parent);
                PhoneInfo.localPosition = RoomidRect.localPosition;
                RoomidRect.gameObject.SetActive(false);
            }
        }

        protected void ResetHuangZhuangData()
        {
            SetMahjongCntFont(true);
            //MahjongCnt.gameObject.SetActive(true);
            //if (HuangZhuangCnt != null) HuangZhuangCnt.gameObject.SetActive(false);
            if (HuangZhuangSign != null) HuangZhuangSign.SetActive(false);
            if (HuangZhuangImage != null) HuangZhuangImage.SetActive(false);
            IsFristShow = false;
            StopCoroutine("ShowHuangZhuangImage");
            StopCoroutine("ShowTooltip");
        }

        private void CheackHuangZhuangAndShow()
        {
            if (GameCenter.DataCenter.ConfigData.HuangZhuang && GameCenter.DataCenter.Game.HuangZhuang && !IsFristShow)
            {
                SetMahjongCntFont(false);
                //MahjongCnt.color = Color.red;
                //if (HuangZhuangCnt != null)
                //{
                //    HuangZhuangCnt.gameObject.SetActive(true);
                //    MahjongCnt.gameObject.SetActive(false);
                //    HuangZhuangCnt.text = GameCenter.DataCenter.LeaveMahjongCnt.ToString();
                //}
                if (HuangZhuangImage != null)
                {
                    //显示即将黄庄界面
                    StartCoroutine("ShowHuangZhuangImage");
                }
                if (HuangZhuangSign != null)
                {
                    //闪烁剩余牌数的提示框
                    StartCoroutine("ShowTooltip");
                }
                IsFristShow = true;
            }
        }

        private IEnumerator ShowHuangZhuangImage()
        {
            var show = new WaitForSeconds(ShowTime * 2);
            var hide = new WaitForSeconds(HideTime * 2);
            var anim = HuangZhuangImage.GetComponent<Animator>();
            if (anim != null)
            {
                HuangZhuangImage.SetActive(true);
                anim.enabled = true;
            }
            else
            {
                for (int i = 0; i < TwinkleNum; i++)
                {
                    HuangZhuangImage.SetActive(true);
                    yield return show;
                    HuangZhuangImage.SetActive(false);
                    yield return hide;
                }
            }
        }

        private IEnumerator ShowTooltip()
        {
            var show = new WaitForSeconds(ShowTime * 2);
            var hide = new WaitForSeconds(HideTime * 2);
            var image = HuangZhuangSign.GetComponent<Image>();
            if (image != null)
            {
                HuangZhuangSign.SetActive(true);
                image.DOFade(1, 0);
                yield return show;
                while (true)
                {
                    image.DOFade(0, HideTime);
                    yield return hide;
                    image.DOFade(1, ShowTime);
                    yield return show;
                }
            }
            else
            {
                while (true)
                {
                    HuangZhuangSign.SetActive(true);
                    yield return hide;
                    HuangZhuangSign.SetActive(false);
                    yield return show;
                }
            }

        }
    }
}