#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Drawing;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;
using HeuristicLab.Collections;

namespace HeuristicLab.Optimization {
  /// <summary>
  /// A run in which an algorithm is executed a given number of times.
  /// </summary>
  [Item("Batch Run", "A run in which an algorithm is executed a given number of times.")]
  [Creatable("Testing & Analysis")]
  [StorableClass]
  public sealed class BatchRun : NamedItem, IOptimizer {
    public override Image ItemImage {
      get {
        if (ExecutionState == ExecutionState.Prepared) return HeuristicLab.Common.Resources.VS2008ImageLibrary.ExecutablePrepared;
        else if (ExecutionState == ExecutionState.Started) return HeuristicLab.Common.Resources.VS2008ImageLibrary.ExecutableStarted;
        else if (ExecutionState == ExecutionState.Paused) return HeuristicLab.Common.Resources.VS2008ImageLibrary.ExecutablePaused;
        else if (ExecutionState == ExecutionState.Stopped) return HeuristicLab.Common.Resources.VS2008ImageLibrary.ExecutableStopped;
        else return HeuristicLab.Common.Resources.VS2008ImageLibrary.Event;
      }
    }

    [Storable]
    private ExecutionState executionState;
    public ExecutionState ExecutionState {
      get { return executionState; }
      private set {
        if (executionState != value) {
          executionState = value;
          OnExecutionStateChanged();
          OnItemImageChanged();
        }
      }
    }

    [Storable]
    private TimeSpan executionTime;
    public TimeSpan ExecutionTime {
      get {
        if ((Algorithm != null) && (Algorithm.ExecutionState != ExecutionState.Stopped))
          return executionTime + Algorithm.ExecutionTime;
        else
          return executionTime;
      }
      private set {
        executionTime = value;
        OnExecutionTimeChanged();
      }
    }

    [Storable]
    private IAlgorithm algorithm;
    public IAlgorithm Algorithm {
      get { return algorithm; }
      set {
        if (algorithm != value) {
          if (algorithm != null) DeregisterAlgorithmEvents();
          algorithm = value;
          if (algorithm != null) RegisterAlgorithmEvents();
          OnAlgorithmChanged();
          Prepare(true);
        }
      }
    }

    [Storable]
    private int repetitions;
    public int Repetitions {
      get { return repetitions; }
      set {
        if (repetitions != value) {
          repetitions = value;
          OnRepetitionsChanged();
          if ((runs.Count < repetitions) && (Algorithm != null) && (Algorithm.ExecutionState == ExecutionState.Stopped))
            Prepare();
        }
      }
    }

    [Storable]
    private RunCollection runs;
    public RunCollection Runs {
      get { return runs; }
    }

    private bool stopPending;

    public BatchRun()
      : base() {
      name = ItemName;
      description = ItemDescription;
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      repetitions = 10;
      runs = new RunCollection();
      stopPending = false;
    }
    public BatchRun(string name)
      : base(name) {
      description = ItemDescription;
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      repetitions = 10;
      runs = new RunCollection();
      stopPending = false;
    }
    public BatchRun(string name, string description)
      : base(name, description) {
      executionState = ExecutionState.Stopped;
      executionTime = TimeSpan.Zero;
      repetitions = 10;
      runs = new RunCollection();
      stopPending = false;
    }
    [StorableConstructor]
    private BatchRun(bool deserializing)
      : base(deserializing) {
      stopPending = false;
    }

    [StorableHook(HookType.AfterDeserialization)]
    private void Initialize() {
      if (algorithm != null) RegisterAlgorithmEvents();
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      if (ExecutionState == ExecutionState.Started) throw new InvalidOperationException(string.Format("Clone not allowed in execution state \"{0}\".", ExecutionState));
      BatchRun clone = (BatchRun)base.Clone(cloner);
      clone.executionState = executionState;
      clone.executionTime = executionTime;
      clone.algorithm = (IAlgorithm)cloner.Clone(algorithm);
      clone.repetitions = repetitions;
      clone.runs = (RunCollection)cloner.Clone(runs);
      clone.stopPending = stopPending;
      clone.Initialize();
      return clone;
    }

