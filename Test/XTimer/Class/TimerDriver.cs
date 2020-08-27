/******************************************************
 * FlieName:时间轮驱动类
 * Auth:    Gasol.X
 * Date:    2020.8.14 23:08
 * Version: V1.0
 ******************************************************/

using XTimer;
    using UnityEngine;

    internal class TimerDriver : MonoBehaviour
    {
        //游戏初始化时自动生成脚本
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoadRuntimeMethod() {        
            GameObject driver = new GameObject("TimerDriver");
            driver.AddComponent<TimerDriver>();
            DontDestroyOnLoad(driver);
        }



        private void Update() {
            TimerCore.Instance.FrameCheckTimer();
        }
        
    }

