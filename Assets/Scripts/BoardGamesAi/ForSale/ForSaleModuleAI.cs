using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ForSaleModuleAI : BaseAIPlayer<ForSaleContext>
{ 
	public List<ModuleForSaleAndWeight> modules;

	public ForSaleModuleAI(List<ModuleForSaleAndWeight> modules, BaseGame<ForSaleContext> game)
	{
		this.modules = modules;
		this.game = game;
	}

	public override IEnumerator GetTurn()
	{
		var answer = new Dictionary<int, float>();

		foreach (var module in modules)
		{
			var dict = module.module.GetResult(game.context);
			if (dict.Count == 0)
				Debug.LogError("Недопустимый ответ от модуля");
			foreach (var ans in dict)
			{
				if (!answer.ContainsKey(ans.Key))
					answer[ans.Key] = ans.Value * module.weight;
				else
					answer[ans.Key] += ans.Value * module.weight;
			}
		}

		float max = 0f;
		int max_res = -1;
		foreach( var res in answer)
		{
			if (res.Value >= max)
			{
				max = res.Value;
				max_res = res.Key;
			}
		}

		game.context.SetResult(max_res);

		yield return null;

	}

	public class ModuleForSaleAndWeight
	{
		public float weight;
		public ForSaleAIModule module;

		public ModuleForSaleAndWeight(float weight, ForSaleAIModule module)
		{
			this.weight = weight;
			this.module = module;
		}
	}

}

//Для первой фазы -1 - это пас

public abstract class ForSaleAIModule
{
	protected Dictionary<int, float> _answers;

	protected void Normalize()
	{
		var sum = _answers.Sum(x => x.Value);
		var dict = new Dictionary<int, float>();
		if (sum == 0)
			sum = 1;
		foreach (var answers in _answers)
		{
			dict.Add(answers.Key, answers.Value / sum);
		}
		_answers = dict;
	}

	public abstract Dictionary<int, float> GetResult(ForSaleContext context);
}

public class RationalForSaleAIModule : ForSaleAIModule
{
	ForSaleContext context;
	float a, b;
	public RationalForSaleAIModule()
	{
		a = 1.0f;
		b = 1.0f;
	}

	public RationalForSaleAIModule(float a, float b)
	{
		this.a = a;
		this.b = b;
	}

	public override Dictionary<int, float> GetResult(ForSaleContext context)
	{
		_answers = new Dictionary<int, float>();
		this.context = context;

		var current = context.currentPlayerNumber;

		if (context.phase == ForSaleContext.ePhase.FIRST)
		{
			
			var currentBet = context.playerBet[current];

			//Пас
			_answers[-1] = System.Math.Max(0, context.activeCards.First() * a - (currentBet / 2) * b);

			//Остальные значения
			for (int bet = context.currentBet + 1; bet <= context.playerMoney[current]; ++bet)
			{
				_answers[bet] = System.Math.Max(0f, phaseOne((current + 1) % context.playersCount, 1.0f, 0, bet, current));
			}

		}
		else if (context.phase == ForSaleContext.ePhase.SECOND)
		{
			phaseTwo((current + 1) % context.playersCount, current, new List<int>());
		}
		Normalize();
		return _answers;
	}

	private float phaseOne(int playerIndex, float coef, int cardIndex, int lastBet, int currentIndex)
	{
		if (coef == 0)
			return 0f;

		while (context.passRound[playerIndex])
		{
			playerIndex++;
			playerIndex %= context.playersCount;
		}

		if (playerIndex != currentIndex)
		{
			var lastMoney = context.playerMoney[playerIndex] - (lastBet + 1);
			var chance = System.Math.Max(0f, lastMoney / 10);
			if (chance > 1.0f)
				chance = 1.0f;

			return phaseOne((playerIndex + 1) % context.playersCount, coef * chance, cardIndex, lastBet + 1, currentIndex)
				+ phaseOne((playerIndex + 1) % context.playersCount, coef * (1.0f - chance), cardIndex + 1, lastBet, currentIndex);
		}
		else
		{
			if (cardIndex == context.playersCount - 1)
			{
				return coef * context.activeCards.Last() * a - lastBet * b;
			}
			else
			{
				if (lastBet < 2)
					lastBet = 2;
				//lastBet/100 - чтобы при прочих равных большая ставка была приоритетнее
				return coef * context.activeCards[cardIndex] * a - lastBet/2 * b ;
			}
		}
	}

