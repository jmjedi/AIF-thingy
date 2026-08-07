using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInterface : MonoBehaviour
{
    public TextMeshProUGUI chips;
    public TextMeshProUGUI score;
    public TextMeshProUGUI timer;

    float minutes = 5;
    float seconds = 0;
    float miliseconds = 0;
    
    private void CalculateTimer()
    {
        //Check if milliseconds are less than or equal to 0
        //If so, reset it to 100
        if(miliseconds <= 0)
        {
            //If seconds are less than or equal to 0, reset it to 59
            //That meants a minuet has passed, meaning it should lose minuets
            if(seconds <= 0)
            {
                minutes--;
                seconds = 59;
            }
            //If it's greater however, keep decreasing seconds
            else if(seconds >= 0)
            {
                seconds--;
            }
            //Reset milliseconds
            miliseconds = 100;
        }
        
        //Lose Seconds
        miliseconds -= Time.deltaTime * 100;
        
        //Debug.Log(string.Format("{0}:{1}:{2}", minutes, seconds, (int)miliseconds));
        //Format the time
        //How it works is the {} representsa digit in a value of how many it is put
        //For example, "{0}{1}" would come out as "00"
        timer.text = string.Format("{0}:{1}:{2}", minutes, seconds, (int)miliseconds) + " :Timer"; 
    }

    private void CalculateScore()
    {
        //Calculate the score
        //NUMBERS ARE TEMPOARY
        score.text = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", 1, 2, 3, 4, 5, 6, 7, 8, 9);
    }

    private void CalculateChips()
    {
        //Calculate how much chips we have
        //NUMBERS ARE TEMPOARY
        chips.text = "Chips: " + string.Format("{0}{1}{2}", 0, 0, 0);
    }

    private void Update()
    {
        //Update all the text functions and calculations
        CalculateTimer();
        CalculateScore();
        CalculateChips();
    }
}
