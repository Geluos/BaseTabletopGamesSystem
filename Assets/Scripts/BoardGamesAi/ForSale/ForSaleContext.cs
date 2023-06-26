using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ForSaleContext
{
	public ForSaleContext(int playersCount)
	{
		this.playersCount = playersCount;

		phase = ePhase.FIRST;
		round = 0;

		currentPlayerNumber = UnityEngine.Random.Range(0, playersCount);

		passRound = new List<bool>(Enumerable.Repeat(false, playersCount));

		playerMoney = playersCount <= 4 ? new List<int>(Enumerable.Repeat(18, playersCount)) : new List<int>(Enumerable.Repeat(14, playersCount));

		currentBet = 0;

		buildingCards = new List<int>(Enumerable.Range(1, 30));

		Shuffle(buildingCards);

		salesCards = new List<int>(Enumerable.Range(2, 14));
		salesCards.AddRange(Enumerable.Range(2, 14));
		salesCards.Add(0);
		salesCards.Add(0);

		Shuffle(salesCards);

		activeCards = new List<int>();

		buildingCardsInHand = new List<List<int>>();

		walletCardsInHand = new List<List<int>>();

		for(int i=0; i< playersCount; ++i)
		{
			buildingCardsInHand.Add(new List<int>());
			walletCardsInHand.Add(new List<int>());
		}
	}

	//Вспомогательная функция для перемешивания колоды
	private void Shuffle(IList list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			var elem = list[i];

			var randIndex = UnityEngine.Random.Range(0, list.Count);

			list[i] = list[randIndex];
			list[randIndex] = elem;
		}
	}

	//Контекст раунда
	public enum ePhase
	{
		FIRST,
		SECOND,
		END
	}

	public ePhase phase { get; private set; }

	//Использование int вместо более подходящего uint обуслено необходимостью взаимодействия с системными функциями без конвертации

	//Нумерация раундов с 1
	public int round { get; private set; }

	//Контекст состояния игры
	public int playersCount { get; private set; }

	//Нумерация игроков с 0
	public int currentPlayerNumber { get; private set; }

	public List<int> playerMoney { get; private set; }

	public List<int> playerBet { get; private set; }

	//Контекст для раунда 
	public int currentBet { get; private set; }

	public List<bool> passRound { get; private set; }

	//Сгенерированный на игру порядок карт
	public List<int> buildingCards { get; private set; }

	public List<int> salesCards { get; private set; }

	//Карты, выложенные на столе
	public List<int> activeCards { get; private set; }

	//Карты у пользователей
	public List<List<int>> buildingCardsInHand { get; private set; }
	public List<List<int>> walletCardsInHand { get; private set; }
	//Карты выбранные во второй фазе для розыгрыша
	private List<int> chosenCard { get; set; }

	public void EndOfPhase()
	{
		if (phase == ePhase.FIRST)
		{
			phase = ePhase.SECOND;
			round = 0;
			currentPlayerNumber = 0;
		}
		else
		{
			Debug.Log("Игра успешно завершена");
			phase = ePhase.END;
		}
	}

	public void DrawCards()
	{
		if (phase == ePhase.FIRST)
		{
			activeCards = new List<int>();
			for (int i = 0; i < playersCount; ++i)
			{
				activeCards.Add(buildingCards[0]);
				buildingCards.RemoveAt(0);
			}
		}
		else if (phase == ePhase.SECOND)
		{
			activeCards = new List<int>();
			for (int i = 0; i < playersCount; ++i)
			{
				activeCards.Add(salesCards[0]);
				salesCards.RemoveAt(0);
			}
		}
		activeCards.Sort();
	}

	public void StartRound()
	{
		DrawCards();
		++round;

		Debug.Log("Начало нового раунда " + round);
		if (phase == ePhase.FIRST)
		{
			currentBet = 0;
			playerBet = new List<int>(Enumerable.Repeat(0, playersCount));
			passRound = new List<bool>(Enumerable.Repeat(false, playersCount));
		}
		else if (phase == ePhase.SECOND)
		{
			chosenCard = new List<int>(Enumerable.Repeat(0, playersCount));
		}
	}

	public bool isRoundEnd()
	{
		return passRound.All(x => x);
	}

	public bool isLastRound()
	{
		if (phase == ePhase.FIRST)
		{
			return buildingCards.Count < playersCount;
		}
		else
		{
			return salesCards.Count < playersCount;
		}
	}

	public bool isLastStand()
	{
		var cnt = 0;
		foreach(var pass in passRound)
		{
			if (!pass)
				++cnt;
		}

		return cnt == 1;
	}

	public void IncPlayerNumber()
	{
		currentPlayerNumber++;
		currentPlayerNumber %= playersCount;
	}

	public void SecondPhaseResult()
	{
		for(int ind = 0; ind<playersCount; ++ind)
			buildingCardsInHand[ind].Remove(chosenCard[ind]);


		SortedDictionary<int, int> result = new SortedDictionary<int, int>();
		for(int i=0; i<playersCount; i++)
		{
			result[chosenCard[i]] = i;
		}
		foreach (var index in result)
		{
			var ind = index.Value;
			walletCardsInHand[ind].Add(activeCards.First());

			Debug.Log("Игрок " + ind + " получил акцию в размере " + activeCards.First());

			activeCards.RemoveAt(0);
		}
	}

	public void SetResult(int res)
	{
		if (phase == ePhase.FIRST)
		{
			if (res == -1)
			{
				int spend = playerBet[currentPlayerNumber];
				if (!isLastStand())
				{
					spend /= 2;
				}
				playerMoney[currentPlayerNumber] -= spend;
				passRound[currentPlayerNumber] = true;

				buildingCardsInHand[currentPlayerNumber].Add(activeCards.First());
				Debug.Log("Игрок " + currentPlayerNumber + " взял карту " + activeCards.First() + " за "  + spend);
				activeCards.RemoveAt(0);
			}
			else
			{
				playerBet[currentPlayerNumber] = res;
				currentBet = res;
			}
			IncPlayerNumber();
		}
		else if (phase == ePhase.SECOND)
		{
			chosenCard[currentPlayerNumber] = res;
			Debug.Log("Игрок " + currentPlayerNumber + " выложил карту " + res);
			IncPlayerNumber();
		}
	}
}
