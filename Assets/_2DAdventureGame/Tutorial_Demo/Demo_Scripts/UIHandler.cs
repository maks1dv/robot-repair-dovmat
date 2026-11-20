using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Beginner2D
{
    /// <summary>
    /// Handle the game UI, changing health bar, enemies counter etc.
    /// </summary>
    public class UIHandler : MonoBehaviour
    {
        public static UIHandler instance { get; private set; }

        public float displayTime = 4.0f;
        private VisualElement m_NonPlayerDialogue;
        private VisualElement m_WinScreen;
        private VisualElement m_LoseScreen;

        private Label m_RobotCounter;
        private float m_TimerDisplay;

        private VisualElement m_Healthbar;

        private Button m_ReturnToMain;

        private void Awake()
        {
            instance = this;
            InitializeUI();
        }

        private void InitializeUI()
        {
            UIDocument uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null || uiDocument.rootVisualElement == null) return;

            var root = uiDocument.rootVisualElement;

            if (m_Healthbar == null)
            {
                m_Healthbar = root.Q<VisualElement>("Healthbar");
                if (m_Healthbar != null) SetHealthValue(1.0f);
            }

            if (m_NonPlayerDialogue == null)
            {
                m_NonPlayerDialogue = root.Q<VisualElement>("NPCDialogue") ?? root.Q<VisualElement>("DialogueWindow");
                if (m_NonPlayerDialogue != null)
                {
                    m_NonPlayerDialogue.style.display = DisplayStyle.None;
                    m_TimerDisplay = -1.0f;
                }
            }

            if (m_LoseScreen == null)
                m_LoseScreen = root.Q<VisualElement>("LoseScreenContainer");

            if (m_WinScreen == null)
                m_WinScreen = root.Q<VisualElement>("WinScreenContainer");

            if (m_RobotCounter == null)
                m_RobotCounter = root.Q<Label>("CounterLabel") ?? root.Q<VisualElement>("RobotCounter")?.Q<Label>();

            if (m_ReturnToMain == null)
            {
                m_ReturnToMain = root.Q<Button>("ReturnToMain");
                if (m_ReturnToMain != null)
                    m_ReturnToMain.clicked += () => { SceneManager.LoadScene(0); };
            }
        }

        public void SetHealthValue(float percentage)
        {
            if (m_Healthbar == null) InitializeUI();
            if (m_Healthbar != null)
                m_Healthbar.style.width = Length.Percent(100 * percentage);
        }

        private void Update()
        {
            if (m_TimerDisplay > 0)
            {
                m_TimerDisplay -= Time.deltaTime;
                if (m_TimerDisplay < 0 && m_NonPlayerDialogue != null)
                {
                    m_NonPlayerDialogue.style.display = DisplayStyle.None;
                }
            }
        }

        public void DisplayDialogue()
        {
            if (m_NonPlayerDialogue == null) InitializeUI();
            if (m_NonPlayerDialogue != null)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
                m_NonPlayerDialogue.style.opacity = 1.0f;
                m_TimerDisplay = displayTime;
            }
        }

        public void DisplayWinScreen()
        {
            if (m_WinScreen == null) InitializeUI();
            if (m_WinScreen != null)
                m_WinScreen.style.opacity = 1.0f;
        }

        public void DisplayLoseScreen()
        {
            if (m_LoseScreen == null) InitializeUI();
            if (m_LoseScreen != null)
                m_LoseScreen.style.opacity = 1.0f;
        }

        public void SetCounter(int current, int enemies)
        {
            if (m_RobotCounter == null) InitializeUI();
            if (m_RobotCounter != null)
                m_RobotCounter.text = $"{current} / {enemies}";
        }
    }
}