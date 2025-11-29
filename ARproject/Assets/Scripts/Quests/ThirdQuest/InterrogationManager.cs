using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Основной контроллер сцены допроса.
/// Все тексты восстановлены (как в ТЗ).
/// Финальный вопрос показывается при входе и скрывается после выбора.
/// Правильный ответ — А.
/// Auto Size с fontSizeMax = 90f.
/// </summary>
public class InterrogationManager : MonoBehaviour
{
    public enum GameState
    {
        IntroSelection,
        DossierReading,
        InterrogationScene,
        Interrogating,
        FinalVerdict,
        GameOver
    }

    private GameState currentState;

    // ───────────────────── UI ─────────────────────
    [Header("➤ ЭТАП: Введение")]
    public GameObject introPanel;
    public Button[] infoButtons;
    public TMP_Text infoTitle;
    public TMP_Text infoBody;
    public Button proceedToDossiersButton;

    [Header("➤ ЭТАП: Досье")]
    public GameObject dossierPanel;
    public Button[] dossierButtons;
    public TMP_Text dossierTitle;
    public TMP_Text dossierBody;
    public Button goToInterrogationButton;

    [Header("➤ ЭТАП: Допрос")]
    public GameObject interrogationPanel;
    public Image suspectImage;
    public TMP_Text suspectNameText;
    public TMP_Text suspectDialogueText;
    public Button[] questionButtons;
    public Button returnFromInterrogationButton;

    [Header("➤ ЭТАП: Вердикт")]
    public GameObject verdictPanel;
    public TMP_Text verdictQuestionText;   // ← Показывается в FinalVerdict, скрывается после
    public Toggle[] verdictToggles;
    public Button submitVerdictButton;
    public TMP_Text verdictResultText;
    public Button restartButton;

    [Header("➤ ПЕРЕЗАПУСК")]
    public SceneController sceneController;
    public int sceneIndexForRestart = 0;

    // ───────────────────── НАДПИСИ ─────────────────────
    private readonly string[] infoButtonTitles = { "Информация #1", "Информация #2", "Информация #3" };
    private readonly string[] dossierButtonTitles = { "Досье 1", "Досье 2", "Досье 3" };
    private readonly string[] questionButtonTitles = {
        "С какой целью вы нарушили запрет?",
        "Кто сопровождает вас?",
        "Осознаёте ли вы риски с Кощеем?",
        "На что вы готовы пойти?"
    };

    // ✅ ФИНАЛЬНЫЙ ТЕКСТ (полный, как в ТЗ)
    private readonly string finalQuestionText =
@"Какое из утверждений о спектакле «Краса ненаглядная» Воронежского театра кукол имени В.А. Вольховского является верным?

А. Булат-богатырь, несмотря на свое криминальное прошлое, является верным названым братом Ивана-царевича и помогает ему в поисках Красы, украденной Кощеем Бессмертным.

Б. Спектакль является адаптацией Шекспира, а Иван-царевич и Кощей Бессмертный — давние друзья, вместе ищущие Бабу-ягу.

В. Главный антагонист — Змей Горыныч, а целью Ивана является возвращение золота, украденного Булатом.

Г. Пьеса была написана Валерием Вольховским, а действие происходит в Санкт-Петербурге, где Иван-царевич ищет Кощеево царство.";

    private readonly string[] suspectNames = { "Иван-царевич", "Булат-богатырь", "Кощей Бессмертный" };
    public Sprite[] suspectSprites;

