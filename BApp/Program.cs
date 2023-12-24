// See https://aka.ms/new-console-template for more information
using BLib;

Render renderer = new Render();
Character character = new Character(100, 100, 100, 100, renderer);
Fridge fridge = new Fridge(character);
Games games = new Games();
renderer.StartRender(character, fridge, games);
AppDomain.CurrentDomain.ProcessExit += new EventHandler(character.SaveProgress);