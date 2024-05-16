using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.App {
    public class AppVersionChecker : MonoBehaviour
    {
        public bool CheckUpdate(VersionInfo versionInfo) {
            if (versionInfo == null) return false;
            int[] currentVersions = Utils.GetVersions(Utils.GetAppVersion());
            if (currentVersions == null) return false;
            int[] nextVersions = Utils.GetVersions(versionInfo.requiredAppVersion);
            if (nextVersions == null) return false;
            if (nextVersions[0] > currentVersions[0]) return true;
            if (nextVersions[1] > currentVersions[1]) return true;
            if (nextVersions[2] > currentVersions[2]) return true;
            return false;
        }
    }
}
