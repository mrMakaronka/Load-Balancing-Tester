﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.18408
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------



[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IStatefulDuplexService", CallbackContract = typeof(IStatefulDuplexServiceCallback), SessionMode = System.ServiceModel.SessionMode.Required)]
public interface IStatefulDuplexService
{

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IStatefulDuplexService/Start", ReplyAction = "http://tempuri.org/IStatefulDuplexService/StartResponse")]
    void Start(int initValue);

    [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IStatefulDuplexService/Start", ReplyAction = "http://tempuri.org/IStatefulDuplexService/StartResponse")]
    System.Threading.Tasks.Task StartAsync(int initValue);

    [System.ServiceModel.OperationContractAttribute(IsInitiating = false, Action = "http://tempuri.org/IStatefulDuplexService/AddTo", ReplyAction = "http://tempuri.org/IStatefulDuplexService/AddToResponse")]
    void AddTo(int n);

    [System.ServiceModel.OperationContractAttribute(IsInitiating = false, Action = "http://tempuri.org/IStatefulDuplexService/AddTo", ReplyAction = "http://tempuri.org/IStatefulDuplexService/AddToResponse")]
    System.Threading.Tasks.Task AddToAsync(int n);

    [System.ServiceModel.OperationContractAttribute(IsTerminating = true, IsInitiating = false, Action = "http://tempuri.org/IStatefulDuplexService/Stop", ReplyAction = "http://tempuri.org/IStatefulDuplexService/StopResponse")]
    void Stop();

    [System.ServiceModel.OperationContractAttribute(IsTerminating = true, IsInitiating = false, Action = "http://tempuri.org/IStatefulDuplexService/Stop", ReplyAction = "http://tempuri.org/IStatefulDuplexService/StopResponse")]
    System.Threading.Tasks.Task StopAsync();
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IStatefulDuplexServiceCallback
{

    [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IStatefulDuplexService/ServerId")]
    void ServerId([System.ServiceModel.MessageParameterAttribute(Name = "serverId")] int serverId1);

    [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IStatefulDuplexService/Equals")]
    void Equals(int result);

    [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IStatefulDuplexService/Equation")]
    void Equation(string eqn);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IStatefulDuplexServiceChannel : IStatefulDuplexService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class StatefulDuplexServiceClient : System.ServiceModel.DuplexClientBase<IStatefulDuplexService>, IStatefulDuplexService
{

    public StatefulDuplexServiceClient(System.ServiceModel.InstanceContext callbackInstance) :
        base(callbackInstance)
    {
    }

    public StatefulDuplexServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) :
        base(callbackInstance, endpointConfigurationName)
    {
    }

    public StatefulDuplexServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) :
        base(callbackInstance, endpointConfigurationName, remoteAddress)
    {
    }

    public StatefulDuplexServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
        base(callbackInstance, endpointConfigurationName, remoteAddress)
    {
    }

    public StatefulDuplexServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
        base(callbackInstance, binding, remoteAddress)
    {
    }

    public void Start(int initValue)
    {
        base.Channel.Start(initValue);
    }

    public System.Threading.Tasks.Task StartAsync(int initValue)
    {
        return base.Channel.StartAsync(initValue);
    }

    public void AddTo(int n)
    {
        base.Channel.AddTo(n);
    }

    public System.Threading.Tasks.Task AddToAsync(int n)
    {
        return base.Channel.AddToAsync(n);
    }

    public void Stop()
    {
        base.Channel.Stop();
    }

    public System.Threading.Tasks.Task StopAsync()
    {
        return base.Channel.StopAsync();
    }
}