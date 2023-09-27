using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PP = UnityEngine.PlayerPrefs;

namespace NinjaTools {
    public class PlayerPrefs {
        public enum PPKey { HIGHSCORE }
        public static void SetInt(PPKey key, int number)
        {
            var logId = "SetInt";
            Utils.logd(logId, "Setting Key=" + key + " to " + number);
            PP.SetInt(key.ToString(), number);
        }
        public static void SetFloat(PPKey key, float number)
        {
            var logId = "SetFloat";
            Utils.logd(logId, "Setting Key=" + key + " to " + number);
            PP.SetFloat(key.ToString(), number);
        }
        public static void SetString(PPKey key, string text)
        {
            var logId = "SetString";
            Utils.logd(logId, "Setting Key=" + key + " to " + text);
            PP.SetString(key.ToString(), text);
        }
        public static int GetInt(PPKey key) {
            var logId = "GetInt";
            var toReturn = PP.GetInt(key.ToString());
            Utils.logd(logId, "Getting Key=" + key + " => " + toReturn.logf());
            return toReturn;
        }
        public static float GetFloat(PPKey key) {
            var logId = "GetFloat";
            var toReturn = PP.GetFloat(key.ToString());
            Utils.logd(logId, "Getting Key=" + key + " => " + toReturn.logf());
            return toReturn;
        }
        public static string GetString(PPKey key) {
            var logId = "GetString";
            var toReturn = PP.GetString(key.ToString());
            Utils.logd(logId, "Getting Key=" + key + " => " + toReturn.logf());
            return toReturn;
        }
    }

}
