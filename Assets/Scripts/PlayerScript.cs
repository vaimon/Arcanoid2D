using System;
using System.Collections;
using System.Collections.Generic;
using Bonuses;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerScript : MonoBehaviour
{
    const int MaxLevel = 30;

    AudioSource audioSrc;
    
    private bool isMenuActive;

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
    public GameObject pauseCanvas;
    public GameObject mainCanvas;

    public GameDataScript gameData;

    static Collider2D[] colliders = new Collider2D[50];
    static ContactFilter2D contactFilter;
    static bool gameStarted;

    public static float volumeScale = 10;

    int requiredPointsToBall
    {
        get { return 400 + (level - 1) * 20; }
    }

    void Start()
    {
        mainCanvas = GameObject.Find("Canvas");
        pauseCanvas = GameObject.Find("PauseCanvas");
        pauseCanvas.SetActive(false);
        audioSrc = Camera.main.GetComponent<AudioSource>();

        pauseCanvas.GetComponentInChildren<PauseControls>().attachToPlayerScript(this); // Привязываем к кнопке обработчик в этом скрипте
        if (!gameStarted)
        {
            gameStarted = true;
            if (gameData.resetOnStart)
                gameData.FullReset();
            SetMusic();
            showMainMenu();
            return;
        }
        
        hideMainMenu();
        hidePauseMenu();
        level = gameData.level;
        SetMusic();
        Cursor.visible = false;
        StartLevel();
    }

    void SetBackground()
    {
        var bg = GameObject.Find("BackgroundImage").GetComponent<SpriteRenderer>();
        bg.sprite = Resources.Load(level.ToString("d2"),
            typeof(Sprite)) as Sprite;
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

    void SetMusic()
    {
        if (gameData.music)
        {
            audioSrc.volume = gameData.musicValue;
            audioSrc.Play();
        }
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuActive)
            {
                hidePauseMenu();
            }
            else
            {
                showPauseMenu();
            }
        }
        if (isMenuActive)
        {
            return;
        }
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var pos = transform.position;
        pos.x = mousePos.x;
        transform.position = pos;
    }

    void OnGUI()
    {
        if (isMenuActive)
        {
            return;
        }
        GUI.Label(new Rect(5, 4, Screen.width - 10, 100),
            string.Format(
                "<color=white><size=30>Level <b>{0}</b> Balls <b>{1}</b>" +
                " Score <b>{2}</b></size></color>",
                gameData.level, gameData.balls, gameData.points));
    }

    void showMainMenu(bool isEnd = false)
    {
        isMenuActive = true;
        Time.timeScale = 0f;
        mainCanvas.SetActive(true);
        var controls = mainCanvas.GetComponentInChildren<MenuControls>();
        controls.musicSlider.value = gameData.musicValue;
        controls.soundSlider.value = gameData.soundValue;
        controls.musicToggle.isOn = gameData.music;
        controls.soundToggle.isOn = gameData.sound;
        controls.quitBtn.gameObject.SetActive(isEnd);
        if (isEnd)
        {
            Cursor.visible = true;
            gameData.level = 1; // Чтобы начать сначала в конце
        }
    }

    void hideMainMenu()
    {
        isMenuActive = false;
        mainCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
    
    void showPauseMenu()
    {
        isMenuActive = true;
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);
        Cursor.visible = true;
        var controls = pauseCanvas.GetComponentInChildren<PauseControls>();
        controls.musicPauseSlider.value = gameData.musicValue;
        controls.soundPauseSlider.value = gameData.soundValue;
        controls.musicPauseToggle.isOn = gameData.music;
        controls.soundPauseToggle.isOn = gameData.sound;
    }

    public void hidePauseMenu()
    {
        isMenuActive = false;
        pauseCanvas.SetActive(false);
        Cursor.visible = false;
        Time.timeScale = 1f;
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
            audioSrc.PlayOneShot(pointSound, volumeScale);
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
            addBallsToStash(1);
            gameData.pointsToBall -= requiredPointsToBall;
            if (gameData.sound)
                StartCoroutine(PlayAchievementSoundCoroutine());
        }
    }

    public void changeBallsVelocity(float shift)
    {
        var balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (var ball in balls)
        {
            var rigidbody2D = ball.GetComponent<Rigidbody2D>();
            var velocity = rigidbody2D.velocity;
            velocity.Set(velocity.x * (1 + shift), velocity.y * (1 + shift));
            rigidbody2D.velocity = velocity;
        }
    }

    public void addBallsToStash(int number)
    {
        gameData.balls += number;
    }

    public void addBallsToGame(int number)
    {
        var ball = GameObject.FindGameObjectsWithTag("Ball")[0];
        var ballRb = ball.GetComponent<Rigidbody2D>();
        if (ballRb.isKinematic)
        {
            return;
        }
        
        addBallsToStash(number);
        for (int i = 0; i < number; i++)
        {
            var newBall = Instantiate(ballPrefab, ball.transform.position, Quaternion.identity);
            var newBallRb = newBall.GetComponent<Rigidbody2D>();
            newBallRb.isKinematic = false;
            newBallRb.velocity = Quaternion.AngleAxis(Random.Range(1, 359), Vector3.forward) * ballRb.velocity;
        }
    }

    public void playBonusSound()
    {
        if (gameData.sound)
        {
            audioSrc.PlayOneShot(bonusSound, volumeScale);
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
            {
                gameData.level++;
                SceneManager.LoadScene("MainScene");
            }
            else
            {
                showMainMenu(true);
            }
        }
    }

    IEnumerator PlayAchievementSoundCoroutine()
    {
        for (int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(0.3f);
            audioSrc.PlayOneShot(pointSound, volumeScale);
        }
    }
}