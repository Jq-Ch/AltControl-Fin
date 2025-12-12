using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
	[Header("Timer")]
	public float totalSeconds = 300f; // 5 minutes
	public bool autoStart = true;

	[Header("Result")]
	public string resultSceneName = "SettlementScene";

	[Header("Optional UI")]
	public TextMeshProUGUI timeText;

	private float _remainingSeconds;
	private bool _running;

	void Start()
	{
		_remainingSeconds = Mathf.Max(0f, totalSeconds);
		_running = autoStart;
		UpdateTimeText();
	}

	void Update()
	{
		if (!_running) return;

		_remainingSeconds -= Time.deltaTime;
		if (_remainingSeconds <= 0f)
		{
			_remainingSeconds = 0f;
			_running = false;
			UpdateTimeText();
			LoadSettlementScene();
			return;
		}

		UpdateTimeText();
	}

	public void StartTimer()
	{
		_running = true;
	}

	public void PauseTimer()
	{
		_running = false;
	}

	public void ResetTimer()
	{
		_remainingSeconds = Mathf.Max(0f, totalSeconds);
		UpdateTimeText();
	}

	private void UpdateTimeText()
	{
		if (timeText == null) return;

		int secondsLeft = Mathf.CeilToInt(_remainingSeconds);
		int minutes = secondsLeft / 60;
		int seconds = secondsLeft % 60;
		timeText.text = $"{minutes:00}:{seconds:00}";
	}

	private void LoadSettlementScene()
	{
		SceneManager.LoadScene(resultSceneName);
	}
}

