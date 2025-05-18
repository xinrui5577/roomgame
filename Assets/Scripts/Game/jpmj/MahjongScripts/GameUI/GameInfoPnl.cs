using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class GameInfoPnl : MonoBehaviour
    {

        public Text MahjongCnt;
        public RectTransform FanPaiParent;
        public GameObject FanPaiBg;
        public GameObject StartGameAnimate;

        public Text RoundInfo;
        [Tooltip("局数类型信息")]
        public Text RoundTypeInfo;

        public Text RoomID;

        private GameObject _fanPaiCard;

        public RectTransform Round;
        public RectTransform PhoneInfo;
        public RectTransform RoomIDTF;

        public RectTransform Up;
        public RectTransform Down;

        public bool MahjongCntTip = false;
        public int TipNum = 0;

        public virtual void SetMahjongCnt(int cnt)
        {
            MahjongCnt.text = cnt + "";

            if (MahjongCntTip)
            { 
                var gradient = MahjongCnt.GetComponent<Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.UI.Gradient>();
                if (cnt <= TipNum)
                {
                    gradient.enabled = false;
                    MahjongCnt.color = Color.red;
                }                       
                else
                    gradient.enabled = true;
            }          
        }

        public virtual void SetFanPai(int fanpai)
        {
            Tweener move = FanPaiParent.DOMove(FanPaiParent.position,0.3f);
            move.SetEase(Ease.InQuint);
            FanPaiParent.localPosition = Vector3.zero;
            move.OnComplete(OnFanPaiPosFinish);

            _fanPaiCard = D2MahjongMng.Instance.GetMj(fanpai, EnD2MjType.Me);
            _fanPaiCard.transform.parent = FanPaiParent;
            _fanPaiCard.transform.localPosition = Vector3.zero;
            _fanPaiCard.transform.localScale = Vector3.one;
        }

        public virtual void SetFanPaiWithOutAnimation(int fanpai)
        {
            if (fanpai == -1) return;
            FanPaiBg.SetActive(true);

            _fanPaiCard = D2MahjongMng.Instance.GetMj(fanpai, EnD2MjType.Me);
            _fanPaiCard.transform.SetParent(FanPaiParent);
            _fanPaiCard.transform.localPosition = Vector3.zero;
            _fanPaiCard.transform.localScale = Vector3.one;
        }

        public virtual void OnFanPaiPosFinish()
        {
            FanPaiBg.SetActive(true);
        }

        public virtual void HideFanPai()
        {
            FanPaiBg.SetActive(false);
            if(_fanPaiCard!=null)
                Destroy(_fanPaiCard);
        }

        public virtual void SetStartGame()
        {
            //StartGameAnimate.SetActive(true);
            //Timer.StartTimer(1, () =>
            //{
            //    StartGameAnimate.SetActive(false);
            //});
        }

        public virtual void SetRound(string str)
        {
            if(RoundInfo!=null)RoundInfo.text = str;
        }

        public virtual void SetGameInfo(RoomInfo info)
        {
            if (info.RoomType == EnRoomType.YuLe)
            {
                OnYuLeFang();
            }
            else if(info.RoomType==EnRoomType.FanKa)
            {
                SetRound(info.CurrRound + "/" + info.MaxRound);

                if (RoomID != null) RoomID.text = "" + info.RoomID;
            }
            if (RoundTypeInfo)
            {
                var showInfo =info.IsQuanExist? "圈数" : "局数";
                RoundTypeInfo.text = string.Format("对局{0}:", showInfo);
            }
        }

        public virtual void OnYuLeFang()
        {
            if (Up != null && Down != null && PhoneInfo != null && RoomIDTF!=null)
            {
                Down.localPosition = Up.localPosition;
                Up.gameObject.SetActive(false);
                PhoneInfo.SetParent(RoomIDTF.parent);
                PhoneInfo.localPosition = RoomIDTF.localPosition;
                RoomIDTF.gameObject.SetActive(false);                
            }
        }
    }
}
