using System.Collections.Immutable;

namespace Utils;

public class StateSearch
{
	public interface SearchState<T> where T : class
	{
		T Key { get; }
		int Score { get; }
		int ScoreHeuristic { get; }
		bool IsFinal { get; }

		IEnumerable<SearchState<T>> GetNextStates();
	}

	public static List<S> FindBestPath<S, SK>(S initialState) where S : SearchState<SK> where SK : class
	{
		var queue = new PriorityQueue<S, int>();
		queue.Enqueue(initialState, 0);

		var lowest = int.MaxValue;
		var bestScores = new Dictionary<SK, int>();
		var bestStates = new List<S>();

		while (queue.Count > 0)
		{
			var state = queue.Dequeue();
			if (state.IsFinal)
			{
				lowest = Math.Min(lowest, state.Score);
				bestStates.Add(state);
				continue;
			}

			var currentBest = bestScores.GetValueOrDefault(state.Key, int.MaxValue);

			if (state.Score > currentBest || state.Score + state.ScoreHeuristic > lowest)
			{
				continue;
			}

			bestScores[state.Key] = state.Score;

			foreach (var nextState in state.GetNextStates())
			{
				queue.Enqueue((S)nextState, state.ScoreHeuristic);
			}
		}

		return bestStates.Where(s => s.Score == lowest).ToList();
	}
}
