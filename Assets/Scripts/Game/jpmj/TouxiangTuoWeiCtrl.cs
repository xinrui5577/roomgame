using System.Collections;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameUI;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj
{
    /// <summary>
    /// 头像拖尾效果问题的的控制
    /// </summary>
    public class TouxiangTuoWeiCtrl : MonoBehaviour
    {

        [SerializeField]
        protected GameObject CurrParticle;

        [SerializeField] protected UserInfo CurUserInfo;

        void Awake()
        {
            CurUserInfo.OnIsCurrValueChange = OnIsCurrValueChange;
        }

        private bool _isCurr;
        private Coroutine _playParticle;
        private void OnIsCurrValueChange(bool value)
        {
            _isCurr = value;
            if (_playParticle!=null)
            {
                StopCoroutine(_playParticle);
            }
            if (_isCurr)
                _playParticle=StartCoroutine(PlayCurParticle());
            else
                CurrParticle.SetActive(false);
        }
        /// <summary>
        /// 标记是否已经初始化了边框特效
        /// </summary>
        private bool _hasPlayCurparitcle;
        private IEnumerator PlayCurParticle()
        {
            if (_hasPlayCurparitcle) yield break;
            yield return new WaitForSeconds(0.5f);
            CurrParticle.SetActive(false);
            CurrParticle.SetActive(_isCurr);
            _hasPlayCurparitcle = _isCurr;
    
        }
    }
}
