using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.Adapters;

/*===================================================
 *文件名称:     FlyLabel.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-07-10
 *描述:        	飞字效果
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class FlyLabel : MonoBehaviour 
    {
        #region UI Param
        [Tooltip("文本")]
        public YxBaseLabelAdapter Label;

        #endregion

        #region Data Param

        private float ExistTime = 2;
        #endregion

        #region Local Data

        private float _addTime;
        #endregion

        #region Life Cycle

        void Awake()
        {
            InvokeRepeating("GetTime",0,1);
        }

        void GetTime()
        {
            _addTime += 1;
            if (_addTime>=ExistTime)
            {
                Invoke("TweenAplha",0);
            }
        }

        void TweenAplha()
        {
            CancelInvoke("GetTime");
            TweenAlpha alpha=TweenAlpha.Begin(gameObject,0.5f,0);
            alpha.PlayForward();
            alpha.SetOnFinished(delegate()
            {
                Destroy(gameObject);
            });
            
        }

        #endregion

        #region Function

        public void SetLabel(string value)
        {
            Label.TrySetComponentValue(value);
        }

        #endregion
    }
}
