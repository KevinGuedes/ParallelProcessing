using System.Diagnostics;


const int longProcessesCount = 3;
var watch = new Stopwatch();


Console.WriteLine("Using multiple awaits");
watch.Start();
for (int i = 0; i < longProcessesCount; i++)
    await SomethingWithLongProcess(i);
watch.Stop();
Console.WriteLine($"Elapsed time using await with {longProcessesCount} long processes: {watch.ElapsedMilliseconds}ms.");
//Equal to the sum of all awaited processes


Console.WriteLine("\n\n");
Console.WriteLine("Using Task.WhenAll");
watch.Restart();
await UsingTaskWhenAll(longProcessesCount);
watch.Stop();
Console.WriteLine($"Elapsed time using Task.WhenAll with {longProcessesCount} long processes: {watch.ElapsedMilliseconds}ms.");
//Equal to the longest process running on Task.WhenAll


Console.WriteLine("\n\n");
Console.WriteLine("Using Parallel.ForEachAsync");
watch.Restart();
await UsingParallelForEach(longProcessesCount);
watch.Stop();
Console.WriteLine($"Elapsed time using Parallel.ForEachAsync with {longProcessesCount} long processes: {watch.ElapsedMilliseconds}ms.");
//Equal to the longest process running on Parallel.ForEach


Console.WriteLine("\n\n");
var data = new List<int>();
Console.WriteLine("Normal operation using For");
watch.Restart();
for (int i = 0; i < longProcessesCount; i++)
{
    data = new List<int>();
    data.AddRange(Enumerable.Range(0, i * 100000000));
}
watch.Stop();
Console.WriteLine($"Elapsed time using For to generate a range of numbers: {watch.ElapsedMilliseconds}ms.");


Console.WriteLine("\n\n");
Console.WriteLine("Normal operation using For");
watch.Restart();
Parallel.For(0, longProcessesCount, value => {
    data = new List<int>();
    data.AddRange(Enumerable.Range(0, value * 100000000));
});
watch.Stop();
Console.WriteLine($"Elapsed time using ParallelFor to generate a range of numbers: {watch.ElapsedMilliseconds}ms.");
//For relatively lower values, the ParallelFor has similar time to the common For


#region Parallel Processing Methods
static async Task SomethingWithLongProcess(int seconds)
{
    var processId = Guid.NewGuid();
    Console.WriteLine($"Running long process {processId}");
    await Task.Delay(1000 * seconds);
    Console.WriteLine($"Doing something after the long process {processId}");
}

static Task UsingTaskWhenAll(int longProcessesCount)
{
    var tasks = new List<Task>();

    for (int i = 0; i < longProcessesCount; i++)
        tasks.Add(SomethingWithLongProcess(i));

    return Task.WhenAll(tasks);
}

static Task UsingParallelForEach(int longProcessesCount)
{
    var data = new List<int>();
    for (int i = 0; i < longProcessesCount; i++)
        data.Add(i);

    return Parallel.ForEachAsync(data, async (item , ct) => await SomethingWithLongProcess(item));
}
#endregion