/******************************************************
 * FlieName:计时任务管理类
 * Auth:    Gasol.X
 * Date:    2020.8.14 23:08
 * Version: V1.0
 * Fix:     2020.8.24 22:47【添加一个真实时间推动的时间轮】
 ******************************************************/
namespace XTimer
{
    using System.Collections.Generic;
    using UnityEngine;
    internal class TimerCore{
        #region 单例
        private static TimerCore m_Instance;
        internal static TimerCore Instance {
            get {
                if (m_Instance == null) {
                    m_Instance = new TimerCore();
                    m_Instance.Init();
                }
                return m_Instance;
            }
        }
        #endregion
        
        #region 外部调用
        //添加延时任务
        internal void AddTask(ITask _task, float _delayTime,bool _isIgnorTimeScale = false) {  
            if (_task == null) {
                return;
            }
            if (_delayTime <= 0) {
                _task.Execute();
                return;
            }
            DelayTask delayTask = new DelayTask(_task, _isIgnorTimeScale ? Time.realtimeSinceStartup + _delayTime : Time.time + _delayTime);

            if (_isIgnorTimeScale) {
                m_RealTimerWheel.AddTask(delayTask);
            }
            else {
                m_TimerWheel.AddTask(delayTask);
            }
        }
        //添加计时器
        internal void AddTimer(Timer _timer) {
            m_AllTimers.Add(_timer);
        }
        //移除计时器
        internal void RemoveTimer(Timer _timer) {
            m_AllTimers.Remove(_timer);
        }
        //每帧驱动
        internal void FrameCheckTimer() {
            //驱动时间轮
            m_TimerWheel.FrameCheck(Time.time);
            m_RealTimerWheel.FrameCheck(Time.time);
            //驱动计时器
            for (int i = 0; i < m_AllTimers.Count; i++) {
                m_AllTimers[i].FrameCheck();
            }
        }
        #endregion

        #region 内部实现
        //【游戏时间】
        private TimerWheel m_TimerWheel;
        //【运行时间】
        private TimerWheel m_RealTimerWheel;

        private List<Timer> m_AllTimers;

        private void Init() {
            m_TimerWheel = new TimerWheel(0.01f, 200, Time.time);
            m_RealTimerWheel = new TimerWheel(0.01f, 200, Time.realtimeSinceStartup);
            m_AllTimers = new List<Timer>();
        }
        #endregion
    }
}
