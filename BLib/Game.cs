using BLib;
using Spectre.Console;

public class Game: IRightRenderable {
    public static PossibleGames CurrentGame;
    private Layout MainLayout;
    public Layout rendered {
        get {
            return MainLayout;
        }
    }
    private readonly Character Character;
    public Game(Character character) {
        Character = character;
        MainLayout = new Layout();
    }

    public bool HandleInput(ConsoleKeyInfo key) {
        return false;
    }
}