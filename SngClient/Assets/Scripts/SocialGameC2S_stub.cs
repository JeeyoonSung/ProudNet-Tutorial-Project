﻿




// Generated by PIDL compiler.
// Do not modify this file, but modify the source .pidl file.

using System;
using System.Net;	     

namespace SocialGameC2S
{
	internal class Stub:Nettention.Proud.RmiStub
	{
public AfterRmiInvocationDelegate AfterRmiInvocation = delegate(Nettention.Proud.AfterRmiSummary summary) {};
public BeforeRmiInvocationDelegate BeforeRmiInvocation = delegate(Nettention.Proud.BeforeRmiSummary summary) {};

		public delegate bool RequestLogonDelegate(Nettention.Proud.HostID remote,Nettention.Proud.RmiContext rmiContext, String villeName, String nickName, bool isNewVille);  
		public RequestLogonDelegate RequestLogon = delegate(Nettention.Proud.HostID remote,Nettention.Proud.RmiContext rmiContext, String villeName, String nickName, bool isNewVille)
		{ 
			return false;
		};
		public delegate bool RequestAddTreeDelegate(Nettention.Proud.HostID remote,Nettention.Proud.RmiContext rmiContext, UnityEngine.Vector3 position);  
		public RequestAddTreeDelegate RequestAddTree = delegate(Nettention.Proud.HostID remote,Nettention.Proud.RmiContext rmiContext, UnityEngine.Vector3 position)
		{ 
			return false;
		};
		public delegate bool RequestRemoveTreeDelegate(Nettention.Proud.HostID remote,Nettention.Proud.RmiContext rmiContext, int treeID);  
		public RequestRemoveTreeDelegate RequestRemoveTree = delegate(Nettention.Proud.HostID remote,Nettention.Proud.RmiContext rmiContext, int treeID)
		{ 
			return false;
		};
	public override bool ProcessReceivedMessage(Nettention.Proud.ReceivedMessage pa, Object hostTag) 
	{
		Nettention.Proud.HostID remote=pa.RemoteHostID;
		if(remote==Nettention.Proud.HostID.HostID_None)
		{
			ShowUnknownHostIDWarning(remote);
		}

		Nettention.Proud.Message __msg=pa.ReadOnlyMessage;
		int orgReadOffset = __msg.ReadOffset;
        Nettention.Proud.RmiID __rmiID = Nettention.Proud.RmiID.RmiID_None;
        if (!__msg.Read( out __rmiID))
            goto __fail;
					
		switch(__rmiID)
		{
        case Common.RequestLogon:
            ProcessReceivedMessage_RequestLogon(__msg, pa, hostTag, remote);
            break;
        case Common.RequestAddTree:
            ProcessReceivedMessage_RequestAddTree(__msg, pa, hostTag, remote);
            break;
        case Common.RequestRemoveTree:
            ProcessReceivedMessage_RequestRemoveTree(__msg, pa, hostTag, remote);
            break;
		default:
			 goto __fail;
		}
		return true;
__fail:
	  {
			__msg.ReadOffset = orgReadOffset;
			return false;
	  }
	}
    void ProcessReceivedMessage_RequestLogon(Nettention.Proud.Message __msg, Nettention.Proud.ReceivedMessage pa, Object hostTag, Nettention.Proud.HostID remote)
    {
        Nettention.Proud.RmiContext ctx = new Nettention.Proud.RmiContext();
        ctx.sentFrom=pa.RemoteHostID;
        ctx.relayed=pa.IsRelayed;
        ctx.hostTag=hostTag;
        ctx.encryptMode = pa.EncryptMode;
        ctx.compressMode = pa.CompressMode;

        String villeName; SngClient.Marshaler.Read(__msg,out villeName);	
String nickName; SngClient.Marshaler.Read(__msg,out nickName);	
bool isNewVille; SngClient.Marshaler.Read(__msg,out isNewVille);	
core.PostCheckReadMessage(__msg, RmiName_RequestLogon);
        if(enableNotifyCallFromStub==true)
        {
        string parameterString = "";
        parameterString+=villeName.ToString()+",";
parameterString+=nickName.ToString()+",";
parameterString+=isNewVille.ToString()+",";
        NotifyCallFromStub(Common.RequestLogon, RmiName_RequestLogon,parameterString);
        }

        if(enableStubProfiling)
        {
        Nettention.Proud.BeforeRmiSummary summary = new Nettention.Proud.BeforeRmiSummary();
        summary.rmiID = Common.RequestLogon;
        summary.rmiName = RmiName_RequestLogon;
        summary.hostID = remote;
        summary.hostTag = hostTag;
        BeforeRmiInvocation(summary);
        }

        long t0 = Nettention.Proud.PreciseCurrentTime.GetTimeMs();

        // Call this method.
        bool __ret =RequestLogon (remote,ctx , villeName, nickName, isNewVille );

        if(__ret==false)
        {
        // Error: RMI function that a user did not create has been called. 
        core.ShowNotImplementedRmiWarning(RmiName_RequestLogon);
        }

        if(enableStubProfiling)
        {
        Nettention.Proud.AfterRmiSummary summary = new Nettention.Proud.AfterRmiSummary();
        summary.rmiID = Common.RequestLogon;
        summary.rmiName = RmiName_RequestLogon;
        summary.hostID = remote;
        summary.hostTag = hostTag;
        summary.elapsedTime = Nettention.Proud.PreciseCurrentTime.GetTimeMs()-t0;
        AfterRmiInvocation(summary);
        }
    }
    void ProcessReceivedMessage_RequestAddTree(Nettention.Proud.Message __msg, Nettention.Proud.ReceivedMessage pa, Object hostTag, Nettention.Proud.HostID remote)
    {
        Nettention.Proud.RmiContext ctx = new Nettention.Proud.RmiContext();
        ctx.sentFrom=pa.RemoteHostID;
        ctx.relayed=pa.IsRelayed;
        ctx.hostTag=hostTag;
        ctx.encryptMode = pa.EncryptMode;
        ctx.compressMode = pa.CompressMode;

        UnityEngine.Vector3 position; SngClient.Marshaler.Read(__msg,out position);	
core.PostCheckReadMessage(__msg, RmiName_RequestAddTree);
        if(enableNotifyCallFromStub==true)
        {
        string parameterString = "";
        parameterString+=position.ToString()+",";
        NotifyCallFromStub(Common.RequestAddTree, RmiName_RequestAddTree,parameterString);
        }

        if(enableStubProfiling)
        {
        Nettention.Proud.BeforeRmiSummary summary = new Nettention.Proud.BeforeRmiSummary();
        summary.rmiID = Common.RequestAddTree;
        summary.rmiName = RmiName_RequestAddTree;
        summary.hostID = remote;
        summary.hostTag = hostTag;
        BeforeRmiInvocation(summary);
        }

        long t0 = Nettention.Proud.PreciseCurrentTime.GetTimeMs();

        // Call this method.
        bool __ret =RequestAddTree (remote,ctx , position );

        if(__ret==false)
        {
        // Error: RMI function that a user did not create has been called. 
        core.ShowNotImplementedRmiWarning(RmiName_RequestAddTree);
        }

        if(enableStubProfiling)
        {
        Nettention.Proud.AfterRmiSummary summary = new Nettention.Proud.AfterRmiSummary();
        summary.rmiID = Common.RequestAddTree;
        summary.rmiName = RmiName_RequestAddTree;
        summary.hostID = remote;
        summary.hostTag = hostTag;
        summary.elapsedTime = Nettention.Proud.PreciseCurrentTime.GetTimeMs()-t0;
        AfterRmiInvocation(summary);
        }
    }
    void ProcessReceivedMessage_RequestRemoveTree(Nettention.Proud.Message __msg, Nettention.Proud.ReceivedMessage pa, Object hostTag, Nettention.Proud.HostID remote)
    {
        Nettention.Proud.RmiContext ctx = new Nettention.Proud.RmiContext();
        ctx.sentFrom=pa.RemoteHostID;
        ctx.relayed=pa.IsRelayed;
        ctx.hostTag=hostTag;
        ctx.encryptMode = pa.EncryptMode;
        ctx.compressMode = pa.CompressMode;

        int treeID; SngClient.Marshaler.Read(__msg,out treeID);	
core.PostCheckReadMessage(__msg, RmiName_RequestRemoveTree);
        if(enableNotifyCallFromStub==true)
        {
        string parameterString = "";
        parameterString+=treeID.ToString()+",";
        NotifyCallFromStub(Common.RequestRemoveTree, RmiName_RequestRemoveTree,parameterString);
        }

        if(enableStubProfiling)
        {
        Nettention.Proud.BeforeRmiSummary summary = new Nettention.Proud.BeforeRmiSummary();
        summary.rmiID = Common.RequestRemoveTree;
        summary.rmiName = RmiName_RequestRemoveTree;
        summary.hostID = remote;
        summary.hostTag = hostTag;
        BeforeRmiInvocation(summary);
        }

        long t0 = Nettention.Proud.PreciseCurrentTime.GetTimeMs();

        // Call this method.
        bool __ret =RequestRemoveTree (remote,ctx , treeID );

        if(__ret==false)
        {
        // Error: RMI function that a user did not create has been called. 
        core.ShowNotImplementedRmiWarning(RmiName_RequestRemoveTree);
        }

        if(enableStubProfiling)
        {
        Nettention.Proud.AfterRmiSummary summary = new Nettention.Proud.AfterRmiSummary();
        summary.rmiID = Common.RequestRemoveTree;
        summary.rmiName = RmiName_RequestRemoveTree;
        summary.hostID = remote;
        summary.hostTag = hostTag;
        summary.elapsedTime = Nettention.Proud.PreciseCurrentTime.GetTimeMs()-t0;
        AfterRmiInvocation(summary);
        }
    }
#if USE_RMI_NAME_STRING
// RMI name declaration.
// It is the unique pointer that indicates RMI name such as RMI profiler.
public const string RmiName_RequestLogon="RequestLogon";
public const string RmiName_RequestAddTree="RequestAddTree";
public const string RmiName_RequestRemoveTree="RequestRemoveTree";
       
public const string RmiName_First = RmiName_RequestLogon;
#else
// RMI name declaration.
// It is the unique pointer that indicates RMI name such as RMI profiler.
public const string RmiName_RequestLogon="";
public const string RmiName_RequestAddTree="";
public const string RmiName_RequestRemoveTree="";
       
public const string RmiName_First = "";
#endif
		public override Nettention.Proud.RmiID[] GetRmiIDList { get{return Common.RmiIDList;} }
		
	}
}

