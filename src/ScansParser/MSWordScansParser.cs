using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Word;
using System.IO;

namespace ScansParser
{
    public class MSWordScansParser : IDisposable
    {
        _Document m_Document = null;
        _Application m_WordApplication = null;
        const string FIRST_DELIM = "1.";
        const string SECOND_DELIM = "2.";

        public MSWordScansParser()
        {
            m_WordApplication = new Application();
        }

        public void Dispose()
        {
            if (m_Document != null)
                m_Document.Close();

            if (m_WordApplication != null)
                m_WordApplication.Quit();
        }

        public void ConvertToTxt(string path)
        {
            if (m_Document != null)
                m_Document.Close();

            m_Document = m_WordApplication.Documents.OpenNoRepairDialog(path);

            String read = string.Empty;

            List<Tuple<string,string>> data = new List<Tuple<string,string>>();
            List<string> dataFrames = new List<string>();
            Paragraphs paragraphs = m_Document.Paragraphs;
            int paragraphsCount = paragraphs.Count;
            Paragraph paragraph = paragraphs.First;
            int k = 0;
            string outputPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(path) + "_Body.txt");
            string outputPath2 = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(path) + "_Frames.txt");
            string outputPath3 = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(path) + "_Exceptions.txt");
            while(paragraph != null)
            {
                k++;
                Range rng = paragraph.Range;
                
                string temp = rng.Text.Trim();

                if (temp != string.Empty)
                {
                    if (rng.Frames != null && rng.Frames.Count > 0)
                        dataFrames.Add(temp);
                    else
                    {
                        Tuple<string, string> output = ParseRange(outputPath3, rng);
                        data.Add(output);
                    }
                }
                paragraph = paragraph.Next();
            }
            foreach (Tuple<string, string> term in data)
            {
                File.AppendAllText(outputPath, string.Format("{0}{1}\r\n", FIRST_DELIM, term.Item1), Encoding.Unicode);
                File.AppendAllText(outputPath, string.Format("{0}{1}\r\n", SECOND_DELIM, term.Item2), Encoding.Unicode);
            }
            
            File.AppendAllLines(outputPath2, dataFrames, Encoding.Unicode);
        }

        private Tuple<string, string> ParseRange(string exceptionsPath, Range rng)
        {
            //rng.Find.ClearFormatting();
            //rng.Find.Font.Bold = -1;
            //rng.Find.Execute(Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true, Type.Missing, true);

            //string a = rng.Find.Text;
            //int edge = (rng.End - rng.Start) / 2;
            //Selection sel = m_WordApplication.Selection;
            //Range selRange = sel.Range;
            //selRange.SetRange(rng.Start, rng.Start + edge);
            //bool isBold = (selRange.Bold != 0);
            //bool boldState = isBold;
            //int i = 1;
            //if (!isBold)
            //    i = -1;

            //while (boldState == isBold)
            //{
            //    selRange.Move(WdUnits.wdCharacter, i);
            //    edge += i;
            //    isBold = (selRange.Bold != 0);
            //}
            //edge -= i;

            //selRange.SetRange(rng.Start, rng.Start + edge);
            //string output1 = selRange.Text;
            //selRange.SetRange(rng.Start + edge, rng.End);
            //string output2 = selRange.Text;
            StringBuilder sb = new StringBuilder();

            string output1 = "";
            string output2 = "";
            bool boldState = true;
            for (Range rng1 = rng.Words.First; rng1 != null && rng1.Start < rng.End; rng1 = rng1.Next(WdUnits.wdWord))
            {
                string s = rng1.Text;
                if (string.IsNullOrWhiteSpace(s))
                    continue;

                bool isBold = (rng1.Bold != 0);
                if (isBold != boldState)
                {
                    if (boldState)
                    {
                        boldState = false;
                        output1 = sb.ToString();
                        sb.Clear();
                    }
                    else
                    {
                        if (rng.End - rng1.End > 1)
                        {
                            File.AppendAllText(exceptionsPath, string.Format("{0}\r\n", rng.Text), Encoding.Unicode);
                        }
                        break;
                    }
                }
                sb.Append(s);
            }
            output2 = sb.ToString();
            //Range rangeCharacter = null;
            //Range characterPrev = null;

            //Characters chars = rng.Characters;

            //int rangeLength = rng.End - rng.Start;
            //int rangeCharactersCount = chars.Count;

            //string output1 = "";
            //string output2 = "";

            //StringBuilder sb = new StringBuilder();
            //Selection selection = m_WordApplication.Selection;
            //bool boldState = true;
            //for (int i = 1; (i <= rangeCharactersCount) && (0 < rangeLength); i++)
            //{
            //    if (rangeLength != rangeCharactersCount)
            //    {
            //        // Use the character's range to select a char in the field instead of the field's result range
            //        // when the nonprinting or hidden characters are included in the result range.
            //        rangeCharacter = chars[i];

            //        if (i == 1)
            //        {
            //            // Use End position to select the first character because the nonprinting or
            //            // hidden characters can be placed in the starting positions for the first character's range.
            //            int rangeEnd = rangeCharacter.End;
            //            selection.SetRange(rangeEnd - 1, rangeEnd);
            //        }
            //        else
            //        {
            //            // Use Start position to select the end character because the nonprinting or
            //            // hidden characters can be placed in the ending positions for the end character's range.
            //            // Use the same method for other characters except of the first character.
            //            int rangeStart = rangeCharacter.Start;
            //            selection.SetRange(rangeStart, rangeStart + 1);
            //        }
            //    }
            //    else
            //    {
            //        // Use the result range to select a char in the field when 
            //        // the nonprinting or hidden characters are  not included in the result range.
            //        int rangeStart = rng.Start;
            //        selection.SetRange(rangeStart + (i - 1), rangeStart + i);
            //        rangeCharacter = selection.Range;
            //    }

            //    characterPrev = rangeCharacter;

            //    string temp = rangeCharacter.Text;
            //    if( temp == "\r" )
            //        continue;

            //    bool isBold = (rangeCharacter.Bold != 0);

            //    if (isBold != boldState)
            //    {
            //        //if (!boldState )
            //            //throw new Exception(string.Format("Incorrent format of Range: {0}", rng.Text));
            //        if (!boldState)
            //        {
            //            boldState = isBold;
            //            output1 = sb.ToString();
            //            sb.Clear();
            //        }

            //    }
            //    sb.Append(temp);                
            //}
            //output2 = sb.ToString();

            return new Tuple<string, string>(output1, output2);;
        }

        public void Test()
        {
            string s = "автокореляц³йний";
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                bool b = Char.IsSymbol(c);
                b = char.IsControl(c);
                int k = 1;
            }

        }
        
    }
}
