using Godot;
using System;

public partial class UI : CanvasLayer
{
	[Export] private Control panelMenu;
	
	private bool isPanelOpen = false;
	private float closedX;
	private float openX;
}
