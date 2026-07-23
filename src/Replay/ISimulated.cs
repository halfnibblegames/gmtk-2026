namespace HalfNibbleGame.Replay;

public interface ISimulated {
  void Advance(RoundInfo info);
  void Reset();
}
