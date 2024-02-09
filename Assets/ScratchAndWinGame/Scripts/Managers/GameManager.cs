using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main Game Loop
public class GameManager : MonoBehaviour
{
    public TicketHolder ticketHolder;
    private Ticket CurrentTicket;


    private IEnumerator Start()
    {
        SaveLoadManager.instance.LoadDataFromPlayerPrefs();
        ScoreBoardManager.instance.UpdateMoneyAndScore(SaveLoadManager.instance.MoneyScore,SaveLoadManager.instance.BonusScore);

        yield return StartSequenceManager.instance.StartSequence();
        yield return MainMenuManager.instance.waitUntilClicked();

        StartCoroutine(UpdateTicketSettings());
    }

    public IEnumerator UpdateTicketSettings()
    {
        CurrentTicket = ticketHolder.ActivateRandomTicket();
        CurrentTicket.PrepareTicket();
        yield return CurrentTicket.ProcessTicket();
        StartCoroutine(UpdateTicketSettings());
    }
}