    // ✅ ВСЕ ТЕКСТЫ ВОССТАНОВЛЕНЫ (как в твоём ТЗ)
    private readonly string[] introTexts = {
        "«Краса ненаглядная» — легендарная постановка, занимающая особое место в репертуаре, которую представляет Воронежский театр кукол имени В.А. Вольховского.\n\nСпектакль создан по пьесе Евгения Сперанского и является образцом бережного сохранения традиций русского фольклора. Режиссерское решение опирается на исконные сказочные мотивы, где переплетаются подвиг, волшебство и настоящая мужская дружба.\n\nЭто история, рассказанная выразительным языком театра кукол, которая будет интересна зрителям всех возрастов. Постановка, к которой приложил руку мастер Валерий Вольховский, по праву считается «золотым фондом» театра, сохраняя свою актуальность и глубину на протяжении многих лет.",

        "В центре повествования — судьба Ивана-царевича, который отправляется в долгий путь на поиски своей суженой — Красы ненаглядной.\n\nОднако дорога к счастью полна смертельных опасностей, преодолеть которые в одиночку герою не под силу. Его верным соратником становится Булат-богатырь (в прошлом — лесной разбойник), олицетворяющий удаль и безграничную преданность.\n\nВместе названым братьям предстоит противостоять коварству Кощея Бессмертного и хитрости Бабы-яги. Спектакль утверждает вечные ценности: святость данного слова, честь и готовность к самопожертвованию ради друга. Финал истории доказывает, что искренность и смелость способны разрушить любые чары.",

        "«Краса ненаглядная» — это масштабное эпическое полотно. Визуальный ряд спектакля погружает зрителя в атмосферу былинной Руси: сценография и куклы выполнены в стилистике традиционной деревянной резьбы и народного промысла.\n\nВоронежский театр кукол имени В.А. Вольховского использует в этой работе сложные приемы кукловождения, чтобы вдохнуть жизнь в деревянных персонажей.\n\nОсобая атмосфера создается благодаря музыкальному оформлению и световым решениям, которые превращают сцену то в дремучий лес, то в мрачное подземелье Кощея, делая кукольный спектакль осязаемым и живым."
    };

    private readonly string[] dossierTexts = {
        "МИНИСТЕРСТВО ОХРАНЫ СКАЗОЧНОГО ПОРЯДКА ГЛАВНОЕ УПРАВЛЕНИЕ ПО БЛАГОЧИНИЮ ЛИЧНАЯ КАРТОЧКА ФИГУРАНТА\n\nФ.И.О.: Царевич Иван (отчество не установлено).\n\nДата рождения: Не установлена (возраст на вид 18–20 лет).\n\nМесто рождения: Царские палаты, центральный регион Тридевятого государства.\n\nРод занятий: Наследник престола, временно не работающий.\n\nОсобые приметы: Одет в яркий кафтан.\n\nОперативная информация: Самовольно покинул место жительства, нарушив волю законных представителей. Склонен к рискованному поведению. Причина побега — поиски Красы Ненаглядной.\n\nСтатус: РАЗЫСКИВАЕТСЯ (как без вести пропавший).",

        "МИНИСТЕРСТВО ОХРАНЫ СКАЗОЧНОГО ПОРЯДКА УПРАВЛЕНИЕ ПО НАДЗОРУ ЗА ЛЕСНЫМИ ОБЪЕДИНЕНИЯМИ СПРАВКА НА ЛИЦО, СОСТОЯЩЕЕ НА ПРОФИЛАКТИЧЕСКОМ УЧЕТЕ\n\nФ.И.О.: Булат (кличка «Богатырь»).\n\nДата рождения: Точная дата неизвестна (предположительно 13 век).\n\nМесто регистрации: Лицо без определенного места жительства.\n\nРод занятий: Вольный воин, ранее — руководитель вольной лесной дружины.\n\nОсобые приметы: Крупное атлетическое телосложение. Обладает значительной физической силой. Вооружен холодным оружием (меч-кладенец).\n\nОперативная информация: Ранее судим за разбой и незаконное изъятие имущества. В настоящее время выступает в роли защитника и поддельника Царевича Ивана. Предан. Владеет приемами партизанских действий.\n\nСтатус: ПОД ОПЕРАТИВНЫМ НАБЛЮДЕНИЕМ.",

        "КОРОЛЕВСКАЯ СЛУЖБА ПО НАДЗОРУ ЗА МАГИЧЕСКИМИ ПРЕСТУПЛЕНИЯМИ ОТДЕЛ ПО БОРЬБЕ С НАРУШЕНИЕМ МАГИЧЕСКОГО БАЛАНСА ОРИЕНТИРОВКА НА ОСОБО ОПАСНОГО ФИГУРАНТА\n\nФ.И.О.: Бессмертный Кощей.\n\nГод рождения: Доисторический период.\n\nМесто жительства: Замок на высокой горе/в подземелье (адрес скрыт).\n\nНарушения: Незаконное лишение свободы (гр. Красы Ненаглядной), Создание и руководство запрещенными сказочными сообществами, Незаконное хранение магических артефактов.\n\nОсобые приметы: Скелетообразный, неуязвим для обычного оружия.\n\nУязвимости (ВАЖНО!): Жизнеобеспечение привязано к артефакту «Игла».\n\nХарактеристика: Патологическая жадность, мизантропия, мания величия.\n\nСтатус: ОСОБО ОПАСЕН. ПОДЛЕЖИТ ЗАДЕРЖАНИЮ."
    };

