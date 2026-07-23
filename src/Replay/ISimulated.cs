namespace HalfNibbleGame.Replay;

public interface ISimulated {
  void Advance(RoundContext context);
  void Reset();
}
