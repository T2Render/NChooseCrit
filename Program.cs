using System;
using System.Collections.Generic;

class NChooseCrit
{
    // Function to print the subsets whose 
    // sum is equal to the given target K 
    public static List<double> sumSubsets(
        double[] set, int n, double target)
    {
        // Create the new array with size 
        // equal to array set[] to create 
        // binary array as per n(decimal number) 
        int[] x = new int[set.Length];
        int j = set.Length - 1;
        List<double> resultSet = new List<double>();

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
                    resultSet.Add(set[i]);
            }
        }
        return resultSet;
    }

    // Function to find the subsets with sum K 
    public static List<List<double>> findSubsets(double[] arr, int K)
    {
        // Calculate the total no. of subsets 
        int x = (int)Math.Pow(2, arr.Length);
        Console.WriteLine("here we are!");
        // Run loop till total no. of subsets 
        // and call the function for each subset 
        List<List<double>> resultSets = new List<List<double>>();
        List<double> tmp_list = new List<double>();
        for (int i = 1; i < x; i++)
        { 
            tmp_list = sumSubsets(arr, i, K);
            if (tmp_list.Count != 0) { 
                resultSets.Add(tmp_list);
            }
        }
        return resultSets;
    }

    public static void printResult(List<List<double>> results)
    {
        foreach (List<double> element in results)
        {
            foreach (double number in element)
            {
                Console.Write(number + ",");
            }
            Console.WriteLine("");
        }
    }

    public static void Main(String[] args)
    {
        // Input array: 
        double[] arr = { 5, 10, 12, 13, 15, 18 };
        // Goal:
        int K = 30;
        List<List<double>> results = new List<List<double>>();
        results = findSubsets(arr, K);

        // Print results to console for debugging:
        printResult(results);

    }
}