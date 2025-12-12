using UnityEngine;
using TMPro;

public class SettlementUI : MonoBehaviour
{
	[Header("TextMeshPro UI")]
	public TextMeshProUGUI successText;
	public TextMeshProUGUI failureText;

	void Start()
	{
		int success = (StatsManager.Instance != null) ? StatsManager.Instance.SuccessCount : 0;
		int failure = (StatsManager.Instance != null) ? StatsManager.Instance.FailureCount : 0;

		if (successText != null) successText.text = success.ToString();
		if (failureText != null) failureText.text = failure.ToString();
	}
}

