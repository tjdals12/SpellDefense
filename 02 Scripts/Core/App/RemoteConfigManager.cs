using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Newtonsoft.Json;

namespace Core.App {
    public class RemoteConfigManager : MonoBehaviour
    {
        IDictionary<string, ConfigValue> values;

        public async Task Initialize() {
            FirebaseRemoteConfig remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            Task fetchTask = remoteConfig.FetchAsync(TimeSpan.Zero);
            await fetchTask;
            if (fetchTask.IsCompleted == false) throw new Exception("Retrieval hasn't finished.");
            var info = remoteConfig.Info;
            if (info.LastFetchStatus != LastFetchStatus.Success) throw new Exception($"FetchRemoveConfig was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            Task<bool> activateTask = remoteConfig.ActivateAsync();
            await activateTask;
            this.values = remoteConfig.AllValues;
        }

        public AppSetting GetAppSetting() {
            if (values.TryGetValue("AppSetting", out ConfigValue value)) {
                AppSetting appSetting = JsonConvert.DeserializeObject<AppSetting>(value.StringValue);
                return appSetting;
            }
            return null;
        }
    }
}
