using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Game.GameCtrl
{
    public class InteraptMenu : MonoSingleton<InteraptMenu>
    {
        [SerializeField] private Transform _formTrans;

        [SerializeField] private UIGrid _grid;

        private bool _isShowing;

        [SerializeField] private Transform _toTrans;

        public GameObject[] Btns;

        /// <summary>
        ///     当前玩家
        /// </summary>
        public GameObject Target;

        public void ShowMenu(MulSetting mulSetting, int timeOut)
        {
            gameObject.SetActive(true);
            _isShowing = true;
            //胡杠碰吃过         
            YxDebug.Log("mulSetting is " + mulSetting.GetValue());
            for (var i = -1; i < Btns.Length - 1; i++)
            {
                var isActive = mulSetting.IsAllowAt(i);
                Btns[i + 1].SetActive(isActive);
            }
            Btns[0].SetActive(true);
            _grid.repositionNow = true;
            var tween = GetComponent<TweenPosition>();
            tween.from = _formTrans.position;
            tween.to = _toTrans.position;
            tween.enabled = true;
        }

        public void SetTarget(GameObject target)
        {
            gameObject.SetActive(false);
            Target = target;
        }

        public void Chi()
        {
            Revert();
            Target.SendMessage("OnChiClick");
        }

        public void Peng()
        {
            Revert();
            Target.SendMessage("OnPengClick");
        }

        public void Gang()
        {
            Revert();
            Target.SendMessage("OnGangClick");
        }

        public void Hu()
        {
            Revert();
            Target.SendMessage("OnHuClick");
        }

        public void JueGang()
        {
            Revert();
            Target.SendMessage("OnJueClick");
        }

        public void XuanFengGang()
        {
            Revert();
            Target.SendMessage("OnXuanFengGangClick");
        }

        public void Liang()
        {
            Revert();
            YxDebug.LogError("目前没有亮牌的处理");
            Target.SendMessage("OnLiangClick");
        }

        public void Yao()
        {
            Revert();
            Target.SendMessage("OnYaoClick");
        }

        public void BuZ()
        {
            Revert();
            Target.SendMessage("OnBuZClick");
        }


        public void Revert()
        {
            if (_isShowing)
            {
                _isShowing = false;
                var tween = GetComponent<TweenPosition>();
                tween.from = _toTrans.position;
                tween.to = _formTrans.position;
                tween.enabled = true;
                for (var i = 0; i < Btns.Length; i++)
                {
                    Btns[i].SetActive(false);
                }
            }
        }

        public void CancelClick()
        {
            Revert();
            Target.SendMessage("OnCancelClick");
        }

        private void OnFinished(UITweener tween)
        {
            gameObject.SetActive(false);
        }

        public void Ting()
        {
            Revert();
            Target.SendMessage("OnTingClick");
        }
    }
}