using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameTtResultItem : MonoBehaviour
    {
        public UILabel CurRound;
        public List<UILabel> UserWin;

        public void SetCurRound(int value)
        {
            CurRound.text = value.ToString();
        }

        public void SetUserWin(int i,int value)
        {
            var str = "";
            if (value >= 0)
            {
                str += string.Format("[FFFF32FF]+{0}", value);
                UserWin[i].effectColor=new Color(60/255f,0/255f,0/255f);
            }
            else
            {
                str += string.Format("[00C8FFFF]{0}", value);
                UserWin[i].effectColor = new Color(20 / 255f, 0 / 255f, 30 / 255f);
            }
            UserWin[i].text = str;
        }

    }
}
