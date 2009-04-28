﻿using System;
using HeuristicLab.Persistence.Core;
using HeuristicLab.Persistence.Interfaces;
using System.Collections.Generic;
using HeuristicLab.Persistence.Default.Decomposers.Storable;

namespace HeuristicLab.Persistence.Default.Decomposers {

  [EmptyStorableClass]
  public class EnumDecomposer : IDecomposer {

    public int Priority {
      get { return 100; }
    }

    public bool CanDecompose(Type type) {
      return type.IsEnum || type == typeof(Enum);
    }

    public IEnumerable<Tag> CreateMetaInfo(object obj) {
      yield return new Tag(Enum.Format(obj.GetType(), obj, "G"));
    }

    public IEnumerable<Tag> Decompose(object obj) {
      return new Tag[] { };
    }

    public object CreateInstance(Type t, IEnumerable<Tag> metaInfo) {
      IEnumerator<Tag> it = metaInfo.GetEnumerator();
      try {
        it.MoveNext();
        return Enum.Parse(t, (string)it.Current.Value);
      } catch (InvalidOperationException e) {
        throw new PersistenceException("not enough meta information to recstruct enum", e);
      } catch (InvalidCastException e) {
        throw new PersistenceException("invalid meta information found while trying to reconstruct enum", e);
      }
    }

    public void Populate(object instance, IEnumerable<Tag> elements, Type t) {
      // Enums are already populated during instance creation.
    }
  }
}