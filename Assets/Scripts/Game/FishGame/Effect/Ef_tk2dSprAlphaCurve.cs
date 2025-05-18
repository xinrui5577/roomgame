using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    /// <summary>
    /// 有曲线变换spr的alpha值
    /// </summary>
    /// <remarks>
    /// 效率非常低,一个大概在开发机上需要1MS.
    /// 如果做比较简单的动画,直接使用算法比较好
    /// </remarks>
    public class Ef_tk2dSprAlphaCurve : MonoBehaviour
    {
        public AnimationCurve CurveAlpha;//alpha曲线
        public float TimeOneLoop = 1.5F;//一个循环所用时间(单位:秒)

        private Color mColorCurrent;
        private tk2dSprite mSpr;

        void Awake () {
            mSpr = GetComponent<tk2dSprite>();
            if (mSpr == null)
            {
                Destroy(this);
                return;
            }

            mColorCurrent = mSpr.color;
        }
	
        void Update () {
            mColorCurrent.a = CurveAlpha.Evaluate((Time.time / TimeOneLoop) % 2F);
            //Debug.Log(((Time.time / TimeOneLoop)%2F));
            //mColorCurrent.a = Mathf.Cos(Time.time / TimeOneLoop * Mathf.PI)*0.5F+0.5F;
            mSpr.color = mColorCurrent;
        
        }
    }
}
