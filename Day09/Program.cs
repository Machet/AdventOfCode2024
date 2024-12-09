var input = File.ReadAllText("input.txt");

var blocks = input.Select((c, i) => new Block((i % 2) == 0 ? i / 2 : null, int.Parse(c.ToString()))).ToList();

// part 1;
var buffer1 = new LinkedList<Block>(blocks);
var currentItem = buffer1.First;
var fileToMove = FindFilesToMove(buffer1.Last!).First();
var lastFileId = 0;

while (currentItem != null && fileToMove!.Value.Id > lastFileId)
{
	if (currentItem.Value.IsFile)
	{
		lastFileId = currentItem.Value.Id!.Value;
		currentItem = currentItem.Next;
		continue;
	}

	var toFill = Math.Min(fileToMove.Value.Length, currentItem.Value.Length);
	var remaining = fileToMove.Value.Length - toFill;
	fileToMove.Value = new Block(fileToMove.Value.Id, remaining);

	currentItem = FillEmptySpace(buffer1, new Block(fileToMove.Value.Id, toFill), currentItem);
	fileToMove = fileToMove.Value.Length != 0 ? fileToMove : FindFilesToMove(fileToMove.Previous).FirstOrDefault();

	currentItem = currentItem.Next;
}

Console.WriteLine("Result 1: " + CalculateChecksum(buffer1));

// part 2
var buffer2 = new LinkedList<Block>(blocks);
var files = FindFilesToMove(buffer2.Last).ToList();

foreach (var file in files)
{
	var emptySpace = FindAvailableSpace(file);

	if (emptySpace != null)
	{
		FillEmptySpace(buffer2, file.Value, emptySpace);
		FreeSpace(buffer2, file);
	}
}

Console.WriteLine("Result 2: " + CalculateChecksum(buffer2));

static IEnumerable<LinkedListNode<Block>> FindFilesToMove(LinkedListNode<Block>? start)
{
	while (start != null)
	{
		if (start.Value.IsFile == true)
		{
			yield return start;
		}

		start = start.Previous;
	}
}

static LinkedListNode<Block>? FindAvailableSpace(LinkedListNode<Block> item)
{
	var current = item.List?.First;
	while (current != null && current != item)
	{
		if (current.Value.IsEmpty && current.Value.Length >= item.Value.Length)
		{
			return current;
		}

		current = current.Next;
	}

	return null;
}

static LinkedListNode<Block> FillEmptySpace(LinkedList<Block> list, Block item, LinkedListNode<Block> emptySpace)
{
	var remaining = emptySpace.Value.Length - item.Length;
	if (remaining > 0)
	{
		list.AddAfter(emptySpace, new Block(null, remaining));
	}

	var newNode = list.AddAfter(emptySpace, item);
	list.Remove(emptySpace);

	return newNode;
}

static LinkedListNode<Block> FreeSpace(LinkedList<Block> list, LinkedListNode<Block> item)
{
	var newEmptySpaceLen = item.Value.Length;
	if (item.Previous?.Value.IsEmpty == true)
	{
		newEmptySpaceLen += item.Previous.Value.Length;
		list.Remove(item.Previous);
	}

	if (item.Next?.Value.IsEmpty == true)
	{
		newEmptySpaceLen += item.Next.Value.Length;
		list.Remove(item.Next);
	}

	var newNode = list.AddAfter(item, new Block(null, newEmptySpaceLen));
	list.Remove(item);
	return newNode;
}

long CalculateChecksum(IEnumerable<Block> blocks)
{
	long pos = 0;
	long acc = 0;
	foreach (var block in blocks)
	{
		for (int i = 0; i < block.Length; i++)
		{
			acc += block.Id.HasValue ? pos * block.Id.Value : 0;
			pos += 1;
		}
	}

	return acc;
}

public record Block(int? Id, int Length)
{
	public bool IsFile => Id.HasValue;
	public bool IsEmpty => !Id.HasValue;
}