    private readonly string[][] suspectAnswers = {
        new string[] {
            "Не мил мне стал белый свет без Красы ненаглядной. Услышал я о ней и покой потерял. Я сбежал, чтобы найти свою любовь, даже если придется пройти через огонь и воду.",
            "Встретил я в лесу Булата. Теперь он мне как брат родной. Без его силы и меча острого пропал бы я в первом же овраге.",
            "Знаю, что Кощей страшен и хитер. Но любовь моя сильнее страха. Я иду не за богатством, а за своим счастьем.",
            "На все, что честь позволяет. Я готов биться до последнего вздоха, чтобы освободить невесту из темницы."
        },
        new string[] {
            "Понравился мне Иван. Увидел я в нем душу чистую. Надоело мне по кустам прятаться, решил я доброе дело сделать — помочь парню счастье найти.",
            "Иван — он для любви и высоких слов, а я — для драки и тяжелой работы. Я его щит и меч в этом походе.",
            "Слыхал я про Кощея многое. Тут подход нужен особый. Знаю я про смерть его, что в игле спрятана.",
            "Не за награду я иду. Дружба — она дороже золота. Я слово дал — я его сдержу."
        },
        new string[] {
            "Красота — это редкий алмаз, и он должен быть в моей оправе. Она — трофей, украшение моего царства. Я так решил, и этого достаточно.",
            "Я бессмертен! Моя жизнь надежно спрятана там, куда ни один герой не доберется. Я играю с вами, как кошка с мышью.",
            "У меня много слуг и ловушек. Я поссорю их, обману, превращу в камень. Никто еще не уходил от меня живым.",
            "Не смейте приближаться к дубу на острове! Не смейте трогать сундук! Это единственное, что мне дорого."
        }
    };

    // ───────────────────── СОСТОЯНИЕ ─────────────────────
    private bool[] dossierRead = new bool[3];
    private bool[] suspectsInterrogated = new bool[3];
    private int currentSuspectIndex = -1;
    private int errorsCount = 0;
    private const int MAX_ERRORS = 3;


    // ───────────────────── ИНИЦИАЛИЗАЦИЯ ─────────────────────
    void Start()
    {
        InitializeUIText();
        SetupEventListeners();
        SwitchState(GameState.IntroSelection);
    }

    void InitializeUIText()
    {
        SetupButtonWithAutoSize(infoButtons, infoButtonTitles);
        SetupButtonWithAutoSize(dossierButtons, dossierButtonTitles);
        SetupButtonWithAutoSize(questionButtons, questionButtonTitles);

        SetButtonTextWithAutoSize(proceedToDossiersButton, "Ознакомиться с досье");
        SetButtonTextWithAutoSize(goToInterrogationButton, "Перейти к допросу");
        SetButtonTextWithAutoSize(returnFromInterrogationButton, "Вернуться к персонажам");
        SetButtonTextWithAutoSize(submitVerdictButton, "Подтвердить вердикт");
        SetButtonTextWithAutoSize(restartButton, "Начать заново");

        // Финальный текст — устанавливаем, но скрываем до FinalVerdict
        if (verdictQuestionText != null)
        {
            verdictQuestionText.text = finalQuestionText;
            ConfigureTMPForAutoSize(verdictQuestionText);
            verdictQuestionText.gameObject.SetActive(false);
        }

        if (verdictResultText != null) verdictResultText.gameObject.SetActive(false);
        if (restartButton != null) restartButton.gameObject.SetActive(false);
    }

