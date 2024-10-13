using DelegateRetry.Core;

public record Record1(int Input1, Record2 Input2);
public record Record2(char X);

public class Someclass 
{
    /// <summary>
    /// A method to process two Record1 instances.
    /// This method is registered for use with a delegate.
    /// </summary>
    /// <param name="rec1">The first Record1 instance.</param>
    /// <param name="rec2">The second Record1 instance.</param>
    /// <returns>A string representation of the processed data.</returns>
    [Register] // This method must be static for registration
    public static string Func1(Record1 rec1, Record1 rec2)
    {
        var resultData = $"Processed input: {rec1}";
        // Uncomment to simulate an exception case
        // throw new Exception("exp: just to try error case");
        return resultData;
    }

    /// <summary>
    /// A method that performs an action without returning a value.
    /// </summary>
    [Register]
    public static void Func2()
    {
        Console.WriteLine("Func2 executed without parameters.");
    }

    /// <summary>
    /// A method that takes primitive parameters and returns their sum.
    /// </summary>
    /// <param name="a">First integer parameter.</param>
    /// <param name="b">Second integer parameter.</param>
    /// <returns>The sum of the two integers.</returns>
    [Register]
    public static int Func3(int a, int b)
    {
        return a + b;
    }

    /// <summary>
    /// A method that takes a collection of Record1 and returns a collection of strings.
    /// </summary>
    /// <param name="records">A collection of Record1 instances.</param>
    /// <returns>A collection of processed string representations of the input records.</returns>
    [Register]
    public static IEnumerable<string> Func4(IEnumerable<Record1> records)
    {
        int i = 0;
        foreach (var record in records)
        {
            yield return $"Processed Record1: {++i}";
        }
    }
}