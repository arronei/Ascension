using System;
using System.Threading;

namespace CatalogBuilder
{
    public class Reader
    {
        private static readonly AutoResetEvent GetInput;
        private static readonly AutoResetEvent GotInput;
        private static string _input;

        static Reader()
        {
            GetInput = new AutoResetEvent(false);
            GotInput = new AutoResetEvent(false);
            var inputThread = new Thread(InnerReader) { IsBackground = true };
            inputThread.Start();
        }

        private static void InnerReader()
        {
            while (true)
            {
                GetInput.WaitOne();
                _input = Console.ReadLine();
                GotInput.Set();
            }
        }

        public static string ReadLine(int timeOutMillisecs)
        {
            GetInput.Set();
            if (GotInput.WaitOne(timeOutMillisecs))
            {
                return _input;
            }
            throw new TimeoutException("User did not provide input within the timelimit.");
        }
    }
}