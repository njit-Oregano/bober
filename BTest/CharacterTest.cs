using BLib;

namespace BTest;

[TestClass]
public class CharacterTest
{
    [TestMethod]
    public void StatusbarPropertiesTest()
    {
        Render renderer = new Render();
        Character character = new Character(100,100,100,100, renderer);
        character.LoseHealth(5);
        Assert.AreEqual(95, character.Health);
        character.AddWater(100);
        Assert.AreEqual(100, character.Water);
        character.AddFood(400);
        Assert.AreEqual(100, character.Food);
        character.LoseFood(50);
        Assert.AreEqual(50, character.Food);
    }
    [TestMethod]
    public void CharacterDeadTest()
    {
        Render renderer = new Render();
        Character character = new Character(100,100,100,100, renderer);
        character.LoseHealth(100);
        Assert.AreEqual(0, character.Health);
        Assert.AreEqual(true, character.Dead);
    }
}