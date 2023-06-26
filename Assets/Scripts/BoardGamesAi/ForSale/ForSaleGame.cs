using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ForSaleGame : BaseGame<ForSaleContext>
{
	public override ForSaleContext GetContext()
	{
		throw new System.NotImplementedException();
	}

	public List<int> winners = new List<int> { 0, 0, 0, 0, 0, 0 };

	public override IEnumerator StartGame()
	{
		while (context.phase == ForSaleContext.ePhase.FIRST)
		{
			if (context.buildingCards.Count >= context.playersCount)
			{
				context.StartRound();
				while (!context.isRoundEnd())
				{
					if (!context.passRound[context.currentPlayerNumber])
						yield return players[context.currentPlayerNumber].GetTurn();
					else
						context.IncPlayerNumber();
				}
			}
			else
			{
				context.EndOfPhase();
			}
		}

		Debug.Log("Начинаем вторую фазу");

		while (context.phase == ForSaleContext.ePhase.SECOND)
		{
			if (context.salesCards.Count >= context.playersCount)
			{
				context.StartRound();
				foreach (var player in players)
					yield return player.GetTurn();
				context.SecondPhaseResult();
			}
			else
			{
				context.EndOfPhase();
			}
		}

		SortedDictionary<int, int> result = new SortedDictionary<int, int>();
		for(int i=0; i<context.playersCount; ++i)
		{
			result[i] = context.playerMoney[i];

			foreach (var wallet in context.walletCardsInHand[i])
			{
				result[i] += wallet;
			}
		}

		int max = 0;
		int winIndex = 0;
		foreach (var res in result)
		{
			Debug.Log(res);
			if (res.Value>max)
			{
				max = res.Value;
				winIndex = res.Key;
			}
		}

		winners[winIndex]++;
		string s = "Winners: ";
		foreach (var winner in winners)
			s += winner + " :: ";
		Debug.Log(s);
	}
}
