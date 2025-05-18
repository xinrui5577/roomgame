using UnityEngine;

namespace Assets.Scripts.Game.LXGameScripts
{
    public class MoveJettonIcon : MonoBehaviour
    {
        [HideInInspector]
        public float DisY;

        public virtual bool Move(float dis)
        {
            UISprite temp = GetComponent<UISprite>();
            if (temp == null)
            {
                Debug.LogError("------> Move object is null in MoveJettonIcon");
                return false;
            }
            transform.localPosition=new Vector3(transform.localPosition.x,transform.localPosition.y-dis,transform.localPosition.z);
            if (Mathf.Abs(transform.localPosition.y) >= DisY)
                 return true;
            return false;
        }
    }
}

