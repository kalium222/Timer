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
        public void Testwhat()
        {
            HierachicalTimeWheel instance = HierachicalTimeWheel.Instance;
        }
    }
}

