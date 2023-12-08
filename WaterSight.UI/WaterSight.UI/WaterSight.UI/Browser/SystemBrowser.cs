using IdentityModel.OidcClient.Browser;
using Serilog;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace WaterSight.UI.Browser;

public class SystemBrowser : IBrowser
{
    #region Constructor
    public SystemBrowser(int? port = null, string? path = null)
    {
        _path = path;
        Port = port.HasValue ? port.Value : GetRandomUnusedPort();
    }
    #endregion

    #region Public Methods
    public static void OpenBrowser(string url)
    {
        Process? process = null;
        try
        {
            Log.Information($"About to start: {url}");
            process = Process.Start(url);
        }
        catch (Exception ex)
        {
            url = url.Replace("&", "^&", StringComparison.InvariantCulture);
            try
            {
                process = Process.Start(
                    new ProcessStartInfo(
                        "cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            catch (Exception)
            {
                if (process?.Id <= 0)
                {
                    Log.Error(ex, $"...while opening up a URL: '{url}'");
                    throw;
                }
            }
        }

        if (process != null)
            Log.Debug($"Process started. Id: {process.Id}, Name: {process.ProcessName}");

    }
    public async Task<BrowserResult> InvokeAsync(
        BrowserOptions options,
        CancellationToken cancellationToken)
    {
        using (var listener = new LoopbackHttpListener(Port, _path))
        {
            OpenBrowser(options.StartUrl);

            try
            {
                var result = await listener.WaitForCallbackAsync();
                if (string.IsNullOrWhiteSpace(result))
                {
                    return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = "Empty response." };
                }

                return new BrowserResult { Response = result, ResultType = BrowserResultType.Success };
            }
            catch (TaskCanceledException ex)
            {
                return new BrowserResult { ResultType = BrowserResultType.Timeout, Error = ex.Message };
            }
            catch (Exception ex)
            {
                return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = ex.Message };
            }
        }
    }
    #endregion

    #region Private Methods
    private static int GetRandomUnusedPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();

        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
    #endregion

    #region Public Propeties
    public int Port { get; }
    #endregion

    #region Field
    private readonly string? _path;
    #endregion
}
