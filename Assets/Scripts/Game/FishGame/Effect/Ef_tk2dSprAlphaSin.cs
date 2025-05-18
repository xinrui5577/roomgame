using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_tk2dSprAlphaSin : MonoBehaviour {
        public float TimeOneLoop = 1.5F;//一个循环所用时间(单位:秒)
        private Color mColorCurrent;
        private tk2dSprite mSpr;

        void Awake()
        {
            mSpr = GetComponent<tk2dSprite>();
            if (mSpr == null)
            {
                Destroy(this);
                return;
            }

            mColorCurrent = mSpr.color;
        }

        void Update()
        {
            mColorCurrent.a = Mathf.Cos((1000000F+Time.time) / TimeOneLoop * Mathf.PI) * 0.5F + 0.5F;
            mSpr.color = mColorCurrent;

        }
    }
}
