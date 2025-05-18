using UnityEngine;

namespace Assets.Scripts.Game.Fishing.entitys
{
    public class Map : MonoBehaviour
    {

        public SpriteRenderer TheSpriteRenderer;
        public string FillAmountName = "_FillAmount";
        private Material _material;
        void Awake()
        {
            _material = TheSpriteRenderer.sharedMaterial;
            _material.SetFloat(FillAmountName, 1);
        }

        public void MateriaPrivatization()
        {
            _material = Instantiate(TheSpriteRenderer.sharedMaterial);
            TheSpriteRenderer.sharedMaterial = _material;
            SetFill(1);
        }


        public void SetFill(float rate)
        {
            _material.SetFloat(FillAmountName, rate);
        }
    }
}
