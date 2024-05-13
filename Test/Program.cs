
class Program
{
    static void Main(string[] args)
    {
        string Test = "test@texture/block/air";
        var temp = Test.Split(new char[] { '@', '/' });
        foreach(string t in temp)
        {
            Console.WriteLine(t);
        }
    }
}