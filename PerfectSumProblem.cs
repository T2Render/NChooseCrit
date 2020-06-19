using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

public class Program
{
    static readonly Action<object> Write = Console.Write;
    static readonly Func<ConsoleKeyInfo> ReadKey = Console.ReadKey;

    // Composition root
    static void Main(string[] args)
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

        // Use timer for checking algo time
        var sw = new Stopwatch();
        sw.Start();


        // -----------------------------------------
        // ** INSERT YOUR DATA HERE **
        // Init config,
        var config = new Config
        {
            MaxSolutions = 1,          // restrict result count
            ShowProgress = false,       // show backtrack progress
            ShowStatus = false,         // show iteration progress
            DisplaySolutions = false,   // Show solution list
            DetectCodeError = false,   // slower running time if true
        };

        // set
        var set = new List<decimal> { 1.2M, 3.4M, 6.4M, 1.7M, 2.4M, -7, -3, -2, 5, 8, 1, 23, 4, 1, 234, 354, 235, 546, 5467, 689, 34, 34, 7, 4567455, 567, 234, 3, 4, 56, 67, 456, 867, 76, 89, 765, 687, 65, 4564567455, 567, 234, 3, 4, 56, 67, 456, 867, 76, 89, 765, 687, 65, 1000985, 8857650, 23485, 3214992, 32000, 32, 5, 6, 7, 8, 2334, 65, 7, 123 };
        // subset sum
        decimal sum = 4.6M ;

        // ** END INSERT DATA **
        // -----------------------------------------


        // Init data
        var P = new Data(set, sum, config);

        // Init backtrack algo
        var bt = new Backtrack(Write, P);

        // Find solutions
        bt.Run();

        // Algo done, stop time
        sw.Stop();

        // Show results
        bt.Show();

        // returns values and indices of values.
        // NB: list is sorted
        List<string> results = bt.GetResult();
        List<string> indices = bt.getIndices();

        Write(string.Format("\nSec: {0}\nPress a key to exit\n",
            sw.ElapsedMilliseconds / 1000d));

        ReadKey();
    }
}

class Config
{
    public int MaxSolutions { get; set; }
    public bool ShowProgress { get; set; }
    public bool ShowStatus { get; set; }
    public bool DisplaySolutions { get; set; }
    public bool DetectCodeError { get; set; }
    public Config() { MaxSolutions = 1; }
}

class Backtrack
{
    readonly Action<object> _write;
    private readonly Data _p;
    private bool _maxSolutionsReachedExitBt;

    public Backtrack(Action<object> write, Data P)
    {
        _write = write;
        _p = P;
    }

    void UndoUpdate(Data P, Candidate c, int depth)
    {
        P.Remove(c);
        if (P.Config.ShowProgress) { Print(P, c, depth); }
    }

    void Update(Data P, Candidate c, int depth)
    {
        P.Add(c);
        if (P.Config.ShowProgress) { Print(P, c, depth); }
    }

    /*    
    wikipedia:
    http://en.wikipedia.org/wiki/Backtracking
     
    procedure bt(P,c)
        if reject(P,c) then return
        update(P,c) // update data
        if accept(P,c) then output(P,c)
        s ← first(P,c)
        while s ≠ Λ do
            bt(P,s)
            s ← next(P,s)
        end while
        undoUpdate(P,c) // backtracking starts here
     end procedure
    */

    // O(?)
    void Bt(Data P, Candidate c, int depth)
    {
        if (Reject(P, c)) { return; }

        // Update data
        Update(P, c, depth);

        if (P.Config.ShowStatus) { ShowStatus(P, c); } // print status

        if (Accept(P, c)) { SaveResult(P, c); }

        if (_maxSolutionsReachedExitBt) { return; } // custom exit

        var s = First(P, c);

        while (s != null)
        {
            Bt(P, s, depth + 1);
            s = Next(P, s);
        }

        if (_maxSolutionsReachedExitBt) { return; } // custom exit

        // Leaf, dead end, backtrack, roll back data
        UndoUpdate(P, c, depth);
    }

    // O(1)
    bool Accept(Data P, Candidate c)
    {
        const decimal epsilon = 0.000001M; //0.000001f
        if (Math.Abs(P.Sum - P.WorkingSum) < epsilon) { return true; }

        return false;
    }

    // O(1)
    bool Reject(Data P, Candidate c)
    {
        if (_maxSolutionsReachedExitBt) { return true; }

        if (c == null) { throw new ArgumentNullException("c"); }

        var i = P.Numbers[c.Index];

        // Optimization vs. iterate all combinations
        // Reject if rest of sorted list contains positive numbers
        if (i >= 0 && i + P.WorkingSum > P.Sum) { return true; }

        return false;
    }

    public void Run()
    {
        var setsum = _p.Numbers.Sum();
        if (_p.Sum > setsum)
        {
            _write(string.Format("Sum: {0} is bigger than set sum {1}, no solutions exists\n",
                _p.Sum, setsum));
            return;
        }

        for (int i = 0; i < _p.Numbers.Length; i++)
        {
            Bt(_p, new Candidate { Index = i }, 1);
        }
    }