	private void phaseTwo(int playerIndex, int currentIndex, List<int> otherCards)
	{
		if (playerIndex != currentIndex)
		{
			foreach( var val in context.buildingCardsInHand[playerIndex])
			{ 
				var other = new List<int>(otherCards);
				other.Add(val);
				phaseTwo((playerIndex + 1) % context.playersCount, currentIndex, other);
			}
		}
		else
		{
			foreach (var val in context.buildingCardsInHand[currentIndex])
			{
				int index = 0;
				otherCards.Sort();
				while (index < otherCards.Count && val > otherCards[index])
					index++;
				if (!_answers.ContainsKey(val))
					_answers[val] = 0;
				_answers[val] += context.activeCards[index] / (float)val;
			}
		}
	}
}

public class PerfectionForSaleAIModule : ForSaleAIModule
{
	ForSaleContext context;

	public override Dictionary<int, float> GetResult(ForSaleContext context)
	{
		_answers = new Dictionary<int, float>();
		this.context = context;

		var current = context.currentPlayerNumber;

		if (context.phase == ForSaleContext.ePhase.FIRST)
		{
			var startMoney = context.playersCount <= 4 ? 18.0f : 14f;
			var perfectMoney = startMoney - startMoney*((float)context.round / (30/context.playersCount));
			var perfect = Math.Max(0, context.playerMoney[current] - perfectMoney);

			_answers[-1] = 1f + 1.0f / (Math.Abs(perfect - (context.playerBet[current] / 2)) + 1);

			for (int bet = context.currentBet + 1; bet <= context.playerMoney[current]; ++bet)
			{
				_answers[bet] = 1f + 1.0f / (Math.Abs(perfect - bet) + 1);
			}
		}
		else if (context.phase == ForSaleContext.ePhase.SECOND)
		{
			List<int> cards = new List<int>(context.buildingCardsInHand[current]);

			cards.Sort();

			float perf = (float)cards.Count / 2.0f;

			for(int i = 0; i < cards.Count; ++i)
			{
				_answers[cards[i]] = 1f + 1.0f /( Math.Abs(i - perf) + 1);
			}
		}

		Normalize();
		return _answers;
	}
}

public class RandomForSaleAIModule : ForSaleAIModule
{
	ForSaleContext context;

	float a;

	public RandomForSaleAIModule(float a)
	{
		this.a = a;
	}

	public RandomForSaleAIModule()
	{
		a = 1f;
	}

	public override Dictionary<int, float> GetResult(ForSaleContext context)
	{
		_answers = new Dictionary<int, float>();
		this.context = context;

		var current = context.currentPlayerNumber;

		if (context.phase == ForSaleContext.ePhase.FIRST)
		{
			_answers[-1] = a + UnityEngine.Random.Range(0.0001f, 1.0f);

			for (int bet = context.currentBet + 1; bet <= context.playerMoney[current]; ++bet)
			{
				_answers[bet] = a + UnityEngine.Random.Range(0.0001f, 1.0f);
			}
		}
		else if (context.phase == ForSaleContext.ePhase.SECOND)
		{
			List<int> cards = new List<int>(context.buildingCardsInHand[current]);

			for (int i = 0; i < cards.Count; ++i)
			{
				_answers[cards[i]] = a + UnityEngine.Random.Range(0.0001f, 1.0f);
			}
		}

		Normalize();
		return _answers;
	}
}

/*

*/
