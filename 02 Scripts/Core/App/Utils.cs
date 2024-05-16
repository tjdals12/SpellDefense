using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.App {
    public enum Environment {
        Production = 0,
        Development,
    }

    public class Utils
    {
        public static bool CheckIsValidVersion(string version) {
            Regex regex = new Regex(@"^([0-9]+\.[0-9]+\.[0-9]+)$");
            Match m1 = regex.Match(version);
            return m1.Success;
        }

        public static string GetVersion(string version) {
            Regex regex = new Regex(@"^([0-9]+\.[0-9]+\.[0-9]+)");
            Match m1 = regex.Match(version);
            if (m1.Success == false) return null;
            string semver = m1.Groups[0].Value;
            return semver;
        }

        public static int[] GetVersions(string version) {
            string semver = GetVersion(version);
            if (semver == null) return null;
            int[] versions = semver.Split(".").Select((v) => int.Parse(v)).ToArray();
            return versions;
        }

        public static string GetAppVersion() {
            string version = GetVersion(Application.version);
            return version;
        }

        public static Environment GetCurrentEnv() {
            string currentVersion = Application.version;
            if (currentVersion.EndsWith("prod")) {
                return Environment.Production;
            }
            return Environment.Development;
        }
    }
}
