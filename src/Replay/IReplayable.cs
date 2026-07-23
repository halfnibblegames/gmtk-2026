namespace HalfNibbleGame.Replay;

public interface IReplayable {
  void Advance();
  void Rollback();
}
