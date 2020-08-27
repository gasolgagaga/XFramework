/******************************************************
 * FlieName:调用静态接口
 * Auth:    Gasol.X
 * Date:    2020.8.14 23:08
 * Version: V1.0
 * Fix:     2020.8.24 22:49【添加一个计时器功能】
 ******************************************************/
namespace XTimer
{
    using System;
    using UnityEngine;

    public class Timer
    {
        //添加延时任务
        public static void AddDelayTask(Action _onComplete, float _delayTime) {
            TimerCore.Instance.AddTask(Task.CreateTask(_onComplete), _delayTime);
        }

        /*-----开启计时器-----*/

        //【不循环】
        public static Timer AddTimer(float _interval, Action<float> _onTimer, float _duration = 0, Action _onComplete = null, bool _isIgnoreTimeScale = false) {
            Timer timer = CachePool<Timer>.Pop();
            timer.m_Interval = _interval;
            timer.m_OnTimer = _onTimer;
            timer.m_IsIgnorTimeScale = _isIgnoreTimeScale;
            timer.m_IsLoop = _duration == 0;
            timer.m_ExpireTime = _isIgnoreTimeScale ? Time.realtimeSinceStartup + _duration : Time.time + _duration;
            timer.m_OnComplete = _onComplete;
            TimerCore.Instance.AddTimer(timer);
            return timer;
        }
        //【循环】
        public static Timer AddTimer(float _interval, Action<float> _onTimer,bool _isIgnoreTimeScale = false) {
            Timer timer = CachePool<Timer>.Pop();
            timer.m_Interval = _interval;
            timer.m_OnTimer = _onTimer;
            timer.m_IsIgnorTimeScale = _isIgnoreTimeScale;
            timer.m_IsLoop = false;          
            TimerCore.Instance.AddTimer(timer);
            return timer;
        }

        //每帧检测
        internal void FrameCheck() {
            if (m_IsPause) {
                return;
            }
            float currentTime = m_IsIgnorTimeScale ? Time.realtimeSinceStartup : Time.time;
            if (currentTime - m_lastCheckTime >= m_Interval) {
                m_OnTimer?.Invoke(currentTime);
                if (!m_IsLoop && currentTime >= m_ExpireTime) {
                    m_OnComplete?.Invoke();
                    Stop();
                }
                m_lastCheckTime = currentTime;
            }
        }

        #region 链式方法
        //回收计时器
        public void Stop() {
            TimerCore.Instance.RemoveTimer(this);
            CachePool<Timer>.Push(this);
        }
        //开始计时器
        public Timer Play() {
            m_IsPause = false;
            return this;
        }
        //暂停计时器
        public Timer Pause() {
            m_IsPause = true;
            return this;
        }
        //设置计时中执行方法
        public Timer OnTimer(Action<float> _onTimer) {
            if (_onTimer != null) {
                m_OnTimer = _onTimer;
            }
            return this;
        }
        //设置计时器完成方法【非循环下】
        public Timer OnComplete (Action _onComplete) {
            if (_onComplete != null) {
                m_OnComplete = _onComplete;
            }
            return this;
        }
        //设置循环
        public Timer SetLoop(bool _loop) {
            m_IsLoop = _loop;
            return this;
        }
        //设置是否使用真实时间
        public Timer IgnorTimeScale(bool _ignor) {
            m_IsIgnorTimeScale = _ignor;
            return this;
        }
        #endregion
       

        #region 内部实现
        //完成后执行
        private Action m_OnComplete;
        //计时中执行
        private Action<float> m_OnTimer;
        //计时间隔
        private float m_Interval;
        //过期时间
        private float m_ExpireTime;
        //上一次计时时间
        private float m_lastCheckTime;
        //是否循环
        private bool m_IsLoop;
        //是否使用真实时间
        private bool m_IsIgnorTimeScale;
        //是否暂停
        private bool m_IsPause;

       

        #endregion
    }

    public static class TimerExtention
    {
        
    }
}

