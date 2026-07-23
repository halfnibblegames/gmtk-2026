using System.Collections.Generic;

namespace HalfNibbleGame.Extensions;

public static class ListExtensions {
  extension<T>(List<T> list) {
    public void Push(T element) {
      list.Add(element);
    }

    public T Pop() {
      var element = list[^1];
      list.RemoveAt(list.Count - 1);
      return element;
    }
  }
}
