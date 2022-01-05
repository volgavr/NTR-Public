using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MutexTest
{
    // В этом классе содержится общий ресурс в виде переменной Count,
    // а так же мьютекс mtx
    class SharedRes
    {
        public static int Count;
        public static Mutex mtx = new Mutex();
    }

    // В этом классе Count инкрементируется
    class IncThread
    {
        int num;
        public Thread Thrd;

        public IncThread(string name, int n)
        {
            Thrd = new Thread(this.Run);
            num = n;
            Thrd.Name = name;
            Thrd.Start();
        }

        // Точка входа в поток
        void Run()
        {
            Console.WriteLine(Thrd.Name + " waits mutex");

            // Получить мьютекс
            SharedRes.mtx.WaitOne();

            Console.WriteLine(Thrd.Name + " gets mutex");

            do
            {
                Thread.Sleep(500);
                SharedRes.Count++;
                Console.WriteLine("in thread {0}, Count={1}", Thrd.Name, SharedRes.Count);
                num--;
            } while (num > 0);

            Console.WriteLine(Thrd.Name + " releases mutex");

            SharedRes.mtx.ReleaseMutex();
        }
    }

    class DecThread
    {
        int num;
        public Thread Thrd;

        public DecThread(string name, int n)
        {
            Thrd = new Thread(new ThreadStart(this.Run));
            num = n;
            Thrd.Name = name;
            Thrd.Start();
        }

        // Точка входа в поток
        void Run()
        {
            Console.WriteLine(Thrd.Name + " waits mutex");

            // Получить мьютекс
            SharedRes.mtx.WaitOne();

            Console.WriteLine(Thrd.Name + " gets mutex");

            do
            {
                Thread.Sleep(500);
                SharedRes.Count--;
                Console.WriteLine("in thread {0}, Count={1}", Thrd.Name, SharedRes.Count);
                num--;
            } while (num > 0);

            Console.WriteLine(Thrd.Name + " releases mutex");

            SharedRes.mtx.ReleaseMutex();
        }
    }

    
    class Program
    {
        static void Main(string[] args)
        {
            IncThread mt1 = new IncThread("Inc thread", 5);

            // разрешить инкременирующему потоку начаться
            Thread.Sleep(1);

            DecThread mt2 = new DecThread("Dec thread", 5);

            mt1.Thrd.Join();
            mt2.Thrd.Join();

            Console.ReadLine();
        }
    }
}
