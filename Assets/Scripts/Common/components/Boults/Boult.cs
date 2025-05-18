using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Common.Components.Boults
{
    /// <summary>
    /// 筛子
    /// </summary>
    public class Boult : MonoBehaviour
    {
        /// <summary>
        /// 最终显示的样式
        /// </summary>
        public YxBaseSpriteAdapter RealStyle;
        /// <summary>
        /// 动画显示
        /// </summary>
        public YxBaseAnimationAdapter AnimationStyle;

        /// <summary>
        /// 样式名称前缀
        /// </summary>
        public string StyleNamePrefix = "{0}";


        protected void Awake()
        {
            RealStyle.SetActive(true);
            AnimationStyle.SetActive(false);
        }

        /// <summary>
        /// 转动
        /// </summary>
        public void Turn()
        {
            RealStyle.SetActive(false);
            AnimationStyle.SetActive(true);
            AnimationStyle.Play();
        }

        /// <summary>
        /// 停止并显示结果
        /// </summary>
        /// <param name="dot"></param>
        public void Stop(int dot)
        {
            RealStyle.SetActive(true);
            RealStyle.SetSpriteName(string.Format(StyleNamePrefix, dot));
            AnimationStyle.SetActive(false);
        }
    }
}
