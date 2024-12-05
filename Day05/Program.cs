var input = File.ReadAllLines("input.txt");

var rules = input
	.TakeWhile(l => l != string.Empty)
	.Select(ParseRule)
	.ToList();

var pages = input
	.SkipWhile(l => l != string.Empty)
	.Where(l => l != string.Empty)
	.Select(ParsePages);

var validPages = pages
	.Where(p => IsValidPage(p, rules));

var fixedPages = pages
	.Where(p => !IsValidPage(p, rules))
	.Select(p => FindMiddleElement(p, rules));

var result1 = validPages.Select(GetMiddleItem).Sum();
var result2 = fixedPages.Sum();

Console.WriteLine("Result 1: " + result1);
Console.WriteLine("Result 2: " + result2);

bool IsValidPage(List<int> page, List<Rule> rules)
{
	foreach (var rule in rules)
	{
		var indexFormer = page.FindIndex(i => i == rule.Former);
		var indexLatter = page.FindIndex(i => i == rule.Latter);

		if (indexFormer != -1 && indexLatter != -1 && indexFormer > indexLatter)
		{
			return false;
		}
	}

	return true;
}

int FindMiddleElement(List<int> pages, List<Rule> rules)
{
	var toProcess = new Queue<int>(pages);
	var rulesToApply = rules.Where(r => pages.Contains(r.Former) && pages.Contains(r.Latter)).ToList();

	var formerCount = rulesToApply.GroupBy(r => r.Former)
		.Select(g => (Item: g.Key, Count: g.Count()))
		.ToList();

	var latterCount = rulesToApply.GroupBy(r => r.Latter)
		.Select(g => (Item: g.Key, Count: g.Count()))
		.ToList();

	var middleItem = formerCount.Single(fc => latterCount.Any(lc => lc.Item == fc.Item && fc.Count == lc.Count));
	return middleItem.Item;
}

int GetMiddleItem(List<int> p)
{
	return p[p.Count / 2];
}

Rule ParseRule(string value)
{
	var parts = value.Split("|");
	return new Rule(int.Parse(parts[0]), int.Parse(parts[1]));
}

List<int> ParsePages(string value)
{
	return value.Split(",").Select(int.Parse).ToList();
}

record Rule(int Former, int Latter);