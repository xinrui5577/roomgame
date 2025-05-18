using UnityEngine;

namespace Assets.Scripts.Game.sanpian.item
{
    public class MyItemArea : MonoBehaviour
    {

        UIEventListener ev;
        public  static MyItemArea instance;
        public UIGrid grid;
        public  bool CanDrag;
        void Start()
        {
            //ev = UIEventListener.Get(gameObject);
            //ev.onDrag = MyDrag;
            //ev.onDragStart = MyDargStart;
            //ev.onDragEnd = MyDargEnd;
            instance = this;
            CanDrag = false;
            //grid = gameObject.GetComponent<UIGrid>();
        }

        private void MyDargStart(GameObject go)
        {
            print("Start");
        }
        private void MyDargEnd(GameObject go)
        {
            print("MyDargEnd");
        }

        private void MyDrag(GameObject go, Vector2 delta)
        {
            print(delta.x + "," + delta.y);
        }

        //public void RePos(int cardslen)
        //{
        //    grid.cellWidth = MaxWidth / (cardslen-1);
        //    grid.Reposition();
        //}

    }
}
