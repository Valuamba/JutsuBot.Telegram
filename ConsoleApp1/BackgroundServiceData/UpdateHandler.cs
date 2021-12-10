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
    public class UpdateHandler : CustomBackgroundService
    {
        private readonly ChannelReader<string> _channel;

        public UpdateHandler(Channel<string> channel)
        {
            _channel = channel.Reader;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            await foreach (var update in _channel.ReadAllAsync(stoppingToken))
            {
                Console.WriteLine($"Channel responds: {update}");
            }
        }
    }
}
