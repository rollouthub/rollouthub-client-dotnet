using System;
using System.Threading;
using System.Threading.Tasks;
using Toggle.Client;

namespace Toggle.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new ToggleClient("dc8720db-8611-4747-a5e9-3a6aa85655b3");

            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(20));
                var result = client.IsEnabled("some.new.toggle", false);
                Console.WriteLine("some.new.toggle is: " + result);
            }
        }
    }
}