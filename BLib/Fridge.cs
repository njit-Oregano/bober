using Spectre.Console;

namespace BLib;

public class Fridge: IRightRenderable {
    public Layout rendered {
        get {
            return new Layout("Fridge");
        }
    }

    public bool HandleInput(ConsoleKeyInfo keyInfo) {
        return false;
    }
}