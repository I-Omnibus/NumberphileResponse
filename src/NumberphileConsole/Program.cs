/// <summary>https://www.youtube.com/watch?v=_gCKX6VMvmU
/// <para>C# response to Numberphile YouTube video ( single source file for https://dotnetfiddle.net/ ).</para>
/// </summary>
BradysNumber.RequiredDigits = 1024;
foreach (var digit in BradysNumber.Evaluation) {
	System.Console.Write(digit);
}

#region >>> BradysNumber					<<<
static class BradysNumber {
	internal static int RequiredDigits { get; set; }
	static System.Numerics.BigInteger Value { get; set; } = new();
	static System.Numerics.BigInteger Divisor { get; set; } = 1;
	static RoundRobinCache Recent { get; set; } = new();
	internal static System.Collections.Generic.IEnumerable<string> Evaluation
		=> GetEnumerator();

	static System.Collections.Generic.IEnumerable<string> GetEnumerator() {
		var iterations = 0;
		foreach (var prime in Primes.All) {
			Value += ((prime - 1) * Scaling(Exponent(Divisor) + RequiredDigits)) / (Divisor * Scaling(Exponent(Divisor)));
			Recent.Insert(Value.ToString());
			yield return Recent.GetNextDigit();
			Divisor *= prime;
			if (iterations++ > RequiredDigits) System.Environment.Exit(0);
		}
	}

	static System.Numerics.BigInteger Scaling(int length) {
		var result = new System.Numerics.BigInteger(1);
		for (var i = 0; i < length; i++) result *= 10;
		return result;
	}

	static int Exponent(System.Numerics.BigInteger big) => big.ToString().Length;
}
#endregion

#region >>> Primes							<<<
static class Primes {
	internal static System.Collections.Generic.List<ulong> PrimeCache = new();
	internal static System.Collections.Generic.IEnumerable<ulong> All
		=> GetEnumerator();
	private static System.Collections.Generic.IEnumerable<ulong> GetEnumerator() {
		PrimeCache.Add(2);
		yield return System.Linq.Enumerable.Last(PrimeCache);
		PrimeCache.Add(3);
		yield return System.Linq.Enumerable.Last(PrimeCache);
		while (true) yield return NextPrime();
	}
	private static ulong NextPrime() {
		var candidate = System.Linq.Enumerable.Last(PrimeCache) + 2;
		var factor = ulong.MinValue;
		for (; factor < System.Math.Sqrt(candidate); candidate += 2) {
			var assumedAsPrime = true;
			foreach (var prime in PrimeCache) {
				factor = prime;
				if (candidate % factor == 0) {
					assumedAsPrime = false;
					break;
				}
			}
			if (assumedAsPrime) {
				PrimeCache.Add(candidate);
				break;
			}
		}
		return System.Linq.Enumerable.Last(PrimeCache);
	}
}
#endregion

#region >>> RoundRobinCache				<<<
class RoundRobinCache {
	internal const int Size = 4;
	int DigitsFound { get; set; } = 0;
	System.Collections.Generic.LinkedList<string> Cache { get; init; } = new();
	internal void Insert(string entry) {
		System.Action Add = Cache.Count switch {
			0 => () => {
				Cache.AddFirst(entry);
			}
			,
			Size => () => {
				Cache.RemoveLast();
				Cache.AddBefore(Cache.First, entry);
			}
			,
			_ => () => {
				Cache.AddBefore(Cache.First, entry);
			}
		};
		Add.Invoke();
	}

	internal string GetNextDigit() {
		var result = string.Empty;
		if (Cache.Count == Size) {
			var mostRecent = Cache.First.Value;
			var mismatch = false;
			foreach (var other in Cache) {
				var index = DigitsFound;
				for (index = DigitsFound; index < mostRecent.Length; index++) {
					if (mostRecent[index] != other[index]) {
						mismatch = true;
						break;
					}
					if (mismatch) break;
				}
				if (mismatch) break;
			}
			if (mismatch) {
				if (DigitsFound > 1) {
					result += mostRecent[DigitsFound - 2];
					result += ((DigitsFound == 2) ? "." : "");
					result += (DigitsFound % 64 == 0) ? "\n" : "";
				}
				DigitsFound++;
			}
		}
		return result;
	}
}
#endregion
