using Assets.Scripts.Game.jpmj.MahjongScripts.Public.Adpater;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameTable
{
    /// <summary>
    ///1.333333333		1		440
    ///1.5				0.9	 	405
    ///1.5				0.9		405
    ///1.6				0.8		380
    ///1.666666667		0.8	 	380
    ///1.775			0.75	362
    ///1.777777778		0.75	362
    /// </summary>
    public class HandCamera : MonoBehaviour
    {
        private Vector3 _position;
        private float _orthographicSize;
        protected void Start()
        {
            _position = transform.localPosition;
            float big = Screen.width;
            float small = Screen.height;
            ////float rateDef = 1.8f;
            float posYDef = 2.75f;
            float sizeDef = 1.55f;
            float rate = big > small ? big / small : small / big;

            _orthographicSize = sizeDef;
            _position.y = posYDef;
            //if (rate < 1.3f)//5:4
            //{
            //    _orthographicSize = 2.1f;
            //    _position.y = 3.3f;
            //}
            //else if (rate < 1.4f)//4:3
            //{
            //    _orthographicSize = 2f;
            //    _position.y = 3.2f;
            //}
            //else if (rate < 1.59f)
            //{
            //    _orthographicSize = 1.75f;
            //    _position.y = 2.91f;
            //}
            //else if (rate < 1.7)
            //{
            //    _orthographicSize = 1.66f;
            //    _position.y = 2.83f;
            //}
            //if (rate < 1.8f)
            //{
            //    _orthographicSize = 1.55f;
            //    _position.y = 2.75f;
            //}
            //else if (rate < 1.9f)
            //{
            //    _orthographicSize = 1.49f;
            //    _position.y = 2.7f;
            //}
            //else
            //{
            //    _orthographicSize = 1.42f;
            //    _position.y = 2.57f;
            //}

            if (null != GameAdpaterManager.Singleton)
            {
                transform.localPosition = GameAdpaterManager.Singleton.GetConfig.HandCardCamera_Pos;
                GetComponent<Camera>().orthographicSize = GameAdpaterManager.Singleton.GetConfig.HandCardCamera_Size;
                transform.localRotation = Quaternion.Euler(15, 180, 0);
            }
            else
            {
                transform.localPosition = _position;
                GetComponent<Camera>().orthographicSize = _orthographicSize;
                transform.localRotation = Quaternion.Euler(15, 180, 0);
            }
        }
    }
}