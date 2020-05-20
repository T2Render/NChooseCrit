using System;
using System.Collections.Generic;

class NChooseCrit
{
    public struct Result
    {
        public List<double> resultSet;
        public List<int> resultIndexSet;
    }
    public struct ResultLists
    {
        public List<List<double>> resultSets;
        public List<List<int>> resultIndexSets;
    }

    // Function to print the subsets whose 
    // sum is equal to the given target K 
    public static Result sumSubsets(
        double[] set, int n, double target)
    {
        // Create the new array with size 
        // equal to array set[] to create 
        // binary array as per n(decimal number) 
        int[] x = new int[set.Length];
        int j = set.Length - 1;
        List<double> resultSet = new List<double>();
        List<int> resultIndexSet = new List<int>();

        // Convert the array into binary array 
        while (n > 0)
        {
            x[j] = n % 2;
            n = n / 2;
            j--;
        }

        double sum = 0;

        // Calculate the sum of this subset 
        for (int i = 0; i < set.Length; i++)
            if (x[i] == 1)
                sum = sum + set[i];

        // Check whether sum is equal to target 
        // if it is equal, then print the subset 
        if (sum == target)
        {
            for (int i = 0; i < set.Length; i++)
            {
                if (x[i] == 1)
                {
                    // Values:
                    resultSet.Add(set[i]);
                    // Index of values:
                    resultIndexSet.Add(i);
                }

            }
        }
        var result = new Result
        {
            resultSet = resultSet,
            resultIndexSet = resultIndexSet
        };
        return result;
    }

    // Function to find the subsets with sum K 
    public static ResultLists findSubsets(double[] arr, double K)
    {
        // Calculate the total no. of subsets 
        int x = (int)Math.Pow(2, arr.Length);
        // Run loop till total no. of subsets 
        // and call the function for each subset 
        List<List<double>> resultSets = new List<List<double>>();
        List<List<int>> resultIndexSets = new List<List<int>>();
        List<double> tmp_list = new List<double>();
        List<int> tmp_index_list = new List<int>();
        for (int i = 1; i < x; i++)
        {
            Result test = sumSubsets(arr, i, K);
            tmp_list = test.resultSet;
            tmp_index_list = test.resultIndexSet;
            if (tmp_list.Count != 0) { 
                resultSets.Add(tmp_list);
                resultIndexSets.Add(tmp_index_list);
            }
        }
        var result = new ResultLists
        {
            resultSets = resultSets,
            resultIndexSets = resultIndexSets
        };
        return result;
    }

    public static void printResult(List<List<double>> results)
    {
        foreach (List<double> element in results)
        {
            foreach (double number in element)
            {
                Console.Write(number + " - ");
            }
            Console.WriteLine("");
        }
    }

    public static void printResultIndex(List<List<int>> results)
    {
        foreach (List<int> element in results)
        {
            foreach (int number in element)
            {
                Console.Write(number + " - ");
            }
            Console.WriteLine("");
        }
    }

    public static void Main(String[] args)
    {
        // Input array: 
        //double[] arr = { 1.2, 1.3, 1.2, 1.3, 1.4, 1.5, 1.2, 2, 0.8, 0.2, 0.5 };
        double[] arr = { 1.2, 1.3, 1.4, 1.5, 2, 0.8, 0.2, 0.5 };
        // Goal:
        double K = 3.2;
        ResultLists results = findSubsets(arr, K);
        List<List<double>> resultsvals = results.resultSets;
        List<List<int>> resultIndexvals = results.resultIndexSets;

        // Print results to console for debugging:
        Console.WriteLine("Values");
        printResult(resultsvals);
        Console.WriteLine("Index");
        printResultIndex(resultIndexvals);
    }
}
