using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseAIPlayer<T>
{
	protected BaseGame<T> game;

	public abstract IEnumerator GetTurn();
	
}
