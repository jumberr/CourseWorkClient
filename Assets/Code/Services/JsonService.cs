using Code.Models;
using UnityEngine;

namespace Code.Services
{
    public static class JsonService
    {
        public static string ToJson<T>(T data)
        {
            return JsonUtility.ToJson(data);
        }

        public static T FromJson<T>(string data)
        {
            return JsonUtility.FromJson<T>(data);
        }
        
        public static T[] FromJsonToList<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>("{\"Items\":" + json + "}");
            return wrapper.Items;
        }
    }
}