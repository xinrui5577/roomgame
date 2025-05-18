using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.jh.ui{
    public class JhUserButton : MonoBehaviour
    {

        public Color FrontNomal;

        public Color FrontDisable;

        public UILabel Front;

        public void SetState(UIButtonColor.State state, bool immediate)
        {
            UIButton btn = GetComponent<UIButton>();
            btn.SetState(state, immediate);
            if (state == UIButtonColor.State.Disabled)
            {
                btn.isEnabled = false;
            }else{
                btn.isEnabled = true;
            }
            if (state == UIButton.State.Disabled)
            {
                Front.color = FrontDisable;
            }
            else
            {
                Front.color = FrontNomal;
            }
        }

        public void SetText(string text)
        {
            Front.text = text;
        }

        public void SetOnClick(List<EventDelegate> dels)
        {
            UIButton btn = GetComponent<UIButton>();
            btn.onClick = dels;
        }
    }

}
