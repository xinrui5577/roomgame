using UnityEngine;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhTableInfo : MonoBehaviour {

        public UILabel RoomID;

        public UILabel Junshu;

        public UILabel Lunshu;

        public void SetLunShu(int curlun, int maxlun)
        {
            if (Lunshu != null)
            {
                Lunshu.text = "" + curlun + "/" + maxlun;
            }
        }

        public void SetJuShu(int curJu, int maxJu)
        {
            if (Junshu != null)
            {
                Junshu.text = "" + curJu + "/" + maxJu;
            }
        }

        public void SetRoomId(int id)
        {
            if (RoomID != null)
            {
                RoomID.text = "" + id;
            }
        }
    }
}
