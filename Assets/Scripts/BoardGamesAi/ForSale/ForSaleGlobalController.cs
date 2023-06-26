using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForSaleGlobalController : BaseGlobalController<ForSaleContext>
{
	public void Start()
	{
		int cnt = 4;
		game.players = new List<BaseAIPlayer<ForSaleContext>>(cnt);

		//Создание прототипов
		ForSaleModuleAI joey, chandler, ross, rachel, monica, phoebe;

		
		{
			var modules = new List<ForSaleModuleAI.ModuleForSaleAndWeight>();
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.0f, new RationalForSaleAIModule(1.0f, 4.5f)));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.0f, new PerfectionForSaleAIModule()));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(1.0f, new RandomForSaleAIModule(0.2f)));
			joey = new ForSaleModuleAI(modules, game);
			game.players.Add(joey);
		}
		

		//for (int i = 0; i < cnt; i++)
		/*
		{
			var modules = new List<ForSaleModuleAI.ModuleForSaleAndWeight>();
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.8f, new RationalForSaleAIModule(1.0f, 4.5f)));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.0f, new PerfectionForSaleAIModule()));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.2f, new RandomForSaleAIModule(0.25f)));
			chandler = new ForSaleModuleAI(modules, game);
			game.players.Add(chandler);
		}

		{
			var modules = new List<ForSaleModuleAI.ModuleForSaleAndWeight>();
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(1.0f, new RationalForSaleAIModule(1.0f, 5.0f)));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.5f, new PerfectionForSaleAIModule()));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.2f, new RandomForSaleAIModule(0.25f)));
			ross = new ForSaleModuleAI(modules, game);
			game.players.Add(ross);
		}*/

		{
			var modules = new List<ForSaleModuleAI.ModuleForSaleAndWeight>();
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.7f, new RationalForSaleAIModule(1.0f, 5.0f)));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.1f, new PerfectionForSaleAIModule()));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.5f, new RandomForSaleAIModule(0.05f)));
			rachel = new ForSaleModuleAI(modules, game);
			game.players.Add(rachel);
		}

		{
			var modules = new List<ForSaleModuleAI.ModuleForSaleAndWeight>();
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.6f, new RationalForSaleAIModule(1.0f, 4.5f)));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(1.0f, new PerfectionForSaleAIModule()));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.1f, new RandomForSaleAIModule(0.4f)));
			monica = new ForSaleModuleAI(modules, game);
			game.players.Add(monica);
		}

		
		{
			var modules = new List<ForSaleModuleAI.ModuleForSaleAndWeight>();
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.5f, new RationalForSaleAIModule(1.5f, 4.0f)));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.05f, new PerfectionForSaleAIModule()));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(1.0f, new RandomForSaleAIModule(0.25f)));
			phoebe = new ForSaleModuleAI(modules, game);
			game.players.Add(phoebe);
		}
		


		/*
		for (int i = 0; i < cnt; ++i)
		{
			var modules = new List<ForSaleModuleAI.ModuleForSaleAndWeight>();
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(1.0f, new RationalForSaleAIModule(1.0f, 4.5f)));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.4f, new PerfectionForSaleAIModule()));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.1f, new RandomForSaleAIModule(0.25f)));
			var moduleAI = new ForSaleModuleAI(modules, game);
			game.players.Add(moduleAI);
		}
		*/

		List<int> winners = new List<int>() { 0, 0, 0, 0, 0, 0 };

		for (int i = 0; i < 1; ++i)
		{
			StartCoroutine(StartGame());
		}
	}

	BaseGame<ForSaleContext> CreateGame()
	{
		var game = new ForSaleGame();

		int cnt = 6;
		game.players = new List<BaseAIPlayer<ForSaleContext>>(cnt);

		//Создание прототипов
		ForSaleModuleAI joey, chandler, ross, rachel, monica, phoebe;

		/*
		{
			var modules = new List<ForSaleModuleAI.ModuleForSaleAndWeight>();
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.0f, new RationalForSaleAIModule(1.0f, 4.5f)));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.0f, new PerfectionForSaleAIModule()));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(1.0f, new RandomForSaleAIModule(0.2f)));
			joey = new ForSaleModuleAI(modules, game);
			game.players.Add(joey);
		}
		*/

		for (int i = 0; i < cnt; i++)
		{
			var modules = new List<ForSaleModuleAI.ModuleForSaleAndWeight>();
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.8f, new RationalForSaleAIModule(1.0f, 4.5f)));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.0f, new PerfectionForSaleAIModule()));
			modules.Add(new ForSaleModuleAI.ModuleForSaleAndWeight(0.2f, new RandomForSaleAIModule(0.25f)));
			chandler = new ForSaleModuleAI(modules, game);
			game.players.Add(chandler);
		}

		return game;
	}

	IEnumerator StartGame()
	{
		for(int i=0; i<200; ++i)
		{
			//var game = CreateGame();
			game.context = new ForSaleContext(4);
			yield return game.StartGame();
			//yield return new WaitForSeconds(4.0f);
		}
			
	}
}
