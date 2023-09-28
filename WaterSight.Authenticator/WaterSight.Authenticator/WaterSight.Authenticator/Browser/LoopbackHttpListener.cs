
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Text;

namespace WaterSight.Authenticator.Browser;

public class LoopbackHttpListener : IDisposable
{
    #region Constants
    private const int DefaultTimeout = 60 * 5; // 5 mins (in seconds)
    #endregion

    #region Constructor
    public LoopbackHttpListener(int port, string? path = null)
    {
        path = path ?? string.Empty;
        if (!path.StartsWith("/", StringComparison.InvariantCulture))
            path = "/" + path;
        _path = path;

        _url = $"http://127.0.0.1:{port}/";

        try
        {
            _host = new WebHostBuilder()
                .UseKestrel()
            .UseUrls(_url)
                .Configure(Configure)
                .Build();
            _host.Start();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"...while starting a web-host to sign-in. URL: {_url}");
            throw;
        }
    }
    #endregion

    #region Public Methods
    public Task<string> WaitForCallbackAsync(int timeoutInSeconds = DefaultTimeout)
    {
        Task.Run(async () =>
        {
            await Task.Delay(timeoutInSeconds * 1000);
            _source.TrySetCanceled();
        });

        return _source.Task;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion

    #region Overridden Mehods
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _host?.Dispose();
        }
    }
    #endregion

    #region Private Methods
    private void Configure(IApplicationBuilder app)
    {
        app.UsePathBase(_path);

        app.Run(async ctx =>
        {
            if (ctx.Request.Method == "GET")
            {
                await SetResultAsync(ctx.Request.QueryString.Value, ctx);
            }
            else if (ctx.Request.Method == "POST")
            {
                if (!ctx.Request.ContentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                {
                    ctx.Response.StatusCode = 415;
                }
                else
                {
                    using (var sr = new StreamReader(ctx.Request.Body, Encoding.UTF8))
                    {
                        var body = await sr.ReadToEndAsync();
                        await SetResultAsync(body, ctx);
                    }
                }
            }
            else
            {
                ctx.Response.StatusCode = 405;
            }
        });
    }

    private async Task SetResultAsync(string value, HttpContext ctx)
    {
        try
        {
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentType = "text/html";
            ctx.Response.Headers.Add("Date", DateTimeOffset.UtcNow.ToString());
            await ctx.Response.WriteAsync(
                "<body>" +
                "   <h3>You can now return to the application. This page can be closed.</h3>" +
                "</body>");
            ctx.Response.Body.Flush();

            _source.TrySetResult(value);
        }
        catch (Exception)
        {
            ctx.Response.StatusCode = 400;
            ctx.Response.ContentType = "text/html";
            await ctx.Response.WriteAsync("<h3>Status 400: Invalid request.</h3>");
            ctx.Response.Body.Flush();
        }
    }
    #endregion

    #region Public Properties
    public string Url => _url + "/" + _path;
    #endregion

    #region Fields
    private readonly IWebHost _host;
    private readonly string _path;
    private readonly string _url;
    private readonly TaskCompletionSource<string> _source = new TaskCompletionSource<string>();
    #endregion
}
