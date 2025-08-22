using System;
using System.Collections.Generic;
using System.Drawing;

namespace LabelDesigner.Services.Barcode
{
    /// <summary>
    /// Minimal Code128B encoder/renderer sufficient for demo purposes.
    /// </summary>
    public static class Code128
    {
        // Code 128 patterns: each 11 modules (6 elements). Values per ISO/IEC 15417.
        // For brevity, subset B only.
        private static readonly int[][] Patterns = new int[][] {
            new []{2,1,2,2,2,2}, // 0
            new []{2,2,2,1,2,2}, // 1
            new []{2,2,2,2,2,1}, // 2
            new []{1,2,1,2,2,3}, // 3
            new []{1,2,1,3,2,2}, // 4
            new []{1,3,1,2,2,2}, // 5
            new []{1,2,2,2,1,3}, // 6
            new []{1,2,2,3,1,2}, // 7
            new []{1,3,2,2,1,2}, // 8
            new []{2,2,1,2,1,3}, // 9
            new []{2,2,1,3,1,2}, // 10
            new []{2,3,1,2,1,2}, // 11
            new []{1,1,2,2,3,2}, // 12
            new []{1,2,2,1,3,2}, // 13
            new []{1,2,2,2,3,1}, // 14
            new []{1,1,3,2,2,2}, // 15
            new []{1,2,3,1,2,2}, // 16
            new []{1,2,3,2,2,1}, // 17
            new []{2,2,3,2,1,1}, // 18
            new []{2,2,1,1,3,2}, // 19
            new []{2,2,1,2,3,1}, // 20
            new []{2,1,3,2,1,2}, // 21
            new []{2,2,3,1,1,2}, // 22
            new []{3,1,2,1,3,1}, // 23
            new []{3,1,1,2,2,2}, // 24
            new []{3,2,1,1,2,2}, // 25
            new []{3,2,1,2,2,1}, // 26
            new []{3,1,2,2,1,2}, // 27
            new []{3,2,2,1,1,2}, // 28
            new []{3,2,2,2,1,1}, // 29
            new []{2,1,2,1,2,3}, // 30
            new []{2,1,2,3,2,1}, // 31
            new []{2,3,2,1,2,1}, // 32
            new []{1,1,1,3,2,3}, // 33
            new []{1,3,1,1,2,3}, // 34
            new []{1,3,1,3,2,1}, // 35
            new []{1,1,2,3,1,3}, // 36
            new []{1,3,2,1,1,3}, // 37
            new []{1,3,2,3,1,1}, // 38
            new []{2,1,1,3,1,3}, // 39
            new []{2,3,1,1,1,3}, // 40
            new []{2,3,1,3,1,1}, // 41
            new []{1,1,2,1,3,3}, // 42
            new []{1,1,2,3,3,1}, // 43
            new []{1,3,2,1,3,1}, // 44
            new []{1,1,3,1,2,3}, // 45
            new []{1,1,3,3,2,1}, // 46
            new []{1,3,3,1,2,1}, // 47
            new []{3,1,3,1,2,1}, // 48
            new []{2,1,1,3,3,1}, // 49
            new []{2,3,1,1,3,1}, // 50
            new []{2,1,3,1,1,3}, // 51
            new []{2,1,3,3,1,1}, // 52
            new []{2,1,3,1,3,1}, // 53
            new []{3,1,1,1,2,3}, // 54
            new []{3,1,1,3,2,1}, // 55
            new []{3,3,1,1,2,1}, // 56
            new []{3,1,2,1,1,3}, // 57
            new []{3,1,2,3,1,1}, // 58
            new []{3,3,2,1,1,1}, // 59
            new []{3,1,4,1,1,1}, // 60
            new []{2,2,1,4,1,1}, // 61
            new []{4,3,1,1,1,1}, // 62
            new []{1,1,1,2,2,4}, // 63
            new []{1,1,1,4,2,2}, // 64
            new []{1,2,1,1,2,4}, // 65
            new []{1,2,1,4,2,1}, // 66
            new []{1,4,1,1,2,2}, // 67
            new []{1,4,1,2,2,1}, // 68
            new []{1,1,2,2,1,4}, // 69
            new []{1,1,2,4,1,2}, // 70
            new []{1,2,2,1,1,4}, // 71
            new []{1,2,2,4,1,1}, // 72
            new []{1,4,2,1,1,2}, // 73
            new []{1,4,2,2,1,1}, // 74
            new []{2,4,1,2,1,1}, // 75
            new []{2,2,1,1,1,4}, // 76
            new []{4,1,3,1,1,1}, // 77
            new []{2,4,1,1,1,2}, // 78
            new []{1,3,4,1,1,1}, // 79
            new []{1,1,1,2,4,2}, // 80
            new []{1,2,1,1,4,2}, // 81
            new []{1,2,1,2,4,1}, // 82
            new []{1,1,4,2,1,2}, // 83
            new []{1,2,4,1,1,2}, // 84
            new []{1,2,4,2,1,1}, // 85
            new []{4,1,1,2,1,2}, // 86
            new []{4,2,1,1,1,2}, // 87
            new []{4,2,1,2,1,1}, // 88
            new []{2,1,2,1,4,1}, // 89
            new []{2,1,4,1,2,1}, // 90
            new []{4,1,2,1,2,1}, // 91
            new []{1,1,1,1,4,3}, // 92
            new []{1,1,1,3,4,1}, // 93
            new []{1,3,1,1,4,1}, // 94
            new []{1,1,4,1,1,3}, // 95
            new []{1,1,4,3,1,1}, // 96
            new []{4,1,1,1,1,3}, // 97
            new []{4,1,1,3,1,1}, // 98
            new []{1,1,3,1,4,1}, // 99
            new []{1,1,4,1,3,1}, // 100
            new []{3,1,1,1,4,1}, // 101 START A
            new []{4,1,1,1,3,1}, // 102 START B
            new []{2,1,1,4,1,2}, // 103 START C
            new []{2,3,3,1,1,1,2} // 104 STOP (13 modules)
        };

