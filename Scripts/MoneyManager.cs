using Godot;
using System;

public partial class MoneyManager : Node
{
	private int currentMoney = 100; //Dinero que se tiene
	
	[Signal] public delegate void MoneyChangedEventHandler(int newAmount); //Señal que se comunica con la interfaz
	
	public int GetMoney()
	{
		//Dinero disponible actual
		return currentMoney;
	}
	
	public void AddMoney(int amount)
	{
		//Añade dinero y envía una señal para el cambio de la interfaz
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
		
		//Reduce el dinero y envia una señal para el cambio de la interfaz
		currentMoney -= cost;
		EmitSignal(SignalName.MoneyChanged, currentMoney);
		return true;
	}
}
