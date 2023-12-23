// See https://aka.ms/new-console-template for more information
using BLib;

Render renderer = new Render();
Character character = new Character(100, 100, 100, 100, "src/young1.png", renderer);
Fridge fridge = new Fridge(character);
renderer.StartRender(character, fridge);
AppDomain.CurrentDomain.ProcessExit += new EventHandler(character.SaveProgress);