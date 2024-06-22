using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Player UI")]
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image armorBar;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private Image energyBar;
    [SerializeField] private TextMeshProUGUI energyText;

    [Header("UI Extra")]
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI levelTMP;
    [SerializeField] private TextMeshProUGUI completedTMP;
    [SerializeField] private TextMeshProUGUI coinsTMP;

    [Header("UI Weapon")]
    [SerializeField] private GameObject weaponPanel;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponEnergyTMP;

    [Header("Question UI")]
    [SerializeField] private GameObject panelQA;
    [SerializeField] private TextMeshProUGUI textQuestion;
    [SerializeField] private Button answer1;
    [SerializeField] private Button answer2;
    [SerializeField] private Button answer3;
    [SerializeField] private Button answer4;

    [Header("Game Finished UI")]
    [SerializeField] private GameObject gameFinishedPanel;
    [SerializeField] private Button backToMainButton;

    private Action<bool> onQuestionAnswered;


    private void Update()
    {
        UpdatePlayerUI();
        coinsTMP.text = CoinManager.Instance.Coins.ToString();
    }

    private void UpdatePlayerUI()
    {
        PlayerConfig playerConfig = GameManager.Instance.Player;
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount,
            playerConfig.CurrentHealth / playerConfig.MaxHealth, 10f * Time.deltaTime);
        armorBar.fillAmount = Mathf.Lerp(armorBar.fillAmount,
            playerConfig.Armor / playerConfig.MaxArmor, 10f * Time.deltaTime);
        energyBar.fillAmount = Mathf.Lerp(energyBar.fillAmount,
            playerConfig.Energy / playerConfig.MaxEnergy, 10f * Time.deltaTime);

        healthText.text = $"{playerConfig.CurrentHealth}/{playerConfig.MaxHealth}";
        armorText.text = $"{playerConfig.Armor}/{playerConfig.MaxArmor}";
        energyText.text = $"{playerConfig.Energy}/{playerConfig.MaxEnergy}";
    }

    public void FadeNewDungeon(float value)
    {
        StartCoroutine(Helpers.IEFade(fadePanel, value, 1.5f));
    }

    public void UpdateLevelText(string levelText)
    {
        levelTMP.text = levelText;
    }

    public void PlayButton()
    {
        SceneManager.LoadScene("Main");
    }

    private void RoomCompletedCallback()
    {
        StartCoroutine(IERoomCompleted());
    }

    private IEnumerator IERoomCompleted()
    {
        completedTMP.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        completedTMP.gameObject.SetActive(false);
    }

    private void WeaponUIUpdateCallback(Weapon currentWeapon)
    {
        if (weaponPanel.activeSelf == false)
        {
            weaponPanel.SetActive(true);
        }

        weaponEnergyTMP.text = currentWeapon.ItemWeapon.RequiredEnergy.ToString();
        weaponIcon.sprite = currentWeapon.ItemWeapon.Icon;
    }

    private void PlayerDeadCallback()
    {
        gameOverPanel.SetActive(true);
    }

    // SET QUESTION()
    public void ShowQuestion(Question question, Action<bool> callback)
    {
        panelQA.SetActive(true);
        textQuestion.text = question.questionText;
        answer1.GetComponentInChildren<TextMeshProUGUI>().text = question.answerA;
        answer2.GetComponentInChildren<TextMeshProUGUI>().text = question.answerB;
        answer3.GetComponentInChildren<TextMeshProUGUI>().text = question.answerC;
        answer4.GetComponentInChildren<TextMeshProUGUI>().text = question.answerD;

        answer1.onClick.RemoveAllListeners();
        answer2.onClick.RemoveAllListeners();
        answer3.onClick.RemoveAllListeners();
        answer4.onClick.RemoveAllListeners();

        answer1.onClick.AddListener(() => CheckAnswer(question.answerA, question.correctAnswer));
        answer2.onClick.AddListener(() => CheckAnswer(question.answerB, question.correctAnswer));
        answer3.onClick.AddListener(() => CheckAnswer(question.answerC, question.correctAnswer));
        answer4.onClick.AddListener(() => CheckAnswer(question.answerD, question.correctAnswer));

        onQuestionAnswered = callback;
    }

    private void CheckAnswer(string selectedAnswer, string correctAnswer)
    {
        bool isCorrect = selectedAnswer == correctAnswer;
        panelQA.SetActive(false);
        onQuestionAnswered?.Invoke(isCorrect);
    }
    private void BackToMainMenu()
    {
        SceneManager.LoadScene("Main");
    }
    public void ShowGameFinishedPanel()
    {
        gameFinishedPanel.SetActive(true);
        backToMainButton.onClick.RemoveAllListeners();
        backToMainButton.onClick.AddListener(BackToMainMenu);
    }
    private void OnEnable()
    {
        PlayerWeapon.OnWeaponUIUpdateEvent += WeaponUIUpdateCallback;
        PlayerHealth.OnPlayerDeadEvent += PlayerDeadCallback;
        LevelManager.OnRoomCompletedEvent += RoomCompletedCallback;
    }

    private void OnDisable()
    {
        PlayerWeapon.OnWeaponUIUpdateEvent -= WeaponUIUpdateCallback;
        PlayerHealth.OnPlayerDeadEvent -= PlayerDeadCallback;
        LevelManager.OnRoomCompletedEvent -= RoomCompletedCallback;
    }
}

