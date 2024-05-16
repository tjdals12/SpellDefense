using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.App {
    public class VersionInfo {
        public string latestAppVersion { get; private set; }
        public string latestDataTableVersion { get; private set; }
        public string requiredAppVersion { get; private set; }
        public string requiredDataTableVersion { get; private set; }
        public string defaultDataTableversion { get; private set; }
        public VersionInfo(string latestAppVersion, string latestDataTableVersion, string requiredAppVersion, string requiredDataTableVersion, string defaultDataTableversion) {
            this.latestAppVersion = latestAppVersion;
            this.latestDataTableVersion = latestDataTableVersion;
            this.requiredAppVersion = requiredAppVersion;
            this.requiredDataTableVersion = requiredDataTableVersion;
            this.defaultDataTableversion = defaultDataTableversion;
        }
    }

    public class AppSetting {
        public VersionInfo aosVersionInfo;
        public VersionInfo iosVersionInfo;
        public AppSetting(VersionInfo aosVersionInfo, VersionInfo iosVersionInfo) {
            this.aosVersionInfo = aosVersionInfo;
            this.iosVersionInfo = iosVersionInfo;
        }
        public VersionInfo GetVersionInfo() {
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.LinuxEditor) {
                return aosVersionInfo;
            } else if (Application.platform == RuntimePlatform.Android) {
                return aosVersionInfo;
            } else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer) {
                return iosVersionInfo;
            }
            return null;
        }
    }
}
