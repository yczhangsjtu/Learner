using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudyCardObject : MonoBehaviour
{
    public string topic;
    public List<StudyMaterialPageObject> pages;

    [Serializable]
    public class StudyTestNodeObject {
        [SerializeField]
        public StudyTestPageObject page;

        [SerializeField]
        public int nextPageCorrect;

        [SerializeField]
        public int nextPageWrong;

        [SerializeField]
        public int score;
    }

    [SerializeField]
    public List<StudyTestNodeObject> nodes;

    public int startNode;
    public int endNode;
}