// STORED QUESTION
[System.Serializable]
public class Question
{
    public int ques_id;
    public string questionText;
    public string answerA;
    public string answerB;
    public string answerC;
    public string answerD;
    public string correctAnswer; // Stores the correct answer text
    public bool status;
    public Question(int id, string text, string a, string b, string c, string d, string correct)
    {
        ques_id = id;
        questionText = text;
        answerA = a;
        answerB = b;
        answerC = c;
        answerD = d;
        correctAnswer = correct;
        status = true;
    }
    public static class QuestionDatabase
    {
        public static List<Question> questions = new List<Question>
        {
            new Question(1, "What is 2 + 2?", "3", "4", "5", "6", "4"),
            new Question(2, "What is the capital of France?", "Berlin", "Madrid", "Paris", "Rome", "Paris"),
            new Question(3, "What is the largest planet?", "Earth", "Mars", "Jupiter", "Venus", "Jupiter"),
            new Question(4, "What is the boiling point of water?", "90°C", "100°C", "110°C", "120°C", "100°C"),
            new Question(5, "Who wrote 'To Kill a Mockingbird'?", "Harper Lee", "J.K. Rowling", "Ernest Hemingway", "F. Scott Fitzgerald", "Harper Lee"),
            new Question(6, "What is the chemical symbol for gold?", "Au", "Ag", "Pb", "Fe", "Au"),
            new Question(7, "What is the capital city of Japan?", "Beijing", "Seoul", "Tokyo", "Bangkok", "Tokyo"),
            new Question(8, "Who painted the Mona Lisa?", "Vincent van Gogh", "Pablo Picasso", "Leonardo da Vinci", "Claude Monet", "Leonardo da Vinci"),
            new Question(9, "What is the largest ocean on Earth?", "Atlantic Ocean", "Indian Ocean", "Arctic Ocean", "Pacific Ocean", "Pacific Ocean"),
            new Question(10, "Who discovered penicillin?", "Marie Curie", "Albert Einstein", "Alexander Fleming", "Isaac Newton", "Alexander Fleming"),
            new Question(11, "Which planet is known as the Red Planet?", "Earth", "Mars", "Jupiter", "Saturn", "Mars"),
            new Question(12, "What is the square root of 64?", "6", "7", "8", "9", "8"),
            new Question(13, "Which element has the atomic number 1?", "Oxygen", "Hydrogen", "Helium", "Carbon", "Hydrogen")
        };

        public static Question GetRandomQuestion()
        {
            List<Question> availableQuestions = questions.FindAll(q => q.status);
            if (availableQuestions.Count == 0) return null;

            int randomIndex = UnityEngine.Random.Range(0, availableQuestions.Count);
            return availableQuestions[randomIndex];
        }

