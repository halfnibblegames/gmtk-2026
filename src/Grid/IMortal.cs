using System.Collections.Generic;
using HalfNibbleGame.Replay;

namespace HalfNibbleGame.Grid;

public interface IMortal {
  void CheckAgainstHazards(List<IHazard> hazards, RoundContext context);
}
