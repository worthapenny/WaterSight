using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WaterSight.Web.Support;

namespace WaterSight.Web.Core;

public static class Request
{

    #region Public Static Methods

    #region CRUD Operation
    public static async Task<HttpResponseMessage> Get(string url)
    {
        Logger.Debug($"GET request, URL: {url}");

        // URL to get the list of DTs
        var uri = new Uri(url);

        // add the bearer token
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);


        try
        {
            var sw = Util.StartTimer();
            var res = await HttpClient.GetAsync(uri);

            if ((int)res.StatusCode >= 400)
            {
                Logger.Error($"Request status code: {res.StatusCode}, reason: {res.ReasonPhrase}");
                Debugger.Break();
            }

            Logger.Debug($"Get request time-taken: {sw.Elapsed}. {url}");
            Logger.Debug($"Request status code: {res.StatusCode}, reason: {res.ReasonPhrase}");
            sw.Stop();

            return res;
        }
        catch (HttpRequestException ex)
        {
            Logger.Error(ex, "...while performing the get request.");
        }

        return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
    }
    public static async Task<HttpResponseMessage> Put(string url, HttpContent? content)
    {
        WS.Logger?.Debug($"Put request, URL: {url}");

        // URL to get the list of DTs
        var uri = new Uri(url);

        // add the bearer token
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);

        try
        {

            var sw = Util.StartTimer();
            var res = await HttpClient.PutAsync(uri, content);

            if (res.StatusCode == HttpStatusCode.Unauthorized)
            {
                Debugger.Break();
            }

            Logger.Debug($"Put request time-taken: {sw.Elapsed}. {url}");
            Logger.Debug($"Request status code: {res.StatusCode}, reason: {res.ReasonPhrase}");
            sw.Stop();

            return res;
        }
        catch (HttpRequestException ex)
        {
            Logger.Error(ex, "...while performing the put request.");
        }

        return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
    }
    public static async Task<HttpResponseMessage> Post(string url, HttpContent? content, TimeSpan? timeout = null)
    {
        WS.Logger?.Debug($"Post request, URL: {url}");

        var uri = new Uri(url);

        // add the bearer token
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);

        try
        {
            var sw = Util.StartTimer();


            // Set the timeout to given value
            // and to do so, we must start a new HttpClient instance
            var httpClient = HttpClient;
            if (timeout.HasValue)
            {
                httpClient = new HttpClient();
                // add the bearer token
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);

                httpClient.Timeout = timeout.Value;
            }

            var res = await httpClient.PostAsync(uri, content);

            if (res.StatusCode == HttpStatusCode.Unauthorized)
            {
                Debugger.Break();
            }

            Logger.Debug($"Post request time-taken: {sw.Elapsed}. {url}");
            Logger.Debug($"Request status code: {res.StatusCode}, reason: {res.ReasonPhrase}");
            sw.Stop();

            return res;
        }
        catch (HttpRequestException ex)
        {
            Logger.Error(ex, "...while performing the post request.");
        }

        return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
    }
    public static async Task<HttpResponseMessage> Delete(string url)
    {
        WS.Logger?.Debug($"Delete request, URL: {url}");

        var uri = new Uri(url);

        // add the bearer token
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BearerToken);

        try
        {
            var sw = Util.StartTimer();
            var res = await HttpClient.DeleteAsync(uri);

            if (res.StatusCode == HttpStatusCode.Unauthorized)
            {
                Debugger.Break();
            }

            Logger.Debug($"Delete request time-taken: {sw.Elapsed}. {url}");
            Logger.Debug($"Request status code: {res.StatusCode}, reason: {res.ReasonPhrase}");
            sw.Stop();

            return res;
        }
        catch (HttpRequestException ex)
        {
            Logger.Error(ex, $"...while performing the delete request.");
        }

        return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
    }

    #endregion

    public static async Task<T?> GetJsonAsync<T>(HttpResponseMessage response)
    {
        var retValue = default(T);
        if (response.IsSuccessStatusCode)
        {
            try
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    FloatFormatHandling = FloatFormatHandling.Symbol,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var text = await response.Content.ReadAsStringAsync();
                if (text != "" || text != "[]" || text != "{}")
                {
                    retValue = JsonConvert.DeserializeObject<T>(text, jsonSerializerSettings);
                    Logger.Verbose($"Deserialized, text length: {text.Length}");
                }
                else
                {
                    Logger.Warning($"Empty object received. Text: {text}");
                }

                return retValue;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "...while deserializing the response text.");
            }
        }
        else
        {
            Logger.Error($"Message: {await response.Content.ReadAsStringAsync()}");
        }

        return retValue;
    }
    public static async Task<HttpResponseMessage> PostJsonString(string url, string jsonString)
    {
        return await Post(
            url,
            new StringContent(jsonString, Encoding.UTF8, "application/json"));
    }


    public static async Task<HttpResponseMessage> PostFile(string url, FileInfo fileInfo, TimeSpan? timeout = null)
    {
        if (!fileInfo.Exists)
            throw new FileNotFoundException(fileInfo.FullName);

        WS.Logger.Debug($"🆙 About to upload a file... Path: {fileInfo.FullName}");

        try
        {
            var content = new StreamContent(File.OpenRead(fileInfo.FullName));
            content.Headers.ContentType = new MediaTypeHeaderValue(System.Web.MimeMapping.GetMimeMapping(fileInfo.Name)); //fcontent.Headers.Add("Content-Type", "application/octet-stream");
            content.Headers.Add("Content-Disposition", "form-data; name=\"file\"; filename=\"" + fileInfo.Name + "\"");

            var formContent = new MultipartFormDataContent();
            formContent.Add(content, "file", fileInfo.Name);

            var res = await Post(url, formContent, timeout);

            formContent.Dispose();
            return res;
        }
        catch (IOException ex)
        {
            Logger.Error(ex, $"...while trying to prepare the file for upload. Path: {fileInfo.FullName}");
            return new HttpResponseMessage(HttpStatusCode.Conflict);
        }
    }
    public static async Task<HttpResponseMessage> PutJsonString(string url, string jsonString)
    {
        return await Put(
            url,
            new StringContent(jsonString, Encoding.UTF8, "application/json"));
    }


    public static async Task<bool> WaitForLRO(HttpResponseMessage res, int checkIntervalSeconds = 5)
    {
        if (!res.IsSuccessStatusCode)
        {
            WS.Logger.Error($"Given response is not valid for LRO");
            return true;
        }

        var lroStatusUrlPart = res.Headers.Where(h => h.Key == "Operation-Location");

        if (!lroStatusUrlPart.Any())
        {
            WS.Logger.Warning($"No valid headers found");
            return false;
        }

        var lroStatusUrl = string.Empty;
        lroStatusUrl = lroStatusUrlPart.First().Value.First();
        lroStatusUrl = res.RequestMessage?.RequestUri?.ToString().Split(new[] { "api" }, StringSplitOptions.None)[0] + lroStatusUrl;
        var isLroSuccessful = false;

        if (lroStatusUrl == null)
        {
            WS.Logger.Warning($"No valid URL could be constructed from the response headers");
            return false;
        }

        Logger.Debug(Util.LogSeparator("LRO Started", Util.Star));

        var stopwatch = Util.StartTimer();
        var completed = false;
        while (!completed)
        {
            var doneRes = await Get(lroStatusUrl);
            if (!doneRes.IsSuccessStatusCode)
            {
                WS.Logger.Error($"Request to check LRO status failed.");
                break;
            }

            var jsonData = await GetJsonAsync<Dictionary<string, object>>(doneRes);
            object? statusCode = -1;
            if (jsonData?.TryGetValue("Status", out statusCode) ?? false)
            {
                statusCode = Convert.ToInt32(statusCode.ToString());
                var percentDone = jsonData["PercentComplete"]?.ToString();
                var message = jsonData["StatusMessage"]?.ToString();
                var source = jsonData["ServiceName"]?.ToString();

                if ((int)statusCode == 0 || (int)statusCode == 1)
                {
                    WS.Logger.Information($"LRO status {statusCode}, [{percentDone}%] Message: {message}, Source: {source}. Sleeping for {checkIntervalSeconds} seconds...");
                    await Task.Delay(checkIntervalSeconds * 1000);
                }
                else if ((int)statusCode == 2)
                {
                    WS.Logger.Information($"LRO completed. [{percentDone}%] Message: {message}, Source: {source}, URL: {lroStatusUrl}");
                    completed = true;
                    isLroSuccessful = true;
                }
                else if ((int)statusCode == 3)
                {
                    WS.Logger.Error($"LRO status is Failed. [{percentDone}%] Message: {message}, Source: {source} URL: {lroStatusUrl}");
                    completed = true;
                }
            }
        }

        var timeTaken = stopwatch.Elapsed;
        stopwatch.Stop();

        WS.Logger.Information($"Time-taken by LRO: {timeTaken}");
        Logger.Debug(Util.LogSeparator("LRO Ended", Util.XSmall));

        return isLroSuccessful;
    }

    #endregion

    #region Private Static Methods
    private static string WaterSightAccessToken()
    {
        if (!string.IsNullOrEmpty(Options.RestToken))
            return Options.RestToken;


        var token = string.Empty;
        RegistryKey? key = null;
        try
        {
            Logger.Verbose($"About to pull token from registry. {Options.TokenRegistryPath}");
            if (string.IsNullOrWhiteSpace(Options.TokenRegistryPath))
                throw new InvalidOperationException($"Given {nameof(Options.TokenRegistryPath)} is invalid: '{Options.TokenRegistryPath}'");

            var registryPathParts = Options.TokenRegistryPath.Split(Path.DirectorySeparatorChar);

            if (registryPathParts == null || registryPathParts.Length == 0)
            {
                Logger.Error("Invalid registry path in settings file. Please update the settings JSON file.");
                return String.Empty;
            }


            // The last part in the path will be name of the registry path
            var name = registryPathParts.Last();

            // except the last part, will have the registry path
            var registryPathRoot = string.Join(Path.DirectorySeparatorChar.ToString(), registryPathParts.Take(registryPathParts.Length - 1)); //.SkipLast(1));
            Logger.Verbose($"Registry path: {registryPathRoot}");

            key = Registry.CurrentUser.OpenSubKey(registryPathRoot);

            if (Util.IsAdministrator())
                key = Registry.LocalMachine.OpenSubKey(registryPathRoot);

            if (key == null)
            {
                Logger.Fatal($"Could not open up the registry path: {registryPathRoot}, key: {key}");
                Debugger.Break();
            }
            else
            {
                token = key.GetValue(name)?.ToString();
                if(string.IsNullOrEmpty(token) ) 
                    Debugger.Break();

                Logger.Verbose($"StatQueryValue obtained from registry has a length of {token?.Length}, key: {key}");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "...while pulling data from registry");
        }
        finally
        {
            key?.Close();
        }

        if (token == null)
            Logger.Fatal($"Bearer token from registry is null!");

        return token ?? "";
    }

    #endregion

    #region Public Static Properties
    public static Options Options => options;
    public static Options options;

    // Do not cache the token as it could change ever .5 hrs or so
    public static string BearerToken => WaterSightAccessToken();
    #endregion

    #region Private Static Properties      
    private static HttpClient HttpClient => httpClient ??= new HttpClient();
    private static ILogger Logger => WS.Logger;


    #endregion

    #region Private Static Fields
    private static HttpClient? httpClient;
    #endregion
}
