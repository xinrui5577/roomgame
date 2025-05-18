using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.UI
{
    public class UI_VolumeViewer : MonoBehaviour
    {
        public UI_VolumeBar Prefab_VolBar;

        private UI_VolumeBar[] mVolBars;
        void Start()
        {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtSoundVolumeChanged += Handle_SoundVolChanged;
        }

        void Handle_SoundVolChanged(float volPercent)
        {
            if (mVolBars == null)
            {
                GameMain gm = GameMain.Singleton;
                mVolBars = new UI_VolumeBar[gm.ScreenNumUsing];
                for (int i = 0; i != gm.ScreenNumUsing; ++i)
                {
                    mVolBars[i] = Instantiate(Prefab_VolBar) as UI_VolumeBar;
                    mVolBars[i].transform.parent = transform;
                    mVolBars[i].transform.position = new Vector3(
                        gm.WorldDimension.xMin + Defines.WorldDimensionUnit.width * (0.5F + i)
                        , gm.WorldDimension.yMin + gm.WorldDimension.height * 0.5F
                        , Defines.GlobleDepth_GameDataViewer);
                    mVolBars[i].ChangeVol(volPercent);
                }

                StopCoroutine("_Coro_DestroyVolSetting");
                StartCoroutine("_Coro_DestroyVolSetting");
            }
            else
            {
                foreach (UI_VolumeBar b in mVolBars)
                {
                    b.ChangeVol(volPercent);
                }
            } 
        }
     

        /// <summary>
        /// 一段时间之后删除音量调节界面
        /// </summary>
        /// <returns></returns>
        IEnumerator _Coro_DestroyVolSetting()
        {
            yield return new WaitForSeconds(3F);
            if (mVolBars != null)
            {
                foreach (UI_VolumeBar b in mVolBars)
                {
                    Destroy(b.gameObject);
                }
                mVolBars = null;
            }
        }
 
    }
}
