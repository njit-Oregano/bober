using Spectre.Console;

namespace BLib;

public class SwitchingMenu: IRightRenderable {
    public Layout rendered {
        get {
            return new Layout("SwitchingMenu");
        }
    }

    public bool HandleInput(ConsoleKeyInfo keyInfo) {
        return false;
    }
}