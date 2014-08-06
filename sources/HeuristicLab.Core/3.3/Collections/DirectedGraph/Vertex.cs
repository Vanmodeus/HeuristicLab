﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2014 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Common;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Core {
  [Item("Vertex", "An object representing a vertex in the graph. It can have a text label, a weight, and an additional data object.")]
  [StorableClass]
  public class Vertex : Item, IVertex {
    [Storable]
    private string label;
    public string Label {
      get { return label; }
      set {
        if (label.Equals(value)) return;
        label = value;
        OnChanged(this, EventArgs.Empty);
      }
    }

    [Storable]
    private double weight;
    public double Weight {
      get { return weight; }
      set {
        if (weight.Equals(value)) return;
        weight = value;
        OnChanged(this, EventArgs.Empty);
      }
    }

    [Storable]
    private IDeepCloneable data;
    public IDeepCloneable Data {
      get { return data; }
      set {
        if (data == value) return;
        data = value;
        OnChanged(this, EventArgs.Empty);
      }
    }

    private readonly List<IArc> inArcs = new List<IArc>();
    public IEnumerable<IArc> InArcs {
      get { return inArcs; }
    }

    private readonly List<IArc> outArcs = new List<IArc>();
    public IEnumerable<IArc> OutArcs {
      get { return outArcs; }
    }

    public int InDegree { get { return InArcs.Count(); } }
    public int OutDegree { get { return OutArcs.Count(); } }
    public int Degree { get { return InDegree + OutDegree; } }

    [StorableConstructor]
    public Vertex(bool deserializing) : base(deserializing) { }

    [StorableHook(HookType.AfterDeserialization)]
    private void AfterDeserialization() { }

    public Vertex(IDeepCloneable data) {
      this.data = data;
    }

    protected Vertex(Vertex original, Cloner cloner)
      : base(original, cloner) {
      data = cloner.Clone(original.Data);
      label = original.Label;
      weight = original.Weight;

      // we do not clone the arcs here (would cause too much recursion and ultimately a stack overflow)
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Vertex(this, cloner);
    }

    public void AddArc(IArc arc) {
      if (this != arc.Source && this != arc.Target)
        throw new ArgumentException("The current vertex must be either the arc source or the arc target.");

      if (arc.Source == arc.Target)
        throw new ArgumentException("Arc source and target must be different.");

      if (this == arc.Source) {
        if (outArcs.Contains(arc)) throw new InvalidOperationException("Arc already added.");
        outArcs.Add(arc);
      }
      if (this == arc.Target) {
        if (inArcs.Contains(arc)) throw new InvalidOperationException("Arc already added.");
        inArcs.Add(arc);
      }
      OnArcAdded(this, new EventArgs<IArc>(arc));
    }

    public void RemoveArc(IArc arc) {
      if (this != arc.Source && this != arc.Target)
        throw new ArgumentException("The current vertex must be either the arc source or the arc target.");

      if (this == arc.Source) {
        if (!outArcs.Remove(arc)) throw new InvalidOperationException("Arc is not present in this vertex' outgoing arcs.");
      }
      if (this == arc.Target) {
        if (!inArcs.Remove(arc)) throw new InvalidOperationException("Arc is not present in this vertex' incoming arcs.");
      }
      OnArcRemoved(this, new EventArgs<IArc>(arc));
    }

    #region events
    // use the same event to signal a change in the content, weight or label
    public event EventHandler Changed;
    protected virtual void OnChanged(object sender, EventArgs args) {
      var changed = Changed;
      if (changed != null)
        changed(sender, args);
    }

    public event EventHandler<EventArgs<IArc>> ArcAdded;
    protected virtual void OnArcAdded(object sender, EventArgs<IArc> args) {
      var added = ArcAdded;
      if (added != null)
        added(sender, args);
    }

    public event EventHandler<EventArgs<IArc>> ArcRemoved;
    protected virtual void OnArcRemoved(object sender, EventArgs<IArc> args) {
      var removed = ArcRemoved;
      if (removed != null)
        removed(sender, args);
    }
    #endregion
  }

  [StorableClass]
  public class Vertex<T> : Vertex, IVertex<T> where T : class,IItem {
    public new T Data {
      get { return (T)base.Data; }
      set { base.Data = value; }
    }

    [StorableConstructor]
    protected Vertex(bool deserializing) : base(deserializing) { }

    protected Vertex(Vertex<T> original, Cloner cloner)
      : base(original, cloner) {
    }

    public Vertex(IDeepCloneable data)
      : base(data) {
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      return new Vertex<T>(this, cloner);
    }
  }
}