using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongCamera : SceneManagerBase
    {
        public Camera HandCamera;
        public GameObject CameraServered;

        public override void OnInitalization()
        {
            GameCenter.EventHandle.Subscriber((int)EventKeys.AiAgency, AiAgency);
        }

        public void AiAgency(EvtHandlerArgs args)
        {
            CameraServered.SetActive((args as AiAgencyArgs).State);
        }
    }
}