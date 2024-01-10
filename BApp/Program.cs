// See https://aka.ms/new-console-template for more information
using BLib;

Render renderer = new Render();

Character character = new Character(100, 100, 100, 100, renderer);
Fridge fridge = new Fridge(character);
Game game = new Game(character);
Games games = new Games(game);
renderer.StartRender(character, fridge, games, game);
AppDomain.CurrentDomain.ProcessExit += character.Dead ? new EventHandler(character.RemoveProgress) : new EventHandler(character.SaveProgress);