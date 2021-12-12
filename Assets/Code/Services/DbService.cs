using System;
using System.Collections;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Services
{
    public class DbService : MonoBehaviour
    {
        public static DbService Instance;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        public IEnumerator GetRequest(string uri)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                string[] pages = uri.Split('/');
                int page = pages.Length - 1;

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                        break;
                }
            }
        }
        
        public async UniTask PostWithoutResponse<T>(string url, T data)
        {
            var str = JsonService.ToJson(data);
            
            //var request = new UnityWebRequest(url, "POST");
            //byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            //request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            //request.downloadHandler = new DownloadHandlerBuffer();
            //request.SetRequestHeader("Content-Type", "application/json");
 //
            //yield return request.SendWebRequest();
 //
            //Debug.Log("Status Code: " + request.responseCode);

            using (var request = UnityWebRequest.Put(url, str))
            {
                request.method = "POST";
                request.SetRequestHeader("Content-Type", "application/json");
                request.certificateHandler = new CertificateWhore();
                await request.SendWebRequest();
                Debug.Log("Status Code: " + request.responseCode);
            }
        }
        
        public async UniTask<bool> PostWithResponse<T>(string url, T data)
        {
            var str = JsonService.ToJson(data);

            using (var request = UnityWebRequest.Put(url, str))
            {
                request.method = "POST";
                request.SetRequestHeader("Content-Type", "application/json");
                request.certificateHandler = new CertificateWhore();
                await request.SendWebRequest();
                Debug.Log("Status Code: " + request.responseCode);

                var json = request.downloadHandler.text;
                var res = Convert.ToBoolean(json);
                //var res = JsonService.FromJson<TResult>(json);
                return res;
            }
        }
    }
}
