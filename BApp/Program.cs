// See https://aka.ms/new-console-template for more information
using BLib;

Character character = new Character(100, 100, 100, 100, "test.png");
Render renderer = new Render(character);
renderer.StartRender();