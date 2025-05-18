using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.Helper
{
    public class TrusteeshipHelper : MonoBehaviour
    {
        public int[] TrusteeshipOpArray =
        {
            MjRequestData.MJOpreateTypeGang,
            MjRequestData.MJOpreateTypeHu,                  
            MjRequestData.MJOpreateTypeTing,     
            1 << 12,
        };

        public static TrusteeshipHelper Instance;

        public GameObject DisableTrusteeshipBtn;

        //发牌前不允许托管 ,听之后也不允许托管
        public bool IsAllowTrusteeship
        {
            get { return mIsAllowTrusteeship; }
            set 
            {           
                if (IsTrusteeship && !value)
                {
                    TrusteeshipContorl(false);
                }
                mIsAllowTrusteeship = value;
            }
        }

        public int GameOpreateMenu
        {
            get { return mGameOpreateMenu; }
            set { mGameOpreateMenu = value; }
        }

        public bool IsTrusteeship
        {
            get { return mIsTrusteeship; }
            set { mIsTrusteeship = value; }
        }

        //是否托管
        private bool mIsTrusteeship;
        //是否允许托管
        private bool mIsAllowTrusteeship;
        //操作列表
        private int mGameOpreateMenu;

        void Awake()
        {
            Instance = this;
            EventDispatch.Instance.RegisteEvent((int)UIEventId.DisableTrusteeship, OnDisableTrusteeship);
        }

        //托管
        public virtual void OnTrusteeshipClick()
        {
            if (IsAllowTrusteeship)
            {
                if (mIsTrusteeship) return;
                TrusteeshipContorl(true);
                EventDispatch.Dispatch((int)UIEventId.FgConfirmgBtnCtrl, new EventData(false));
            }
        }

        //取消托管
        public void OnDisableTrusteeshipClick()
        {
            if (mIsTrusteeship) TrusteeshipContorl(false);
        }

        //有操作时，取消托管
        public void OnDisableTrusteeship(int eventId, EventData evn)
        {
            if (mIsTrusteeship) TrusteeshipContorl(false);
        }

        private void TrusteeshipContorl(bool isOn)
        {
            mIsTrusteeship = isOn;
            EventDispatch.Dispatch((int)GameEventId.OnTrusteeship, new EventData() { data1 = mIsTrusteeship });
            DisableTrusteeshipBtn.gameObject.SetActive(mIsTrusteeship);
        }

        public bool IsHaveOperator()
        {
            bool flag = false;
            if (mGameOpreateMenu == 0) return flag;

            for (int i = 0; i < TrusteeshipOpArray.Length; i++)
                if ((mGameOpreateMenu & TrusteeshipOpArray[i]) == TrusteeshipOpArray[i])
                    flag = true;
            return flag;
        }


        public bool IsHaveOperator(int op)
        {
            bool flag = false;
            if (op == 0) return flag;      

            for (int i = 0; i < TrusteeshipOpArray.Length; i++)
                if ((TrusteeshipOpArray[i] & op) == TrusteeshipOpArray[i])
                    flag = true;
            return flag;
        }
    }
}
