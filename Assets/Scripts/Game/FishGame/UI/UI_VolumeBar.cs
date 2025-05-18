using UnityEngine;

namespace Assets.Scripts.Game.FishGame.UI
{
    public class UI_VolumeBar : MonoBehaviour
    {

        public GameObject Prefab_Spr_VolSettingBG;
        public GameObject Prefab_Spr_VolSettingTile;

        private GameObject[] mSprVolSettingTiles;//音量块,用来标记是否正在调节音量大小

        private static readonly int NumVolumeTile = 16;//音量显示方块的数量
        void Awake()
        {
            if (mSprVolSettingTiles == null)
            {
                //初始化ui

                GameObject sprBG = Instantiate(Prefab_Spr_VolSettingBG) as GameObject;//-0.35
                sprBG.transform.parent = transform;
                sprBG.transform.localPosition = new Vector3(0F, 18.1F, 0F);

                float startLocalPosX = -150.5F;
                float advanceX = 20F;
                mSprVolSettingTiles = new GameObject[NumVolumeTile];
                for (int i = 0; i != NumVolumeTile; ++i)
                {
                    mSprVolSettingTiles[i] = Instantiate(Prefab_Spr_VolSettingTile) as GameObject;
                    mSprVolSettingTiles[i].transform.parent = sprBG.transform;
                    mSprVolSettingTiles[i].transform.localPosition = new Vector3(startLocalPosX + advanceX * i, 0F, -0.1F);
                    mSprVolSettingTiles[i].GetComponent<Renderer>().enabled = false;
                }
            }
        }
        /// <summary>
        /// 改变音量,大小0~1
        /// </summary>
        /// <param name="vec"></param>
        public void ChangeVol(float volPercent)
        { 

            int curVolTileIdx = Mathf.RoundToInt(volPercent * NumVolumeTile);

            for (int i = 0; i != NumVolumeTile; ++i)
            {
                if (i < curVolTileIdx)
                    mSprVolSettingTiles[i].GetComponent<Renderer>().enabled = true;
                else
                    mSprVolSettingTiles[i].GetComponent<Renderer>().enabled = false;
            } 

        }
 
 
    }
}
