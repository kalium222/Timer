namespace Timer {

    // command line interface
    public class TimerCLI {

    }

    // daemon
    public class Timerd {

    }

    // entrypoint
    public class Program {
        static public void Main(string[] args) {
            Timer timer = new Timer(0, 0, 0, 0, ()=>{
                    Console.WriteLine("Hello!");
                    });
            timer.DoTask();
        }
    }

}
