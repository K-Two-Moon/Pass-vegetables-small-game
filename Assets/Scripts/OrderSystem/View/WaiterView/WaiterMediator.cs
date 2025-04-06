
/*=========================================
* Author: Administrator
* DateTime:2017/6/20 19:21:23
* Description:$safeprojectname$
==========================================*/

using System;
using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;

namespace OrderSystem
{
    internal class WaiterMediator : Mediator
    {
        private WaiterProxy waiterProxy = null;
        public new const string NAME = "WaiterMediator";
        public WaiterView WaiterView
        {
            get { return (WaiterView) base.ViewComponent; }
        }

        //todo 订单代理
        private OrderProxy orderProxy = null;

        public WaiterMediator( WaiterView view ) : base(NAME, view)
        {
            WaiterView.CallWaiter += ( ) => { SendNotification(OrderSystemEvent.CALL_WAITER); };
            WaiterView.Order += data => { SendNotification(OrderSystemEvent.ORDER , data); };
            WaiterView.Pay += ( ) => { SendNotification(OrderSystemEvent.PAY); };
            WaiterView.CallCook += ( ) => { SendNotification(OrderSystemEvent.CALL_COOK); };
            WaiterView.ServerFood += item => {     SendNotification(OrderCommandEvent.selectWaiter, item, "WANSHI"); //付完款之和将服务员状态变更
                                                                                                                    };
        }

        public override void OnRegister( )
        {
            base.OnRegister();
            waiterProxy = Facade.RetrieveProxy(WaiterProxy.NAME) as WaiterProxy;
            orderProxy = Facade.RetrieveProxy(OrderProxy.NAME) as OrderProxy;
            if ( null == waiterProxy )
                throw new Exception(WaiterProxy.NAME + "is null,please check it!");
            if ( null == orderProxy )
                throw new Exception(OrderProxy.NAME + "is null,please check it!");
            WaiterView.UpdateWaiter(waiterProxy.Waiters);
        }

        public override IList<string> ListNotificationInterests( )
        {
            IList<string> notifications = new List<string>();
            notifications.Add(OrderSystemEvent.CALL_WAITER);
            notifications.Add(OrderSystemEvent.ORDER);
            notifications.Add(OrderSystemEvent.GET_PAY);
            notifications.Add(OrderSystemEvent.FOOD_TO_CLIENT);
            notifications.Add(OrderSystemEvent.ResfrshWarite);
            return notifications;
        }
        public override void HandleNotification( INotification notification )
        {
            switch (notification.Name)
            {
                case OrderSystemEvent.CALL_WAITER:
                    ClientItem client = notification.Body as ClientItem;
                    SendNotification(OrderCommandEvent.GET_ORDER, client, "Get");//请求获取菜单的命令 GetAndExitOrderCommed
                    break;
                case OrderSystemEvent.ORDER:
                    //TODO   这个位置省略了一步查找空闲服务员 提交菜单给厨师
                    SendNotification(OrderSystemEvent.CALL_COOK , notification.Body);
                    break;
                case OrderSystemEvent.GET_PAY:
                    Debug.Log(" 服务员拿到顾客的付款 ");
                    ClientItem item = notification.Body as ClientItem;
                    // SendNotification(OrderSystemEvent.selectWaiter, item, "WANSHI"); //付完款之和将服务员状态变更
                    SendNotification(OrderCommandEvent.GUEST_BE_AWAY, item, "Remove");
                    break;
                case OrderSystemEvent.FOOD_TO_CLIENT:
                    Debug.Log(" 服务员上菜 ");
                    // Debug.Log(notification.Body.GetType());
                    WaiterItem waiterItem = notification.Body as WaiterItem;
                    //TODO  涉及到的客人桌子的状态变化走Command
                    //waiterItem.order.client.state++;
                    SendNotification(OrderCommandEvent.ChangeClientState, waiterItem.order,"Eating");
                    SendNotification(OrderSystemEvent.PAY, waiterItem);
                    break;
                case OrderSystemEvent.ResfrshWarite:
                    waiterProxy = Facade.RetrieveProxy(WaiterProxy.NAME) as WaiterProxy;
                    WaiterView.Move(waiterProxy.Waiters);//刷新一下服务员的状态
                    break;
            }
        }
        
    }
}