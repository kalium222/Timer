namespace Timer
{
    public class TimeWheel
    {
        // private
        private int m_tickMs;
        private int m_wheelSize;
        private int m_current;
        private List<TimerList> m_bucketList;

        // public method
        public TimeWheel(int tickMs, int wheelSize)
        {
            m_current = 0;
            m_tickMs = tickMs;
            m_wheelSize = wheelSize;
            m_bucketList = new List<TimerList>(wheelSize);
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

        // return the current TimeList
        // and remove it from the m_bucketList
        public TimerList TakeCurrentTimers()
        {
            TimerList current_list = m_bucketList[m_current];
            
            // TODO:
            return new TimerList();
        }

        public TimerList GetCurrentTimers()
        {
            return m_bucketList[m_current];
        }

        public int AddTimer(Timer timer)
        {
            return 0;
        }

        public Timer RemoveTimer(Timer timer)
        {
            return new Timer(0, 0, 0, 0, ()=>{});
        }

        public int ModifyTimer(Timer timer)
        {
            return 0;
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
