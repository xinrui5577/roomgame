using UnityEngine;

namespace Assets.Scripts.Game.BaiTuan
{

    public class SetResultList : MonoBehaviour
    {

        public UISprite[] Sprites;

        public void AddResult(int[] r)
        {
            int temp = 0;
            for (int i = 0; i < r.Length; i++)
            {
                temp = temp + r[i];
            }

            for (int i = 0; i < r.Length; i++)
            {

                if (temp == 3)
                {
                    Sprites[i].spriteName = "8";
                    continue;
                }
                if (temp == 0)
                {
                    Sprites[i].spriteName = "7";
                    continue;
                }
                if (r[i] == 1)
                {
                    Sprites[i].spriteName = "5";
                }
                else
                {
                    Sprites[i].spriteName = "6";
                }
            }
        }

        public void AddResultOnFrist(int[] r)
        {
            int temp = 0;
            for (int i = 0; i < r.Length; i++)
            {
                temp = temp + r[i];
            }

            for (int i = 0; i < r.Length; i++)
            {

                if (temp == 3)
                {
                    Sprites[i].spriteName = "8";
                    continue;
                }
                if (temp == -3)
                {
                    Sprites[i].spriteName = "7";
                    continue;
                }
                if (r[i] == 1)
                {
                    Sprites[i].spriteName = "5";
                }
                else
                {
                    Sprites[i].spriteName = "6";
                }
            }
        }
    }
}