        public static List<int> Encode(string input)
        {
            // Subset B: ASCII 32..127; map to 0..94 (code set B value = (char-32))
            var codes = new List<int>();
            int startCode = 104; // START B index = 102 in terms of symbol, but our pattern table index 102; we will use code value 104 for checksum formula
            codes.Add(104); // start B code value
            foreach (char ch in input)
            {
                int val = ch - 32;
                if (val < 0 || val > 95) val = 0; // fallback to space
                codes.Add(val);
            }
            // checksum
            int checksum = codes[0];
            for (int i = 1; i < codes.Count; i++)
            {
                checksum += codes[i] * i;
            }
            checksum %= 103;
            codes.Add(checksum);
            codes.Add(106); // STOP
            return codes;
        }

        public static void Draw(Graphics g, List<int> codes, RectangleF bounds)
        {
            // Convert code values to pattern bars and draw scaled to bounds width
            // Compute total module count
            int modules = 0;
            foreach (var code in codes)
            {
                int idx = code;
                if (idx == 106) // STOP has 13 modules
                {
                    modules += 13;
                }
                else
                {
                    modules += 11;
                }
            }

            float moduleWidth = bounds.Width / modules;
            float x = bounds.X;
            float y = bounds.Y;
            float height = bounds.Height;

            foreach (var code in codes)
            {
                int[] pattern;
                if (code == 106) pattern = Patterns[103]; // STOP pattern index mapping (last in our array)
                else if (code == 104) pattern = Patterns[102]; // START B pattern
                else pattern = Patterns[code];

                // draw bars/spaces: start with bar
                for (int i = 0; i < pattern.Length; i++)
                {
                    float w = pattern[i] * moduleWidth;
                    if (i % 2 == 0)
                    {
                        g.FillRectangle(Brushes.Black, x, y, w, height);
                    }
                    x += w;
                }
            }
        }
    }
}
