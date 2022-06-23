//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Net.Http;
//using System.Threading.Tasks;
//using WaterSight.Web.Support;

//namespace WaterSight.Web.Core;

//public static class CRUDBase
//{
//    //
//    // ADD / CREATE
//    //
//    public static async Task<int?> AddAsync<T>(this WS ws, T t, string url, string typeName)
//    {
//        var res = await Request.PostJsonString(url, JsonConvert.SerializeObject(t));
//        int? id = null;
//        if (res.IsSuccessStatusCode)
//        {
//            var idString = await res.Content.ReadAsStringAsync();
//            id = Convert.ToInt32(idString.Replace("\"", ""));
//            WS.Logger.Information($"{typeName} added, id: {id}.");
//        }
//        else
//            WS.Logger.Error($"Failed to add {typeName}. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

//        return id;
//    }
//    public static async Task<T> AddAsync<T>(this WS ws, string url, string typeName)
//    {
//        var res = await Request.Post(url, null);
//        T retValue = default;
//        if (res.IsSuccessStatusCode)
//        {
//            var responseString = await res.Content.ReadAsStringAsync();
//            retValue = JsonConvert.DeserializeObject<T>(responseString);
//            WS.Logger.Information($"{typeName} added. {retValue}");
//        }
//        else
//            WS.Logger.Error($"Failed to add {typeName}. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

//        return retValue;
//    }

//    //
//    // GET / READ
//    //
//    public static async Task<T> GetAsync<T>(this WS ws, string url, int? id, string typeName)
//    {
//        var res = await Request.Get(url);
//        T t = default;

//        if (res.IsSuccessStatusCode)
//        {
//            try
//            {
//                var jsonString = await res.Content.ReadAsStringAsync();
//                t = JsonConvert.DeserializeObject<T>(jsonString);
//                WS.Logger.Information($"{typeName} info found {(id == null ? "" : $"for id: {id}")}, {t}.");
//                return t;
//            }
//            catch (Exception ex)
//            {
//                WS.Logger.Error(ex, $"...while getting {typeName} from id: {id} \nMessage:{ex.Message}");
//                return t;
//            }
//        }
//        else
//        {
//            WS.Logger.Error($"Failed to get {typeName} data for id: {id}. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
//            return t;
//        }
//    }

//    //
//    // GET Many / READ many
//    //
//    public static async Task<List<T>> GetManyAsync<T>(this WS ws, string url, string typeName)
//    {
//        var res = await Request.Get(url);
//        var t = new List<T>();

//        if (res.IsSuccessStatusCode)
//        {
//            t = await Request.GetJsonAsync<List<T?>>(res) ?? t;
//            WS.Logger.Information($"Number of {typeName} received. Count = {t.Count}.");
//        }
//        else
//            WS.Logger.Error($"Failed to get {typeName} data. Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");

//        return t;
//    }

//    //
//    // UPDATE
//    //
//    public static async Task<bool> UpdateAsync<T>(this WS ws, int? id, T t, string url, string typeName, bool usePostMethod = false)
//    {
//        if (t == null)
//        {
//            WS.Logger.Error($"{typeName} cannot be null");
//            return false;
//        }

//        HttpResponseMessage? res = null;
//        if (!usePostMethod)
//            res = await Request.PutJsonString(url, JsonConvert.SerializeObject(t));
//        else
//            res = await Request.PostJsonString(url, JsonConvert.SerializeObject(t));

//        if (res?.IsSuccessStatusCode ?? false)
//            WS.Logger.Information($"{typeName} updated successfully.");

//        else
//            WS.Logger.Error($"Failed to update {typeName} with id: {id} ({t}). Reason: {res?.ReasonPhrase}. Text: {await res?.Content.ReadAsStringAsync()}. URL: {url}");

//        return res.IsSuccessStatusCode;
//    }

//    // 
//    // DELETE Single
//    //
//    public static async Task<bool> DeleteAsync(this WS ws, int? id, string url, string typeName, bool supportsLRO = false)
//    {
//        var res = await Request.Delete(url);
//        if (res.IsSuccessStatusCode)
//        {
//            WS.Logger.Information($"{typeName}'s delete request was successful for id: {id}");
//            if (supportsLRO)
//                _ = await Request.WaitForLRO(res);
//        }
//        else
//        {
//            WS.Logger.Warning($"{typeName}'s delete request failed for id: {id}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
//        }

//        return res.IsSuccessStatusCode;
//    }

//    // 
//    // DELETE Many
//    //
//    public static async Task<bool> DeleteManyAsync(this WS ws, string url, string typeName, bool supportsLRO = false)
//    {
//        var res = await Request.Delete(url);
//        if (res.IsSuccessStatusCode)
//        {
//            WS.Logger.Information($"{typeName}'s delete request for  was successful");
//            if (supportsLRO)
//                _ = await Request.WaitForLRO(res);
//        }
//        else
//        {
//            WS.Logger.Warning($"{typeName}'s delete request failed. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
//        }

//        return res.IsSuccessStatusCode;
//    }

//    //
//    // POST (Any)
//    //
//    public static async Task<bool> PostAsync(this WS ws, string url, HttpContent? content, string typeName, bool supportsLRO = false, string additionalInfo = "")
//    {
//        var res = await Request.Post(url, content);
//        if (res.IsSuccessStatusCode)
//        {
//            WS.Logger.Information($"{typeName}'s post request for  was successful. {additionalInfo}");
//            if (supportsLRO)
//                _ = await Request.WaitForLRO(res);
//        }
//        else
//        {
//            WS.Logger.Warning($"{typeName}'s post request failed. {additionalInfo} Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
//        }

//        return res.IsSuccessStatusCode;
//    }

//    //
//    // POST FIle
//    //
//    public static async Task<bool> PostFile(this WS ws, string url, FileInfo fileInfo, string fileTypeName = "Excel")
//    {
//        if (!fileInfo.Exists)
//        {
//            WS.Logger.Error($"Given {fileTypeName} file path is not valid. Path: {fileInfo.FullName}");
//            return false;
//        }

//        if (Util.IsFileInUse(fileInfo.FullName))
//            Debugger.Break();

//        var res = await Request.PostFile(url, fileInfo);
//        if (res.IsSuccessStatusCode)
//        {
//            WS.Logger.Information($"{fileTypeName} file uploaded successfully. Path: {fileInfo.FullName}");
//            return true;
//        }
//        else
//        {
//            WS.Logger.Error($"Failed to upload the {fileTypeName} file. Path: {fileInfo.FullName} Reason: {res.ReasonPhrase}. Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
//            return false;
//        }
//    }

//    //
//    // PUT (Any)
//    //
//    public static async Task<bool> PutAsync(this WS ws, string url, HttpContent? content, string typeName, bool supportsLRO = false, string additionalInfo = "")
//    {
//        var res = await Request.Put(url, content);
//        if (res.IsSuccessStatusCode)
//        {
//            WS.Logger.Information($"{typeName}'s put request for  was successful. {additionalInfo}");
//            if (supportsLRO)
//                _ = await Request.WaitForLRO(res);
//        }
//        else
//        {
//            WS.Logger.Warning($"{typeName}'s put request failed. {additionalInfo} Text: {await res.Content.ReadAsStringAsync()}. URL: {url}");
//        }

//        return res.IsSuccessStatusCode;
//    }
//}