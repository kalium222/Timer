namespace Timer 
{
    public class Timer 
    {
        // private
        private uint m_id;
        private ulong m_postpone;
        private ulong m_interval;
        private uint m_times;
        // 以该类为list node
        private Timer? prev;
        private Timer? next;

        // public
        public event Action Task;

        public Timer()
        {
            m_id = 0;
            m_postpone = 0;
            m_interval = 0;
            m_times = 1;
            Task += () => {};
        }

        public Timer(uint id, ulong postpone, Action task) 
        {
            m_id = id;
            m_postpone = postpone;
            m_interval = 0;
            m_times = 1;
            Task += task;
        }

        public Timer(uint id, ulong postpone, ulong interval, uint times, Action task) 
        {
            m_id = id;
            m_postpone = postpone;
            m_interval = interval;
            m_times = times;
            Task += task;
        }

        public uint Id {
            get { return m_id; }
        }

        public ulong Postpone {
            get { return m_postpone; }
            set { m_postpone = value; }
        }
        
        public ulong Interval
        {
            get { return m_interval; }
            set { m_interval = value; }
        }

        public uint Times
        {
            get { return m_times; }
            set { m_times = value; }
        }

        public Timer? Prev
        {
            get { return prev; }
            set { prev = value; }
        }

        public Timer? Next
        {
            get { return next; }
            set { next = value; }
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

        // move constructor
        public static TimerList Move(TimerList timerList)
        {
            TimerList dest = new TimerList();
            dest.m_timerList = timerList.m_timerList;
            timerList.m_timerList = null;
            return dest;
        }

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

        public int Length
        {
            get { return m_timerList?.Count ?? -1; }
        }
    }
}
