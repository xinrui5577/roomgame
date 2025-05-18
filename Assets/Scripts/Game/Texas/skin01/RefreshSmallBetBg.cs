using UnityEngine;
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Assets.Scripts.Game.Texas.skin01
{
    public class RefreshSmallBetBg : MonoBehaviour
    {

        [SerializeField]
		UIGrid _smallBetGrid = null;

        [SerializeField]
		UIGrid _linesGrid = null;

        [SerializeField]
		GameObject _bg = null;

        [SerializeField]
		UISprite _bgSprite = null;


        public void ShowSmallBetBg()
        {
            if (_smallBetGrid == null)
            {
                Debug.LogError("UIGrid is null !! - From RefreshSmallBetBG");
                return;
            }

            int smallBetCount = _smallBetGrid.GetChildList().Count - 1;

            if (smallBetCount < 0)
                return;

            int lineCount = smallBetCount / _smallBetGrid.maxPerLine;
            for (int i = 0; i < lineCount; i++)
            {
                _linesGrid.transform.GetChild(i).gameObject.SetActive(true);
            }
            _linesGrid.Reposition();

            int bgUnit = 30;    //每行的宽度
            _bgSprite.height = (lineCount + 1) * bgUnit;
            _bg.SetActive(smallBetCount >= 0);
        }


        public void HideSmallBetBg()
        {
            Transform linesParent = _smallBetGrid.transform;
            for (int i = 0; i < linesParent.childCount; i++)
            {
                linesParent.GetChild(i).gameObject.SetActive(false);
            }
            _bg.SetActive(false);
        }

    }
}