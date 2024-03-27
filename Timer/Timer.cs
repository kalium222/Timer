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

        public Timer(int id, int postpone, Action task) 
        {
            m_id = id;
            m_postpone = postpone;
            m_interval = 0;
            m_times = 1;
            Task += task;
        }

        public Timer(int id, int postpone, int interval, int times, Action task) 
        {
            m_id = id;
            m_postpone = postpone;
            m_interval = interval;
            m_times = times;
            Task += task;
        }

        public int Id {
            get { return m_id; }
        }

        public int Postpone {
            get { return m_postpone; }
            set { m_postpone = value; }
        }
        
        public int Interval
        {
            get { return m_interval; }
            set { m_interval = value; }
        }

        public int Times
        {
            get { return m_times; }
            set { m_times = value; }
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
        
        // public method
        public void Add(Timer timer)
        {
            if ( m_timerList == null )
            {
                m_timerList = new LinkedList<Timer>();
            }
            m_timerList.AddLast(timer);
        }

        public bool Remove(Timer timer)
        {
            return m_timerList?.Remove(timer) ?? false;
        }
    }
}
