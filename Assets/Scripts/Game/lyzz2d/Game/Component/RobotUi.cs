using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Game.Component
{
    public class RobotUi : MonoSingleton<RobotUi>
    {
        [SerializeField] private GameObject _showParent;

        public override void Awake()
        {
            base.Awake();
            _showParent.SetActive(false);
        }

        public void ToggleRobot(GameObject obj = null)
        {
            var toggleState = App.GetGameData<Lyzz2DGlobalData>().IsInRobot;
            _showParent.SetActive(!toggleState);
            App.GetGameData<Lyzz2DGlobalData>().IsInRobot = !toggleState;
        }
    }
}