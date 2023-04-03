namespace CodeCollection.Funcs;

public class Demo : IDemo
{
    public void Run()
    {
        SimpleInstanceDemo();
    }

    private static void SimpleInstanceDemo()
    {
        Console.WriteLine("Running SimpleInstanceDemo...");
        OutputMessage("instantiating Foo now and calling Bar");
        IFoo foo = new Foo();
        foo.Bar();

        OutputMessage("creating expression variable");
        Func<IFoo> fooExpression = () => new Foo();

        Sleep5s();
        /* 
        This pause is just to demonstrate that despite the variable 
        being created, Foo's constructor isn't actually invoked until 
        the expression gets evaluated.
        */

        OutputMessage("calling expression variable's Bar method");


        fooExpression().Bar();

        OutputMessage("Voilà!");
    }

    public static void OutputMessage(string message)
    {
        Console.WriteLine($"{GetCurrentTime()} - {message}");
    }

    public static string GetCurrentTime()
    {
        return DateTime.Now.ToLongTimeString();
    }


    public static void Sleep5s()
    {
        OutputMessage("sleeping for 5s");
        for (int i = 0; i < 5; i++)
        {
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine(i + 1);
        }
    }
}

public interface IFoo
{
    void Bar();
}

public class Foo : IFoo
{
    public Foo()
    {
        Demo.OutputMessage("\tFoo constructor code");
    }

    public void Bar()
    {
        Demo.OutputMessage("\tFoo.Bar() method code");
    }
}

public interface IBar
{
    void Baz();
}

public class Bar : IBar
{
    public Bar()
    {
        Demo.OutputMessage("\tBar constructor code");
    }

    public void Baz()
    {
        Demo.OutputMessage("\tBar.Baz() method code");
    }
}