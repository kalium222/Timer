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

    }
}

