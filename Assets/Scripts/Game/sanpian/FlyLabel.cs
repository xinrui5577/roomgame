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
namespace Assets.Scripts.Game.sanpian
{
    public class FlyLabel : MonoBehaviour 
    {
        #region UI Param
        [Tooltip("文本")]
        public YxBaseLabelAdapter Label;
        [Tooltip("飞字胜利颜色")]
        public Color FlyLabelWinColor;
        [Tooltip("飞字失败颜色")]
        public Color FlyLabelLoseColor;
        
        #endregion

        #region Data Param
        [Tooltip("胜利格式")]
        public string WinFormat = "+{0}";
        
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
            if (Label)
            {
                var getValue = float.Parse(value);
                if (getValue > 0)
                {
                    value = string.Format(WinFormat, value);
                    Label.Color = FlyLabelWinColor;
                }
                else
                {
                    value = GetLoseString(value);
                    Label.Color = FlyLabelLoseColor;
                }
                Label.TrySetComponentValue(value);
            }
        }

        private string GetLoseString(string value)
        {
            var str = "";
            foreach (var charItem in value)
            {
                if (charItem != '-')
                {
                    str += string.Format("({0}", charItem);
                }
                else
                {
                    str += charItem;
                }
            }
            return str;
        }

        #endregion
    }
}
