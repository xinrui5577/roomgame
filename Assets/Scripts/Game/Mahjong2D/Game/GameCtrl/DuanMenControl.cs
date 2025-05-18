/** 
 *文件名称:     DuanMenControl.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-04-11 
 *描述:         处理选断门效果的脚本
 *历史记录: 
*/

using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    public class DuanMenControl : MonoSingleton<DuanMenControl>
    {
        /// <summary>
        /// 选断门放置位置
        /// </summary>
        [SerializeField]
        private Transform _toPos;
        /// <summary>
        /// 选段们图片
        /// </summary>
        [SerializeField]
        private UISprite _duanMenSprite;
        /// <summary>
        /// 来源位置
        /// </summary>
        [SerializeField]
        private Transform _fromPos;
        /// <summary>
        /// 选段门提示界面显示时间
        /// </summary>
        [SerializeField]
        private float _duanMenNoticeShowTime=5;
        /// <summary>
        /// 选段门显示剩余时间
        /// </summary>
        private float _duanMenShowTime;
        /// <summary>
        /// 买断门
        /// </summary>
        [SerializeField]
        private GameObject _duanMenBtn;
        /// <summary>
        /// 不断门
        /// </summary>
        [SerializeField]
        private GameObject _buDuanMenBtn;
        [SerializeField]
        private float _scaleTime=0.3f;
        /// <summary>
        /// 非庄家的买断门提示
        /// </summary>
        [SerializeField]
        private GameObject _otherDuanMenNotice;
        /// <summary>
        /// 其它人断门提示文本
        /// </summary>
        [SerializeField] private UILabel _duanMenNoticeLabel;

        /// <summary>
        /// 其他人的断门基础提示信息
        /// </summary>
        [SerializeField]
        private string _otherDuanMenBaseInfo="庄家正在选择买断门";
        /// <summary>
        ///其他人断门的增加标记
        /// </summary>
        [SerializeField]
        private string _otherDuanMenAddTag=".";

        private float _delaTime;
        [SerializeField]
        private float _noticeFreshTime=0.3f;

        private int _duanMenBeginIndex=3;

        private int _showIndex=0;

        Mahjong2DGameServer Mahjong2DGame
        {
            get
            {
               return App.GetRServer<Mahjong2DGameServer>();
            }
        }

        public override void Awake()
        {
            base.Awake();
            Reset();
        }
        public void ShowDuanMenBtns(bool isSelf)
        {
            if (isSelf)
            {
                SetDuanMenBtns(true);
            }
            else
            {
                _showIndex = 0;
                if (_otherDuanMenNotice)
                {
                    _otherDuanMenNotice.SetActive(true);
                    InvokeRepeating("OtherDuanMenNotice", 0, _noticeFreshTime);
                }
            }
        }

        public void SetDuanMen(float time)
        {
            Reset();
            _duanMenSprite.gameObject.SetActive(true);
            _duanMenSprite.transform.localPosition = _fromPos.localPosition;
            _delaTime = time;
            float showScaleTime;
            if (time>0)
            {
                showScaleTime = _scaleTime;
            }
            else
            {
                showScaleTime = 0;
            }
            iTween.ScaleTo(_duanMenSprite.gameObject,Vector3.one, showScaleTime);
            Invoke("SacleTo", showScaleTime);
        }

        private void SacleTo()
        {
            CancelInvoke("SacleTo");
            iTween.MoveTo(_duanMenSprite.gameObject, _toPos.position,_delaTime);
            Invoke("Moto",_delaTime);
        }

        private void Moto()
        {
            CancelInvoke("Moto");
            _duanMenSprite.transform.localPosition = _toPos.localPosition;
        }

        public void Reset()
        {
            if (_duanMenSprite)
            {
                CancelInvoke("OtherDuanMenNotice");
                if (_duanMenNoticeLabel)
                {
                    _duanMenNoticeLabel.depth = 3;
                }
                if (_otherDuanMenNotice)
                {
                    _otherDuanMenNotice.SetActive(false);
                }
                _duanMenSprite.gameObject.SetActive(false);
                _duanMenSprite.transform.localPosition = Vector3.zero;
                _duanMenSprite.transform.localScale = Vector3.zero;
                SetDuanMenBtns(false);
            }
        }

        public void OnClickDuanMenBtn(GameObject obj)
        {
            int num = int.Parse(obj.name);
            _duanMenShowTime = 0;
            SetDuanMenBtns(false);
            Mahjong2DGame.ResuqestDuanMen(num);
        }

        private void SetDuanMenBtns(bool state)
        {
            _duanMenBtn.SetActive(state);
            _buDuanMenBtn.SetActive(state);
        }

        private void OtherDuanMenNotice()
        {
            
            string addStr = "";
            for (int i = 0; i < _duanMenBeginIndex+_showIndex; i++)
            {
                addStr += _otherDuanMenAddTag;
            }
            _showIndex = (_showIndex + 1)%3;
            if (_duanMenNoticeLabel)
            {
                _duanMenNoticeLabel.text = string.Format("{0}{1}", _otherDuanMenBaseInfo, addStr);
            }
        }
    }
}
