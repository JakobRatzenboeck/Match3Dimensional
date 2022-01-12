using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHub : MonoBehaviour
{
    GameManager GM;
    [SerializeField]
    private RectTransform startField;
    [SerializeField]
    private RectTransform continueField;
    [SerializeField]
    private RectTransform backField;

    [SerializeField]
    private TMP_Text mode;
    [SerializeField]
    private ToggleGroup toogleGroup;

    #region Gamesize
    [SerializeField]
    private TMP_InputField inputFieldx;
    [SerializeField]
    private TMP_InputField inputFieldy;
    [SerializeField]
    private TMP_InputField inputFieldz;
    #endregion

    [SerializeField]
    private TMP_InputField seed;

    [SerializeField]
    private Canvas SavedGames;
    [SerializeField]
    private Canvas menu;


    void Awake()
    {
        GM = GameManager.Instance;
        GameManager.seed = 0;
        GM.OnStateChange += OnStateChangePlay;
        GM.OnStateChange += OnStateChangeContinue;
        GM.OnStateChange += OnStateChangeBack;
        if (Resources.LoadAll(GameManager.Constants._SavedFilesPath).Length >= 0)
        {
            continueField.gameObject.SetActive(true);
            startField.anchoredPosition = Vector3.up * 40;
            backField.anchoredPosition = Vector3.up * -40;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        inputFieldx.onValueChanged.AddListener(delegate { OnInputSizeChanged(inputFieldx); });
        inputFieldy.onValueChanged.AddListener(delegate { OnInputSizeChanged(inputFieldy); });
        inputFieldz.onValueChanged.AddListener(delegate { OnInputSizeChanged(inputFieldz); });

        inputFieldx.onDeselect.AddListener(delegate { OnDeselectSubmitSize(Axis.X, inputFieldx); });
        inputFieldy.onDeselect.AddListener(delegate { OnDeselectSubmitSize(Axis.Y, inputFieldy); });
        inputFieldz.onDeselect.AddListener(delegate { OnDeselectSubmitSize(Axis.Z, inputFieldz); });
        inputFieldx.onSubmit.AddListener(delegate { OnDeselectSubmitSize(Axis.X, inputFieldx); });
        inputFieldy.onSubmit.AddListener(delegate { OnDeselectSubmitSize(Axis.Y, inputFieldy); });
        inputFieldz.onSubmit.AddListener(delegate { OnDeselectSubmitSize(Axis.Z, inputFieldz); });

        seed.onValueChanged.AddListener(delegate { OnValueChangedSeed(seed); });
        seed.onSubmit.AddListener(OnDeselectSubmitSeed);
        seed.onDeselect.AddListener(OnDeselectSubmitSeed);
    }

    public void On2DModeChanged()
    {
        inputFieldz.interactable = !inputFieldz.interactable;
        if (inputFieldz.interactable)
            GameManager.Constants.Z = 0;
        else
            GameManager.Constants.Z = int.Parse(inputFieldz.text);
    }

    public void OnModeChanged()
    {
        mode.text = "Pop Mode" != mode.text ? "Pop Mode" : "Classic Mode";
    }

    public void OnInputSizeChanged(TMP_InputField input)
    {
        input.text = int.Parse(input.text) < 4 ? "3" : int.Parse(input.text) > 13 ? "13" : input.text;
    }

    public void OnDeselectSubmitSize(Axis axisSize, TMP_InputField input)
    {
        if (axisSize == Axis.X) GameManager.Constants.X = int.Parse(input.text);
        if (axisSize == Axis.Y) GameManager.Constants.Y = int.Parse(input.text);
        if (axisSize == Axis.Z) GameManager.Constants.Z = int.Parse(input.text);
    }

    public void OnValueChangedSeed(TMP_InputField input)
    {
        seed.text = input.text.ToUpper();
    }

    public void OnDeselectSubmitSeed(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            GameManager.seed = new System.Random().Next(0, int.MaxValue);
        }
        else
        {
            GameManager.seed = Convert36toInt(input);
        }
    }

    public void OnStateChangePlay()
    {
        GM.SetGameState(GameState.Game);
    }
    public void OnStateChangeContinue()
    {
        SavedGames.gameObject.SetActive(true);
        foreach (Selectable sb in menu.transform.GetComponentsInChildren<Selectable>())
        {
            sb.interactable = false;
        }

        Color col = new Color(Random.Range(200 / 255f, 1f), Random.Range(200 / 255f, 1f), Random.Range(200 / 255f, 1f));
        col[Random.Range(0, 2)] = 1;
        SavedGames.transform.Find("Scroll View").GetComponent<Image>().color = col;
    }

    public void OnStateChangeBack()
    {
        GM.SetGameState(GameState.Hub);
    }

    private int Convert36toInt(string value)
    {
        int intvalue = 0;
        int subdivision = 0;
        for (int index = 0; index < value.Length; index++)
        {
            if (!int.TryParse(value.Substring(index, 1), out subdivision))
                subdivision = value.Substring(index, 1).ToCharArray()[0] - 'A' + 10;

            intvalue += (subdivision * (int)Mathf.Pow(36, (value.Length - index - 1)));
        }
        return intvalue;
    }

}
