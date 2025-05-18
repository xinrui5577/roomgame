using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Language
{
    public class SliceSpriteDim_LanguageAutoSelect : MonoBehaviour {
 

        public Vector2[] Dims;

        // Use this for initialization
        void OnEnable()
        {


            if (Dims == null || Dims.Length == 0)
            {
                YxDebug.LogError("Dims语言组件成员未赋值错误.");
                Destroy(this);
                return;
            }
            //GameMain.EvtLanguageChange += Handle_LanguageChanged;
            //transform.localScale = Dims[(int)GameMain.Singleton.BSSetting.LaguageUsing.Val];
            int lIdx = (int)GameMain.Singleton.BSSetting.LaguageUsing.Val;
            if (lIdx >= 0 && lIdx < Dims.Length)
            {
                tk2dSlicedSprite ss = GetComponent<tk2dSlicedSprite>();
                if (ss != null)
                    ss.dimensions = Dims[lIdx];
            }

        }

        void OnDisable()
        {
            //GameMain.EvtLanguageChange -= Handle_LanguageChanged;
        }
        void Handle_LanguageChanged(global::Assets.Scripts.Game.FishGame.Common.core.Language l)
        {
            if ((int)l < 0 || (int)l >= Dims.Length)
                return;

            tk2dSlicedSprite ss = GetComponent<tk2dSlicedSprite>();
            if (ss != null)
                ss.dimensions = Dims[(int)l];
        }
    }
}
