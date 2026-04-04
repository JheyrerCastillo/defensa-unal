using Godot;
using System;

public partial class MoneyManager : Node
{
	private int currentMoney = 100;
	
	[Signal] public delegate void MoneyChangedEventHandler(int newAmount);
	
	public int GetMoney()
	{
		//Dinero disponible actual
		return currentMoney;
	}
	
	public void AddMoney(int amount)
	{
		//Añade dinero
		currentMoney += amount;
		EmitSignal(SignalName.MoneyChanged, currentMoney);
	}
	
	public bool CanAfford(int cost)
	{
		//Se determina si el dinero alcanza para lo que se va a gastar
		return currentMoney >= cost;
	}
	
	public bool SpendMoney(int cost)
	{
		//Resta dinero si se puede gastar
		if (!CanAfford(cost)) return false;
		
		currentMoney -= cost;
		EmitSignal(SignalName.MoneyChanged, currentMoney);
		return true;
	}
}
