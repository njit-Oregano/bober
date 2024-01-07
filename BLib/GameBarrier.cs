namespace BLib;

class GameBarrier {
    private const string BarrierEmoji = ":white_large_square:";

    private Game Game;
    private int TickDelay = 0;
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
            CheckCollide();
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
        if (TickDelay < Game.TickDelayOnBarriers) {
            TickDelay++;
            return;
        }
        TickDelay = 0;
        if (Game.CurrentGame == PossibleGames.River) {
            Position++;
        }
    }

    public void CheckCollide() {
        if (Game.CurrentGame == PossibleGames.River) {
            if (Position == Game.PlayerPositionY) {
                if (Game.PlayerPositionX < HoleStart || Game.PlayerPositionX > HoleEnd) {
                    Game.GameOver();
                }
            }
        }
    }
}