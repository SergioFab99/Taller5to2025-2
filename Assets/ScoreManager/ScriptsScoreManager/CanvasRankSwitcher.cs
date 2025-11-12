using UnityEngine;
using TMPro;

public class CanvasRankSwitcher : MonoBehaviour
{
    [Header("Canvases")]
    public Canvas mainCanvas; 
    public Canvas rankCanvas;  

    [Header("Rank Canvas Elements")]
    public TMP_Text scoreResultText;
    public TMP_Text rankResultText;      
    public TMP_Text rankLetterText;      

    public void ShowRankPanel()
    {
        mainCanvas.enabled = false;
        rankCanvas.enabled = true;

       
        if (ScoreManager.Instance != null && ScoreManager.Instance.scoreText != null)
        {
            scoreResultText.text = ScoreManager.Instance.scoreText.text;
        }
        else
        {
            int totalScore = ScoreManager.Instance != null ? ScoreManager.Instance.GetTotalScore() : 0;
            scoreResultText.text = "Score: " + totalScore;
        }

        
        string rank = ScoreManager.Instance != null ? ScoreManager.Instance.GetRank() : "D";
        if (rankLetterText != null)
            rankLetterText.text = rank;


        if (rankResultText != null)
            rankResultText.text = "Rank: " + rank;
    }

    public void ShowMainCanvas()
    {
        mainCanvas.enabled = true;
        rankCanvas.enabled = false;
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.UpdateScoreDisplay();
    }
}
