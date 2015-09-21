/// <reference path="scripts/typings/SlimBroker/SlimBroker.d.ts" />

interface IChatMessage {
    Sender: string;
    Message : string;
}

class App {
    private chatRoomDiv: HTMLElement;
    private serverUrlInput: HTMLInputElement;
    private messageInput: HTMLInputElement;
    private connectButton: HTMLButtonElement;
    private sendButton: HTMLButtonElement;
    private bus: SlimBroker.ServiceBus;
    private cnxState: HTMLSpanElement;

    constructor(chatRoomElement: HTMLElement, serverUrlElement: HTMLInputElement, connectButton: HTMLButtonElement,
        messageElement: HTMLInputElement, sendButton: HTMLButtonElement, stateElement: HTMLSpanElement) {
        this.chatRoomDiv = chatRoomElement;
        this.serverUrlInput = serverUrlElement;
        this.connectButton = connectButton;
        this.messageInput = messageElement;
        this.sendButton = sendButton;
        this.cnxState = stateElement;
        this.connectButton.onclick = () => { this.connect(this.serverUrlInput.value) };
        this.sendButton.onclick = () => { this.sendMessage(this.messageInput.value) };
    }

    public connect(serverUrl: string): void {
        this.bus = new SlimBroker.ServiceBus();
        this.bus.addChannel(new SlimBroker.ClientSideSignalRChannel(serverUrl, () => {
                this.cnxState.style.color = "green";
                this.cnxState.innerText = "Connected";
                this.bus.register((msg: IChatMessage) => { this.displayMessage(msg); }, "Chat.Model.ChatMessage, Chat.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            },
            () => {
                this.cnxState.style.color = "red";
                this.cnxState.innerText = "Failed to connect";
                console.log("failed to connect");
            })
        );
    }

    public displayMessage(message: IChatMessage): void {
        var msgSpan = document.createElement('div');
        msgSpan.innerText = `[${message.Sender}] : ${message.Message}`;
        this.chatRoomDiv.appendChild(msgSpan);
    }

    public sendMessage(msg: string): void {
        var chatMessage: IChatMessage = { Sender: "Web Guy", Message: msg };
        this.bus.publish(chatMessage, "Chat.Model.ChatMessage, Chat.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
    }
}


window.onload = () => {
    var chatLog = document.getElementById('content');
    var serverUrl: HTMLInputElement = <HTMLInputElement>(document.getElementById('ServerUrl'));
    var message: HTMLInputElement = <HTMLInputElement>(document.getElementById('message'));
    var connect: HTMLButtonElement = <HTMLButtonElement>(document.getElementById('connect'));
    var send: HTMLButtonElement = <HTMLButtonElement>(document.getElementById('send'));
    var stateSpan: HTMLSpanElement = <HTMLSpanElement>(document.getElementById('cnxState'));

    var app = new App(chatLog, serverUrl, connect, message, send, stateSpan);
};