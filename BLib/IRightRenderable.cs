using Spectre.Console;

namespace BLib;

interface IRightRenderable {
    Layout rendered {
        get;
    }
    bool HandleInput(ConsoleKeyInfo keyInfo);
}