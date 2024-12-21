using System.Collections.Immutable;

namespace Utils;

public class StateSearch
{
	public interface SearchState<T> 
	{
		T Key { get; }
		int Length { get; }
		int ScoreHeuristic { get; }
		bool IsFinal { get; }

		IEnumerable<SearchState<T>> GetNextStates();
	}

	public static List<S> FindBestPaths<S, SK>(S initialState) where S : SearchState<SK>
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
				lowest = Math.Min(lowest, state.Length);
				bestStates.Add(state);
				continue;
			}

			var currentBest = bestScores.GetValueOrDefault(state.Key, int.MaxValue);

			if (state.Length >= currentBest || state.Length + state.ScoreHeuristic > lowest)
			{
				continue;
			}

			bestScores[state.Key] = state.Length;

			foreach (var nextState in state.GetNextStates())
			{
				queue.Enqueue((S)nextState, state.ScoreHeuristic);
			}
		}

		return bestStates.Where(s => s.Length == lowest).ToList();
	}

	public static S? FindBestPath<S, SK>(S initialState) where S : class, SearchState<SK> where SK : class 
	{
		var queue = new PriorityQueue<S, int>();
		queue.Enqueue(initialState, 0);

		var lowest = int.MaxValue;
		var visited = new HashSet<SK>();
		var states = new List<S>();

		while (queue.Count > 0)
		{
			var state = queue.Dequeue();
			if (state.IsFinal)
			{
				states.Add(state);
				continue;
			}

			if (visited.Contains(state.Key) || state.Length + state.ScoreHeuristic > lowest)
			{
				continue;
			}

			visited.Add(state.Key);

			foreach (var nextState in state.GetNextStates())
			{
				queue.Enqueue((S)nextState, state.ScoreHeuristic);
			}
		}

		return states.OrderBy(s => s.Length).First();
	}
}
