using System;
using System.Collections.Generic;
using System.Linq;

namespace CraftWarePrintPauser
{
    class Program
    {
        static void Main(string[] args)
        {
            // Obtain user parameters. Use default values for parameters not provided.
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

            // Read specified g-code file
            string[] gCodeOrig = System.IO.File.ReadAllLines(args[0]);

            // Create list for holding modified g-code instructions
            List<string> gCode = new List<string>();
            int count = 0;
            foreach(string instruction in gCodeOrig)
            {
                // Place all non layer comment lines into the g-code list as-is
                if(!instruction.StartsWith("; Layer #"))
                {
                    gCode.Add(instruction);
                }
                // Insert layer pause instructions when layer comment lines are found
                else
                {
                    // Read layer number from comment
                    int layer = int.Parse(instruction.Substring(("; Layer #").Length));
                    count++;
                    // Determine if this is a layer at which we pause
                    if(count>layersBeforePause)
                    {
                        // Reset counter for determining at which layers we insert pause instructions
                        count = 1;
                        // Insert the specified number of 1 second pause instructions
                        for (int p = 0; p < pauseSeconds; p++)
                        {
                            // Add instruction for displaying a message on printer indicating how many seconds of the pause remain
                            gCode.Add("M117 Layer " + (layer - 1) + " Done (" + (pauseSeconds - p) + ") ; Display Pause Message");
                            // Add instruction for a 1 second pause
                            gCode.Add("G4 S001 ; Pause For 1 Second");
                        }
                    }
                    // Add instruction for displaying the current layer
                    gCode.Add("M117 Layer "+layer+" Printing");
                }
            }

            // Generate the modified g-code instruction file using a different file name (thus preserving original g-code file)
            System.IO.File.WriteAllLines(args[0].ToUpper().Replace(".GCODE","") + "." + pauseSeconds + "s.Every" + layersBeforePause + "Layers.gCode", gCode);
        }
    }
}
