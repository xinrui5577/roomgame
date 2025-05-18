using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class OperateButtonItem : MonoBehaviour
    {
        public OperateMenuType OperateType;

        public void SetActive(List<KeyValuePair<int, bool>> opMenu)
        {
            gameObject.SetActive(false);
            KeyValuePair<int, bool> value;
            for (int i = 0; i < opMenu.Count; i++)
            {
                value = opMenu[i];
                if ((int)OperateType == value.Key)
                {
                    gameObject.SetActive(value.Value);
                }
                if (OperateType == OperateMenuType.OpreateGuo)
                {
                    if (value.Key == (int)OperateMenuType.OpreateHideGuo)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
