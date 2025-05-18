using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Language
{
    public class GameObjectEx_LanguageAutoSelect : MonoBehaviour {
        public GameObject[] Prefab_GoInstanceByLangauge;
        private GameObject mGo;
        // Use this for initialization
        void OnEnable()
        {
            //GameMain.EvtLanguageChange += Handle_LanguageChanged;

            if (Prefab_GoInstanceByLangauge == null || Prefab_GoInstanceByLangauge.Length == 0)
            {
                YxDebug.LogError("语言组件成员未赋值错误.");
                Destroy(this);
                return;
            }
            if (mGo != null)
                Destroy(mGo);

            mGo = Instantiate(Prefab_GoInstanceByLangauge[(int)GameMain.Singleton.BSSetting.LaguageUsing.Val]) as GameObject;
            mGo.transform.parent = transform;
            mGo.transform.localPosition = Vector3.zero;
        }

        void OnDisable()
        {
       
            //GameMain.EvtLanguageChange -= Handle_LanguageChanged;
        }
        void Handle_LanguageChanged(global::Assets.Scripts.Game.FishGame.Common.core.Language l)
        {
            if (mGo != null)
                Destroy(mGo);
            mGo = Instantiate(Prefab_GoInstanceByLangauge[(int)GameMain.Singleton.BSSetting.LaguageUsing.Val]) as GameObject;
            mGo.transform.parent = transform;
            mGo.transform.localPosition = Vector3.zero;
        }
    }
}
