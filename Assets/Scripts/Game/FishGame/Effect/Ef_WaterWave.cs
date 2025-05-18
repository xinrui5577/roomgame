using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_WaterWave : MonoBehaviour {

        public Material[] Mtrl_Waterwaves;
        public float Fps = 10F;
        private Renderer mRenderer;
        private int mCurTexIdx = 0;
        private static int MtrlUnitTileX = 4;//一个场景材质的Tile

        //IEnumerator Start()
        //{
        //    foreach (Material m in Mtrl_Waterwaves)
        //    {
        //        Vector2 texScale = m.mainTextureScale;
        //        texScale.x = MtrlUnitTileX*GameMain.Singleton.ScreenNumUsing;
        //        m.mainTextureScale = texScale;
        //    }

        //    Vector3 ls = transform.localScale;
        //    ls.x *= GameMain.Singleton.ScreenNumUsing;

        //    gameObject.isStatic = false;
        //    transform.localScale = ls;
        //    transform.position = new Vector3(0F, 0F, Defines.GlobleDepth_WaterWave);
        //    gameObject.isStatic = true;

        //    int curTexIdx = 0;
        //    int maxTexIdx = Mtrl_Waterwaves.Length;
        //    mRenderer = renderer;
        //    float waitTime = 1/Fps;
        //    while (true)
        //    {
        //        curTexIdx = (curTexIdx + 1)%maxTexIdx;
        //        //renderer.sharedMaterial.mainTexture = Mtrl_Waterwaves[curTexIdx];
        //        mRenderer.material = Mtrl_Waterwaves[curTexIdx];

        //        yield return new WaitForSeconds(waitTime);
        //    }
        //}

        void Start()
        {
            foreach (Material m in Mtrl_Waterwaves)
            {
                Vector2 texScale = m.mainTextureScale;
                texScale.x = MtrlUnitTileX * GameMain.Singleton.ScreenNumUsing;
                m.mainTextureScale = texScale;
            }

            Vector3 ls = transform.localScale;
            ls.x *= GameMain.Singleton.ScreenNumUsing;

            gameObject.isStatic = false;
            transform.localScale = ls;
            transform.position = new Vector3(0F, 0F, Defines.GlobleDepth_WaterWave);
            gameObject.isStatic = true;
        
            mRenderer = GetComponent<Renderer>();
        }
    
        void Update()
        {
            int texIdxNew = (int)(Time.time * Fps) % Mtrl_Waterwaves.Length;
            if (texIdxNew != mCurTexIdx)
            {
                mCurTexIdx = texIdxNew;
                mRenderer.material = Mtrl_Waterwaves[mCurTexIdx];
            }
        
        }
    }
}
