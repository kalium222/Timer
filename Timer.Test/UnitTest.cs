namespace Timer.Test
{
    public class TestTimer
    {
        [Test]
        public void TestNode()
        {
            Timer leader1 = new Timer();
            Timer leader2 = new Timer();
            Timer node = new Timer(1, 1, ()=>{});
            leader1.Next = node;
            node.Prev = leader1;
            // move
            leader1.Next = null;
            leader2.Next = node;
            node.Prev = node;
            Assert.IsNotNull(leader2.Next);
        }
    }

    public class TestTimerList
    {
        [SetUp]
        public void Setup() {}

        [Test]
        public void TestAdd()
        {
            TimerList l1 = new TimerList();
            for ( int i=0; i<10; i++ )
            {
                l1.Add(new Timer((uint)i, 0, ()=>{}));
            }
            Timer? p = l1.Head.Next;
            int count = 9;
            while ( p != null )
            {
                Assert.IsTrue(((int)p.Id)==count);
                count--;
                p = p.Next;
            }
        }

        [Test]
        public void TestDetach()
        {
            TimerList l1 = new TimerList();
            Timer t1 = new Timer(1);
            Timer t2 = new Timer(2);
            Timer t3 = new Timer(3);
            Timer t4 = new Timer(4);
            l1.Add(t1);
            l1.Add(t2);
            l1.Add(t3);
            l1.Add(t4);
            TimerList.Detach(t2);
            Assert.IsTrue(t3.Next==t1);
            Assert.IsTrue(t1.Prev==t3);
            TimerList.Detach(t1);
            Assert.IsNull(t3.Next);
        }

        [Test]
        public void TestIterator()
        {
            TimerList l1 = new TimerList();
            Timer t1 = new Timer(1);
            Timer t2 = new Timer(2);
            Timer t3 = new Timer(3);
            Timer t4 = new Timer(4);
            l1.Add(t1);
            l1.Add(t2);
            l1.Add(t3);
            l1.Add(t4);
            foreach ( Timer timer in l1 )
            {
                Assert.IsNotNull(timer.Id);
            }
        }

    }

    public class TestTimeWheel
    {
        [Test]
        public void TestTick()
        {
            TimeWheel wheel = new TimeWheel(1, 60);
            for (int i=0; i<59; i++)
            {
                Assert.IsTrue((ulong)i==wheel.GetCurrentTime());
                wheel.Tick();
            }
            Assert.IsTrue(wheel.Tick());
        }
    }

    public class TestHierachicalTimeWheel
    {
        [Test]
        public void TestAdd()
        {
            HierachicalTimeWheel instance = HierachicalTimeWheel.Instance;
            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.GetCurrentTime());
            instance.AddTimer(1, 1, 1, ()=>{});
            instance.AddTimer(1000+1, 1, 1, ()=>{});
            instance.AddTimer(1000*60+1, 1, 1, ()=>{});
            instance.AddTimer(1000*60*60+1, 1, 1, ()=>{});
            instance.AddTimer(1000*60*60*24+1, 1, 1, ()=>{});
            List<int> res = instance.GetDistri();
            List<int> expect = new List<int>(new int[] {1,1,1,1,1});
            for ( int i=0; i<5; i++ )
                Assert.IsTrue(res[i]==expect[i]);
        }
        
        [Test]
        public void TestRemove()
        {
            HierachicalTimeWheel instance = HierachicalTimeWheel.Instance;
            instance.ClearAll();
            uint id1 = instance.AddTimer(1, 1, 1, ()=>{});
            uint id2 = instance.AddTimer(1000+1, 1, 1, ()=>{});
            uint id3 = instance.AddTimer(1000*60+1, 1, 1, ()=>{});
            uint id4 = instance.AddTimer(1000*60*60+1, 1, 1, ()=>{});
            uint id5 = instance.AddTimer(1000*60*60*24+1, 1, 1, ()=>{});
            List<int> res;

            instance.RemoveTimer(id1);
            res = instance.GetDistri();
            List<int> expect1 = new List<int>(new int[] {0, 1, 1, 1, 1});
            for ( int i=0; i<5; i++ )
                Assert.IsTrue(res[i]==expect1[i]);

            instance.RemoveTimer(id2);
            res = instance.GetDistri();
            List<int> expect2 = new List<int>(new int[] {0, 0, 1, 1, 1});
            for ( int i=0; i<5; i++ )
                Assert.IsTrue(res[i]==expect2[i]);

            instance.RemoveTimer(id3);
            res = instance.GetDistri();
            List<int> expect3 = new List<int>(new int[] {0, 0, 0, 1, 1});
            for ( int i=0; i<5; i++ )
                Assert.IsTrue(res[i]==expect3[i]);

            instance.RemoveTimer(id4);
            res = instance.GetDistri();
            List<int> expect4 = new List<int>(new int[] {0, 0, 0, 0, 1});
            for ( int i=0; i<5; i++ )
                Assert.IsTrue(res[i]==expect4[i]);

            instance.RemoveTimer(id5);
            res = instance.GetDistri();
            List<int> expect5 = new List<int>(new int[] {0, 0, 0, 0, 0});
            for ( int i=0; i<5; i++ )
                Assert.IsTrue(res[i]==expect5[i]);
        }

        [Test]
        public void TestModify()
        {
            HierachicalTimeWheel instance = HierachicalTimeWheel.Instance;
            instance.ClearAll();
            uint id1 = instance.AddTimer(1, 1, 1, ()=>{});
            uint id2 = instance.AddTimer(1000+1, 1, 1, ()=>{});
            uint id3 = instance.AddTimer(1000*60+1, 1, 1, ()=>{});
            uint id4 = instance.AddTimer(1000*60*60+1, 1, 1, ()=>{});
            uint id5 = instance.AddTimer(1000*60*60*24+1, 1, 1, ()=>{});
            List<int> res;

            instance.ModifyTimer(id1, 1000+1, 1, 1);
            res = instance.GetDistri();
            List<int> expect1 = new List<int>(new int[] {0, 2, 1, 1, 1});
            for ( int i=0; i<5; i++ )
                Assert.IsTrue(res[i]==expect1[i]);

            instance.ModifyTimer(id1, 1000*60+1, 1, 1);
            res = instance.GetDistri();
            List<int> expect2 = new List<int>(new int[] {0, 1, 2, 1, 1});
            for ( int i=0; i<5; i++ )
                Assert.IsTrue(res[i]==expect2[i]);
        }
        
        [Test]
        public void TestTick()
        {
            HierachicalTimeWheel instance = HierachicalTimeWheel.Instance;
            instance.ClearAll();
            uint id1 = instance.AddTimer(1, 10, 10, ()=>{
                    Console.WriteLine("t1!");
                    });
            uint id2 = instance.AddTimer(1000+1, 1, 1, ()=>{
                    Console.WriteLine("t2!");
                    });
            uint id3 = instance.AddTimer(1000*60+1, 1, 1, ()=>{});
            uint id4 = instance.AddTimer(1000*60*60+1, 1, 1, ()=>{});
            uint id5 = instance.AddTimer(1000*60*60*24+1, 1, 1, ()=>{});
            for ( int i=0; i<1001; i++ )
            {
                instance.Tick();
            }
        }

        [Test]
        public void TestRun()
        {
            HierachicalTimeWheel instance = HierachicalTimeWheel.Instance;
            instance.ClearAll();
            uint id1 = instance.AddTimer(1, 10, 10, ()=>{
                    Console.WriteLine("t1!");
                    });
            uint id2 = instance.AddTimer(1000+1, 1, 1, ()=>{
                    Console.WriteLine("t2!");
                    });
            uint id3 = instance.AddTimer(1000*60+1, 1, 1, ()=>{});
            uint id4 = instance.AddTimer(1000*60*60+1, 1, 1, ()=>{});
            uint id5 = instance.AddTimer(1000*60*60*24+1, 1, 1, ()=>{});
            instance.Start();
            Thread.Sleep(10000);
        }
    }
}

