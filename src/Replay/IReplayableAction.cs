namespace HalfNibbleGame.Replay;

public interface IReplayableAction {
  void Do();
  void Undo();
}
