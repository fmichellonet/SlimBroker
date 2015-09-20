/// <reference path="scripts/typings/signalr/signalr.d.ts" />
interface SignalR {
    channelHubConnection: ISlimBrokerHubConnection;
}
interface ISlimBrokerHubConnection extends HubConnection {
    register(methodName: string, callbackName: string, type: string): void;
    publish<TMessage>(message: TMessage): void;
}
declare module SlimBroker {
    class ServiceBus {
        private clientSideChannels;
        constructor();
        addChannel(channel: ClientSideSignalRChannel): void;
        register(callback: (message: any) => void, messageType: string): void;
        publish(message: any, messageType: string): void;
    }
    class ClientSideSignalRChannel {
        private hubProxy;
        private con;
        constructor(sinkUrl: string, successCallback: () => void, failCallback: () => void);
        register(callback: (message: any) => void, messageType: string): void;
        publish(message: any, messageType: string): void;
    }
}
