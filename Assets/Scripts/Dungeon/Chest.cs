using System;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Transform itemPos;
    
    [Header("Item")]
    [SerializeField] private bool usePredefinedChest;
    [SerializeField] private GameObject predefinedItem;

    private Animator animator;
    private bool openedChest;

    private Question currentQuestion;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void ShowItem()
    {
        if (usePredefinedChest)
        {
            Instantiate(predefinedItem, transform.position,
                Quaternion.identity, itemPos);
        }
        else // Random Item
        {
            GameObject randomItem = LevelManager.Instance.GetRandomItemForChest();
            Instantiate(randomItem, transform.position,
                Quaternion.identity, itemPos);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (openedChest) return;
        if (other.CompareTag("Player") == false) return;

        currentQuestion = Question.QuestionDatabase.GetRandomQuestion();
        if (currentQuestion != null)
        {
            UIManager.Instance.ShowQuestion(currentQuestion, OnQuestionAnswered);
        }
    }

    private void OnQuestionAnswered(bool correct)
    {
        if (correct)
        {
            animator.SetTrigger("OpenChest");
            ShowItem();
            openedChest = true;
            Question.QuestionDatabase.MarkQuestionAsUsed(currentQuestion);
        }
        else
        {
            // DO NOTHING FOR USER ANSWER AGAIN
            // Optionally, provide feedback to the player that the answer was incorrect
            //Debug.Log("Incorrect answer. Try again.");
        }
    }
    // OLD VERSION - CHEST() -> RANDOM WEAPON
    /*private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void ShowItem()
    {
        if (usePredefinedChest)
        {
            Instantiate(predefinedItem, transform.position,
                Quaternion.identity, itemPos);
        }
        else // Random Item
        {
            GameObject randomItem = LevelManager.Instance.GetRandomItemForChest();
            Instantiate(randomItem, transform.position, 
                Quaternion.identity, itemPos);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (openedChest) return;
        if (other.CompareTag("Player") == false) return;
        animator.SetTrigger("OpenChest");
        ShowItem();
        openedChest = true;
    }*/
}
