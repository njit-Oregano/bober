// See https://aka.ms/new-console-template for more information
using BLib;

Render renderer = new Render();
Character character = new Character(100, 100, 100, 100, "test.png", renderer);
Fridge fridge = new Fridge();
renderer.StartRender(character, fridge);
AppDomain.CurrentDomain.ProcessExit += new EventHandler(character.SaveProgress);