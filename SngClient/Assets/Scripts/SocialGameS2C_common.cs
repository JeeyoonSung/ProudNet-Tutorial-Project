﻿ 





// Generated by PIDL compiler.
// Do not modify this file, but modify the source .pidl file.

using System;
namespace SocialGameS2C
{
	internal class Common
	{
		// Message ID that replies to each RMI method. 
			public const Nettention.Proud.RmiID ReplyLogon = (Nettention.Proud.RmiID)4000+1;
			public const Nettention.Proud.RmiID NotifyAddTree = (Nettention.Proud.RmiID)4000+2;
			public const Nettention.Proud.RmiID NotifyRemoveTree = (Nettention.Proud.RmiID)4000+3;
			public const Nettention.Proud.RmiID NotifyPlayerJoin = (Nettention.Proud.RmiID)4000+4;
		// List that has RMI ID.
		public static Nettention.Proud.RmiID[] RmiIDList = new Nettention.Proud.RmiID[] {
			ReplyLogon,
			NotifyAddTree,
			NotifyRemoveTree,
			NotifyPlayerJoin,
		};
	}
}

				 
