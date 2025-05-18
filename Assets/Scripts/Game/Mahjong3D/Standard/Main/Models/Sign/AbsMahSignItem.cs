using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public abstract class AbsMahSignItem : MonoBehaviour
    {
        public MahSignType SignType = MahSignType.None;

        public abstract void SetSprite(Sprite sprite);

        public virtual void OnReset()
        {
            SetState(false);
        }

        public virtual void SetState(bool isOn)
        {
            gameObject.SetActive(isOn);
        }

        public virtual void SetTranslate(Vector3 pos)
        {
            transform.localPosition = pos;
        }

    }
}