    void SetupButtonWithAutoSize(Button[] buttons, string[] texts)
    {
        for (int i = 0; i < buttons.Length && i < texts.Length; i++)
        {
            if (buttons[i] != null)
            {
                TMP_Text text = buttons[i].GetComponentInChildren<TMP_Text>();
                if (text != null)
                {
                    text.text = texts[i];
                    ConfigureTMPForAutoSize(text);
                }
            }
        }
    }

    void SetButtonTextWithAutoSize(Button button, string text)
    {
        if (button != null)
        {
            TMP_Text t = button.GetComponentInChildren<TMP_Text>();
            if (t != null)
            {
                t.text = text;
                ConfigureTMPForAutoSize(t);
            }
        }
    }

    void ConfigureTMPForAutoSize(TMP_Text tmpText)
    {
        if (tmpText == null) return;
        tmpText.enableAutoSizing = true;
        tmpText.fontSizeMin = 14f;
        tmpText.fontSizeMax = 90f; // ← как ты просил
    }

    void SetupEventListeners()
    {
        for (int i = 0; i < infoButtons.Length; i++)
        {
            int index = i;
            infoButtons[i].onClick.AddListener(() => ShowIntroText(index));
        }
        proceedToDossiersButton.onClick.AddListener(() => SwitchState(GameState.DossierReading));

        for (int i = 0; i < dossierButtons.Length; i++)
        {
            int index = i;
            dossierButtons[i].onClick.AddListener(() => ShowDossier(index));
        }
        goToInterrogationButton.onClick.AddListener(() => SwitchState(GameState.InterrogationScene));

        for (int i = 0; i < questionButtons.Length; i++)
        {
            int index = i;
            questionButtons[i].onClick.AddListener(() => ShowAnswer(index));
        }
        returnFromInterrogationButton.onClick.AddListener(ExitInterrogation);

        submitVerdictButton.onClick.AddListener(CheckVerdict);
        restartButton.onClick.AddListener(RestartGame);
    }


    // ───────────────────── ПЕРЕКЛЮЧЕНИЕ СОСТОЯНИЙ ─────────────────────
    void SwitchState(GameState newState)
    {
        HideAllPanels();
        currentState = newState;

        switch (newState)
        {
            case GameState.IntroSelection:
                introPanel.SetActive(true);
                break;
            case GameState.DossierReading:
                dossierPanel.SetActive(true);
                break;
            case GameState.InterrogationScene:
                goToInterrogationButton.interactable = AreAllDossiersRead();
                break;
            case GameState.Interrogating:
                interrogationPanel.SetActive(true);
                break;
            case GameState.FinalVerdict:
                if (AreAllSuspectsInterrogated())
                {
                    verdictPanel.SetActive(true);
                    if (verdictQuestionText != null)
                    {
                        verdictQuestionText.gameObject.SetActive(true); // ← ПОКАЗЫВАЕМ
                    }
                }
                else
                {
                    SwitchState(GameState.InterrogationScene);
                }
                break;
            case GameState.GameOver:
                verdictResultText.text = "Вы ошиблись 3 раза. Дело начнётся заново.";
                verdictPanel.SetActive(true);
                verdictResultText.gameObject.SetActive(true);
                restartButton.gameObject.SetActive(true);
                submitVerdictButton.gameObject.SetActive(false);
                foreach (var t in verdictToggles) t.gameObject.SetActive(false);
                if (verdictQuestionText != null)
                    verdictQuestionText.gameObject.SetActive(false); // ← СКРЫВАЕМ
                break;
        }
    }

    void HideAllPanels()
    {
        introPanel.SetActive(false);
        dossierPanel.SetActive(false);
        interrogationPanel.SetActive(false);
        verdictPanel.SetActive(false);
        if (verdictResultText != null) verdictResultText.gameObject.SetActive(false);
        if (restartButton != null) restartButton.gameObject.SetActive(false);
        if (verdictQuestionText != null) verdictQuestionText.gameObject.SetActive(false);
    }

