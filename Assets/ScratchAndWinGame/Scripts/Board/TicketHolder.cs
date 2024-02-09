using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketHolder : MonoBehaviour
{
    public List<Ticket> myTickets;

    private void Start()
    {
        DeactivateAllTickets();
    }

    public void DeactivateAllTickets()
    {
        foreach (var item in myTickets)
        {
            item.gameObject.SetActive(false);
        }
    }

    // method which activate random ticket from myTicket List
    public Ticket ActivateRandomTicket()
    {
        DeactivateAllTickets();

        Ticket myTicket = Utility.RandomFromList(myTickets);
        myTicket.gameObject.SetActive(true);
        return myTicket;
    }
}
