using System;
using System.Collections.Generic;
using UnityEngine;

namespace Learner.StudyCard {
    public class StudyMaterial {

        public readonly List<StudyMaterialPage> pages;

        public StudyMaterial(List<StudyMaterialPage> pages) {
            this.pages = pages;
        }
    }

    public class StudyMaterialPage {

        public readonly string imageAssetName;
        public readonly string text;

        public StudyMaterialPage(string imageAssetName = null, string text = null) {
            Debug.Assert(!string.IsNullOrEmpty(imageAssetName) ||
                         !string.IsNullOrEmpty(text));
            this.imageAssetName = imageAssetName;
            this.text = text;
        }
    }
}
