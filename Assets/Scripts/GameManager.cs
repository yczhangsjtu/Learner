﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    enum State {
        menu,
        reviewing,
        testing,
        scoreboard
    }

    [Header("Content")]
    public List<StudyCardObject> studyCardObjects;

    [Header("Menu")]
    public GameObject menu;
    public TMPro.TMP_Dropdown dropdown;
    public TMPro.TextMeshProUGUI dropdownLabel;

    [Header("Studying")]
    public GameObject studyCard;
    public GameObject imageScrollView;
    public GameObject wideImageScrollView;
    public Image image;
    public Image wideImage;
    public GameObject contentScrollView;
    public GameObject wideContentScrollView;
    public TMPro.TextMeshProUGUI content;
    public TMPro.TextMeshProUGUI wideContent;

    [Header("Reviewing")]
    public GameObject pageControl;

    [Header("Testing")]
    public TMPro.TMP_InputField answer;
    public GameObject score;
    public TMPro.TextMeshProUGUI scoreValue;
    public GameObject scoreBoard;
    public TMPro.TextMeshProUGUI scoreBoardValue;

    State state;
    StudyCardObject selectedStudyCard;
    int currentPage;
    int scoreInt;

    // Start is called before the first frame update
    void Start()
    {
        if(dropdown != null &&
            studyCardObjects != null &&
            studyCardObjects.Count > 0) {
            dropdown.options = new List<TMPro.TMP_Dropdown.OptionData>();
            foreach(var card in studyCardObjects) {
                dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(
                    card.topic
                ));
            }
        }
        if(dropdownLabel != null &&
            studyCardObjects != null &&
            studyCardObjects.Count > 0) {
            dropdownLabel.text = studyCardObjects[0].topic;
        }
        if(studyCardObjects != null && studyCardObjects.Count > 0) {
            selectedStudyCard = studyCardObjects[0];
        }

        BackToMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.scoreboard) {
            if(Input.GetMouseButton(0)) {
                BackToMenu();
            }
        }
    }

    public void onDropDownValueChanged(int index) {
        if(dropdownLabel != null) {
            dropdownLabel.text = studyCardObjects[index].topic;
        }
        selectedStudyCard = studyCardObjects[index];
    }

    public void StartReviewing() {
        menu.SetActive(false);
        answer.gameObject.SetActive(false);
        score.SetActive(false);
        scoreBoard.SetActive(false);
        studyCard.SetActive(true);
        pageControl.SetActive(true);

        currentPage = 0;
        state = State.reviewing;
        UpdateContent();
    }

    public void StartTesting() {
        if(selectedStudyCard.nodes.Count == 0) {
            return;
        }
        menu.SetActive(false);
        answer.gameObject.SetActive(true);
        score.SetActive(true);
        scoreBoard.SetActive(false);
        studyCard.SetActive(true);
        pageControl.SetActive(false);

        currentPage = selectedStudyCard.startNode;
        state = State.testing;
        scoreInt = 0;
        UpdateContent();
    }

    public void BackToMenu() {
        menu.SetActive(true);
        answer.gameObject.SetActive(false);
        score.SetActive(false);
        scoreBoard.SetActive(false);
        studyCard.SetActive(false);
        state = State.menu;
    }

    void UpdateContent() {
        if(state == State.reviewing) {
            if(string.IsNullOrWhiteSpace(selectedStudyCard.pages[currentPage].materialText)) {
                wideImage.sprite = selectedStudyCard.pages[currentPage].materialImage;
                wideImageScrollView.SetActive(true);
                imageScrollView.SetActive(false);
                contentScrollView.SetActive(false);
                wideContentScrollView.SetActive(false);
            } else if(selectedStudyCard.pages[currentPage].materialImage == null) {
                wideContent.text = selectedStudyCard.pages[currentPage].materialText;
                wideImageScrollView.SetActive(false);
                imageScrollView.SetActive(false);
                contentScrollView.SetActive(false);
                wideContentScrollView.SetActive(true);
            } else {
                image.sprite = selectedStudyCard.pages[currentPage].materialImage;
                content.text = selectedStudyCard.pages[currentPage].materialText;
                wideImageScrollView.SetActive(false);
                imageScrollView.SetActive(true);
                contentScrollView.SetActive(true);
                wideContentScrollView.SetActive(false);
            }
        } else if(state == State.testing) {
            if(string.IsNullOrWhiteSpace(selectedStudyCard.nodes[currentPage].page.materialText)) {
                wideImage.sprite = selectedStudyCard.nodes[currentPage].page.materialImage;
                wideImageScrollView.SetActive(true);
                imageScrollView.SetActive(false);
                contentScrollView.SetActive(false);
                wideContentScrollView.SetActive(false);
            } else if(selectedStudyCard.nodes[currentPage].page.materialImage == null) {
                wideContent.text = selectedStudyCard.nodes[currentPage].page.materialText;
                wideImageScrollView.SetActive(false);
                imageScrollView.SetActive(false);
                contentScrollView.SetActive(false);
                wideContentScrollView.SetActive(true);
            } else {
                image.sprite = selectedStudyCard.nodes[currentPage].page.materialImage;
                content.text = selectedStudyCard.nodes[currentPage].page.materialText;
                wideImageScrollView.SetActive(false);
                imageScrollView.SetActive(true);
                contentScrollView.SetActive(true);
                wideContentScrollView.SetActive(false);
            }
            scoreValue.text = $"{scoreInt}";
        }
    }

    public void NextPage() {
        if(currentPage < selectedStudyCard.pages.Count - 1) {
            currentPage++;
            UpdateContent();
        }
    }

    public void PrevPage() {
        if(currentPage > 0) {
            currentPage--;
            UpdateContent();
        }
    }

    public void FinishAnswer() {
        var currentNode = selectedStudyCard.nodes[currentPage];
        if(currentNode.page.check(answer.text)) {
            scoreInt += currentNode.score;
            currentPage = currentNode.nextPageCorrect;
        } else {
            currentPage = currentNode.nextPageWrong;
        }
        answer.text = "";
        if(currentPage == selectedStudyCard.endNode) {
            scoreBoard.SetActive(true);
            studyCard.SetActive(false);
            scoreBoardValue.text = $"{scoreInt}";
            state = State.scoreboard;
        } else {
            UpdateContent();
        }
    }
}
