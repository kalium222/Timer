namespace Timer.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }
    
        [Test]
        public void Test1()
        {
                Console.WriteLine(TimeSpan.MaxValue);
                Timer timer = new Timer(0, 0, 0, 0, ()=>{
                        Console.WriteLine("Test!");
                        });
                timer.DoTask();
    
        }
    }

}

