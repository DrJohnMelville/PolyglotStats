﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Melville.PolyglotStats.Stats.Functional;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace Melville.PolyglotStats.Stats.FileWriters;

 public static class TablePadder
  {
    public static IEnumerable<IEnumerable<XElement>> PadColumns(IEnumerable<IEnumerable<XElement>> table)
    {
      var ret = table.Select(i => i.AsList()).AsList();
      var widths = CountColumns(ret);
      var finalWidth = widths.Max();
      for (int i = 0; i < ret.Count; i++)
      {
        if (widths[i] >= finalWidth) continue;
        ret[i].Add(new XElement("td", new XAttribute("colspan", finalWidth - widths[i])));
      }
      return ret;
    }

    private static int[] CountColumns(IList<IList<XElement>> table)
    {
      var ret = new int[table.Count];
      for (int row = 0; row < ret.Length; row++)
      {
        for (int col = 0; col < table[row].Count; col++)
        {
          var elt = table[row][col];
          var rows = GetIntAttribute(elt, "rowspan");
          var cols = GetIntAttribute(elt, "colspan");
          for (int i = 0; i < rows && (i + row < ret.Length); i++)
          {
            ret[i + row] += cols;
          }
        }
      }
      return ret;

      int GetIntAttribute(XElement elt, string attr) =>
        int.Parse(elt?.Attribute(attr)?.Value ?? "1");
    }

    private static int CountCols(IList<XElement> list)
    {
      return list.Sum(i => int.Parse(i.Attributes("colSpan").FirstOrDefault()?.Value ?? "1"));
    }

  }