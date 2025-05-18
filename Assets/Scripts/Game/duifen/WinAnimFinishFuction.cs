using UnityEngine;

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local

namespace Assets.Scripts.Game.duifen
{
    public class WinAnimFinishFuction : MonoBehaviour {

        void Enable()
        {
            Invoke("HideThis", 2);
        }

        void HideThis()
        {
            gameObject.SetActive(false);
            CancelInvoke();
        }
    }
}
