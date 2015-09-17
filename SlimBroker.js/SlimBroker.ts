/// <reference path="scripts/typings/signalr/signalr.d.ts" />
interface SignalR {
    channelHubConnection: ISlimBrokerHubConnection;
}

interface ISlimBrokerHubConnection extends HubConnection {
    register(methodName: string, callbackName: string, type: string): void;
    publish<TMessage>(message: TMessage): void;
}

module SlimBroker {

    export class ServiceBus {

        private clientSideChannels: Array<ClientSideSignalRChannel>;

        constructor() {
            this.clientSideChannels = new Array<ClientSideSignalRChannel>();
        }

        public addChannel(channel : ClientSideSignalRChannel): void {
            this.clientSideChannels.push(channel);
        }
        
        public register(callback: (message: any) => void, messageType: string): void {
            var idx: number;
            for (idx = 0; idx < this.clientSideChannels.length; idx++) {
                var chn: ClientSideSignalRChannel = this.clientSideChannels[idx];
                chn.register(callback, messageType);
            }
        }

        public publish(message: any, messageType: string): void {
            var idx: number;
            for (idx = 0; idx < this.clientSideChannels.length; idx++) {
                var chn: ClientSideSignalRChannel = this.clientSideChannels[idx];
                chn.publish(message, messageType);
            }
        }
    }

    export class ClientSideSignalRChannel {

        private hubProxy: HubProxy;
        private con: HubConnection;

        constructor(sinkUrl: string, successCallback : () => void, failCallback : () => void ) {
            this.con = $.hubConnection(sinkUrl);

            this.hubProxy = this.con.createHubProxy("SignalRChannelSink")
                .on("fakemethod", () => {
                    console.log("fakemethod called");
                });
            
            this.con.stateChanged((newState: SignalRStateChange) => {
                console.log(`connection state changed : ${newState.oldState.toString()} -> ${newState.newState.toString()}`);
            });

            this.con.start(() => {
                    console.log("connection starting");
                })
                .fail(() => failCallback())
                .done(() => successCallback());
        }

        public register(callback: (message: any) => void, messageType: string): void {
            this.hubProxy.on("Callback", callback);


            this.hubProxy.invoke("Register", "Callback", messageType)
                .done(() => {
                    console.log("registered");
                });

            /*.resolve();*/
        }

        public publish(message: any, messageType : string): void {
            this.hubProxy
                .invoke("Publish", message, messageType);
            //.resolve();
        }
    }
}