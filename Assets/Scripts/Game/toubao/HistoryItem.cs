using UnityEngine;

namespace Assets.Scripts.Game.toubao
{
    public class HistoryItem : MonoBehaviour
    {
        public UISprite Saizi1;
        public UISprite Saizi2;
        public UISprite Saizi3;
        public UILabel Point;
        public UILabel WordResult;

        public void SetValue(int[] points)
        {
            int Sum = 0;
            Saizi1.spriteName = "point" + points[0];
            Saizi2.spriteName = "point" + points[1];
            Saizi3.spriteName = "point" + points[2];
            for (int i = 0; i < points.Length; i++)
            {
                Sum += points[i];
            }
            Point.text = Sum+"";
            if (Sum==3||Sum==18)
            {
                WordResult.text = "通";
                WordResult.color=Color.red;
            }
            else if (Sum>10)
            {
                WordResult.text = "大";
                WordResult.color = Color.green;
            }
            else
            {
                WordResult.text = "小";
                WordResult.color = Color.yellow;
            }
        }
    }
}
