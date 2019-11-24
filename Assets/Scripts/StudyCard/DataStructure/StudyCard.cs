using System;

namespace Learner.StudyCard {
    public class StudyCard {

        public readonly StudyMaterial studyMaterial;
        public readonly StudyTest studyTest;

        public StudyCard(StudyMaterial studyMaterial, StudyTest studyTest) {
            this.studyMaterial = studyMaterial;
            this.studyTest = studyTest;
        }
    }
}
