﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            for (var i = 0; i < 10000; i++)
            {
                var s1 = new Schrodinger();
                var val1 = s1.Value;

                var s2 = new Schrodinger();
                var val2 = s2.Value;

                var eq = s1.CompareTo(s2);

                if (val1 != null)
                {
                    Console.WriteLine(val1);
                }

                if (val2 != null)
                {
                    Console.WriteLine(val2);
                }

                Console.WriteLine();

                Debug.WriteLine(i);
                Thread.Sleep(1000);
            }
        }
    }
}
