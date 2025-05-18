using Assets.Scripts.Game.Fishing.enums;
using UnityEngine;

namespace Assets.Scripts.Game.Fishing.datas
{
    public class FishFormationData : MonoBehaviour
    {
        /// <summary>
        /// 鱼的id, 无限循环数组
        /// </summary>
        public int[] FishIds;

        /// <summary>
        /// 路径
        /// </summary>
        public long Path; 
        /// <summary>
        /// 速度
        /// </summary>
        public float Speed;

        public EFishType FishType = EFishType.Fish;


        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 40);
            Gizmos.color = Color.blue;
            var angles = transform.localEulerAngles.z;
            var endPos = Vector3.zero;
            var _startPos = transform.position;
            for (var i = 0; i < 32; i++)
            {
                //朝着这个方向移动
                var newf = Path >> (i * 2);
                var dir = (newf & 3) % 4;//方向
                if (dir > 0)
                {
                    switch (dir)
                    {
                        case 1: //左
                            {
                                for (var j = 0; j < 1000; j++)
                                {
                                    angles += 0.03f;
                                    var a1 = angles * Mathf.Deg2Rad;
                                    var dir1 = Vector3.one;
                                    dir1.x = Mathf.Cos(a1);
                                    dir1.y = Mathf.Sin(a1);
                                    dir1.z = 0;
                                    endPos = _startPos + Speed * dir1 / 1000;
                                    Gizmos.DrawLine(_startPos, endPos);
                                    _startPos = endPos;
                                }
                            }
                            break;
                        case 2: //右
                            {
                                for (var j = 0; j < 1000; j++)
                                {
                                    angles -= 0.03f;
                                    var a1 = angles * Mathf.Deg2Rad;
                                    var dir1 = Vector3.one;
                                    dir1.x = Mathf.Cos(a1);
                                    dir1.y = Mathf.Sin(a1);
                                    dir1.z = 0;
                                    endPos = _startPos + Speed * dir1 / 1000;
                                    Gizmos.DrawLine(_startPos, endPos);
                                    _startPos = endPos;
                                }
                            }
                            break;
                        default:
                            {
                                var a1 = angles * Mathf.Deg2Rad;
                                var dir1 = Vector3.one;
                                dir1.x = Mathf.Cos(a1);
                                dir1.y = Mathf.Sin(a1);
                                dir1.z = 0;
                                endPos = _startPos + Speed * dir1;
                                Gizmos.DrawLine(_startPos, endPos);
                                _startPos = endPos;
                            }
                            break;
                    }
                }
                else
                {
                    var a1 = angles * Mathf.Deg2Rad;
                    var dir1 = Vector3.one;
                    dir1.x = Mathf.Cos(a1);
                    dir1.y = Mathf.Sin(a1);
                    dir1.z = 0;
                    endPos = _startPos + Speed * dir1;
                    Gizmos.DrawLine(_startPos, endPos);
                    _startPos = endPos;
                }

                //                Gizmos.DrawLine();
            }
        }

    }
}
