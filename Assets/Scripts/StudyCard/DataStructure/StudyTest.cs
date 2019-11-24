using System;
using System.Collections.Generic;

namespace Learner.StudyCard {
    public class StudyTest {

        public readonly List<StudyTestNode> studyTestNodes;
        public readonly int endMark;

        public StudyTest(List<StudyTestNode> studyTestNodes) {
            this.studyTestNodes = studyTestNodes;
        }
    }

    public class StudyTestNode {

        public readonly StudyTestPage page;
        public readonly NextPage nextPage;

        public StudyTestNode(StudyTestPage page, NextPage nextPage) {
            this.page = page;
            this.nextPage = nextPage;
        }
    }

    public class StudyTestPage {
    
        public readonly StudyMaterialPage displayContent;    
        public readonly StudyTestAnswerChecker checker;
    
        public StudyTestPage(
            StudyMaterialPage displayContent,
            StudyTestAnswerChecker checker) {

            this.displayContent = displayContent;
            this.checker = checker;    
        }
    }

    public delegate int StudyTestAnswerChecker(string answer);
    public delegate int NextPage(int score, int currentScore, out int newScore);
}