    public void Prepare() {
      Prepare(false);
    }
    public void Prepare(bool clearRuns) {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused) && (ExecutionState != ExecutionState.Stopped))
        throw new InvalidOperationException(string.Format("Prepare not allowed in execution state \"{0}\".", ExecutionState));
      if (Algorithm != null) {
        if (clearRuns) {
          ExecutionTime = TimeSpan.Zero;
          runs.Clear();
        }
        Algorithm.Prepare(clearRuns);
      }
    }
    public void Start() {
      if ((ExecutionState != ExecutionState.Prepared) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Start not allowed in execution state \"{0}\".", ExecutionState));
      if (Algorithm != null) Algorithm.Start();
    }
    public void Pause() {
      if (ExecutionState != ExecutionState.Started)
        throw new InvalidOperationException(string.Format("Pause not allowed in execution state \"{0}\".", ExecutionState));
      if (Algorithm != null) Algorithm.Pause();
    }
    public void Stop() {
      if ((ExecutionState != ExecutionState.Started) && (ExecutionState != ExecutionState.Paused))
        throw new InvalidOperationException(string.Format("Stop not allowed in execution state \"{0}\".", ExecutionState));
      stopPending = true;
      if (Algorithm != null) Algorithm.Stop();
    }

    #region Events
    public event EventHandler ExecutionStateChanged;
    private void OnExecutionStateChanged() {
      EventHandler handler = ExecutionStateChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler ExecutionTimeChanged;
    private void OnExecutionTimeChanged() {
      EventHandler handler = ExecutionTimeChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler AlgorithmChanged;
    private void OnAlgorithmChanged() {
      EventHandler handler = AlgorithmChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler RepetitionsChanged;
    private void OnRepetitionsChanged() {
      EventHandler handler = RepetitionsChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Prepared;
    private void OnPrepared() {
      ExecutionState = ExecutionState.Prepared;
      EventHandler handler = Prepared;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Started;
    private void OnStarted() {
      ExecutionState = ExecutionState.Started;
      EventHandler handler = Started;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Paused;
    private void OnPaused() {
      ExecutionState = ExecutionState.Paused;
      EventHandler handler = Paused;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler Stopped;
    private void OnStopped() {
      ExecutionState = ExecutionState.Stopped;
      EventHandler handler = Stopped;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    public event EventHandler<EventArgs<Exception>> ExceptionOccurred;
    private void OnExceptionOccurred(Exception exception) {
      EventHandler<EventArgs<Exception>> handler = ExceptionOccurred;
      if (handler != null) handler(this, new EventArgs<Exception>(exception));
    }

    private void RegisterAlgorithmEvents() {
      algorithm.ExceptionOccurred += new EventHandler<EventArgs<Exception>>(Algorithm_ExceptionOccurred);
      algorithm.ExecutionTimeChanged += new EventHandler(Algorithm_ExecutionTimeChanged);
      algorithm.Paused += new EventHandler(Algorithm_Paused);
      algorithm.Prepared += new EventHandler(Algorithm_Prepared);
      algorithm.Started += new EventHandler(Algorithm_Started);
      algorithm.Stopped += new EventHandler(Algorithm_Stopped);
      algorithm.Runs.CollectionReset += new CollectionItemsChangedEventHandler<IRun>(Algorithm_Runs_CollectionReset);
      algorithm.Runs.ItemsAdded += new CollectionItemsChangedEventHandler<IRun>(Algorithm_Runs_ItemsAdded);
      algorithm.Runs.ItemsRemoved += new CollectionItemsChangedEventHandler<IRun>(Algorithm_Runs_ItemsRemoved);
    }
    private void DeregisterAlgorithmEvents() {
      algorithm.ExceptionOccurred -= new EventHandler<EventArgs<Exception>>(Algorithm_ExceptionOccurred);
      algorithm.ExecutionTimeChanged -= new EventHandler(Algorithm_ExecutionTimeChanged);
      algorithm.Paused -= new EventHandler(Algorithm_Paused);
      algorithm.Prepared -= new EventHandler(Algorithm_Prepared);
      algorithm.Started -= new EventHandler(Algorithm_Started);
      algorithm.Stopped -= new EventHandler(Algorithm_Stopped);
      algorithm.Runs.CollectionReset -= new CollectionItemsChangedEventHandler<IRun>(Algorithm_Runs_CollectionReset);
      algorithm.Runs.ItemsAdded -= new CollectionItemsChangedEventHandler<IRun>(Algorithm_Runs_ItemsAdded);
      algorithm.Runs.ItemsRemoved -= new CollectionItemsChangedEventHandler<IRun>(Algorithm_Runs_ItemsRemoved);
    }
    private void Algorithm_ExceptionOccurred(object sender, EventArgs<Exception> e) {
      OnExceptionOccurred(e.Value);
    }
    private void Algorithm_ExecutionTimeChanged(object sender, EventArgs e) {
      OnExecutionTimeChanged();
    }
    private void Algorithm_Paused(object sender, EventArgs e) {
      OnPaused();
    }
    private void Algorithm_Prepared(object sender, EventArgs e) {
      if ((ExecutionState == ExecutionState.Paused) || (ExecutionState == ExecutionState.Stopped))
        OnPrepared();
    }
    private void Algorithm_Started(object sender, EventArgs e) {
      stopPending = false;
      if (ExecutionState != ExecutionState.Started)
        OnStarted();
    }
    private void Algorithm_Stopped(object sender, EventArgs e) {
      ExecutionTime += Algorithm.ExecutionTime;

      if (!stopPending && (runs.Count < repetitions)) {
        Algorithm.Prepare();
        Algorithm.Start();
      } else {
        OnStopped();
      }
    }
    private void Algorithm_Runs_CollectionReset(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      Runs.RemoveRange(e.OldItems);
      Runs.AddRange(e.Items);
    }
    private void Algorithm_Runs_ItemsAdded(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      Runs.AddRange(e.Items);
    }
    private void Algorithm_Runs_ItemsRemoved(object sender, CollectionItemsChangedEventArgs<IRun> e) {
      Runs.RemoveRange(e.Items);
    }
    #endregion
  }
}
