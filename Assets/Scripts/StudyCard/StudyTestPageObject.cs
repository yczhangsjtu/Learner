using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyTestPageObject : MonoBehaviour
{
    public StudyMaterialPageObject hintMaterial;
    public List<string> keywords;
    public bool inOrder = false;
    public bool noRedundant = false;

    public bool check(string answer) {
        if(inOrder) {
            foreach(string keyword in keywords) {
                if(noRedundant) {
                    if(!answer.StartsWith(keyword, System.StringComparison.Ordinal)) {
                        return false;
                    }
                } else {
                    var location = answer.IndexOf(keyword, System.StringComparison.Ordinal);
                    if(location < 0) {
                        return false;
                    }
                    answer = answer.Substring(location + keyword.Length);
                }
            }
            return true;
        }
        int totalLength = 0;
        foreach(string keyword in keywords) {
            totalLength += keyword.Length;
            if(!answer.Contains(keyword)) {
                return false;
            }
        }
        return !noRedundant || answer.Length == totalLength;
    }
}
