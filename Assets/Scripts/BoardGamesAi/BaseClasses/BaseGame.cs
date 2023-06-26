using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseGame<T> : MonoBehaviour
{
	public List<BaseAIPlayer<T>> players;

	public abstract IEnumerator StartGame();

	public abstract T GetContext();

	public T context;

}
