using System;
using System.Collections;
using UnityEngine;

namespace Game.Shared
{
    /// <summary>
    /// 一些辅助的静态方法
    /// </summary>
    public static class Helper
    {
        public static IEnumerator DelayToInvoke(Action action, float delaySeconds)  
        {  
            yield return new WaitForSeconds(delaySeconds);  
            action();  
        } 
    }
}