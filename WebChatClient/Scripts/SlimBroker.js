/// <reference path="scripts/typings/signalr/signalr.d.ts" />
var SlimBroker;
(function (SlimBroker) {
    var ServiceBus = (function () {
        function ServiceBus() {
            this.clientSideChannels = new Array();
        }
        ServiceBus.prototype.addChannel = function (channel) {
            this.clientSideChannels.push(channel);
        };
        ServiceBus.prototype.register = function (callback, messageType) {
            var idx;
            for (idx = 0; idx < this.clientSideChannels.length; idx++) {
                var chn = this.clientSideChannels[idx];
                chn.register(callback, messageType);
            }
        };
        ServiceBus.prototype.publish = function (message, messageType) {
            var idx;
            for (idx = 0; idx < this.clientSideChannels.length; idx++) {
                var chn = this.clientSideChannels[idx];
                chn.publish(message, messageType);
            }
        };
        return ServiceBus;
    })();
    SlimBroker.ServiceBus = ServiceBus;
    var ClientSideSignalRChannel = (function () {
        function ClientSideSignalRChannel(sinkUrl, successCallback, failCallback) {
            this.con = $.hubConnection(sinkUrl);
            this.hubProxy = this.con.createHubProxy("SignalRChannelSink").on("fakemethod", function () {
                console.log("fakemethod called");
            });
            this.con.stateChanged(function (newState) {
                console.log(`connection state changed : ${newState.oldState.toString()} -> ${newState.newState.toString()}`);
            });
            this.con.start(function () {
                console.log("connection starting");
            }).fail(function () { return failCallback(); }).done(function () { return successCallback(); });
        }
        ClientSideSignalRChannel.prototype.register = function (callback, messageType) {
            this.hubProxy.on("Callback", callback);
            this.hubProxy.invoke("Register", "Callback", messageType).done(function () {
                console.log("registered");
            });
            /*.resolve();*/
        };
        ClientSideSignalRChannel.prototype.publish = function (message, messageType) {
            this.hubProxy.invoke("Publish", message, messageType);
            //.resolve();
        };
        return ClientSideSignalRChannel;
    })();
    SlimBroker.ClientSideSignalRChannel = ClientSideSignalRChannel;
})(SlimBroker || (SlimBroker = {}));
//# sourceMappingURL=SlimBroker.js.map