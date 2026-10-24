using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }

    private VisualElement m_NonPlayerDialogue;
    private float m_TimerDisplay;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Знаходимо елемент за назвою #NPCDialogue
        m_NonPlayerDialogue = root.Q<VisualElement>("NPCDialogue");

        // Ховаємо діалог на початку гри
        m_NonPlayerDialogue.style.display = DisplayStyle.None;
        m_TimerDisplay = -1.0f;
    }

    private void Update()
    {
        // Таймер для автоматичного закриття діалогу
        if (m_TimerDisplay >= 0)
        {
            m_TimerDisplay -= Time.deltaTime;
            if (m_TimerDisplay < 0)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;
            }
        }
    }

    public void DisplayDialogue()
    {
        m_TimerDisplay = 4.0f; // Вікно показуватиметься 4 секунди
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
    }
}