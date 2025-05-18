using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_HideCursor : MonoBehaviour
    {
        public Texture2D UserCursor; 
        void Start () {
            Cursor.SetCursor(UserCursor, new Vector2(UserCursor.width / 2, UserCursor.height / 2), CursorMode.Auto);
        } 
    }
}
