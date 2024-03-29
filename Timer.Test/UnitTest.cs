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
        public void TestTimerListMove()
        {

        }

        [Test]
        public void TestTimeWheelTakeCurrentTimers()
        {
            TimeWheel timeWheel = new TimeWheel(0, 4);
        }
    }
}
