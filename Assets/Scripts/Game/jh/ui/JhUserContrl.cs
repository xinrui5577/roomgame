using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhUserContrl : MonoBehaviour
    {
        public List<JhUserButton> Buttons;

        public EventDelegate OnBiPaiClick;
        public EventDelegate OnGzyzClick;
        public EventDelegate OnQuXiaoQiPaiCick;

        public EventDelegate OnLiangPaiClick;
        public EventDelegate OnQiPaiCick;


        public const int Qipai = 1 << 0;
        public const int BiPai = 1 << 1;
        public const int KanPai = 1 << 2;
        public const int JiaZhu = 1 << 3;
        public const int GenZhu = 1 << 4;
        public const int ZiDongGenZhu = 1 << 5;

        public readonly int OnTrun = KanPai | BiPai;

        public readonly int NotTrun = GenZhu | JiaZhu | BiPai | KanPai ;

        public readonly int QiPai = Qipai | BiPai | KanPai | JiaZhu | GenZhu | ZiDongGenZhu;

        public readonly int LiangPai =  BiPai | KanPai | JiaZhu | GenZhu | ZiDongGenZhu;

        public readonly int ZDGZ = Qipai | BiPai | KanPai | JiaZhu | GenZhu ;

        public int LastDisableType;
        public int LastOutSide;
        public void SetBtnShow(int disablebyte,int outSide = 0)
        {
            LastDisableType = disablebyte;
            LastOutSide = outSide;
            for (int i = 0; i < Buttons.Count; i++)
            {
                JhUserButton btn = Buttons[i];
                int type = 1 << i;
                if ((outSide & type) != 0)
                {
                    continue;
                }

                if ((disablebyte & (type)) != 0)
                {
                    btn.SetState(UIButtonColor.State.Disabled, false);
                }
                else
                {
                    btn.SetState(UIButtonColor.State.Normal, true);
                }
            }
        }

        public void ReturnToLastShow(int addDisAble)
        {
            LastDisableType |= addDisAble;
            LastOutSide |= addDisAble;
            LastOutSide ^= addDisAble;

            SetBtnShow(LastDisableType,LastOutSide);
        }

        public void OnTrunShow(bool isLook,bool isCompare)
        {
            int wei = OnTrun;
            if (isLook)
            {
                wei |= KanPai;
                wei ^= KanPai;
            }
            if (isCompare)
            {
                wei |= BiPai;
                wei ^= BiPai;
            }

            SetBtnShow(wei);
            SetBtnBipai();
            SetZiDongGenZhu(false);
            SetLiangPai(false);
        }

        public void NotTrunShow(bool isLook)
        {
//            int wei = NotTrun;
//            if (isLook)
//            {
//                wei |= KanPai;
//                wei ^= KanPai;
//            }
//            SetBtnShow(wei);
//            SetBtnBipai();
//            SetZiDongGenZhu(false);
//            SetLiangPai(false);
        }

        public void QiPaiShow()
        {
            SetBtnShow(QiPai);
            SetBtnBipai();
            SetZiDongGenZhu(false);
            SetLiangPai(false);
        }

        public void ZiDongGenZhuShow()
        {
            SetBtnShow(ZDGZ);
            SetZiDongGenZhu(true);
            SetLiangPai(false);
        }

        public void LiangPaiShow()
        {
            SetBtnShow(LiangPai);
            SetBtnBipai();
            SetZiDongGenZhu(false);
            SetLiangPai(true);
        }

        public void GzyzShow(bool isLook)
        {
            int wei =  KanPai | JiaZhu | GenZhu | ZiDongGenZhu;
            if (isLook)
            {
                wei |= KanPai;
                wei ^= KanPai;
            }
            SetBtnShow(wei);

            JhUserButton bipai = Buttons[1];
            bipai.SetText("孤注一掷");
            bipai.SetOnClick(new List<EventDelegate> { OnGzyzClick });
        }

        public void SetBtnBipai()
        {
            JhUserButton bipai = Buttons[1];
            bipai.SetText("比牌");
            bipai.SetOnClick(new List<EventDelegate> { OnBiPaiClick });
        }

        public void SetZiDongGenZhu(bool isZdgz)
        {
            JhUserButton zdgz = Buttons[5];
            if (isZdgz)
            {
                zdgz.SetText("取消自动跟注");
            }
            else
            {
                zdgz.SetText("自动跟注");
            }
        }

        public void SetLiangPai(bool liang)
        {
            JhUserButton liangpai = Buttons[0];
            if (liang)
            {
                liangpai.SetText("亮牌");
                liangpai.SetOnClick(new List<EventDelegate> { OnLiangPaiClick });
            }
            else
            {
                liangpai.SetText("弃牌");
                liangpai.SetOnClick(new List<EventDelegate> { OnQiPaiCick });
            }
        }

        public void SetQuXiaoBiPai(bool quxiao)
        {
            JhUserButton liangpai = Buttons[1];
            if (quxiao)
            {
                liangpai.SetText("取消比牌");
                liangpai.SetOnClick(new List<EventDelegate> { OnQuXiaoQiPaiCick });
            }
            else
            {
                liangpai.SetText("比牌");
                liangpai.SetOnClick(new List<EventDelegate> { OnBiPaiClick });
            }
        }
    }
}
