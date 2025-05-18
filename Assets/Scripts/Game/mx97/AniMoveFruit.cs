using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.mx97
{ /**
 * 移动到判定值回调
 */
    public delegate void DeleMoveToJudge();

/**
 * 移动控制类
 */
    public class AniMoveFruit : MonoBehaviour {

        private DeleMoveToJudge _mDeleMoveToJudge ;
        public void SetMoveToJudgeFun(DeleMoveToJudge deleMoveToJudge)
        {
            _mDeleMoveToJudge = deleMoveToJudge;
        }


        public float JudgeY = 0f;                       // 判定点 由于向上移动 故坐标点大于该值时重置为起始位置


        // Use this for initialization
       protected void Start () {
            if ( JudgeY.Equals(0f))
                YxDebug.LogError("------> StartVec and CenterY and JudgeLen couldn't be null in AniMoveFruit!\n");                    
        }

        // 返回值判定是否移动过需要交换到最下面的位置
        public bool Move(float dis)
        {
            UISprite sprite = gameObject.GetComponent<UISprite>();
            if (sprite == null)
            {
                YxDebug.LogError("------> Move object is null in AniMoveFruit!\n");
                return false;
            }

            sprite.transform.localPosition = new Vector3(sprite.transform.localPosition.x, sprite.transform.localPosition.y + dis, sprite.transform.localPosition.z);

            // 移动Child的下边Y坐标 大于 判定Y坐标 重置为初始位置
            if (JudgeY < Mathf.Abs(sprite.transform.localPosition.y))
            {
                if (_mDeleMoveToJudge != null)
                    _mDeleMoveToJudge();
                return true;
            }

            return false;
        }

    }
}