using System;
using System.Collections.Generic;
using System.Linq;

namespace CraftWarePrintPauser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() == 0)
            {
                Console.WriteLine("\r\n\r\nSyntax: CraftWarePrintPauser file.gCode layersBeforePause secondsToPause\r\n\r\n");
            }
            else if (args.Count() == 1)
            {
                args = new string[] { args[0], "10", "3" };
            }
            else if (args.Count() == 2)
            {
                args = new string[] { args[0], args[1], "3" };
            }

            int layersBeforePause = int.Parse(args[1]);
            int pauseSeconds = int.Parse(args[2]);

            Console.WriteLine("Inserting " + pauseSeconds + " Seconds Delay After Each " + layersBeforePause + " Layers In "+args[0]);

            string[] gCodeOrig = System.IO.File.ReadAllLines(args[0]);
            List<string> gCode = new List<string>();
            int count = 0;
            foreach(string instruction in gCodeOrig)
            {
                if(!instruction.StartsWith("; Layer #"))
                {
                    gCode.Add(instruction);
                }
                else
                {
                    int layer = int.Parse(instruction.Substring(("; Layer #").Length));
                    count++;
                    if(count>layersBeforePause)
                    {
                        count = 1;
                        for (int p = 0; p < pauseSeconds; p++)
                        {
                            gCode.Add("M117 Layer " + (layer - 1) + " Done (" + (pauseSeconds - p) + ") ; Display Pause Message");
                            gCode.Add("G4 S001 ; Pause For 1 Second");
                        }
                    }
                    gCode.Add("M117 Layer "+layer+" Printing");
                }
            }

            System.IO.File.WriteAllLines(args[0].ToUpper().Replace(".GCODE","") + "." + pauseSeconds + "s.Every" + layersBeforePause + "Layers.gCode", gCode);
        }
    }
}
