using OrderSystem;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientChangeStateCommand : SimpleCommand
{
    public override void Execute(INotification notification)
    {
        Order order = notification.Body as Order;
        ClientProxy clientProxy = Facade.RetrieveProxy(ClientProxy.NAME) as ClientProxy; //客人桌的代理
        switch (notification.Type)
        {
            case "WaitFood":
                clientProxy.ChangeClientState(order.client, ClientState.WaitFood);
                break;
            case "Eating":
                clientProxy.ChangeClientState(order.client, ClientState.Eating);
                break;

        }
       
        
    }
}
