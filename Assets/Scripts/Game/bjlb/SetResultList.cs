using UnityEngine;

namespace Assets.Scripts.Game.bjlb
{
    public class SetResultList : MonoBehaviour
    {

        public UISprite[] Sprites;

        public void AddResult(bool[] r)
        {
            for (int i = 1; i < r.Length; i++)
            {

                if (r[i])
                {
                    Sprites[i -1].spriteName = "game_history_win";
                }
                else
                {
                    Sprites[i - 1].spriteName = "game_history_lose";
                }
            }
        }
    }
}
