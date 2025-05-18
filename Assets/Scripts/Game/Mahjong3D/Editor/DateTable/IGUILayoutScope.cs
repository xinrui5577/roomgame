using System;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.EditorTool
{
    public interface IGUILayoutScope : IDisposable
    {
        Rect GetScope();
     
        Rect GetLastRect();
   
        Rect GetRect(float size); 
      
        Rect GetRectEven(int count);

        //从最后得到rect
        Rect GetRectFromEnd(float size);
 
        //获取矩形比率
        Rect GetRectRatio(float ratio);
   
        //获取剩余矩形
        Rect GetRemainingRect();
    }
}
