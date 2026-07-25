namespace HalfNibbleGame.Replay;

public interface ISimulated {
  void Advance(RoundContext context);
  void ResetToRound(RoundContext context);
}