    public List<string> GetResult()
    {
        List<string> resultValues = _p.resultValues.ToList();
        return resultValues;
    }

    public List<string> getIndices()
    {
        List<string> resultIndices = _p.resultIndexes.ToList();
        return resultIndices;
    }

    public void Show()
    {
        _write(string.Format("Max solutions: {0}\n", _p.Config.MaxSolutions));
        _write(string.Format("Sum: {0}\n", _p.Sum));
        _write(string.Format("Set:\n"));
        _p.Numbers.ToList().ForEach(a => _write(a + ", "));

        if (_p.Config.DisplaySolutions)
        {
            _write(string.Format("\n\n*** Solutions:\n\n"));
            _p.SolutionsList.ForEach(_write);
        }

        _write(string.Format("\nUnique Paths Tried: {0}\n", _p.PathsTried));
        _write(string.Format("Solutions count: {0}\n", _p.Solutions));
    }

    // O(n) where n is path length
    void SaveResult(Data P, Candidate c)
    {
        P.Solutions++;

        if (P.Config.DisplaySolutions)
        {
            // Only save result data if to be displayed
            var list = P.Stack.ToList();
            list.Reverse();

            string numbers = list.Aggregate("", (a, b) => a + (P.Numbers[b] + ", "));
            string indexes = list.Aggregate("", (a, b) => a + (b + ", "));
            P.SolutionsList.Add(string.Format("Numbers: {0}\nIndexes: {1}\n\n", numbers, indexes));
            P.resultValues.Add(numbers);
            P.resultIndexes.Add(indexes);
        }


        if (P.Solutions >= P.Config.MaxSolutions) { _maxSolutionsReachedExitBt = true; }
    }

    // First valid child from path
    // O(1)
    Candidate First(Data P, Candidate c)
    {
        int j = c.Index + 1;
        if (j >= P.Numbers.Length) { return null; }

        return new Candidate { Index = j };
    }

    // Next sibling from current leaf
    // O(1)
    Candidate Next(Data P, Candidate c)
    {
        int j = c.Index + 1;
        if (j >= P.Numbers.Length) { return null; }

        return new Candidate { Index = j };
    }

    void Print(Data P, Candidate c, int depth)
    {
        _write(string.Format("path: {0}  \tsum: {1}  \tdepth: {2}\n",
            P.Path.Get(), P.WorkingSum, depth));
    }

    void ShowStatus(Data P, Candidate c)
    {
        // Debug
        if (P.PathsTried % 500000 == 0)
        {
            _write(string.Format("Paths tried: {0:n0}  \tSolutions: {1}  \tPath: {2}\n",
                P.PathsTried, P.Solutions, P.Path.Get()));
        }
    }
}

class Candidate
{
    public int Index { get; set; }
    public override string ToString()
    {
        return string.Format("{0}", Index);
    }
}

class Data
{
    // Init data
    public Config Config;
    public decimal Sum { get; private set; }
    public decimal[] Numbers { get; private set; }

    // Backtracking data
    public bool[] Path { get; private set; }
    public decimal WorkingSum { get; private set; }
    public Stack<int> Stack = new Stack<int>();

    // Result data
    public int PathsTried;
    public List<string> SolutionsList = new List<string>();
    public List<string> resultValues = new List<string>();
    public List<string> resultIndexes = new List<string>();

    public int Solutions { get; set; }

    // Only used for Detect code error
    public HashSet<string> SetPathsTried = new HashSet<string>();

    public Data(List<decimal> numbers, decimal sum, Config config)
    {
        numbers.Sort(); // sort is used for reject condition in bt algo
        Numbers = numbers.ToArray();
        Path = new bool[Numbers.Length];
        Sum = sum;
        Config = config;
    }

    // O(1)
    public void Remove(Candidate c)
    {
        if (c == null) { throw new ArgumentNullException("c"); }

        var i = Stack.Pop();
        UpdatePath(false, i);
        WorkingSum -= Numbers[i];
    }

    // O(1) (or O(n) if detectCodeError)
    public void Add(Candidate c)
    {
        if (c == null) { throw new ArgumentNullException("c"); }

        UpdatePath(true, c.Index);
        WorkingSum += Numbers[c.Index];
        Stack.Push(c.Index);
        PathsTried++;

        if (Config.DetectCodeError)
        {
            var path = Stack.GetString(); // O(n) where n is stack count
            if (SetPathsTried.Contains(path)) // O(1)
            {
                throw new ApplicationException(string.Format("Path already visited: {0}", path));
            }
            SetPathsTried.Add(path);
        }
    }

    // O(1)
    void UpdatePath(bool value, int index)
    {
        if (Path[index] == value)
        {
            throw new ApplicationException(string.Format("Path already set for index: {0}, value: {1}",
                index, Numbers[index]));
        }
        Path[index] = value;
    }
}

static class Extensions
{
    public static string GetString<T>(this IEnumerable<T> arr)
    {
        return arr.Aggregate("", (a, b) => a + string.Format("{0}->", b));
    }
    public static string Get(this IEnumerable<bool> arr)
    {
        return arr.Aggregate("", (a, b) => a + (b ? "1" : "0"));
    }
}
