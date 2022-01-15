using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;

// Game States
// for now we are only using these two
public enum GameState { None, Hub, Menu, GameHub = 4, Game = 8, ContinuedGame = 16 }
public enum GameMode { Classic = 0, HuniePopMode = 1 }

public delegate void OnStateChangeHandler();

public class GameManager
{

    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            if (GameManager.instance == null)
            {
                GameManager.instance = new GameManager();
            }
            return GameManager.instance;
        }
    }

    public static int seed = 0;
    public static System.Random Random;
    public static GameMode Mode { get; set; }
    public static long score;

    #region GameStates(Scenes and States)
    public GameState GameState { get; private set; }
    public event OnStateChangeHandler OnStateChange;
    public void SetGameState(GameState state)
    {
        this.GameState = state;
        if (state != GameState.ContinuedGame)
            SceneManager.LoadScene(state.ToString());
        else SceneManager.LoadScene("Game");
    }
    public static string textname = "";
    #endregion

    #region Static references
    private struct DefaultConstants
    {
        private static readonly int defaultX = 7;
        private static readonly int defaultY = 12;
        private static readonly int defaultZ = 4;
    }

    public struct Constants
    {
        public static int X = 7;
        public static int Y = 12;
        public static int Z = 4;

        public static readonly float AnimationDuration = 0.2f;

        public static readonly float MoveAnimationMinDuration = 0.05f;

        public static readonly float ExplosionDuration = 0.3f;

        public static readonly float WaitBeforePotentialMatchesCheck = 2f;
        public static readonly float OpacityAnimationFrameDelay = 0.05f;

        public static readonly int MinimumMatches = 3;
        public static readonly int MinimumMatchesForBonus = 4;

        public static readonly int Match3Score = 60;
        public static readonly int SubsequentMatchScore = 1000;

        public static readonly string _SelectedBlockTag = "Selected";
        public static readonly string _SavedFilesPath = "Saves/";
        public static readonly string _FullSavedFilesPath = "C:/Users/Jakob/Documents/Unity/GitHub/Match3Connect_v2/Assets/Resources/Saves/";
        //public static readonly string _SavedFiles = Application.persistentDataPath + "/Saves/";
    }

    #endregion



    public void OnApplicationQuit()
    {
        GameManager.instance = null;
    }

}
