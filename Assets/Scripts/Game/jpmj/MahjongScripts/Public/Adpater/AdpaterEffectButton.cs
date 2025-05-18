using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.Public.Adpater
{
    public class AdpaterEffectButton : MonoBehaviour
    {
        void Start()
        {
            if (null != GameAdpaterManager.Singleton)
            {
                transform.localScale = GameAdpaterManager.Singleton.GetConfig.EffextButtonSize;
            }
        }
    }
}