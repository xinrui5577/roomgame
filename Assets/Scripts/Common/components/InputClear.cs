/** 
 *文件名称:     InputClear.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-04-30 
 *描述:         Input置空
 *历史记录: 
*/

using UnityEngine;

namespace Assets.Scripts.Common.components
{
    public class InputClear : MonoBehaviour
    {
        #region UI Param
        [Tooltip("输入框，必要参数")]
        public UIInput Input;

        #endregion

        #region Data Param

        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        void Awake()
        {
            if (Input==null)
            {
                Input = GetComponent<UIInput>();
            }
        }

        #endregion

        #region Function

        public void ClearInput()
        {
            if (Input)
            {
                Input.value = "";
            }
        }
        #endregion

    }
}
