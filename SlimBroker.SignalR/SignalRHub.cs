using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.CSharp.RuntimeBinder;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace SlimBroker.SignalR
{
    [HubName("SignalRChannelSink")]
    public class SignalRHub : Hub
    {
        private readonly SignalRServerSideChannel _channel;

        public SignalRHub()
        {
            _channel = GlobalHost.DependencyResolver.Resolve<SignalRServerSideChannel>();
        }

        public void Register(string callback, string messageType)
        {
            Type msgType = Type.GetType(messageType);

            // Action<TMesg>
            var actionT = typeof (Action<>).MakeGenericType(msgType);

            var buildActionType = typeof (Func<,>).MakeGenericType(typeof (RoutingConfig), actionT);
            Delegate buildActionDelegate = Delegate.CreateDelegate(buildActionType, this,
                                                    GetType().GetMethod("BuildAction", BindingFlags.NonPublic | BindingFlags.Instance)
                                                             .MakeGenericMethod(msgType));

            RoutingConfig config = new RoutingConfig(callback, Context.ConnectionId);
            var unwrappedAction = buildActionDelegate.DynamicInvoke(config);

            // _channel.Register<TMesg>
            MethodInfo channelRegisterMethod = typeof (IChannel).GetMethod("Register").MakeGenericMethod(msgType);

            // _channel.Register( () => {unwrappedAction};)
            channelRegisterMethod.Invoke(_channel, new[] {unwrappedAction});
        }
        
        /// <summary>
        ///     Capture the routing parameter and generate the corresponding Action<TMessage>
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="routing"></param>
        /// <returns></returns>
        private Action<TMessage> BuildAction<TMessage>(RoutingConfig routing)
        {
            Action<TMessage> action = msg => { CallClientBack(msg, routing); };
            return action;
        }

        private void CallClientBack<TMessage>(TMessage msg, RoutingConfig routing)
        {
            var recipient = Clients.Client(routing.ClientConnectionId);

            var callSiteBinder = Binder.InvokeMember(CSharpBinderFlags.None, routing.CallbackMethodName,
                Enumerable.Empty<Type>(), recipient.GetType(),
                new[]
                {
                    CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
                    CSharpArgumentInfo.Create(
                        CSharpArgumentInfoFlags.Constant | CSharpArgumentInfoFlags.UseCompileTimeType, null)
                });
            var callSite = CallSite<Action<CallSite, object, TMessage>>.Create(callSiteBinder);
            callSite.Target(callSite, recipient, msg);
        }
        
        public void Publish(object message, string messageType)
        {
            Type msgType = Type.GetType(messageType);

            // _channel.Dispatch(message)
            MethodInfo method = _channel.GetType().GetMethod("Dispatch").MakeGenericMethod(msgType);
            method.Invoke(_channel, new[] {message});
        }
    }
}