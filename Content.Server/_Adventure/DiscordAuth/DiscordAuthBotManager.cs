using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System;

namespace Content.Server.Adventure.DiscordAuth;

public sealed class DiscordAuthBotManager
{
    public HttpListener listener = default!;
    public void Initialize()
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:3963/");
        listener.Start();
        Task.Run(ListenerThread);
    }

    private async Task ListenerThread()
    {
        while (true)
        {
            HttpListenerContext ctx = listener.GetContext();
            HttpListenerRequest request = ctx.Request;
            using HttpListenerResponse resp = ctx.Response;
            resp.Headers.Set("Content-Type", "text/plain");
            resp.StatusCode = (int) HttpStatusCode.OK;
            resp.StatusDescription = "Status OK";

            Console.WriteLine("Query: {0}", request.QueryString["code"]);
        }
    }
}
