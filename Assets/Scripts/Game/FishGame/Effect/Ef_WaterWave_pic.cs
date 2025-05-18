using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_WaterWave_pic : MonoBehaviour {

        public Texture2D[] Tex_Waterwaves;
        //public Material[] Mtrl_Waterwaves;
        public float Fps = 10F;
        private Renderer mRenderer;
        private int mCurTexIdx = 0;
        private static int MtrlUnitTileX = 4;//一个场景材质的Tile
        void Start()
        {
            mRenderer = GetComponent<Renderer>();
            Vector2 texScale = mRenderer.material.mainTextureScale;
            texScale.x = MtrlUnitTileX * GameMain.Singleton.ScreenNumUsing;
            mRenderer.material.mainTextureScale = texScale;

            Vector3 ls = transform.localScale;
            ls.x *= GameMain.Singleton.ScreenNumUsing;

            gameObject.isStatic = false;
            transform.localScale = ls;
            transform.position = new Vector3(0F, 0F, Defines.GlobleDepth_WaterWave);
            gameObject.isStatic = true;
   
        }

        void Update()
        {
            int texIdxNew = (int)(Time.time * Fps) % Tex_Waterwaves.Length;
            if (texIdxNew != mCurTexIdx)
            {
                mCurTexIdx = texIdxNew;
                mRenderer.material.mainTexture = Tex_Waterwaves[mCurTexIdx];
            }

        } 
    }
}
