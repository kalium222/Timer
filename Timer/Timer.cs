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
        private Timer m_head;
        
        // public method
        public TimerList()
        {
            m_head = new Timer();
        }

        // 不检查各种条件，交给timewheel
        // 及heirachical timewheels
        public void Add(Timer timer)
        {
            timer.Prev = m_head;
            timer.Next = m_head.Next;
            if ( timer.Next != null ) 
                timer.Next.Prev = timer;
            m_head.Next = timer;
        }

        public void Detach(Timer? timer)
        {
            if ( timer==null )
                return;
            if ( timer.Next != null )
                timer.Next.Prev = timer.Prev;
            //timer.Next?.Prev = timer.Prev;
            if ( timer.Prev != null )
                timer.Prev.Next = timer.Next;
            timer.Next = timer.Prev = null;
        }

        public Timer Head
        {
            get { return m_head; }
        }

    }
}
