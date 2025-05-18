using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
	public class InteraptMenu:MonoSingleton<InteraptMenu>
	{
        [SerializeField]
        private Transform _formTrans;
        [SerializeField]
	    private Transform _toTrans;
        [SerializeField]
        private GameObject _tingCancelBtn;
        [Tooltip("操作按钮")]
        public GameObject[] Btns;
        [Tooltip("按钮布局")]
	    public UITable BtnTabelLayout;
        /// <summary>
        /// 目标
        /// </summary>
        private GameObject _target;
        public override void Awake()
	    {
	        base.Awake();
	        ShowTing(false);
	    }

	    public void ShowMenu(MulSetting mulSetting,int timeOut)
        {
            _isShowing = true;
            //胡杠碰吃过
            for (int i =-1; i < Btns.Length-1; i++)
            {
                bool isActive = mulSetting.IsAllowAt(i);
                Btns[i+1].TrySetComponentValue(isActive);
            }
            if (App.GetGameData<Mahjong2DGameData>().XiaoSha)
            {
                if (mulSetting.GetValue() == 128)
                {
                    Btns[10].TrySetComponentValue(true);
                }
            }
            Btns[0].TrySetComponentValue(true);
            BtnTabelLayout.repositionNow = true;
            transform.localPosition = _toTrans.localPosition;
        }

	    public void ShowTing(bool active)
	    {
	        _isShowing = active;
	        if (_tingCancelBtn!=null)
	        {
               _tingCancelBtn.SetActive(active);
            }
	    }

	    public void SetTarget(GameObject target)
        {
	        _target = target;
	    }

        /// <summary>
        /// 是否处于显示菜单状态
        /// </summary>
	    private bool _isShowing;


	    public void Chi()
	    {
            Revert();
            _target.SendMessage("OnChiClick");
	    }

	    public void Peng()
	    {
            Revert();
            _target.SendMessage("OnPengClick");
	    }

	    public void Gang()
        {
            Revert();
            _target.SendMessage("OnGangClick");
	    }

	    public void Hu()
        {
            Revert();
            _target.SendMessage("OnHuClick");
	    }

        public void NiuTing()
        {
            Revert();
            _target.SendMessage("OnNiuTingClick");
        }
        public void Ting()
        {
            Revert();
            _target.SendMessage("OnTingClick");
        }

        public void JueGang()
	    {
            Revert();
            _target.SendMessage("OnJueClick");
        }

        /// <summary>
        /// 不带乱风玩法，传统旋风杠（显示可杠类型）
        /// </summary>
	    public void XuanFengGang()
	    {
            Revert();
            _target.SendMessage("OnXuanFengGangClick");
        }

        /// <summary>
        /// 统一处理风杠（从手牌中选择）
        /// </summary>
	    public void FengGang()
	    {
            Revert();
            _target.SendMessage("OnFengGangClick");
        }

	    public void SiFengGang()
	    {
	        Revert();
	        _target.SendMessage("OnSiFengGangClick");
        }

	    public void ThreeFengGang()
	    {
	        Revert();
	        _target.SendMessage("OnThreeFengGangClick");
	    }

        public void FengSure()
	    {
            Revert();
            _target.SendMessage("OnClickFengSureBtn");
        }

	    public void FengCancle()
	    {
            Revert();
            _target.SendMessage("OnClickFengCancel");
        }

	    public void Liang()
	    {
            Revert();
            YxDebug.LogError("目前没有亮牌的处理");
            _target.SendMessage("OnLiangClick");
	    }

        public void Yao()
        {
            Revert();
            _target.SendMessage("OnYaoClick");
        }

        public void BuZ()
        {
            Revert();
            _target.SendMessage("OnBuZClick");
        }
        public void CaiGang()
        {
            Revert();
            _target.SendMessage("OnCaiGangClick");
        }
        public void XiaoSa()
        {
            Revert();
            _target.SendMessage("OnXiaoSaClick");
        }

	    public void LiGang()
	    {
            Revert();
            _target.SendMessage("OnClickLiGang");
        }

	    public void Revert()
        {
	        if (_isShowing)
            {
                transform.localPosition = _formTrans.localPosition;
                _isShowing = false;
                for (int i = 0; i < Btns.Length; i++)
                {
                    Btns[i].TrySetComponentValue(false);
                }
                ShowTing(false);
            }
	    }

        public void CancelClick()
        {
            Revert();
            _target.SendMessage("OnCancelClick");
        }
        /// <summary>
        /// 取消听
        /// </summary>
	    public void TingCancelClick()
	    {
            Revert();
            _target.SendMessage("OnClickTingCancel");
        }
	}
}