    bool AreAllDossiersRead() => dossierRead[0] && dossierRead[1] && dossierRead[2];
    bool AreAllSuspectsInterrogated() => suspectsInterrogated[0] && suspectsInterrogated[1] && suspectsInterrogated[2];

    // ───────────────────── ТЕКСТЫ ─────────────────────
    void ShowIntroText(int index)
    {
        infoTitle.text = infoButtonTitles[index];
        infoBody.text = introTexts[index];
        infoButtons[index].interactable = false;
    }

    void ShowDossier(int index)
    {
        dossierTitle.text = $"ДОСЬЕ № {index + 1}";
        dossierBody.text = dossierTexts[index];
        dossierRead[index] = true;
        dossierButtons[index].interactable = false;
        goToInterrogationButton.interactable = AreAllDossiersRead();
    }

    void StartInterrogation(int suspectIndex)
    {
        if (suspectsInterrogated[suspectIndex]) return;

        currentSuspectIndex = suspectIndex;
        suspectNameText.text = suspectNames[suspectIndex];
        suspectImage.sprite = suspectSprites[suspectIndex];

        string introLine = suspectIndex switch
        {
            0 => "Я — Иван, царский сын. Всю жизнь провел в тереме родительском. Но душа моя рвалась на волю. Я тот, кто рискнул всем ради мечты.",
            1 => "Зовут Булатом. Раньше промышлял разбоем. Был я вольный как ветер, пока не встретил царевича. Теперь у меня другая дорога — дорога чести и братства.",
            2 => "Я — Кощей. Властелин тьмы и повелитель судеб. Я живу вечно. Я привык брать все, что пожелаю.",
            _ => ""
        };

        suspectDialogueText.text = introLine;
        SwitchState(GameState.Interrogating);
    }

    public void StartInterrogationFromClick(int suspectIndex)
    {
        if (currentState == GameState.InterrogationScene)
        {
            StartInterrogation(suspectIndex);
        }
    }

    void ShowAnswer(int questionIndex)
    {
        if (currentSuspectIndex >= 0 && currentSuspectIndex < suspectAnswers.Length)
        {
            suspectDialogueText.text = suspectAnswers[currentSuspectIndex][questionIndex];
        }
    }

    void ExitInterrogation()
    {
        if (currentSuspectIndex >= 0)
        {
            suspectsInterrogated[currentSuspectIndex] = true;
        }

        if (AreAllSuspectsInterrogated())
        {
            SwitchState(GameState.FinalVerdict);
        }
        else
        {
            SwitchState(GameState.InterrogationScene);
        }
    }

    // ───────────────────── ВЕРДИКТ ─────────────────────
    void CheckVerdict()
    {
        // ✅ Правильный ответ — А
        if (verdictToggles.Length > 0 && verdictToggles[0].isOn)
        {
            verdictResultText.text = "Вы хороший сотрудник Сказочного отдела МВД! Вердикт вынесен верно.";
            verdictResultText.color = Color.green;
        }
        else
        {
            errorsCount++;
            if (errorsCount >= MAX_ERRORS)
            {
                SwitchState(GameState.GameOver);
                return;
            }

            verdictResultText.text = $"Неверный вердикт. Ошибок: {errorsCount}/{MAX_ERRORS}. Попробуйте снова.";
            verdictResultText.color = Color.red;
            foreach (var t in verdictToggles) t.isOn = false;
            return;
        }

        // Скрываем вопрос после выбора
        if (verdictQuestionText != null)
            verdictQuestionText.gameObject.SetActive(false);

        verdictResultText.gameObject.SetActive(true);
        submitVerdictButton.gameObject.SetActive(false);
        foreach (var t in verdictToggles) t.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(true);
    }

    void RestartGame()
    {
        if (sceneController == null)
        {
            Debug.LogError("InterrogationManager: SceneController не назначен!");
            return;
        }

        sceneController.SceneLoad(sceneIndexForRestart);
    }
}