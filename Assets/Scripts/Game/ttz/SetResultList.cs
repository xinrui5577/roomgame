using UnityEngine;

namespace Assets.Scripts.Game.ttz
{
    public class SetResultList : MonoBehaviour
    {

        public UISprite[] Sprites;

        public void AddResult(int[] r)
        {
            for (int i = 0; i < r.Length; i++)
            {

                if (r[i] == 1)
                {
                    Sprites[i].spriteName = "win";
                }
                else if (r[i] == -1)
                {
                    Sprites[i].spriteName = "lose";
                }
                Sprites[i].MakePixelPerfect();
            }
        }
    }
}
