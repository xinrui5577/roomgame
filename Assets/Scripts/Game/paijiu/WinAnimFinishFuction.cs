using UnityEngine;



namespace Assets.Scripts.Game.paijiu
{
    public class WinAnimFinishFuction : MonoBehaviour
    {

        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void Enable()
        {
            Invoke("HideThis", 2);
        }

        // ReSharper disable once UnusedMember.Local
        void HideThis()
        {
            gameObject.SetActive(false);
            CancelInvoke();
        }
    }
}