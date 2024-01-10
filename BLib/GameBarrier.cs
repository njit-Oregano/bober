namespace BLib;

class GameBarrier {
    public static readonly string BarrierEmoji = ":brown_square:";

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
                        if (i < HoleStart || i > HoleEnd) {
                            Game.SetGridCell(i, _Position - 1);
                        }
                    }
                }
                if (value > Game.Height) {
                    Game.RemoveBarrier(Position);
                } else {
                    for (int i = 0; i < Game.Width; i++) {
                        if (i < HoleStart || i > HoleEnd) {
                            Game.SetGridCell(i, _Position, BarrierEmoji);
                        } else if (_Position != Game.Height - 1) {
                            Game.SetGridCell(i, _Position);
                        }
                    }
                }
            } else if (Game.CurrentGame == PossibleGames.Fly) {
                if (_Position < Game.Width - 1) {
                    for (int i = 0; i < Game.Height; i++) {
                        if (i < HoleStart || i > HoleEnd) {
                            Game.SetGridCell(_Position + 1, i);
                        }
                    }
                }
                if (value < -1) {
                    Game.RemoveBarrier(Position);
                } else {
                    for (int i = 0; i < Game.Height; i++) {
                        if (i < HoleStart || i > HoleEnd) {
                            Game.SetGridCell(_Position, i, BarrierEmoji);
                        } else if (_Position != 0) {
                            Game.SetGridCell(_Position, i);
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
        _Position = position;
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
        } else if (Game.CurrentGame == PossibleGames.Fly) {
            Position--;
        }
    }

    public void CheckCollide() {
        switch (Game.CurrentGame) {
            case PossibleGames.River:
                if (Position == Game.PlayerPositionY) {
                    if (Game.PlayerPositionX < HoleStart || Game.PlayerPositionX > HoleEnd) {
                        Game.GameOver();
                    }
                }
                break;
            case PossibleGames.Home:
                break;
            case PossibleGames.Fly:
                break;
        }
    }
}