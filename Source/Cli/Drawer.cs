using System;
using System.Collections.Generic;

namespace Cli;

public class Drawer
{
    public static int index = 0;

    public int DrawMenu(List<string> items)
    {
        Console.CursorVisible = false;
        for (int i = 0; i < items.Count; i++)
        {
            if (i == index)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;

                Console.WriteLine(items[i]);
            }
            else
            {
                Console.WriteLine(items[i]);
            }
            Console.ResetColor();
        }

        ConsoleKeyInfo ckey = Console.ReadKey();
        // ckey.Key switch
        // {
        //     ConsoleKey.DownArrow => _ =>
        //     {
        //         if (index == items.Count - 1)
        //         {
        //             index = 0;
        //         }
        //         else
        //         {
        //             index++;
        //         }
        //     },
        //     ConsoleKey.UpArrow => _ =>
        //     {
        //         if (index <= 0)
        //         {
        //             index = items.Count - 1;
        //         }
        //         else
        //         {
        //             index--;
        //         }
        //     },
        //     ConsoleKey.Enter => index,
        //     _ => -1,
        // };
        // return -2;
        switch (ckey.Key)
        {
            case ConsoleKey.DownArrow:
            {
                if (index == items.Count - 1)
                {
                    index = 0;
                }
                else { index++; }
            }
                break;
            case ConsoleKey.UpArrow:
            {
                if (index <= 0)
                {
                    index = items.Count - 1;
                }
                else { index--; }
            }
                break;
            case ConsoleKey.Enter:
            {
                return index;
            }
            default:
            {
                return -1;
            }
        }
        return -2;
    }
}