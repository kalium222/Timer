using System.Collections;

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
        private Timer? m_prev;
        private Timer? m_next;

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

        public Timer(uint id)
        {
            m_id = id;
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

        public void Destroy()
        {
            this.Next?.Destroy();
            this.Next = null;
            this.Prev = null;
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
            get { return m_prev; }
            set { m_prev = value; }
        }

        public Timer? Next
        {
            get { return m_next; }
            set { m_next = value; }
        }

        // 应该被HierachicalTimeWheel调用
        // 仅smallest timewheel需DoTask
        // 还需要重新调度
        public void DoTask()
        {
            Task.Invoke();
        }
    }

    public class TimerList : IEnumerable
    {
        private Timer m_head;
        
        // public method
        public TimerList()
        {
            m_head = new Timer();
        }

        private class TimerIterator : IEnumerator
        {
            private TimerList m_container;
            private Timer? m_postion;
            public TimerIterator(TimerList container)
            {
                m_container = container;
                m_postion = container.Head;
            }
            void IEnumerator.Reset()
            {
                m_postion = m_container.Head;
            }
            bool IEnumerator.MoveNext()
            {
                m_postion = m_postion?.Next;
                return m_postion != null;
            }
            object IEnumerator.Current
            {
                get
                {
                    if ( m_postion==null )
                        throw new IndexOutOfRangeException();
                    return m_postion;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new TimerIterator(this);
        }

        // 不检查各种条件(是否重复等)，交给timewheel
        // 及heirachical timewheels
        public void Add(Timer timer)
        {
            timer.Prev = m_head;
            timer.Next = m_head.Next;
            if ( timer.Next != null ) 
                timer.Next.Prev = timer;
            m_head.Next = timer;
        }

        public static void Detach(Timer? timer)
        {
            if ( timer==null )
                return;
            if ( timer.Next != null )
                timer.Next.Prev = timer.Prev;
            if ( timer.Prev != null )
                timer.Prev.Next = timer.Next;
            timer.Next = timer.Prev = null;
        }

        public void Clear()
        {
            m_head.Next?.Destroy();
            m_head.Next = null;
        }

        public Timer Head
        {
            get { return m_head; }
        }

        public int Count
        {
            get
            {
                int result = 0;
                Timer? p = m_head.Next;
                while ( p!=null )
                {
                    result++;
                    p = p.Next;
                }
                return result;
            }
        }

    }
}
