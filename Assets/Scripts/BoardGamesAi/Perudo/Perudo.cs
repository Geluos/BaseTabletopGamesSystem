using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PerudoContext
{
	public int betCnt;
	public int betValue;
	public int currentPlayerNumber;
	public List<int> dicesCount;
	public List<List<int>> dices;
}

public class PerudoAIModule
{
	float mn, mx;

	float bet = 0f, lie = 0f, truth = 0f;

	PerudoContext context;

	public PerudoAIModule(float mn, float mx, PerudoContext context)
	{
		this.mn = mn;
		this.mx = mx;
		this.context = context;
	}

	protected void Normalize()
	{
		var sum = bet + lie + truth;
		if (sum == 0f)
			sum = 1f;

		bet /= sum;
		lie /= sum;
		truth /= sum;
	}

	public void GetResult(PerudoContext context)
	{
		bet = lie = truth = 0f;
		if (context.betCnt == -1)
		{
			bet = 1.0f;
			return;
		}

		int cur = context.currentPlayerNumber;
		int thisDicesCnt = context.dicesCount[context.currentPlayerNumber];
		int allDicesCnt = context.dicesCount.Sum();
		int other = allDicesCnt - thisDicesCnt;
		int rightDices = context.dices[context.currentPlayerNumber].Count(x => x == context.betValue || x == 6);

		var pred = 1.0f;
		for (int i = 0; i < context.betValue - rightDices; i++)
			pred -= (float)(System.Math.Pow(1 / 3, i) * System.Math.Pow(2 / 3, other - i));

		if (pred > mx)
			bet = 1.0f;
		else if (pred < mn)
			lie = 1.0f;
		else
		{
			bet = (pred - mn) / (mx - mn);
			lie = 1.0f - bet;
		}

		Normalize();
	}
}
