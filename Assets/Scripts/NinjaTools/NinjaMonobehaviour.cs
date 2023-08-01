using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NinjaTools {
public class NinjaMonoBehaviour : MonoBehaviour {
    Dictionary<string, string> lastIdMessage = new Dictionary<string, string>();
    public void logd(string id, string message, bool ignoreDuplicates=false) {
        if(ignoreDuplicates && lastIdMessage.ContainsKey(id) && lastIdMessage[id]==message) {
            return;
        }
        Debug.Log(name + "::" + id + "->" + message);
        if(lastIdMessage.ContainsKey(id)) {
            lastIdMessage[id] = message;
        } else {
            lastIdMessage.Add(id, message);
        }
    }
    
    public void logw(string id, string message, bool ignoreDuplicates=false) {
        if(ignoreDuplicates && lastIdMessage.ContainsKey(id) && lastIdMessage[id]==message) {
            return;
        }
        Debug.LogWarning(name + "::" + id + "->" + message);
        if(lastIdMessage.ContainsKey(id)) {
            lastIdMessage[id] = message;
        } else {
            lastIdMessage.Add(id, message);
        }
    }
    
    public void loge(string id=null, string message=null, bool ignoreDuplicates=false) {
        if(ignoreDuplicates && lastIdMessage.ContainsKey(id) && lastIdMessage[id]==message) {
            return;
        }
        Debug.LogError(name + "::" + id + "->" + message);
        if(lastIdMessage.ContainsKey(id)) {
            lastIdMessage[id] = message;
        } else {
            lastIdMessage.Add(id, message);
        }

    }
    public void logt(string id=null, string message=null) {
        return;
    }
}
}