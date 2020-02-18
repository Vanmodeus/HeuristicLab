﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeuristicLab.Core;
using HeuristicLab.Data;
using Newtonsoft.Json.Linq;

namespace HeuristicLab.JsonInterface {

  public class IntArrayConverter : BaseConverter {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(IntArray);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      IntArray arr = item as IntArray;
      int[] d = CastValue<int[]>(data);
      bool resizeTmp = arr.Resizable;
      arr.Resizable = true;
      arr.Length = d.Length;
      for (int i = 0; i < d.Length; ++i)
        arr[i] = d[i];
      arr.Resizable = resizeTmp;
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new IntArrayJsonItem() {
        Name = "[OverridableParamName]",
        Description = value.ItemDescription,
        Value = ((IntArray)value).CloneAsArray(),
        Range = new int[] { int.MinValue, int.MaxValue }
      };
  }

  public class DoubleArrayConverter : BaseConverter {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(DoubleArray);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      DoubleArray arr = item as DoubleArray;
      double[] d = CastValue<double[]>(data);
      bool resizeTmp = arr.Resizable;
      arr.Resizable = true;
      arr.Length = d.Length;
      for (int i = 0; i < d.Length; ++i)
        arr[i] = d[i];
      arr.Resizable = resizeTmp;
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new DoubleArrayJsonItem() {
        Name = "[OverridableParamName]",
        Description = value.ItemDescription,
        Value = ((DoubleArray)value).CloneAsArray(),
        Range = new double[] { double.MinValue, double.MaxValue }
      };
  }

  public class PercentArrayConverter : BaseConverter {
    public override int Priority => 2;
    public override Type ConvertableType => typeof(PercentArray);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      PercentArray arr = item as PercentArray;
      double[] d = CastValue<double[]>(data);
      bool resizeTmp = arr.Resizable;
      arr.Resizable = true;
      arr.Length = d.Length;
      for (int i = 0; i < d.Length; ++i)
        arr[i] = d[i];
      arr.Resizable = resizeTmp;
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new DoubleArrayJsonItem() {
        Name = "[OverridableParamName]",
        Description = value.ItemDescription,
        Value = ((PercentArray)value).CloneAsArray(),
        Range = new double[] { 0.0d, 1.0d }
      };
  }

  public class BoolArrayConverter : BaseConverter {
    public override int Priority => 1;
    public override Type ConvertableType => typeof(BoolArray);

    public override void Inject(IItem item, IJsonItem data, IJsonItemConverter root) {
      BoolArray arr = item as BoolArray;
      bool[] d = CastValue<bool[]>(data);
      bool resizeTmp = arr.Resizable;
      arr.Resizable = true;
      arr.Length = d.Length;
      for(int i = 0; i < d.Length; ++i)
        arr[i] = d[i];
      arr.Resizable = resizeTmp;
    }

    public override IJsonItem Extract(IItem value, IJsonItemConverter root) =>
      new BoolArrayJsonItem() {
        Name = "[OverridableParamName]",
        Description = value.ItemDescription,
        Value = ((BoolArray)value).CloneAsArray(),
        Range = new bool[] { false, true }
      };
  }
}