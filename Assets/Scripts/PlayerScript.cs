using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    const int MaxLevel = 30;

    AudioSource audioSrc;

    [Range(1, MaxLevel)] public int level = 1;
    public AudioClip pointSound;
    public AudioClip bonusSound;
    public float ballVelocityMult = 0.02f;
    public GameObject bluePrefab;
    public GameObject redPrefab;
    public GameObject greenPrefab;
    public GameObject yellowPrefab;
    public GameObject ballPrefab;
    public GameObject bonusPrefab;
    public GameDataScript gameData;

    static Collider2D[] colliders = new Collider2D[50];
    static ContactFilter2D contactFilter = new ContactFilter2D();
    static bool gameStarted = false;

    int requiredPointsToBall
    {
        get { return 400 + (level - 1) * 20; }
    }

    void Start()
    {
        Cursor.visible = false;
        audioSrc = Camera.main.GetComponent<AudioSource>();
        if (!gameStarted)
        {
            gameStarted = true;
            if (gameData.resetOnStart)
                gameData.Load();
        }

        level = gameData.level;
        SetMusic();
        StartLevel();
    }

    void StartLevel()
    {
        SetBackground();
        var yMax = Camera.main.orthographicSize * 0.8f;
        var xMax = Camera.main.orthographicSize * Camera.main.aspect * 0.85f;
        CreateBlocks(bluePrefab, xMax, yMax, level, 8);
        CreateBlocks(redPrefab, xMax, yMax, 1 + level, 10);
        CreateBlocks(greenPrefab, xMax, yMax, 1 + level, 12);
        CreateBlocks(yellowPrefab, xMax, yMax, 2 + level, 15);
        CreateBalls();
    }

    void SetBackground()
    {
        var bg = GameObject.Find("Background").GetComponent<SpriteRenderer>();
        bg.sprite = Resources.Load(level.ToString("d2"),
            typeof(Sprite)) as Sprite;
    }

    void SetMusic()
    {
        if (gameData.music)
            audioSrc.Play();
        else
            audioSrc.Stop();
    }

    void CreateBlocks(GameObject prefab, float xMax, float yMax,
        int count, int maxCount)
    {
        if (count > maxCount)
            count = maxCount;
        for (int i = 0; i < count; i++)
        for (int k = 0; k < 20; k++)
        {
            var obj = Instantiate(prefab,
                new Vector3((Random.value * 2 - 1) * xMax,
                    Random.value * yMax, 0),
                Quaternion.identity);
            if (obj.GetComponent<Collider2D>()
                    .OverlapCollider(contactFilter.NoFilter(), colliders) == 0)
                break;
            Destroy(obj);
        }
    }

    void CreateBalls()
    {
        int count = 2;
        if (gameData.balls == 1)
            count = 1;
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(ballPrefab);
            var ball = obj.GetComponent<BallScript>();
            ball.ballInitialForce += new Vector2(10 * i, 0);
            ball.ballInitialForce *= 1 + level * ballVelocityMult;
        }
    }

    void Update()
    {
        if (Time.timeScale > 0)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var pos = transform.position;
            pos.x = mousePos.x;
            transform.position = pos;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            gameData.music = !gameData.music;
            SetMusic();
        }

        if (Input.GetKeyDown(KeyCode.S))
            gameData.sound = !gameData.sound;

        if (Input.GetButtonDown("Pause")) Time.timeScale = Time.timeScale > 0 ? 0 : 1;

        if (Input.GetKeyDown(KeyCode.N))
        {
            gameData.Reset();
            SceneManager.LoadScene("MainScene");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(5, 4, Screen.width - 10, 100),
            string.Format(
                "<color=yellow><size=30>Level <b>{0}</b> Balls <b>{1}</b>" +
                " Score <b>{2}</b></size></color>",
                gameData.level, gameData.balls, gameData.points));
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperRight;
        GUI.Label(new Rect(5, 14, Screen.width - 10, 100),
            string.Format(
                "<color=yellow><size=20><color=white>Space</color>-pause {0}" +
                " <color=white>N</color>-new" +
                " <color=white>J</color>-jump" +
                " <color=white>M</color>-music {1}" +
                " <color=white>S</color>-sound {2}" +
                " <color=white>Esc</color>-exit</size></color>",
                OnOff(Time.timeScale > 0), OnOff(!gameData.music),
                OnOff(!gameData.sound)), style);
    }

    void OnApplicationQuit()
    {
        gameData.Save();
    }

    public void SpawnBonus(Vector3 position)
    {
        var bonus = Instantiate(bonusPrefab, position, Quaternion.identity);
        bonus.AddComponent(BonusFactory.getBonusScript());
    }

    public void OnBlockDestroyed(int points)
    {
        AddPoints(points);
        if (gameData.sound)
            audioSrc.PlayOneShot(pointSound, 5);
        StartCoroutine(CheckForLevelEndCoroutine());
    }

    public void OnBallDestroyed()
    {
        gameData.balls--;
        StartCoroutine(BallDestroyedCoroutine());
    }

    public void AddPoints(int pointsNumber)
    {
        gameData.points += pointsNumber;
        gameData.pointsToBall += pointsNumber;
        if (gameData.pointsToBall >= requiredPointsToBall)
        {
            gameData.balls++;
            gameData.pointsToBall -= requiredPointsToBall;
            if (gameData.sound)
                StartCoroutine(PlayAchievementSoundCoroutine());
        }
    }

    public void playBonusSound()
    {
        if (gameData.sound)
        {
            audioSrc.PlayOneShot(bonusSound, 5.0f);
        }
    }

    IEnumerator BallDestroyedCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        if (GameObject.FindGameObjectsWithTag("Ball").Length == 0)
            if (gameData.balls > 0)
                CreateBalls();
            else
            {
                gameData.Reset();
                SceneManager.LoadScene("MainScene");
            }
    }

    IEnumerator CheckForLevelEndCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        if (GameObject.FindGameObjectsWithTag("Block").Length == 0)
        {
            if (level < MaxLevel)
                gameData.level++;
            SceneManager.LoadScene("MainScene");
        }
    }

    IEnumerator PlayAchievementSoundCoroutine()
    {
        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(0.3f);
            audioSrc.PlayOneShot(pointSound, 5);
        }
    }
    
    string OnOff(bool boolVal)
    {
        return boolVal ? "on" : "off";
    }
}