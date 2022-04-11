// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-09-04 20-37-10  
//修改作者 : 杜鑫 
//修改时间 : 2021-09-04 20-37-10  
//版 本 : 0.1 
// ===============================================
using GameFramework;
using GameFramework.Network;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Deer;
using Deer.Scripts.Framework.Network;
using UnityEngine;
using UnityGameFramework.Runtime;
using Google.Protobuf;
using MetaGameBase;
using System;

[DisallowMultipleComponent]
[AddComponentMenu("Deer/NetConnector")]
public class NetConnectorComponent : GameFrameworkComponent
{

    private Dictionary<string, INetworkChannel> m_ListNetworkChannel = new Dictionary<string, INetworkChannel>();

    public NetworkChannelHelper channelHelper;
    private bool m_NetHaveConnect;
    protected override void Awake()
    {
        base.Awake();

    }

    private void Start()
    {
        GameEntry.Messenger.RegisterEvent(EventName.EVENT_CS_NET_CONNECTED, OnHandleSocketConnected);
        GameEntry.Messenger.RegisterEvent(EventName.EVENT_CS_NET_CLOSE, OnHandleSocketClose);
        GameEntry.Messenger.RegisterEvent(EventName.EVENT_CS_NET_RECEIVE, OnHandleReceiveSocketRequest);
        GameEntry.NetConnector.CreateTcpNetworkChannel(Constant.Net.DEFINE);
    }
    private void OnDestroy()
    {
        GameEntry.Messenger.UnRegisterEvent(EventName.EVENT_CS_NET_CONNECTED, OnHandleSocketConnected);
        GameEntry.Messenger.UnRegisterEvent(EventName.EVENT_CS_NET_CLOSE, OnHandleSocketClose);
        GameEntry.Messenger.UnRegisterEvent(EventName.EVENT_CS_NET_RECEIVE, OnHandleReceiveSocketRequest);
    }
    public INetworkChannel CreateTcpNetworkChannel(string channelName) 
    {
        INetworkChannel networkChannel = null;
        if (m_ListNetworkChannel.ContainsKey(channelName))
        {
            m_ListNetworkChannel.TryGetValue(channelName,out networkChannel);
            return networkChannel;
        }
        channelHelper = ReferencePool.Acquire<NetworkChannelHelper>();
        networkChannel = GameEntry.Network.CreateNetworkChannel(channelName, ServiceType.Tcp, channelHelper);
        m_ListNetworkChannel.Add(channelName, networkChannel);
        return networkChannel;
    }

    public INetworkChannel CreateTcpWithSyncNetworkChannel(string channelName)
    {
        INetworkChannel networkChannel = null;
        if (m_ListNetworkChannel.ContainsKey(channelName))
        {
            m_ListNetworkChannel.TryGetValue(channelName, out networkChannel);
            return networkChannel;
        }
        channelHelper = ReferencePool.Acquire<NetworkChannelHelper>();
        networkChannel = GameEntry.Network.CreateNetworkChannel(channelName, ServiceType.TcpWithSyncReceive, channelHelper);
        m_ListNetworkChannel.Add(channelName, networkChannel);
        return networkChannel;
    }

    public void Connect(string ip, int prot, object userData = null)
    {
        if (m_NetHaveConnect)
        {
            return;
        }
        INetworkChannel networkChannel = null;
        m_ListNetworkChannel.TryGetValue(Constant.Net.DEFINE, out networkChannel);
        if (networkChannel != null)
        {
            networkChannel.Connect(IPAddress.Parse(ip), prot, userData);
        }
        else
        {
            Log.Error($"channelName:{0},is nono", Constant.Net.DEFINE);
        }
    }

    public void Connect(string channelName,string ip, int prot, object userData = null) 
    {
        INetworkChannel networkChannel = null;
        m_ListNetworkChannel.TryGetValue(channelName, out networkChannel);
        if (networkChannel != null)
        {
            networkChannel.Connect(IPAddress.Parse(ip), prot, userData);
        }
        else 
        {
            Log.Error($"channelName:{0},is nono", channelName);
        }
    }

    public void Close(string channelName)
    {
        INetworkChannel networkChannel = null;
        m_ListNetworkChannel.TryGetValue(channelName, out networkChannel);
        if (networkChannel != null)
        {
            networkChannel.Close();
        }
    }

    public void SetHeartBeatInterval(string channelName,float heartBeatInterval)
    {
        INetworkChannel networkChannel = null;
        m_ListNetworkChannel.TryGetValue(channelName, out networkChannel);
        if (networkChannel != null)
        {
            networkChannel.HeartBeatInterval = heartBeatInterval;
        }
    }
    public void Send(short nProtocolId, IMessage msg)
    {
        byte[] v = ProtobufUtils.Serialize(msg);
        Send(Constant.Net.DEFINE, nProtocolId, v);
    }
    public void Send(Protocol nProtocolId, IMessage msg)
    {
        byte[] v = ProtobufUtils.Serialize(msg);
        Send(Constant.Net.DEFINE, (short)nProtocolId, v);
    }

    public void Send(short nProtocolId, byte[] v)
    {
        Send(Constant.Net.DEFINE, nProtocolId, v);
    }

    public void Send(string channelName, short nProtocolId, byte[] v)
    {
        INetworkChannel networkChannel = null;
        m_ListNetworkChannel.TryGetValue(channelName, out networkChannel);
        if (networkChannel != null)
        {
            CSProtoPacket csProtoPacket = ReferencePool.Acquire<CSProtoPacket>();
            csProtoPacket.protoId = nProtocolId;
            csProtoPacket.protoBody = v;
            networkChannel.Send(csProtoPacket);
            ReferencePool.Release(csProtoPacket);
            Log.Info("Network channel '{0}' send msg protoId '{1}'.", channelName, nProtocolId);
        }
        else
        {
            Log.Error($"channelName:{0},is nono", channelName);
        }
    }
    private object OnHandleSocketConnected(object pSender)
    {
        m_NetHaveConnect = true;
        MessengerInfo messengerInfo = (MessengerInfo)pSender;
        string name = messengerInfo.param1.ToString();
        string localEndPoint = messengerInfo.param2.ToString();
        string remoteEndPoint = messengerInfo.param3.ToString();
        Log.Info($"Network channel {name} connected, local address {localEndPoint}, remote address {remoteEndPoint}.");
        return null;
    }
    private object OnHandleSocketClose(object pSender)
    {
        m_NetHaveConnect = false;
        MessengerInfo messengerInfo = (MessengerInfo)pSender;
        Log.Info($"Network channel {name} closed.");
        return null;
    }

    private object OnHandleReceiveSocketRequest(object pSender)
    {
        return null;
    }
}