        public static void MarkQuestionAsUsed(Question question)
        {
            question.status = false;
        }
    }
}



/*using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Player UI")]
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image armorBar;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private Image energyBar;
    [SerializeField] private TextMeshProUGUI energyText;

    [Header("UI Extra")] 
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI levelTMP;
    [SerializeField] private TextMeshProUGUI completedTMP;
    [SerializeField] private TextMeshProUGUI coinsTMP;

    [Header("UI Weapon")] 
    [SerializeField] private GameObject weaponPanel;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI weaponEnergyTMP;

    [Header("Question UI")]
    [SerializeField] private GameObject panelQA;
    [SerializeField] private TextMeshProUGUI textQuestion;
    [SerializeField] private Button answer1;
    [SerializeField] private Button answer2;
    [SerializeField] private Button answer3;
    [SerializeField] private Button answer4;

    private Action<bool> onQuestionAnswered;

    private void Update()
    {
        UpdatePlayerUI();
        coinsTMP.text = CoinManager.Instance.Coins.ToString();
    }

    private void UpdatePlayerUI()
    {
        PlayerConfig playerConfig = GameManager.Instance.Player;
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount,
            playerConfig.CurrentHealth / playerConfig.MaxHealth, 10f * Time.deltaTime);
        armorBar.fillAmount = Mathf.Lerp(armorBar.fillAmount,
            playerConfig.Armor / playerConfig.MaxArmor, 10f * Time.deltaTime);
        energyBar.fillAmount = Mathf.Lerp(energyBar.fillAmount,
            playerConfig.Energy / playerConfig.MaxEnergy, 10f * Time.deltaTime);

        healthText.text = $"{playerConfig.CurrentHealth}/{playerConfig.MaxHealth}";
        armorText.text = $"{playerConfig.Armor}/{playerConfig.MaxArmor}";
        energyText.text = $"{playerConfig.Energy}/{playerConfig.MaxEnergy}";
    }

    public void FadeNewDungeon(float value)
    {
        StartCoroutine(Helpers.IEFade(fadePanel, value, 1.5f));
    }

    public void UpdateLevelText(string levelText)
    {
        levelTMP.text = levelText;
    }

    public void PlayButton() 
    {
        SceneManager.LoadScene("Main");
    }
    
    private void RoomCompletedCallback()
    {
        StartCoroutine(IERoomCompleted());
    }
    
    private IEnumerator IERoomCompleted()
    {
        completedTMP.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        completedTMP.gameObject.SetActive(false);
    }

    private void WeaponUIUpdateCallback(Weapon currentWeapon)
    {
        if (weaponPanel.activeSelf == false)
        {
            weaponPanel.SetActive(true);
        } 

        weaponEnergyTMP.text = currentWeapon.ItemWeapon.RequiredEnergy.ToString();
        weaponIcon.sprite = currentWeapon.ItemWeapon.Icon;
    }

    private void PlayerDeadCallback()
    {
        gameOverPanel.SetActive(true);
    }

    // SET QUESTION()
    public void ShowQuestion(Question question, Action<bool> callback)
    {
        panelQA.SetActive(true);
        textQuestion.text = question.questionText;
        answer1.GetComponentInChildren<TextMeshProUGUI>().text = question.answerA;
        answer2.GetComponentInChildren<TextMeshProUGUI>().text = question.answerB;
        answer3.GetComponentInChildren<TextMeshProUGUI>().text = question.answerC;
        answer4.GetComponentInChildren<TextMeshProUGUI>().text = question.answerD;

        answer1.onClick.RemoveAllListeners();
        answer2.onClick.RemoveAllListeners();
        answer3.onClick.RemoveAllListeners();
        answer4.onClick.RemoveAllListeners();

        answer1.onClick.AddListener(() => CheckAnswer(question.answerA, question.correctAnswer));
        answer2.onClick.AddListener(() => CheckAnswer(question.answerB, question.correctAnswer));
        answer3.onClick.AddListener(() => CheckAnswer(question.answerC, question.correctAnswer));
        answer4.onClick.AddListener(() => CheckAnswer(question.answerD, question.correctAnswer));

        onQuestionAnswered = callback;
    }

    private void CheckAnswer(string selectedAnswer, string correctAnswer)
    {
        bool isCorrect = selectedAnswer == correctAnswer;
        panelQA.SetActive(false);
        onQuestionAnswered?.Invoke(isCorrect);
    }

    private void OnEnable()
    {
        PlayerWeapon.OnWeaponUIUpdateEvent += WeaponUIUpdateCallback;
        PlayerHealth.OnPlayerDeadEvent += PlayerDeadCallback;
        LevelManager.OnRoomCompletedEvent += RoomCompletedCallback;
    }

    private void OnDisable()
    {
        PlayerWeapon.OnWeaponUIUpdateEvent -= WeaponUIUpdateCallback;
        PlayerHealth.OnPlayerDeadEvent -= PlayerDeadCallback;
        LevelManager.OnRoomCompletedEvent -= RoomCompletedCallback;
    }
}

// STORED QUESTION
[System.Serializable]
public class Question
{
    public int ques_id;
    public string questionText;
    public string answerA;
    public string answerB;
    public string answerC;
    public string answerD;
    public string correctAnswer; // Stores the correct answer text
    public bool status;
    public Question(int id, string text, string a, string b, string c, string d, string correct)
    {
        ques_id = id;
        questionText = text;
        answerA = a;
        answerB = b;
        answerC = c;
        answerD = d;
        correctAnswer = correct;
        status = true;
    }
    public static class QuestionDatabase
    {
        public static List<Question> questions = new List<Question>
        {
            new Question(1, "What is 2 + 2?", "3", "4", "5", "6", "4"),
            new Question(2, "What is the capital of France?", "Berlin", "Madrid", "Paris", "Rome", "Paris"),
            new Question(3, "What is the largest planet?", "Earth", "Mars", "Jupiter", "Venus", "Jupiter"),
            new Question(4, "What is the boiling point of water?", "90°C", "100°C", "110°C", "120°C", "100°C"),
            new Question(5, "Who wrote 'To Kill a Mockingbird'?", "Harper Lee", "J.K. Rowling", "Ernest Hemingway", "F. Scott Fitzgerald", "Harper Lee"),
            new Question(6, "What is the chemical symbol for gold?", "Au", "Ag", "Pb", "Fe", "Au"),
            new Question(7, "What is the capital city of Japan?", "Beijing", "Seoul", "Tokyo", "Bangkok", "Tokyo"),
            new Question(8, "Who painted the Mona Lisa?", "Vincent van Gogh", "Pablo Picasso", "Leonardo da Vinci", "Claude Monet", "Leonardo da Vinci"),
            new Question(9, "What is the largest ocean on Earth?", "Atlantic Ocean", "Indian Ocean", "Arctic Ocean", "Pacific Ocean", "Pacific Ocean"),
            new Question(10, "Who discovered penicillin?", "Marie Curie", "Albert Einstein", "Alexander Fleming", "Isaac Newton", "Alexander Fleming"),
            new Question(11, "Which planet is known as the Red Planet?", "Earth", "Mars", "Jupiter", "Saturn", "Mars"),
            new Question(12, "What is the square root of 64?", "6", "7", "8", "9", "8"),
            new Question(13, "Which element has the atomic number 1?", "Oxygen", "Hydrogen", "Helium", "Carbon", "Hydrogen")
        };

        public static Question GetRandomQuestion()
        {
            List<Question> availableQuestions = questions.FindAll(q => q.status);
            if (availableQuestions.Count == 0) return null;

            int randomIndex = UnityEngine.Random.Range(0, availableQuestions.Count);
            return availableQuestions[randomIndex];
        }

        public static void MarkQuestionAsUsed(Question question)
        {
            question.status = false;
        }
    }
}*/