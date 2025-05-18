using Assets.Scripts.Game.fillpit.ImgPress.Main;
using UnityEngine;

namespace Assets.Scripts.Game.fillpit
{


    public class PanelPlayerType : MonoBehaviour
    {

        public UISprite GameTypeSprite ;

        public virtual void ShowGameType(GameRequestType type)
        {
            switch (type)
            {
                case GameRequestType.Fold:
                    GameTypeSprite.spriteName = "Fold";
                    GameTypeSprite.gameObject.SetActive(true);
                    break;
                case GameRequestType.KickSpeak:
                    GameTypeSprite.spriteName = "KickSpeak";
                    GameTypeSprite.gameObject.SetActive(true);
                    break;
                case GameRequestType.NotKick:
                    GameTypeSprite.spriteName = "NotKick";
                    GameTypeSprite.gameObject.SetActive(true);
                    break;
                case GameRequestType.BackKick:
                    GameTypeSprite.spriteName = "BackKick";
                    GameTypeSprite.gameObject.SetActive(true);
                    break;
                default:
                    GameTypeSprite.gameObject.SetActive(false);
                    break;
            }
        }


        public virtual void HideGameType()
        {
            GameTypeSprite.spriteName = string.Empty;
            GameTypeSprite.gameObject.SetActive(false);
        }

        public virtual void Reset()
        {
            GameTypeSprite.gameObject.SetActive(false);
        }
    }
   
}
