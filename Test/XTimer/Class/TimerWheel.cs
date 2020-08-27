/******************************************************
 * FlieName:时间轮实现类
 * Auth:    Gasol.X
 * Date:    2020.8.14 23:08
 * Version: V1.1
 * Fix:     2020.8.21 21:48 【修复跳过节点导致任务无法检测的BUG】
 ******************************************************/
namespace XTimer{
    using System.Collections.Generic;
    //任务轮
    internal class TimerWheel
    {
        #region 外部接口
        //构造一个任务时间轮对象，内部包含了当前轮的任务节点
        internal TimerWheel(float _tick, int _wheelSize, float _currentTime) {
            this.m_Tick = _tick;
            this.m_WheelSize = _wheelSize;
            this.m_CurrentTime = _currentTime;
            this.m_Interval = m_Tick * m_WheelSize;
            this.m_TaskNodes = new TaskNode[m_WheelSize];
            //为时间轮初始化任务节点
            for (int i = 0; i < m_WheelSize; i++) {
                m_TaskNodes[i] = new TaskNode();
            }
            //创键内层时间轮
            if (m_Wheel == null) {
                m_Wheel = this;
            }
        }
    
        //外界用于推动时间轮
        internal void FrameCheck(float _time) {
            if (_time >= m_CurrentTime - m_Tick) {
                m_CurrentTime = _time - (_time % m_Tick);
                int round = (int)(_time / m_Tick) / m_WheelSize;
                int index = (int)(_time / m_Tick) - round * m_WheelSize - 1;
                while (lastIndex < index + 1 ||  lastRound < round) {
                    lastIndex ++;
                    if (lastIndex >= m_WheelSize) {
                        lastIndex -= m_WheelSize;
                        lastRound++;
                    }
                    CheckNode(lastIndex, _time);
                }                
            }
        }
      

        //添加一个延时任务
        internal bool AddTask(DelayTask _delayTask) {
            if (_delayTask.Task == null) {
                return false;
            }
            float delayTime = _delayTask.ExpireTime - m_CurrentTime;
            //当执行时间小于本时间轮最小单位时，无法添加
            if (delayTime < m_Tick) {
                return false;
            }
            else {
                //当延时时间在本时间轮的区间内则放入任务槽中
                if (delayTime < m_Interval) {
                    int index = (int)((delayTime + m_CurrentTime) / m_Tick) % m_WheelSize;
                    m_TaskNodes[index].DelayTasks.AddLast(_delayTask);
                }
                //当延时时间超过本时间轮则将任务放入上一层时间轮中
                else {
                    GetOverTaskWheel().AddTask(_delayTask);
                }
                return true;
            }
        }
        #endregion

        #region 内部实现
        //时间轮的最小时间精度
        private float m_Tick;
        //时间轮任务节点的数量
        private int m_WheelSize;
        //时间轮一轮的间隔
        private float m_Interval;
        //记录当前的时间
        private float m_CurrentTime;
        //上一次检测的节点
        private int lastIndex = -1;
        //上一次检测的轮数
        private int lastRound = 0;
        //时间轮任务节点数组（用于储存任务列表的节点）
        private TaskNode[] m_TaskNodes;
        //上一级的时间轮（当前时间轮无法存放任务时向上递归）
        private TimerWheel m_OverTaskWheel;
        //用于操作的时间轮
        private TimerWheel m_Wheel;
        //弹出一个延时任务（加入下一次时间轮或执行任务）
        private void PopTask(DelayTask _delayTask) {
            if (_delayTask.Task == null) {
                return;
            }
            //从底层添加任务当对应任务槽中
            if (!m_Wheel.AddTask(_delayTask)) {
                //Debug.Log("执行时间" + Time.time);
                _delayTask.Task.Execute();
            }
        }
        //获取上层的时间轮（任务延时时间大于本时间轮时放入上一层）
        private TimerWheel GetOverTaskWheel() {
            if (m_OverTaskWheel == null) {
                m_OverTaskWheel = new TimerWheel(m_Interval, m_WheelSize, m_CurrentTime);
                //将底层的操作时间轮传递到上一层
                m_OverTaskWheel.m_Wheel = m_Wheel;
            }
            return m_OverTaskWheel;
        }
        //延时时间轮任务节点
        private class TaskNode
        {
            public LinkedList<DelayTask> DelayTasks;
            public TaskNode() {
                DelayTasks = new LinkedList<DelayTask>();
            }
        }

        //检测节点
        private void CheckNode(int _index, float _time) {
            foreach (var task in m_TaskNodes[_index].DelayTasks) {
                PopTask(task);
            }
            m_TaskNodes[_index].DelayTasks.Clear();
            if (m_OverTaskWheel != null) {
                m_OverTaskWheel.FrameCheck(_time);
            }
        }
        #endregion
    }

   
}