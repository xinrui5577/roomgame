using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn3d
{
    public class DicAnimotor : MonoBehaviour
    {
        public void AnimotorJieSu()
        {
            App.GetGameManager<Brnn3DGameManager>().TheDicMode.ShowPNumber();
        }
    }
}

