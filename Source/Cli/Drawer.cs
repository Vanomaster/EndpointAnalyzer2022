namespace Cli;

public static class Drawer
{
    private static int index = 0;

    public static int DrawMenu(List<string> items)
    {
        Console.CursorVisible = false;
        for (var i = 0; i < items.Count; i++)
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

        var ckey = Console.ReadKey();

        switch (ckey.Key)
        {
            case ConsoleKey.DownArrow:
            {
                if (index == items.Count - 1)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }

                break;
            }

            case ConsoleKey.UpArrow:
            {
                if (index <= 0)
                {
                    index = items.Count - 1;
                }
                else
                {
                    index--;
                }

                break;
            }

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