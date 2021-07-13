using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Random;

namespace HeuristicLab.Problems.Instances.DataAnalysis {
  public class Feynman97 : FeynmanDescriptor {
    private readonly int testSamples;
    private readonly int trainingSamples;

    public Feynman97() : this((int) DateTime.Now.Ticks, 10000, 10000, null) { }

    public Feynman97(int seed) {
      Seed            = seed;
      trainingSamples = 10000;
      testSamples     = 10000;
      noiseRatio      = null;
    }

    public Feynman97(int seed, int trainingSamples, int testSamples, double? noiseRatio) {
      Seed                 = seed;
      this.trainingSamples = trainingSamples;
      this.testSamples     = testSamples;
      this.noiseRatio      = noiseRatio;
    }

    public override string Name {
      get {
        return string.Format("III.15.27 2*pi*alpha/(n*d) | {0}",
          noiseRatio == null ? "no noise" : string.Format(System.Globalization.CultureInfo.InvariantCulture, "noise={0:g}",noiseRatio));
      }
    }

    protected override string TargetVariable { get { return noiseRatio == null ? "k" : "k_noise"; } }

    protected override string[] VariableNames {
      get { return noiseRatio == null ? new[] { "alpha", "n", "d", "k" } : new[] { "alpha", "n", "d", "k", "k_noise" }; }
    }

    protected override string[] AllowedInputVariables { get { return new[] {"alpha", "n", "d"}; } }

    public int Seed { get; private set; }

    protected override int TrainingPartitionStart { get { return 0; } }
    protected override int TrainingPartitionEnd { get { return trainingSamples; } }
    protected override int TestPartitionStart { get { return trainingSamples; } }
    protected override int TestPartitionEnd { get { return trainingSamples + testSamples; } }

    protected override List<List<double>> GenerateValues() {
      var rand = new MersenneTwister((uint) Seed);

      var data  = new List<List<double>>();
      var alpha = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var n     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();
      var d     = ValueGenerator.GenerateUniformDistributedValues(rand.Next(), TestPartitionEnd, 1, 5).ToList();

      var k = new List<double>();

      data.Add(alpha);
      data.Add(n);
      data.Add(d);
      data.Add(k);

      for (var i = 0; i < alpha.Count; i++) {
        var res = 2 * Math.PI * alpha[i] / (n[i] * d[i]);
        k.Add(res);
      }

      var targetNoise = GetNoisyTarget(k, rand);
      if (targetNoise != null) data.Add(targetNoise);

      return data;
    }
  }
}