using System.Collections.Concurrent;

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
            for (int i=0; i<m_wheelSize; i++)
                m_bucketArray[i] = new TimerList();
        }

        // 将move和dotask交给HierachicalTimeWheel完成
        // Tick仅仅改变current
        // 进位则返回true
        public bool Tick()
        {
            m_current++;
            bool carry = ( m_current==m_wheelSize );
            m_current %= m_wheelSize;
            return carry;
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

        // 不判断条件
        public void AddTimer(Timer timer)
        {
            m_bucketArray[(int)(timer.Postpone/m_tickMs)%m_wheelSize].Add(timer);
        }

        // 从timewheel中移除timer
        public static void DetachTimer(Timer timer)
        {
            TimerList.Detach(timer);
        }

        public void ClearAll()
        {
            m_current = 0;
            foreach ( TimerList l in m_bucketArray )
            {
                l.Clear();
            }
        }

        public ulong MaxTime
        {
            get { return m_tickMs*(ulong)m_wheelSize; }
        }

        public int Count 
        {
            get
            {
                int result = 0;
                foreach ( TimerList l in m_bucketArray )
                {
                    result += l.Count;
                }
                return result;
            }
        }
    }

    public class HierachicalTimeWheel 
    {
        // private field
        private static HierachicalTimeWheel? s_instance;
        private static readonly object m_lock = new object();
        // 由小到大
        private List<TimeWheel> m_timeWheelArray;
        private ConcurrentDictionary<uint, Timer> m_timerTable;
        private uint m_maxId = 0;

        // public method
        // 生成合适大小的一系列timewheels
        private HierachicalTimeWheel()
        {
            // TODO:
            m_timeWheelArray = new List<TimeWheel>();
            // ms, s, min, hour, day
            m_timeWheelArray.Add(new TimeWheel(1, 1000));
            m_timeWheelArray.Add(new TimeWheel(1*1000, 60));
            m_timeWheelArray.Add(new TimeWheel(1*1000*60, 60));
            m_timeWheelArray.Add(new TimeWheel(1*1000*60*60, 24));
            m_timeWheelArray.Add(new TimeWheel(1*1000*60*60*24, 50));
            m_timerTable = new ConcurrentDictionary<uint, Timer>();
        }

        public static HierachicalTimeWheel Instance
        {
            get
            {
                if (s_instance==null)
                {
                    lock (m_lock)
                    {
                        if ( s_instance==null )
                        {
                            s_instance = new HierachicalTimeWheel();
                        }
                    }
                }
                return s_instance;
            }
        }

        // 完成所有在最小timewheel里的Timer
        // 将其他timewheels里当前的timer向下移
        // 重复任务重新放置
        public void Tick()
        {
            bool carry = true;
            foreach ( TimeWheel tw in m_timeWheelArray )
            {
                if ( !carry )
                    break;
                carry = tw.Tick();
                if ( tw == m_timeWheelArray.First() )
                {
                    foreach ( Timer t in tw.GetCurrentTimerList() )
                    {
                        t.DoTask();
                        if ( t.Times<=1 )
                        {
                            this.RemoveTimer(t);
                        }
                        else
                        {
                            this.ModifyTimer(t, t.Interval, 
                                    t.Interval, t.Times-1);
                        }
                    }
                }
                else // 向下移动    
                {
                    foreach ( Timer t in tw.GetCurrentTimerList() )
                        this.RefreshTimer(t);
                }
            }
        }

        public Timer GetTimer(uint id)
        {
            return m_timerTable[id];
        }

        public List<int> GetDistri()
        {
            List<int> result = new List<int>(m_timeWheelArray.Count);
            for ( int i=0; i<result.Capacity; i++ )
            {
                result.Add(m_timeWheelArray[i].Count);
            }
            return result;
        }

        public ulong GetCurrentTime()
        {
            ulong result = 0;
            foreach ( TimeWheel timewheel in m_timeWheelArray )
                result += timewheel.GetCurrentTime();
            return result;
        }

        public ulong GetMaxTime()
        {
            return m_timeWheelArray.Last().MaxTime;
        }

        // 把timer加入Dictionary
        // 根据时间把timer加入合适的timewheel
        public uint AddTimer(ulong postpone, ulong interval, uint times, Action task)
        {
            if ( m_maxId == uint.MaxValue )
                m_maxId = 0;
            while ( m_timerTable.ContainsKey(m_maxId) )
                m_maxId++;
            Timer newTimer = new Timer(m_maxId, postpone, interval, times, task);
            this.AddTimer(newTimer);
            return newTimer.Id;
        }

        public void AddTimer(Timer timer)
        {
            m_timerTable.TryAdd(timer.Id, timer);
            ulong idx = timer.Postpone - this.GetCurrentTime();
            if ( (long)idx <= 0 )
                m_timeWheelArray.First().AddTimer(timer);
            else if ( idx >= this.GetMaxTime() )
                m_timeWheelArray.Last().AddTimer(timer);
            else
            {
                foreach ( TimeWheel tw in m_timeWheelArray )
                {
                    if ( idx <= tw.MaxTime )
                    {
                        tw.AddTimer(timer);
                        break;
                    }
                }
            }
        }

        public Timer? RemoveTimer(Timer timer)
        {
            Timer? res;
            m_timerTable.TryRemove(timer.Id, out res);
            TimerList.Detach(timer);
            return res;
        }

        public Timer? RemoveTimer(uint id)
        {
            Timer? timer = m_timerTable[id];
            m_timerTable.Remove(id, out timer);
            TimerList.Detach(timer);
            return timer;
        }

        public int RefreshTimer(Timer timer)
        {
            this.RemoveTimer(timer);
            this.AddTimer(timer);
            return 0;
        }

        public int ModifyTimer(Timer timer, ulong postpone, ulong interval, uint times)
        {
            timer.Postpone = postpone;
            timer.Interval = interval;
            timer.Times = times;
            this.RefreshTimer(timer);
            return 0;
        }

        public int ModifyTimer(uint id, ulong postpone, ulong interval, uint times)
        {
            Timer modified = GetTimer(id);
            this.ModifyTimer(modified, postpone, interval, times);
            return 0;
        }

        public void ClearAll()
        {
            m_timerTable.Clear();
            foreach ( TimeWheel tw in m_timeWheelArray )
                tw.ClearAll();
        }

    }
}
