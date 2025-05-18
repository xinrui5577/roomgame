using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class DnxbObjItem : MonoBehaviour
    {
        public MeshRenderer Mesh;
        public float AnimationTime = 1f;

        private Tweener mColorTweener;
        private Material mMaterialDark;
        private Material mMaterialLight;

        public bool CurrentState
        {
            set
            {
                if (mMaterialLight == null || mMaterialDark == null) return;
                Mesh.material = value ? mMaterialLight : mMaterialDark;
                if (value)
                {
                    mColorTweener.Kill();
                    Mesh.material.color = Color.white;
                    mColorTweener = Mesh.material.DOColor(new Color(160 / 255f, 160 / 255f, 160 / 255f), AnimationTime);
                    mColorTweener.SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
                    mColorTweener.Play();
                }
            }
        }

        public void OnInitiation(Material mateDark, Material mateLight)
        {
            //设置材质
            mMaterialLight = mateLight;
            mMaterialDark = mateDark;           
            //设置状态       
            CurrentState = false;
        }
    }
}