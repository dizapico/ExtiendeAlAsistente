using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dialogflow.dotnet.Models;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;

namespace dialogflow.dotnet.Controllers
{
    public class HomeController : Controller
    {
        private static Dictionary<object, BufferBlock<string>> clientBufferQueues = new Dictionary<object, BufferBlock<string>>();
        private ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("LongPoll")]

        public async Task<IActionResult> LongPoll()
        {
            var clientId = Request.QueryString.Value;
            var queue = GetOrCreateClientBufferQueue(clientId);
            var page = string.Empty;

            try
            {
                page = await queue.ReceiveAsync(TimeSpan.FromSeconds(15));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return Content(page, "text/plain");
        }

        [Route("Test")]
        public IActionResult Test()
        {
            return Content("Hello World!", "text/plain");
        }

        public static void SetPage(string page)

        {
            lock (clientBufferQueues)
            {
                var toRemove = new List<object>();
                foreach (var buffer in clientBufferQueues)
                {
                    buffer.Value.Post(page);
                    if (buffer.Value.Count > 20)
                    {
                        toRemove.Add(buffer.Key);
                    }
                }
                foreach (var id in toRemove)
                {
                    clientBufferQueues.Remove(id);
                }
            }
        }

        private static BufferBlock<string> GetOrCreateClientBufferQueue(string clientId)
        {
            BufferBlock<string> queue;
            lock (clientBufferQueues)
            {
                if (!clientBufferQueues.TryGetValue(clientId, out queue))
                {
                    queue = new BufferBlock<string>();
                    clientBufferQueues.Add(clientId, queue);
                }
            }

            return queue;
        }
    }
}
