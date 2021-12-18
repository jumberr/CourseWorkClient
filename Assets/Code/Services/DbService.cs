using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Services
{
    [Serializable]
    public class DbService
    {
        private static DbService instance;
        
        public static DbService Instance => 
            instance ??= new DbService();

        public async UniTask PostNoResponse<T>(string url, T data)
        {
            var str = JsonService.ToJson(data);

            using var request = UnityWebRequest.Put(url, str);
            request.method = "POST";
            request.SetRequestHeader("Content-Type", "application/json");
            request.certificateHandler = new CertificateWhore();
            await request.SendWebRequest();
            Debug.Log("Status Code: " + request.responseCode);
        }
        
        public async UniTask<object> PostResponse<T>(string url, T data)
        {
            var str = JsonService.ToJson(data);

            using var request = UnityWebRequest.Put(url, str);
            request.method = "POST";
            request.SetRequestHeader("Content-Type", "application/json");
            request.certificateHandler = new CertificateWhore();
            await request.SendWebRequest();
            Debug.Log("Status Code: " + request.responseCode);

            var json = request.downloadHandler.text;
            return json;
        }
        
        public async UniTask<object> GetResponse(string url)
        {
            using var request = UnityWebRequest.Get(url);
            request.method = "GET";
            request.certificateHandler = new CertificateWhore();
            await request.SendWebRequest();
            Debug.Log("Status Code: " + request.responseCode);

            var json = request.downloadHandler.text;
            return json;
        }
        
        public async UniTask<object> GetResponse<T>(string url, T data)
        {
            var str = JsonService.ToJson(data);

            using var request = UnityWebRequest.Put(url, str);
            request.method = "GET";
            request.SetRequestHeader("Content-Type", "application/json");
            request.certificateHandler = new CertificateWhore();
            await request.SendWebRequest();
            Debug.Log("Status Code: " + request.responseCode);

            var json = request.downloadHandler.text;
            return json;
        }
        
        //public async UniTask<object> DeleteResponse<T>(string url, T data)
        //{
        //    var str = JsonService.ToJson(data);
//
        //    using var request = UnityWebRequest.Delete(url);
        //    request.method = "DELETE";
        //    request.SetRequestHeader("Content-Type", "application/json");
        //    request.certificateHandler = new CertificateWhore();
        //    await request.SendWebRequest();
        //    Debug.Log("Status Code: " + request.responseCode);
//
        //    var json = request.downloadHandler.text;
        //    return json;
        //}
    }
}
