namespace BLib;

class GameBarrier {
    private const string BarrierEmoji = ":white_large_square:";

    private Game Game;

    private int _Position;
    public int Position {
        get {
            return _Position;
        }
        private set {
            if (Game.CurrentGame == PossibleGames.River) {
                if (_Position > 0) {
                    for (int i = 0; i < Game.Width; i++) {
                        Game.SetGridCell(i, _Position - 1);
                    }
                }
                if (value > Game.MaxHeight) {
                    Game.RemoveBarrier(Position);
                } else {
                    for (int i = 0; i < Game.Width; i++) {
                        if (i < HoleStart || i > HoleEnd) {
                            Game.SetGridCell(i, _Position, BarrierEmoji);
                        } else {
                            Game.SetGridCell(i, _Position);
                        }
                    }
                }
            }
            _Position = value;
        }
    }
    public int HoleStart;
    public int HoleEnd;

    public GameBarrier(Game game, int position, int holeStart, int holeEnd) {
        Game = game;
        HoleStart = holeStart;
        HoleEnd = holeEnd;
        Position = position;
    }

    public void Tick() {
        if (Game.CurrentGame == PossibleGames.River) {
            Position++;
        }
    }
}