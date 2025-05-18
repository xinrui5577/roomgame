using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Language
{
    public class TextMeshEx_LanguageAutoSelect : MonoBehaviour {
        public LanguageItem LItem;
        private tk2dTextMesh mTextMesh;
        // Use this for initialization
	 
        void OnEnable()
        {
         
            //GameMain.EvtLanguageChange += Handle_LanguageChanged;
            mTextMesh = GetComponent<tk2dTextMesh>();
            if (LItem == null || mTextMesh == null)
            {
                YxDebug.LogError("语言组件成员未赋值错误.");
                Destroy(this);
                return;
            }

            mTextMesh.text = LItem.CurrentText;
            mTextMesh.Commit();
        }

        void OnDisable()
        {
         
            //GameMain.EvtLanguageChange -= Handle_LanguageChanged;
        }
        void Handle_LanguageChanged(global::Assets.Scripts.Game.FishGame.Common.core.Language l)
        {
            mTextMesh.text = LItem.CurrentText;
            mTextMesh.Commit();
        }
    }
}
