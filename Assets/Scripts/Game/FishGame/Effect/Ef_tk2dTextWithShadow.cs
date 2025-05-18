using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_tk2dTextWithShadow : MonoBehaviour {
        public tk2dTextMesh TextOri;
        public tk2dTextMesh TextShadow;

    
        public string text
        {
            get
            {
                return TextOri.text;
            }
            set
            {
                TextOri.text = value;
                TextShadow.text = value;
            }
        }

        public float Alpha
        {
            get
            {
                return TextOri.color.a;
            }
            set
            {
                Color tmpC = TextOri.color;
                tmpC.a = value;
                TextOri.color = tmpC;

                tmpC = TextOri.color2;
                tmpC.a = value;
                TextOri.color2 = tmpC;


                tmpC = TextShadow.color;
                tmpC.a = value;
                TextShadow.color = tmpC;

            }
        }

        public void Commit()
        {
            TextOri.Commit();
            TextShadow.Commit();
        }
    }
}
