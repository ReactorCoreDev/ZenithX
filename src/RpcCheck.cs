using System;
using Hazel;

namespace ZenithX;

public abstract class RpcCheck : ICheck
{
	public virtual bool Enabled { get; set; } = true;

	public virtual void Validate(PlayerControl player, MessageReader reader, ref bool blockRpc)
	{
	}

	public abstract RpcCalls GetRpcCall();

	public virtual bool IsHostOnly()
	{
		return false;
	}

	public virtual Type GetExpectedNetObject()
	{
		return typeof(PlayerControl);
	}
}
