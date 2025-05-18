using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.Mahjong2D.CommonFile.Tools
{
    public class RobotUi :MonoSingleton<RobotUi>
    {
        [SerializeField]
        private GameObject _showParent;

        private bool _localAuto;

        public override void Awake()
        {
            base.Awake();
            _showParent.SetActive(false);
            if (App.GetGameManager<Mahjong2DGameManager>())
            {
                _localAuto = App.GetGameManager<Mahjong2DGameManager>().AutoStateByLocal;
            }
        }
        public void ToggleRobot()
        {
#if YX_DEVE
            OnClickAuto();
#endif
        }

        public void OnClickAuto()
        {
            bool toggleState = App.GetGameData<Mahjong2DGameData>().IsInRobot;
            if (_localAuto)
            {
                App.GetGameData<Mahjong2DGameData>().IsInRobot = !toggleState;
                ShowAutoState(!toggleState);
            }
            else
            {
                SendRequest(!toggleState);
            }
        }

        private void SendRequest(bool requestState)
        {
            var keyPair = new KeyValuePair<int, bool>(App.GetGameManager<Mahjong2DGameManager>().CurSeat, requestState);
            Facade.EventCenter.DispatchEvent(ConstantData.KeyRobotToggleRequest, keyPair);
        }

        public void ShowAutoState(bool showState)
        {
            _showParent.TrySetComponentValue(showState);
        }
    }
}
