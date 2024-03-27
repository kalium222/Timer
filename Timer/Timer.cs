namespace Timer 
{
    public class Timer 
    {
        // private
        // change to TimeSpan?
        private int m_id;
        private int m_postpone;
        private int m_interval;
        private int m_times;

        // public
        public event Action Task;

        public Timer(int id, int postpone, int interval, int times, Action task) 
        {
            m_id = id;
            m_postpone = postpone;
            m_interval = interval;
            m_times = times;
            Task += task;
        }

        // 应该被HierachicalTimeWheel调用
        // 仅smallest timewheel需DoTask
        // 还需要重新调度
        public void DoTask()
        {
            Task.Invoke();
        }
    }

    public class TimerList 
    {
        private LinkedList<Timer>? m_timerList;
    }
}
