using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ConsoleApp1.BackgroundServiceData
{
    public class WorkerHostedService : CustomBackgroundService
    {
        private readonly ChannelWriter<string> _channelWriter;

        public WorkerHostedService(Channel<string> channel)
        {
            _channelWriter = channel.Writer;
        }

        static int i = 0;
        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            while (!stopToken.IsCancellationRequested)
            {
                var b = Console.ReadLine();

                await _channelWriter.WriteAsync(b);
                //Console.WriteLine("Write more...");
            }

                //await Task.Yield();

                ////Do your preparation (e.g. Start code) here
                //while (!stopToken.IsCancellationRequested)
                //{
                //    await DoSomethingAsync();
                //}
                //Do your cleanup (e.g. Stop code) here
            }

        public async Task DoSomethingAsync()
        {
            while (true)
            {
                //await Task.Run(() =>
                //{
                //    Console.WriteLine(i++);
                //    Thread.Sleep(1000);
                //});
            }
        }
    }
}
