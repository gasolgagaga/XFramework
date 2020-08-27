/******************************************************
 * FlieName:延时任务对象类
 * Auth:    Gasol.X
 * Date:    2020.8.14 23:08
 * Version: V1.0
 ******************************************************/
namespace XTimer
{
    using System;
    using System.Collections.Generic;
   
    #region 延时任务
    //任务接口
    internal interface ITask
    {
        void Execute();
    }
    //任务基类
    internal class Task : ITask
    {
        internal Action OnComplete;

        public virtual void Execute() {
            OnComplete?.Invoke();
            CachePool<Task>.Push(this);
        }

        internal static Task CreateTask(Action _onComplete) {
            var task = CachePool<Task>.Pop();
            task.OnComplete = _onComplete;
            return task;
        }

    }
    //延时任务载体
    internal struct DelayTask
    {
        public ITask Task;
        //延时时间戳
        public float ExpireTime;

        public DelayTask(ITask _task, float _expireTime) {
            Task = _task;
            ExpireTime = _expireTime;
        }
    }
    //缓存池
    internal class CachePool<T> where T: new()
    {
        private static Stack<T> m_Tasks = new Stack<T>();

        internal static T Pop() {
            if (m_Tasks.Count > 0) {
                return m_Tasks.Pop();
            }
            return new T();
        }

        internal static void Push(T _task) {
            m_Tasks.Push(_task);
        }
    }
    
 
    #endregion
    
}
