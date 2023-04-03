namespace CodeCollection.Funcs;

public class Demo : IDemo
{
    /*
    The two methods in this class, SimpleInstanceDemo() and ExpressionsDemo(),
    are intended to demonstrate how Func expressions can be used in
    Dependency Injection to make more efficient use of resources by only
    instantiating classes as they are needed. This contrasts to the typical
    approach with DI where fully instantiated objects are injected, whether
    or not they are actually used.
    */


    public void Run()
    {
        SimpleInstanceDemo();

        ExpressionsDemo();
    }

    private static void SimpleInstanceDemo()
    {
        OutputMessage("Running SimpleInstanceDemo...");
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

    private static void ExpressionsDemo()
    {
        OutputMessage("Running ExpressionsDemo...");
        OutputMessage("new up the expressions demos");

        var demoNone = GetNewExpressionsDemoExample();
        var demoFoo = GetNewExpressionsDemoExample();
        var demoBar = GetNewExpressionsDemoExample();
        var demoBoth = GetNewExpressionsDemoExample();

        OutputMessage("call that doesn't call any of the expressions' methods");
        demoNone.CallNothing();

        OutputMessage("call that only calls Foo's method");
        demoFoo.CallFoo();

        OutputMessage("call that only calls Bar's method");
        demoBar.CallBar();

        OutputMessage("call that only calls both Foo's and Bar's methods");
        demoBoth.CallFooAndBar();
    }

    private static DemoWithExpressions GetNewExpressionsDemoExample()
    {
        return new DemoWithExpressions(() => new Foo(), () => new Bar());
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

public class DemoWithExpressions
{
    private Func<IFoo> foo;
    private Func<IBar> bar;
    public DemoWithExpressions(Func<IFoo> foo, Func<IBar> bar)
    {
        this.foo = foo;
        this.bar = bar;
    }

    public void CallNothing()
    {
        Demo.OutputMessage("\tneither foo nor bar had their methods called");

        /*
        Using expresssions for the injections means that no constructors
        get called in this method. If this is the only method that gets
        called during the lifetime of this object, the expressions for
        foo & bar instance variables never get evaluated and the respective
        constructors never get called, since they're never needed. As a 
        result, that overhead is never incurred, which makes for better
        resource allocation.
        */
    }

    public void CallFoo()
    {
        Demo.OutputMessage("\tcalling foo now");
        foo().Bar();

        /*
        When the foo expression is evaluated, the constructor gets invoked,
        and only then. Despite the fact that IBar has been injected in this
        class, its constructor has not been invoked yet. If this is the only 
        method called during this object's lifespan, or if other methods 
        in this class only call methods on foo, the IBar instance expression 
        variable will never be invoked and the app will not incur that overhead.
        */
    }

    public void CallBar()
    {
        Demo.OutputMessage("\tcalling bar now");
        bar().Baz();

        /*
        Similar to the previous method, if this is the only method called,
        foo's expression will not be evaluated and the app will not incur
        that overhead
        */
    }

    public void CallFooAndBar()
    {
        Demo.OutputMessage("\tcalling foo.Bar now");
        foo().Bar();

        Demo.Sleep5s();

        Demo.OutputMessage("\tcalling bar.Baz now");
        bar().Baz();

        /*
        This method evaluates both expressions and executes their methods; however
        even with both expressions being invoked, their respective constructors
        are still only called immediately before the method invocation happens.
        */
    }
}