using Godot;
using System;

public partial class MoneyManager : Node
{
	private int currentMoney = 100;
	
	[Signal] public delegate void MoneyChangedEventHandler(int newAmount);
	
	public int GetMoney()
	{
		return currentMoney;
	}
	
	public void AddMoney(int amount)
	{
		currentMoney += amount;
		EmitSignal(SignalName.MoneyChanged, currentMoney);
	}
	
	public bool CanAfford(int cost)
	{
		return currentMoney >= cost;
	}
	
	public bool SpendMoney(int cost)
	{
		if (!CanAfford(cost)) return false;
		
		currentMoney -= cost;
		EmitSignal(SignalName.MoneyChanged, currentMoney);
		return true;
	}
}
