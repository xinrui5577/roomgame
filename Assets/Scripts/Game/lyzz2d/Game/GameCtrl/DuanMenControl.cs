/** 
 *文件名称:     DuanMenControl.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-04-11 
 *描述:         处理选断门效果的脚本
 *历史记录: 
*/

using Assets.Scripts.Game.lyzz2d.Utils.Single;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Game.GameCtrl
{
    public class DuanMenControl : MonoSingleton<DuanMenControl>
    {
        /// <summary>
        ///     不断门
        /// </summary>
        [SerializeField] private GameObject _buDuanMenBtn;

        private float _delaTime;

        /// <summary>
        ///     买断门
        /// </summary>
        [SerializeField] private GameObject _duanMenBtn;

        /// <summary>
        ///     选段门提示界面显示时间
        /// </summary>
        [SerializeField] private readonly float _duanMenNoticeShowTime = 5;

        /// <summary>
        ///     选段门显示剩余时间
        /// </summary>
        private float _duanMenShowTime;

        /// <summary>
        ///     选段们图片
        /// </summary>
        [SerializeField] private UISprite _duanMenSprite;

        /// <summary>
        ///     来源位置
        /// </summary>
        [SerializeField] private Transform _fromPos;

        [SerializeField] private readonly float _scaleTime = 0.3f;

        /// <summary>
        ///     选断门放置位置
        /// </summary>
        [SerializeField] private Transform _toPos;

        private Lyzz2DGameServer Lyzz2DGame
        {
            get { return App.GetRServer<Lyzz2DGameServer>(); }
        }

        public override void Awake()
        {
            base.Awake();
            Reset();
        }

        private void Start()
        {
            //DealDuanMen();
        }

        public void DealDuanMen()
        {
            var duanMen = 0;
            _duanMenShowTime = _duanMenNoticeShowTime;
            SetDuanMenBtns(true);
            InvokeRepeating("ShowTime", 1, 1);
        }

        private void ShowTime()
        {
            _duanMenShowTime -= 1;
            if (_duanMenShowTime <= 0)
            {
                _duanMenShowTime = 0;
            }
            JudegeShowTime();
        }

        private void JudegeShowTime()
        {
            if (_duanMenShowTime.Equals(0))
            {
                SetDuanMenBtns(false);
            }
        }

        public void SetDuanMen(float time)
        {
            _duanMenSprite.gameObject.SetActive(true);
            _duanMenSprite.transform.localPosition = _fromPos.localPosition;
            _delaTime = time;
            float showScaleTime;
            if (time > 0)
            {
                showScaleTime = _scaleTime;
            }
            else
            {
                showScaleTime = 0;
            }
            iTween.ScaleTo(_duanMenSprite.gameObject, Vector3.one, showScaleTime);
            Invoke("SacleTo", showScaleTime);
        }

        private void SacleTo()
        {
            CancelInvoke("SacleTo");
            iTween.MoveTo(_duanMenSprite.gameObject, _toPos.position, _delaTime);
            Invoke("Moto", _delaTime);
        }

        private void Moto()
        {
            CancelInvoke("Moto");
            _duanMenSprite.transform.localPosition = _toPos.localPosition;
        }

        public void Reset()
        {
            _duanMenSprite.gameObject.SetActive(false);
            _duanMenSprite.transform.localPosition = Vector3.zero;
            _duanMenSprite.transform.localScale = Vector3.zero;
            SetDuanMenBtns(false);
        }

        public void OnClickDuanMenBtn(GameObject obj)
        {
            var num = int.Parse(obj.name);
            _duanMenShowTime = 0;
            JudegeShowTime();
            Lyzz2DGame.ResuqestDuanMen(num);
        }

        private void SetDuanMenBtns(bool state)
        {
            _duanMenBtn.SetActive(state);
            _buDuanMenBtn.SetActive(state);
        }
    }
}