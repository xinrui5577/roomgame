using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Language
{
    public class LocalPos_LanguageAutoSelect : MonoBehaviour {

        public Vector3[] LocalPosition;
    
        // Use this for initialization
        void OnEnable()
        {


            if (LocalPosition == null   || LocalPosition.Length == 0)
            {
                YxDebug.LogError("LocalPos_LanguageAutoSelect语言组件成员未赋值错误.");
                Destroy(this);
                return;
            }
            //GameMain.EvtLanguageChange += Handle_LanguageChanged;
            transform.localPosition = LocalPosition[(int)GameMain.Singleton.BSSetting.LaguageUsing.Val];
        }

        void OnDisable()
        {
            //GameMain.EvtLanguageChange -= Handle_LanguageChanged;
        }
        void Handle_LanguageChanged(global::Assets.Scripts.Game.FishGame.Common.core.Language l)
        {
            transform.localPosition = LocalPosition[(int)GameMain.Singleton.BSSetting.LaguageUsing.Val];
        }
    }
}
