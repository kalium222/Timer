namespace Timer
{
    public class TimeWheel
    {
        // private
        private readonly ulong m_tickMs;
        private readonly int m_wheelSize;
        private int m_current;
        private TimerList[] m_bucketArray;

        // public method
        public TimeWheel(ulong tickMs, int wheelSize)
        {
            m_current = 0;
            m_tickMs = tickMs;
            m_wheelSize = wheelSize;
            m_bucketArray = new TimerList[wheelSize];
        }

        // 将move和dotask交给HierachicalTimeWheel完成
        // Tick仅仅改变current
        // 进位则返回true
        public bool Tick()
        {
            m_current++;
            bool flag = ( m_current==m_wheelSize );
            m_current %= m_wheelSize;
            return flag;
        }

        // 该timewheel上，现在走过的时间
        public ulong GetCurrentTime()
        {
            return ((ulong)m_current) * m_tickMs;
        }
        
        public TimerList GetCurrentTimerList()
        {
            return m_bucketArray[m_current];
        }

        // 在timewheel中加入Timer
        // 若失败（超时/时间不够/以前）则不加入
        // 并返回false
        public bool AddTimer(Timer timer)
        {
            if ( timer.Postpone <= 0 || 
                    timer.Postpone > m_tickMs*((ulong)m_wheelSize) ) return false;
            m_bucketArray[timer.Postpone%m_tickMs].Add(timer);
            return true;
        }

        // 从timewheel中移除timer
        public bool RemoveTimer(Timer timer)
        {
            return m_bucketArray[timer.Postpone%m_tickMs].Remove(timer);
        }

        // 若timer被修改（多次或主动修改）
        // 则需要放置在合适的位置
        // 若不在该timewheel内
        // 则返回false
        public bool ModifyTimer(Timer timer)
        {
            this.RemoveTimer(timer);
            return this.AddTimer(timer);
        }

    }

    public class HierachicalTimeWheel 
    {
        // public method
        // 生成合适大小的一系列timewheels
        public HierachicalTimeWheel()
        {
            timeWheelList = new LinkedList<TimeWheel>();
            timerTable = new Dictionary<int, Timer>();
        }

        // 完成所有在最小timewheel里的Timer
        // 将其他timewheels里当前的timer向下移
        public void Tick()
        {
            // TODO:
            // finish the timers in the smallest TimeWheel
            // pop the timers to the smaller TimeWheel from current one
            foreach ( TimeWheel tw in timeWheelList )
            {
                if ( !tw.Tick() ) 
                {
                    break;
                }
            }
        }

        // 把timer加入Dictionary
        // id?
        // 根据时间把timer加入合适的timewheel
        public int AddTimer()
        {
            return 0;
        }

        public Timer RemoveTimer()
        {
            return new Timer(0, 0, 0, 0, ()=>{});
        }

        public int ModifyTimer()
        {
            return 0;
        }

        // private field
        private LinkedList<TimeWheel> timeWheelList;
        private Dictionary<int, Timer> timerTable;
    }
}
